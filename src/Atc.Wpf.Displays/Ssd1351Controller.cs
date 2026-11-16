namespace Atc.Wpf.Displays;

/// <summary>
/// Emulates the command set of an SSD1351 128 x 128 colour OLED controller on a <see cref="ColorFrameBuffer"/>:
/// the command and data bytes a microcontroller sends to the chip are applied to the chip's own display RAM, and the RAM is drawn
/// in the buffer as the panel shows it, so a desktop app can mirror a real module byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// The SSD1351 takes only the first byte of a command as a command byte (D/C low, <see cref="WriteCommand"/> or <see cref="WriteCommands"/>);
/// the parameters follow as data bytes (D/C high, <see cref="WriteData"/>), also across calls. Pixels go into the RAM only after the
/// write RAM command 0x5C, until the next command byte; data that is neither a parameter nor RAM data is ignored. Each pixel goes into
/// the RAM at the pointer of the window set by 0x15 / 0x75, which advances horizontally or vertically (0xA0 bit 0) and wraps in the window.
/// The bytes of one pixel may be split across calls; a command byte discards a half-received pixel and the parameters not yet received.
/// Pixel formats (0xA0 bits 7:6): 65k (2 bytes, RGB565 high byte first) and 262k (3 bytes, the colour in bits 5:0); the 16-bit-bus-only
/// format 11 keeps the previous format.
/// </para>
/// <para>
/// The command lock 0xFD follows the chip: after 0xFD 0x16 every command except 0xFD 0x12 and all RAM writes are ignored, and after
/// 0xFD 0xB0 (the power-on state) the commands 0xA2, 0xB1, 0xB3, 0xBB, 0xBE and 0xC1 are ignored until 0xFD 0xB1.
/// The scroll commands (0x96, 0x9E, 0x9F), the timing, voltage, GPIO and grey-scale commands consume their parameters and are ignored;
/// a byte that is not a command of the chip is written to <see cref="Trace"/> the first time each distinct value is seen by this instance.
/// No byte value throws.
/// </para>
/// <para>
/// The remap bits 1, 2 and 4 of 0xA0 are mappings of the RAM to the panel, so a change re-maps the picture already shown.
/// The panel is wired as the common modules are: after Adafruit_SSD1351's rotation 0 (remap 0x74 and the start line set to the panel
/// height) panel pixel (x, y) shows RAM column x, row y. Setting bit 1 mirrors the picture left to right and clearing bit 4 flips it
/// upside down, so the library's other rotations (0x77, 0x66, 0x65) show as on the module. The panel has as many rows as the buffer,
/// at most 128: a 128 x 96 module (a buffer 96 rows high) shows RAM rows 0 to 95 after the library's init. Bit 2 sets the colour order
/// (C, B, A "BGR" when set): the colour shown has red and blue swapped when it differs from <see cref="IsPanelBgr"/>.
/// The display modes 0xA4 to 0xA7 show all pixels black, all pixels white, the RAM or the inverse picture
/// (<see cref="ColorFrameBuffer.IsInverted"/>); 0xAE / 0xAF turn the display off and on. The RAM is never changed by these.
/// The contrast (0xC1) and master contrast (0xC7) are kept but do not change <see cref="ColorFrameBuffer.Brightness"/>, which stays 255.
/// When the buffer is resized the RAM is kept and drawn again for the new size.
/// </para>
/// <para>
/// A new instance and <see cref="Reset"/> apply the chip's power-on state: display off, remap 0x20 (65k colours, horizontal increment),
/// start line 0, offset 0, multiplex ratio 128, the full-RAM window, contrast 0x8A / 0x51 / 0x8A, master contrast 0x0F, normal mode,
/// unlocked with the restricted commands inaccessible. A new instance starts with black RAM; a reset leaves the RAM as it is.
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
public sealed class Ssd1351Controller : IGraphicDisplayController
{
    private const int RamWidth = 128;
    private const int RamHeight = 128;
    private const int MaxParameters = 63;
    private const int Unknown = -1;
    private const byte PowerOnRemap = 0x20;
    private const byte VerticalIncrementBit = 0x01;
    private const byte ColumnRemapBit = 0x02;
    private const byte ColorOrderBit = 0x04;
    private const byte ComReverseBit = 0x10;
    private const byte PanelMappingBits = ColumnRemapBit | ColorOrderBit | ComReverseBit;
    private const int DepthShift = 6;
    private const int Depth262k = 2;
    private const int Depth16BitBus = 3;
    private const byte WriteRamCommand = 0x5C;
    private const byte AllOffMode = 0xA4;
    private const byte AllOnMode = 0xA5;
    private const byte NormalMode = 0xA6;
    private const byte InverseMode = 0xA7;
    private const byte LockCommand = 0xFD;
    private const byte LockValue = 0x16;
    private const byte UnlockValue = 0x12;
    private const byte RestrictValue = 0xB0;
    private const byte AllowValue = 0xB1;
    private const int MinMultiplexRatio = 15;
    private const int WhiteRgb = 0xFFFFFF;

    private readonly Lock sync = new();
    private readonly ColorFrameBuffer buffer;

    // One 18-bit word per pixel: C (bits 17:12), B (bits 11:6), A (bits 5:0).
    private readonly int[] ram = new int[RamWidth * RamHeight];
    private readonly byte[] parameters = new byte[MaxParameters];
    private readonly byte[] pixelBytes = new byte[3];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private int[] rowBuffer = [];
    private FeedMode mode;
    private byte pendingCommand;
    private int pendingCount;
    private int collectedCount;
    private int pixelByteCount;
    private bool isDepth262k;
    private byte displayMode;
    private bool isDisplayOn;
    private bool isDepthTraced;
    private bool isFullRedrawNeeded;
    private int dirtyFirst = int.MaxValue;
    private int dirtyLast = -1;
    private int mappedWidth;
    private int mappedHeight;
    private bool isDisposed;

    // ref: §SSD1351.4, §P2.X item 1: the Adafruit 128 x 128 and 128 x 96 modules show RGB565 correctly with A[2] = 1 (⚠️ derived from the library).
    private bool isPanelBgr = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ssd1351Controller"/> class in the chip's power-on state, with black RAM.
    /// </summary>
    /// <param name="buffer">
    /// The buffer that shows the panel; 128 x 128 or 128 x 96 matches the modules. The panel has as many rows as the buffer, at most 128
    /// (a larger buffer shows black outside the panel).
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    public Ssd1351Controller(ColorFrameBuffer buffer)
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

    private enum FeedMode
    {
        Ignore,
        Parameters,
        WriteRam,
    }

    /// <summary>
    /// Gets or sets a value indicating whether the panel's colour filter is in BGR order: the colour shown has red and blue swapped
    /// when the colour order bit of 0xA0 (bit 2) differs from this value. The default is <see langword="true"/>, the Adafruit
    /// 128 x 128 and 128 x 96 modules, which show RGB565 data correctly after the library's remap 0x74 (bit 2 set).
    /// Set it to <see langword="false"/> for a module wired the other way.
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

    // The 0xCA parameter N (the multiplex ratio is N + 1).
    internal int MultiplexRatio { get; private set; }

    internal byte ContrastA { get; private set; }

    internal byte ContrastB { get; private set; }

    internal byte ContrastC { get; private set; }

    internal int MasterContrast { get; private set; }

    internal bool IsLocked { get; private set; }

    internal bool IsRestrictedAccessible { get; private set; }

    internal int ColumnStart { get; private set; }

    internal int ColumnEnd { get; private set; }

    internal int RowStart { get; private set; }

    internal int RowEnd { get; private set; }

    internal int ColumnPointer { get; private set; }

    internal int RowPointer { get; private set; }

    /// <inheritdoc />
    /// <remarks>The byte is always a command: the parameters of the chip's commands are data bytes.</remarks>
    public void WriteCommand(byte command)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ApplyCommand(command);
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
                ApplyCommand(command);
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The bytes are the parameters of the last command while it waits for them, then pixels in the format of 0xA0 bits 7:6 after 0x5C;
    /// other data is ignored.
    /// </remarks>
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

            foreach (var value in data)
            {
                switch (mode)
                {
                    case FeedMode.Parameters:
                        AddParameter(value);
                        break;
                    case FeedMode.WriteRam:
                        AddPixelByte(value);
                        break;

                    // FeedMode.Ignore, ref: §SSD1351.2: data that is not a pending parameter and not RAM data (⚠️ derived).
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

    internal int[] GetRam()
    {
        lock (sync)
        {
            return (int[])ram.Clone();
        }
    }

    // ref: §SSD1351.2 parameter counts; 0 for a known command without parameters, Unknown for a byte that is not a command.
    private static int ParameterCount(byte command)
        => command switch
        {
            0x15 or 0x75 => 2,
            0xB2 or 0xB4 or 0xC1 => 3,
            0x96 => 5,
            0xB8 => 63,
            0xA0 or 0xA1 or 0xA2 or 0xAB or 0xB1 or 0xB3 or 0xB5 or 0xB6 or 0xBB or 0xBE or 0xC7 or 0xCA or LockCommand => 1,
            WriteRamCommand or 0x5D or (>= AllOffMode and <= InverseMode) or 0xAD or 0xAE or 0xAF or 0xB0 or 0xB9 or 0xD1 or 0xE3 => 0,
            0x9E or 0x9F => 0,
            _ => Unknown,
        };

    // ref: §SSD1351.6: the commands 0xFD 0xB0 makes inaccessible.
    private static bool IsRestricted(byte command)
        => command is 0xA2 or 0xB1 or 0xB3 or 0xBB or 0xBE or 0xC1;

    // 5-bit to 6-bit by repeating the top bit (⚠️ unverified: how the chip stores a 65k colour in its 18-bit RAM, §SSD1351.4).
    private static int Widen5(int value)
        => (value << 1) | (value >> 4);

    private static int Expand6(int value)
        => (value << 2) | (value >> 4);

    // Opened before the lock is taken and disposed after it is released: the buffer raises Changed when the outermost batch ends,
    // so it is never raised while the controller's lock is held.
    private IDisposable BeginBatchOutsideLock()
        => buffer.BeginBatch();

    private void ApplyCommand(byte command)
    {
        // ref: §SSD1351.2 0x5C ("until another command is written"), §P2.X item 4: a command byte ends a RAM write and drops a half pixel
        // and the parameters not yet received (⚠️ derived: a command is applied only when all its parameters arrived).
        mode = FeedMode.Ignore;
        pixelByteCount = 0;

        // ref: §SSD1351.6: a locked chip responds only to 0xFD (whose parameter must then be 0x12); while 0xFD 0xB0 is in force the
        // restricted commands are ignored, and their parameters are stray data.
        if ((IsLocked && command != LockCommand) ||
            (!IsRestrictedAccessible && IsRestricted(command)))
        {
            return;
        }

        var count = ParameterCount(command);
        if (count > 0)
        {
            mode = FeedMode.Parameters;
            pendingCommand = command;
            pendingCount = count;
            collectedCount = 0;
            return;
        }

        if (count == 0)
        {
            ExecuteSimple(command);
            return;
        }

        TraceOnce(command);
    }

    private void AddParameter(byte value)
    {
        parameters[collectedCount++] = value;
        if (collectedCount < pendingCount)
        {
            return;
        }

        mode = FeedMode.Ignore;
        Execute(pendingCommand, parameters.AsSpan(0, pendingCount));
    }

    // ref: §SSD1351.2: the commands without parameters.
    private void ExecuteSimple(byte command)
    {
        switch (command)
        {
            case WriteRamCommand:
                // ref: §SSD1351.5: the pointer is set by 0x15 / 0x75 and kept by 0x5C (⚠️ derived; the library always sends all three).
                mode = FeedMode.WriteRam;
                break;
            case >= AllOffMode and <= InverseMode:
                if (displayMode != command)
                {
                    displayMode = command;
                    isFullRedrawNeeded = true;
                }

                break;
            case 0xAE or 0xAF:
                isDisplayOn = command == 0xAF;
                break;

            // 0x5D read RAM (not modelled), 0x9E / 0x9F scroll stop / start (out of scope, design spec), 0xB9 linear grey scale,
            // 0xAD, 0xB0, 0xD1, 0xE3 NOP.
        }
    }

    private void Execute(
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
                RowStart = arguments[0] & 0x7F;
                RowEnd = arguments[1] & 0x7F;
                RowPointer = RowStart;
                break;
            case 0xA0:
                SetRemap(arguments[0]);
                break;
            case 0xA1:
                // ref: §SSD1351.2: A[6:0], bit 7 don't care (the library sends 128 for its 128-row panel).
                isFullRedrawNeeded |= StartLine != (arguments[0] & 0x7F);
                StartLine = arguments[0] & 0x7F;
                break;
            case 0xA2:
                isFullRedrawNeeded |= DisplayOffset != (arguments[0] & 0x7F);
                DisplayOffset = arguments[0] & 0x7F;
                break;
            case 0xCA:
                // ref: §SSD1351.2: N 15..127; 0..14 are invalid and ignored (⚠️ derived).
                var ratio = arguments[0] & 0x7F;
                if (ratio >= MinMultiplexRatio && ratio != MultiplexRatio)
                {
                    MultiplexRatio = ratio;
                    isFullRedrawNeeded = true;
                }

                break;
            case 0xC1:
                ContrastA = arguments[0];
                ContrastB = arguments[1];
                ContrastC = arguments[2];
                break;
            case 0xC7:
                // ref: §SSD1351.2: A[3:0], the colours scaled by (n + 1) / 16. Kept, not drawn (ledger R15): the current-to-brightness
                // curve is unverified.
                MasterContrast = arguments[0] & 0x0F;
                break;
            case LockCommand:
                SetLock(arguments[0]);
                break;

            // 0x96 scroll setup (out of scope), 0xAB, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB8, 0xBB, 0xBE: consumed (§SSD1351.2).
        }
    }

    // ref: §SSD1351.6: 0x12 unlock, 0x16 lock, 0xB0 / 0xB1 restricted commands inaccessible / accessible; a locked chip takes only 0x12.
    // Other values are ignored (⚠️ unverified).
    private void SetLock(byte value)
    {
        if (IsLocked)
        {
            IsLocked = value != UnlockValue;
            return;
        }

        switch (value)
        {
            case LockValue:
                IsLocked = true;
                break;
            case RestrictValue:
                IsRestrictedAccessible = false;
                break;
            case AllowValue:
                IsRestrictedAccessible = true;
                break;
        }
    }

    // ref: §SSD1351.3: A[0] and A[7:6] act on the following writes; A[1], A[2] and A[4] map the stored RAM to the panel (⚠️ derived:
    // a change re-maps the picture already shown). A[5] (COM split) is kept but not drawn: its interleaving is ⚠️ unverified (§SSD1351.5),
    // and the modules are wired so that the library's split-enabled remaps show the RAM in order.
    private void SetRemap(byte value)
    {
        if (((Remap ^ value) & PanelMappingBits) != 0)
        {
            isFullRedrawNeeded = true;
        }

        Remap = value;
        var depth = value >> DepthShift;
        if (depth != Depth16BitBus)
        {
            isDepth262k = depth == Depth262k;
            return;
        }

        // ref: §SSD1351.4: format 2 needs the 16-bit bus; the setting is ignored and the previous depth kept.
        if (isDepthTraced)
        {
            return;
        }

        isDepthTraced = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1351Controller)}: colour depth 11 (0xA0 0x{value:X2}) needs the 16-bit bus; the previous colour depth is kept."));
    }

    // ref: §SSD1351.4 pixel formats on the 8-bit bus / SPI.
    private void AddPixelByte(byte value)
    {
        pixelBytes[pixelByteCount++] = value;
        if (isDepth262k)
        {
            if (pixelByteCount == 3)
            {
                pixelByteCount = 0;
                WritePixel(((pixelBytes[0] & 0x3F) << 12) | ((pixelBytes[1] & 0x3F) << 6) | (pixelBytes[2] & 0x3F));
            }

            return;
        }

        if (pixelByteCount < 2)
        {
            return;
        }

        pixelByteCount = 0;
        var word = (pixelBytes[0] << 8) | pixelBytes[1];
        WritePixel((Widen5(word >> 11) << 12) | (((word >> 5) & 0x3F) << 6) | Widen5(word & 0x1F));
    }

    // ref: §SSD1351.5: horizontal increment (A[0] = 0) column first, vertical (A[0] = 1) row first; both wrap in the window.
    // ⚠️ unverified: a start after its end (the pointer wraps after every pixel here).
    private void WritePixel(int word)
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

    private void SetRamWord(
        int column,
        int row,
        int word)
    {
        ref var cell = ref ram[(row * RamWidth) + column];
        if (cell == word)
        {
            return;
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
    }

    // ref: §SSD1351.1 reset state. The RAM is not touched.
    // ⚠️ design: the datasheet prints the offset reset as 60h, which its own figure and the library contradict (§SSD1351.1); 0 is used,
    // so a stream that never sets the offset (it is locked by default) shows the picture unshifted.
    private void ApplyPowerOnState()
    {
        mode = FeedMode.Ignore;
        pendingCount = 0;
        collectedCount = 0;
        pixelByteCount = 0;
        Remap = PowerOnRemap;
        isDepth262k = false;
        StartLine = 0;
        DisplayOffset = 0;
        MultiplexRatio = RamHeight - 1;
        ContrastA = 0x8A;
        ContrastB = 0x51;
        ContrastC = 0x8A;
        MasterContrast = 0x0F;
        displayMode = NormalMode;
        isDisplayOn = false;
        IsLocked = false;
        IsRestrictedAccessible = false;
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
                $"{nameof(Ssd1351Controller)}: command byte 0x{value:X2} is not a command of the chip and is ignored."));
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

        // ⚠️ design (R15 precedent): the contrast and master contrast curve is unverified, so the brightness stays full.
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

        var panelHeight = System.Math.Min(height, RamHeight);
        var row = rowBuffer.AsSpan(0, width);
        for (var y = 0; y < height; y++)
        {
            var ramRow = PanelRowToRamRow(y, panelHeight);
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

    // Panel column x shows RAM column x without the column remap A[1] (the upright module, §SSD1351.3), 127 - x with it.
    // ref: §SSD1351.4: with A[2] = 1 the first sub-pixel (C) is red and A blue; with A[2] = 0 the other way round.
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

        var isMirrored = (Remap & ColumnRemapBit) != 0;
        var isRedOnTop = ((Remap & ColorOrderBit) != 0) == isPanelBgr;
        var offset = ramRow * RamWidth;
        for (var x = 0; x < count; x++)
        {
            var word = ram[offset + (isMirrored ? RamWidth - 1 - x : x)];
            var top = Expand6(word >> 12);
            var green = Expand6((word >> 6) & 0x3F);
            var bottom = Expand6(word & 0x3F);
            row[x] = isRedOnTop
                ? (top << 16) | (green << 8) | bottom
                : (bottom << 16) | (green << 8) | top;
        }
    }

    // The RAM row panel row y shows, or -1 for a row that is not lit. ref: §SSD1351.5 (⚠️ derived): panel row y is wired to
    // COM (panelHeight - 1 - y); COM c shows RAM row (sigma + start line) mod 128 with, for A[4] = 0, j = (c + offset) mod 128 and
    // sigma = j, and for A[4] = 1, j = (c - offset) mod 128 and sigma = MUX - 1 - j; the COM is lit when j < MUX.
    private int PanelRowToRamRow(
        int y,
        int panelHeight)
    {
        if (y >= panelHeight)
        {
            return -1;
        }

        var com = panelHeight - 1 - y;
        var multiplex = MultiplexRatio + 1;
        var isReversed = (Remap & ComReverseBit) != 0;
        var line = (isReversed ? com - DisplayOffset : com + DisplayOffset) & (RamHeight - 1);
        if (line >= multiplex)
        {
            return -1;
        }

        var sigma = isReversed ? multiplex - 1 - line : line;
        return (sigma + StartLine) & (RamHeight - 1);
    }
}