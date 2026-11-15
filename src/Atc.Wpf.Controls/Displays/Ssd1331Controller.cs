namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Emulates the command set of an SSD1331 96 x 64 colour OLED controller on a <see cref="ColorFrameBuffer"/>:
/// the command and data bytes a microcontroller sends to the chip are applied to the chip's own display RAM, and the RAM is drawn
/// in the buffer as the panel shows it, so a desktop app can mirror a real module byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// The SSD1331 takes the parameters of a command as command bytes (D/C low): send commands and their parameters with
/// <see cref="WriteCommand"/> or <see cref="WriteCommands"/>, also across calls. The chip counts the parameters of each command, so
/// while a command waits for parameters the next command bytes are its parameters, whatever their value.
/// Data bytes (D/C high, <see cref="WriteData"/>) are pixels only: the chip has no memory-write command, and each pixel goes into the
/// RAM at the pointer of the window set by 0x15 / 0x75, which advances horizontally or vertically (0xA0 bit 0) and wraps in the window.
/// The bytes of one pixel may be split across calls; a command byte discards a half-received pixel.
/// Pixel formats (0xA0 bits 7:6): 65k (2 bytes, RGB565 high byte first), 65k format 2 (3 bytes, the colour in bits 5:0)
/// and 256 colours (1 byte, RGB332).
/// </para>
/// <para>
/// The drawing commands draw into the RAM in RAM coordinates: 0x21 line, 0x22 rectangle (filled with its second colour after 0x26 bit 0),
/// 0x23 copy (with the colours inverted after 0x26 bit 4), 0x24 dim and 0x25 clear. The scroll commands (0x27, 0x2E, 0x2F), the
/// timing, pre-charge and grey-scale commands consume their parameters and are ignored; a byte that is not a command of the chip
/// is written to <see cref="Trace"/> the first time each distinct value is seen by this instance. No byte value throws.
/// </para>
/// <para>
/// The remap bits 1 to 5 of 0xA0 are mappings of the RAM to the panel, so a change re-maps the picture already shown.
/// The panel is wired as the common modules are: after the remap 0x76 of Adafruit_SSD1331's rotation 0 (or 0x72 of its RGB build)
/// panel pixel (x, y) shows RAM column x, row y. Clearing bit 1 mirrors the picture left to right, clearing bit 4 flips it upside down,
/// and the COM split and swap bits (5 and 3) interleave the rows as on the module. Bit 2 sets the colour order (C, B, A "BGR" when set):
/// the colour shown has red and blue swapped when it differs from <see cref="IsPanelBgr"/>.
/// The display modes 0xA4 to 0xA7 show the RAM, all pixels white, all pixels black or the inverse picture (<see cref="ColorFrameBuffer.IsInverted"/>);
/// 0xAE / 0xAF (and the dim mode 0xAC) turn the display off and on. The RAM is never changed by these.
/// The contrast (0x81 to 0x83), master current (0x87) and dim mode are kept but do not change <see cref="ColorFrameBuffer.Brightness"/>,
/// which stays 255. When the buffer is resized the RAM is kept and drawn again for the new size.
/// </para>
/// <para>
/// A new instance and <see cref="Reset"/> apply the chip's power-on state: display off, remap 0x40 (65k colours, horizontal increment),
/// start line 0, offset 0, multiplex ratio 64, the full-RAM window, contrast 0x80, master current 0x0F, normal mode and the command lock off.
/// A new instance starts with black RAM; a reset leaves the RAM as it is.
/// </para>
/// <para>
/// Each call that changes something visible ends in one update of the buffer, so the buffer raises
/// <see cref="ColorFrameBuffer.Changed"/> at most once per call, and not at all when nothing visible changed.
/// </para>
/// <para>
/// Feed the controller from one thread at a time: bytes fed from several threads at once arrive in no defined order.
/// The calls are serialized by an internal lock; the buffer raises <see cref="ColorFrameBuffer.Changed"/> after that lock is released,
/// on the thread that made the change. The buffer it writes to is thread-safe. Dispose the controller to detach it from the buffer; a disposed controller ignores everything sent to it.
/// </para>
/// </remarks>
public sealed class Ssd1331Controller : IGraphicDisplayController
{
    private const int RamWidth = 96;
    private const int RamHeight = 64;
    private const int MaxParameters = 32;
    private const int Unknown = -1;
    private const byte PowerOnRemap = 0x40;
    private const byte VerticalIncrementBit = 0x01;
    private const byte ColumnRemapBit = 0x02;
    private const byte ColorOrderBit = 0x04;
    private const byte ComSwapBit = 0x08;
    private const byte ComReverseBit = 0x10;
    private const byte ComSplitBit = 0x20;
    private const byte PanelMappingBits = ColumnRemapBit | ColorOrderBit | ComSwapBit | ComReverseBit | ComSplitBit;
    private const int DepthShift = 6;
    private const int Depth256 = 0;
    private const int Depth65kFormat2 = 2;
    private const int Depth65kUnlisted = 3;
    private const byte NormalMode = 0xA4;
    private const byte AllOnMode = 0xA5;
    private const byte AllOffMode = 0xA6;
    private const byte InverseMode = 0xA7;
    private const byte LockCommand = 0xFD;
    private const byte LockValue = 0x16;
    private const byte UnlockValue = 0x12;
    private const int MinMultiplexRatio = 15;
    private const int WhiteRgb = 0xFFFFFF;

    private readonly Lock sync = new();
    private readonly ColorFrameBuffer buffer;
    private readonly ushort[] ram = new ushort[RamWidth * RamHeight];
    private readonly byte[] parameters = new byte[MaxParameters];
    private readonly byte[] pixelBytes = new byte[3];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private int[] rowBuffer = [];
    private byte pendingCommand;
    private int pendingCount;
    private int collectedCount;
    private int pixelByteCount;
    private byte displayMode;
    private bool isDisplayOn;
    private bool isLocked;
    private bool isDepthTraced;
    private bool isFullRedrawNeeded;
    private int dirtyFirst = int.MaxValue;
    private int dirtyLast = -1;
    private int mappedWidth;
    private int mappedHeight;
    private bool isDisposed;

    // ref: §SSD1331.4, §P2.X item 1: the Adafruit 0.96 inch module shows RGB565 correctly with A[2] = 1 (⚠️ derived from the library).
    private bool isPanelBgr = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ssd1331Controller"/> class in the chip's power-on state, with black RAM.
    /// </summary>
    /// <param name="buffer">The buffer that shows the panel; 96 x 64 matches the module (a larger buffer shows black outside the panel).</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    public Ssd1331Controller(ColorFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        this.buffer = buffer;

        using (BeginBatchOutsideLock())
        {
            lock (sync)
            {
                ApplyPowerOnState();
                Flush();
            }
        }

        buffer.Changed += OnBufferChanged;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the panel's colour filter is in BGR order: the colour shown has red and blue swapped
    /// when the colour order bit of 0xA0 (bit 2) differs from this value. The default is <see langword="true"/>, the Adafruit 0.96 inch
    /// 96 x 64 module, which shows RGB565 data correctly after the library's default remap 0x76 (bit 2 set).
    /// Set it to <see langword="false"/> for a module wired the other way, the one driven by the library's
    /// <c>SSD1331_COLORORDER_RGB</c> build (remap 0x72, bit 2 clear).
    /// </summary>
    public bool IsPanelBgr
    {
        get
        {
            lock (sync)
            {
                return isPanelBgr;
            }
        }

        set
        {
            using var batch = BeginBatchOutsideLock();

            lock (sync)
            {
                if (isDisposed)
                {
                    return;
                }

                if (isPanelBgr == value)
                {
                    return;
                }

                isPanelBgr = value;
                isFullRedrawNeeded = true;
                Flush();
            }
        }
    }

    internal byte Remap { get; private set; }

    internal int StartLine { get; private set; }

    internal int DisplayOffset { get; private set; }

    // The 0xA8 parameter N (the multiplex ratio is N + 1).
    internal int MultiplexRatio { get; private set; }

    internal byte ContrastA { get; private set; }

    internal byte ContrastB { get; private set; }

    internal byte ContrastC { get; private set; }

    internal int MasterCurrent { get; private set; }

    internal bool IsDimMode { get; private set; }

    internal bool IsFillEnabled { get; private set; }

    internal bool IsReverseCopy { get; private set; }

    internal int ColumnStart { get; private set; }

    internal int ColumnEnd { get; private set; }

    internal int RowStart { get; private set; }

    internal int RowEnd { get; private set; }

    internal int ColumnPointer { get; private set; }

    internal int RowPointer { get; private set; }

    /// <inheritdoc />
    /// <remarks>The byte is a parameter when the previous command still waits for parameters.</remarks>
    public void WriteCommand(byte command)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ApplyCommandByte(command);
            Flush();
        }
    }

    /// <inheritdoc />
    public void WriteCommands(ReadOnlySpan<byte> commands)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var command in commands)
            {
                ApplyCommandByte(command);
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>The bytes are pixels written at the RAM pointer in the format of 0xA0 bits 7:6.</remarks>
    public void WriteData(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            // ref: §SSD1331.7 0xFD: a locked chip "prohibits all commands and memory access".
            if (!isLocked)
            {
                foreach (var value in data)
                {
                    AddPixelByte(value);
                }
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>The hardware reset (RES pin). The RAM is left as it is.</remarks>
    public void Reset()
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ApplyPowerOnState();
            Flush();
        }
    }

    /// <summary>
    /// Detaches the controller from the buffer and makes it inert: the buffer keeps the last picture, and every later write, reset or
    /// setting is ignored (argument checks still apply).
    /// </summary>
    public void Dispose()
    {
        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            buffer.Changed -= OnBufferChanged;
        }
    }

    internal ushort[] GetRam()
    {
        lock (sync)
        {
            return (ushort[])ram.Clone();
        }
    }

    // ref: §SSD1331.2 parameter counts; 0 for a known command without parameters, Unknown for a byte that is not a command.
    private static int ParameterCount(byte command)
        => command switch
        {
            0x15 or 0x75 => 2,
            0x21 => 7,
            0x22 => 10,
            0x23 => 6,
            0x24 or 0x25 => 4,
            0x27 or 0xAB => 5,
            0xB8 => 32,
            0x26 or 0x81 or 0x82 or 0x83 or 0x87 or 0x8A or 0x8B or 0x8C or 0xA0 or 0xA1 or 0xA2 or 0xA8 => 1,
            0xAD or 0xB0 or 0xB1 or 0xB3 or 0xBB or 0xBE or LockCommand => 1,
            0x2E or 0x2F or (>= NormalMode and <= InverseMode) or 0xAC or 0xAE or 0xAF or 0xB9 or 0xBC or 0xBD or 0xE3 => 0,
            _ => Unknown,
        };

    // ref: §SSD1331.6: C and A are 5-bit values in bits 5:1, B a 6-bit value in bits 5:0; the RAM word is C << 11 | B << 5 | A.
    private static ushort ColorWord(ReadOnlySpan<byte> arguments)
        => (ushort)((((arguments[0] >> 1) & 0x1F) << 11) | ((arguments[1] & 0x3F) << 5) | ((arguments[2] >> 1) & 0x1F));

    private static (int Left, int Top, int Right, int Bottom) Corners(
        ReadOnlySpan<byte> arguments)
        => (System.Math.Min(arguments[0], arguments[2]),
            System.Math.Min(arguments[1], arguments[3]),
            System.Math.Max(arguments[0], arguments[2]),
            System.Math.Max(arguments[1], arguments[3]));

    private static int Expand5(int value)
        => (value << 3) | (value >> 2);

    private static int Expand6(int value)
        => (value << 2) | (value >> 4);

    // ref: §SSD1331.6 0x24, Table 15: GS0..GS15 unchanged, GS16..GS63 -> GS / 4. B carries the 6-bit grey level; A and C carry the even
    // levels GS0, GS2 .. GS62 as 5-bit values (⚠️ unverified: the stored value of a 5-bit colour after dimming, §SSD1331.6).
    private static ushort Dim(ushort word)
    {
        var c = DimGreyLevel((word >> 11) * 2) >> 1;
        var b = DimGreyLevel((word >> 5) & 0x3F);
        var a = DimGreyLevel((word & 0x1F) * 2) >> 1;
        return (ushort)((c << 11) | (b << 5) | a);
    }

    private static int DimGreyLevel(int level)
        => level < 16 ? level : level >> 2;

    // ref: §SSD1331.3 Table 12 with the module wiring of the note (⚠️ derived): panel row y is wired to COM pin (63 - y) / 2 for odd y
    // and 32 + (62 - y) / 2 for even y, so the library's case G (A[5:3] = 110) shows RAM row y. Returns the scan slot of the pin before
    // the COM reverse bit (A[4]): Table 12's cases A, B, E and F; cases C, D, G and H are 63 - slot.
    private static int ScanSlot(
        int panelRow,
        byte remap)
    {
        var pin = (panelRow & 1) == 1
            ? (63 - panelRow) / 2
            : 32 + ((62 - panelRow) / 2);
        var isSwapped = (remap & ComSwapBit) != 0;
        if ((remap & ComSplitBit) == 0)
        {
            return isSwapped ? pin ^ 32 : pin;
        }

        return pin < 32
            ? (2 * pin) + (isSwapped ? 1 : 0)
            : (2 * (pin - 32)) + (isSwapped ? 0 : 1);
    }

    // Opened before the lock is taken and disposed after it is released: the buffer raises Changed when the outermost batch ends,
    // so it is never raised while the controller's lock is held.
    private IDisposable BeginBatchOutsideLock()
        => buffer.BeginBatch();

    private void ApplyCommandByte(byte value)
    {
        if (pendingCount > 0)
        {
            AddParameter(value);
            return;
        }

        // ref: §P2.X item 4 (TFT precedent, ⚠️ derived for this chip): a command byte discards a half-received pixel.
        pixelByteCount = 0;

        // ref: §SSD1331.2 0xFD: a locked chip ignores every command except 0xFD (⚠️ derived: the bytes of an ignored command are
        // not counted as its parameters).
        if (isLocked && value != LockCommand)
        {
            return;
        }

        var count = ParameterCount(value);
        if (count > 0)
        {
            pendingCommand = value;
            pendingCount = count;
            collectedCount = 0;
            return;
        }

        if (count == 0)
        {
            ExecuteSimple(value);
            return;
        }

        TraceOnce(value);
    }

    private void AddParameter(byte value)
    {
        parameters[collectedCount++] = value;
        if (collectedCount < pendingCount)
        {
            return;
        }

        var count = pendingCount;
        pendingCount = 0;
        collectedCount = 0;
        Execute(pendingCommand, parameters.AsSpan(0, count));
    }

    // ref: §SSD1331.2, §SSD1331.7: the commands without parameters.
    private void ExecuteSimple(byte command)
    {
        switch (command)
        {
            case >= NormalMode and <= InverseMode:
                if (displayMode != command)
                {
                    displayMode = command;
                    isFullRedrawNeeded = true;
                }

                break;
            case 0xAC:
                // ⚠️ unverified how dim mode looks (§SSD1331.7): the display is on; the brightness is not changed.
                isDisplayOn = true;
                IsDimMode = true;
                break;
            case 0xAE or 0xAF:
                isDisplayOn = command == 0xAF;
                IsDimMode = false;
                break;

            // 0x2E / 0x2F scroll off / on (out of scope, design spec), 0xB9 linear grey scale, 0xBC / 0xBD / 0xE3 NOP.
        }
    }

    private void Execute(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x15 or 0x75 or 0xA0 or 0xA1 or 0xA2 or 0xA8:
                ExecuteAddressing(command, arguments);
                break;
            case 0x21 or 0x22 or 0x23 or 0x24 or 0x25 or 0x26:
                ExecuteDrawing(command, arguments);
                break;
            case 0x81:
                ContrastA = arguments[0];
                break;
            case 0x82:
                ContrastB = arguments[0];
                break;
            case 0x83:
                ContrastC = arguments[0];
                break;
            case 0x87:
                // ref: §SSD1331.2: A[3:0]; the master current scales all colours by (n + 1) / 16. Kept, not drawn: the current-to-brightness
                // curve is ⚠️ unverified (§SSD1331.7), and the Adafruit init's 0x06 would show every replay at 7/16 opacity.
                MasterCurrent = arguments[0] & 0x0F;
                break;
            case LockCommand:
                if (arguments[0] == LockValue)
                {
                    isLocked = true;
                }
                else if (arguments[0] == UnlockValue)
                {
                    isLocked = false;
                }

                break;

            // 0x27 scroll setup (out of scope), 0x8A..0x8C, 0xAB, 0xAD, 0xB0, 0xB1, 0xB3, 0xB8, 0xBB, 0xBE: consumed (§SSD1331.2).
        }
    }

    // ref: §SSD1331.2, §SSD1331.3, §SSD1331.5.
    private void ExecuteAddressing(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x15:
                ColumnStart = arguments[0] & 0x7F;
                ColumnEnd = arguments[1] & 0x7F;
                ColumnPointer = ColumnStart;
                break;
            case 0x75:
                RowStart = arguments[0] & 0x3F;
                RowEnd = arguments[1] & 0x3F;
                RowPointer = RowStart;
                break;
            case 0xA0:
                SetRemap(arguments[0]);
                break;
            case 0xA1:
                isFullRedrawNeeded |= StartLine != (arguments[0] & 0x3F);
                StartLine = arguments[0] & 0x3F;
                break;
            case 0xA2:
                isFullRedrawNeeded |= DisplayOffset != (arguments[0] & 0x3F);
                DisplayOffset = arguments[0] & 0x3F;
                break;
            default:
                // 0xA8, ref: §SSD1331.2: N 15..63; 0..14 are invalid and ignored (⚠️ derived).
                var ratio = arguments[0] & 0x3F;
                if (ratio >= MinMultiplexRatio && ratio != MultiplexRatio)
                {
                    MultiplexRatio = ratio;
                    isFullRedrawNeeded = true;
                }

                break;
        }
    }

    // ref: §SSD1331.3: A[0] and A[7:6] act on the following writes; A[1] to A[5] map the stored RAM to the panel (⚠️ derived: a change
    // re-maps the picture already shown).
    private void SetRemap(byte value)
    {
        if (((Remap ^ value) & PanelMappingBits) != 0)
        {
            isFullRedrawNeeded = true;
        }

        Remap = value;
        if ((value >> DepthShift) != Depth65kUnlisted || isDepthTraced)
        {
            return;
        }

        isDepthTraced = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1331Controller)}: colour depth 11 (0xA0 0x{value:X2}) is not listed by the datasheet; 65k colours are used."));
    }

    // ref: §SSD1331.6. Coordinates are RAM coordinates; the window of 0x15 / 0x75 is not used, and the drawing is clipped to the RAM
    // (⚠️ unverified: clipping, reversed corners (normalized here) and the remap bits are not described for drawing).
    private void ExecuteDrawing(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x21:
                var line = new RamSink(this, ColorWord(arguments[4..]));
                DisplayRasterizer.DrawLine(ref line, arguments[0], arguments[1], arguments[2], arguments[3], RamWidth, RamHeight);
                break;
            case 0x22:
                DrawRectangle(arguments);
                break;
            case 0x23:
                Copy(arguments);
                break;
            case 0x24 or 0x25:
                var (left, top, right, bottom) = Corners(arguments);
                for (var row = top; row <= bottom && row < RamHeight; row++)
                {
                    for (var column = left; column <= right && column < RamWidth; column++)
                    {
                        var word = ram[(row * RamWidth) + column];
                        SetRamWord(column, row, command == 0x24 ? Dim(word) : (ushort)0);
                    }
                }

                break;
            default:
                // 0x26: A[0] fill for rectangles, A[4] reverse copy.
                IsFillEnabled = (arguments[0] & 0x01) != 0;
                IsReverseCopy = (arguments[0] & 0x10) != 0;
                break;
        }
    }

    // ref: §SSD1331.6 0x22: the outline in the first colour; the inside in the second colour only when the fill is enabled.
    private void DrawRectangle(ReadOnlySpan<byte> arguments)
    {
        var (left, top, right, bottom) = Corners(arguments);
        var outline = new RamSink(this, ColorWord(arguments[4..]));
        DisplayRasterizer.DrawRectangleOutline(ref outline, left, top, right - left + 1, bottom - top + 1, RamWidth, RamHeight);
        if (!IsFillEnabled)
        {
            return;
        }

        var fill = ColorWord(arguments[7..]);
        for (var row = top + 1; row < bottom && row < RamHeight; row++)
        {
            for (var column = left + 1; column < right && column < RamWidth; column++)
            {
                SetRamWord(column, row, fill);
            }
        }
    }

    // ref: §SSD1331.6 0x23: copies (col1, row1)-(col2, row2) to the top-left (col3, row3); with reverse copy every grey level of the
    // copy is inverted (GS0 <-> GS63). ⚠️ unverified: the order for overlapping copies; this is a memmove-style copy.
    private void Copy(ReadOnlySpan<byte> arguments)
    {
        var (left, top, right, bottom) = Corners(arguments);
        int targetColumn = arguments[4];
        int targetRow = arguments[5];
        var width = System.Math.Min(System.Math.Min(right, RamWidth - 1) - left + 1, RamWidth - targetColumn);
        var height = System.Math.Min(System.Math.Min(bottom, RamHeight - 1) - top + 1, RamHeight - targetRow);
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var mask = IsReverseCopy ? (ushort)0xFFFF : (ushort)0;
        var isBottomUp = targetRow > top;
        var isRightToLeft = targetRow == top && targetColumn > left;
        for (var i = 0; i < height; i++)
        {
            var offset = isBottomUp ? height - 1 - i : i;
            for (var j = 0; j < width; j++)
            {
                var step = isRightToLeft ? width - 1 - j : j;
                var word = ram[((top + offset) * RamWidth) + left + step];
                SetRamWord(targetColumn + step, targetRow + offset, (ushort)(word ^ mask));
            }
        }
    }

    // Sets one RAM word; an address outside the RAM is ignored. Returns true when the word changed.
    private bool SetRamWord(
        int column,
        int row,
        ushort word)
    {
        if (column < 0 || column >= RamWidth || row < 0 || row >= RamHeight)
        {
            return false;
        }

        ref var cell = ref ram[(row * RamWidth) + column];
        if (cell == word)
        {
            return false;
        }

        cell = word;
        if (row < dirtyFirst)
        {
            dirtyFirst = row;
        }

        if (row > dirtyLast)
        {
            dirtyLast = row;
        }

        return true;
    }

    // ref: §SSD1331.4 pixel formats on the 8-bit bus.
    private void AddPixelByte(byte value)
    {
        pixelBytes[pixelByteCount++] = value;
        switch (Remap >> DepthShift)
        {
            case Depth256:
                // C2 C1 C0 B2 B1 B0 A1 A0 (the note's C4 C3 C2 B5 B4 B3 A4 A3), expanded by repeating the bits from the top:
                // C = C4 C3 C2 C4 C3, B = B5 B4 B3 B5 B4 B3, A = A4 A3 A4 A3 A4. ⚠️ unverified (§SSD1331.4, Figure 11): the note's reading
                // of the garbled figure copies the top bit instead (C = C4 C3 C2 C4 C4, B = B5 B4 B3 B5 B5 B5, A = A4 A3 A4 A4 A4);
                // both agree on all-zero and all-one fields.
                pixelByteCount = 0;
                var c3 = value >> 5;
                var b3 = (value >> 2) & 0x07;
                var a2 = value & 0x03;
                WritePixel((ushort)((((c3 << 2) | (c3 >> 1)) << 11) | (((b3 << 3) | b3) << 5) | (a2 << 3) | (a2 << 1) | (a2 >> 1)));
                break;
            case Depth65kFormat2:
                if (pixelByteCount == 3)
                {
                    pixelByteCount = 0;
                    WritePixel(ColorWord(pixelBytes));
                }

                break;
            default:
                if (pixelByteCount == 2)
                {
                    pixelByteCount = 0;
                    WritePixel((ushort)((pixelBytes[0] << 8) | pixelBytes[1]));
                }

                break;
        }
    }

    // ref: §SSD1331.5: horizontal increment (A[0] = 0) column first, vertical (A[0] = 1) row first; both wrap in the window.
    // ⚠️ unverified: a start after its end (the pointer wraps after every pixel here).
    private void WritePixel(ushort word)
    {
        SetRamWord(ColumnPointer, RowPointer, word);
        if ((Remap & VerticalIncrementBit) == 0)
        {
            if (ColumnPointer < ColumnEnd)
            {
                ColumnPointer++;
                return;
            }

            ColumnPointer = ColumnStart;
            RowPointer = RowPointer < RowEnd ? RowPointer + 1 : RowStart;
            return;
        }

        if (RowPointer < RowEnd)
        {
            RowPointer++;
            return;
        }

        RowPointer = RowStart;
        ColumnPointer = ColumnPointer < ColumnEnd ? ColumnPointer + 1 : ColumnStart;
    }

    // ref: §SSD1331.1 reset state. The RAM is not touched.
    private void ApplyPowerOnState()
    {
        pendingCount = 0;
        collectedCount = 0;
        pixelByteCount = 0;
        Remap = PowerOnRemap;
        StartLine = 0;
        DisplayOffset = 0;
        MultiplexRatio = RamHeight - 1;
        ContrastA = 0x80;
        ContrastB = 0x80;
        ContrastC = 0x80;
        MasterCurrent = 0x0F;
        displayMode = NormalMode;
        isDisplayOn = false;
        IsDimMode = false;
        IsFillEnabled = false;
        IsReverseCopy = false;
        isLocked = false;
        ColumnStart = 0;
        ColumnEnd = RamWidth - 1;
        RowStart = 0;
        RowEnd = RamHeight - 1;
        ColumnPointer = 0;
        RowPointer = 0;
        isFullRedrawNeeded = true;
    }

    private void TraceOnce(byte value)
    {
        if (tracedCommands[value])
        {
            return;
        }

        tracedCommands[value] = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1331Controller)}: command byte 0x{value:X2} is not a command of the chip and is ignored."));
    }

    private void OnBufferChanged(
        object? sender,
        EventArgs e)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed ||
                (buffer.Width == mappedWidth && buffer.Height == mappedHeight))
            {
                return;
            }

            // The buffer was resized: the RAM is kept and drawn again for the new size.
            isFullRedrawNeeded = true;
            Flush();
        }
    }

    // Must be called with the lock held, inside a BeginBatchOutsideLock scope, so the buffer raises Changed after the lock is released.
    private void Flush()
    {
        buffer.IsDisplayOn = isDisplayOn;
        buffer.IsInverted = displayMode == InverseMode;
        buffer.IsSleeping = false;

        // ⚠️ design (R15 precedent): the contrast and master current curve is unverified (§SSD1331.7), so the brightness stays full.
        buffer.Brightness = byte.MaxValue;

        var width = buffer.Width;
        var height = buffer.Height;
        if (width != mappedWidth || height != mappedHeight)
        {
            isFullRedrawNeeded = true;
        }

        if (!isFullRedrawNeeded && dirtyLast < 0)
        {
            return;
        }

        if (rowBuffer.Length < width)
        {
            rowBuffer = new int[width];
        }

        var row = rowBuffer.AsSpan(0, width);
        for (var y = 0; y < height; y++)
        {
            var ramRow = PanelRowToRamRow(y);
            if (!isFullRedrawNeeded && (ramRow < dirtyFirst || ramRow > dirtyLast))
            {
                continue;
            }

            FillPanelRow(row, ramRow);

            // A resize between reading the size and this write is clipped by the buffer; its Changed draws again (OnBufferChanged).
            buffer.WriteRow(y, 0, row);
        }

        mappedWidth = width;
        mappedHeight = height;
        isFullRedrawNeeded = false;
        dirtyFirst = int.MaxValue;
        dirtyLast = -1;
    }

    // Panel column x shows RAM column x with the column remap A[1] set (the upright module, §SSD1331.3), 95 - x without it.
    // ref: §SSD1331.4: with A[2] = 1 the top five bits (C) are red and A blue; with A[2] = 0 the other way round.
    private void FillPanelRow(
        Span<int> row,
        int ramRow)
    {
        var count = System.Math.Min(RamWidth, row.Length);
        row.Clear();
        if (ramRow < 0 || displayMode == AllOffMode)
        {
            return;
        }

        if (displayMode == AllOnMode)
        {
            row[..count].Fill(WhiteRgb);
            return;
        }

        var isMirrored = (Remap & ColumnRemapBit) == 0;
        var isRedOnTop = ((Remap & ColorOrderBit) != 0) == isPanelBgr;
        var offset = ramRow * RamWidth;
        for (var x = 0; x < count; x++)
        {
            var word = ram[offset + (isMirrored ? RamWidth - 1 - x : x)];
            var top = Expand5(word >> 11);
            var green = Expand6((word >> 5) & 0x3F);
            var bottom = Expand5(word & 0x1F);
            row[x] = isRedOnTop
                ? (top << 16) | (green << 8) | bottom
                : (bottom << 16) | (green << 8) | top;
        }
    }

    // The RAM row panel row y shows, or -1 for a row that is not lit. ref: §SSD1331.3 Table 12 and the module wiring (see ScanSlot),
    // combined with the display offset, multiplex ratio and start line as in §SSD1306.5.1 (⚠️ unverified for this chip).
    private int PanelRowToRamRow(int y)
    {
        if (y >= RamHeight)
        {
            return -1;
        }

        var slot = ScanSlot(y, Remap);
        var multiplex = MultiplexRatio + 1;
        var isReversed = (Remap & ComReverseBit) != 0;
        var line = (isReversed ? slot - DisplayOffset : slot + DisplayOffset) & (RamHeight - 1);
        if (line >= multiplex)
        {
            return -1;
        }

        var sigma = isReversed ? multiplex - 1 - line : line;
        return (sigma + StartLine) & (RamHeight - 1);
    }

    // Draws into the RAM for the shared line and rectangle algorithms.
    private readonly struct RamSink : IPixelSink
    {
        private readonly Ssd1331Controller owner;
        private readonly ushort word;

        public RamSink(
            Ssd1331Controller owner,
            ushort word)
        {
            this.owner = owner;
            this.word = word;
        }

        public bool Plot(
            long x,
            long y)
            => x is >= 0 and < RamWidth &&
               y is >= 0 and < RamHeight &&
               owner.SetRamWord((int)x, (int)y, word);

        public bool Fill(
            long x,
            long y,
            long width,
            long height)
        {
            var changed = false;
            for (var row = y; row < y + height; row++)
            {
                for (var column = x; column < x + width; column++)
                {
                    changed |= Plot(column, row);
                }
            }

            return changed;
        }
    }
}