namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Emulates the command set of an ST7735, ST7789 or ILI9341 colour TFT controller on a <see cref="ColorFrameBuffer"/>:
/// the command and data bytes a microcontroller sends to the chip are applied to the chip's own frame memory, and the window
/// of the frame memory the panel shows is drawn in the buffer, so a desktop app can mirror a real module byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// A command is sent with <see cref="WriteCommand"/> (D/C low); its parameters and the pixel data are sent with
/// <see cref="WriteData"/> (D/C high), also across calls, and the bytes of one pixel may be split across calls.
/// A new command ends the previous one: parameters not yet received are dropped (the command is not applied) and a
/// half-received pixel is discarded. Data after 0x2C (memory write) or 0x3C (write continue, not on the ST7735) goes into the
/// window set by 0x2A / 0x2B, ordered and wrapped as the MADCTL bits MV, MX and MY say. Pixel formats (0x3A): 16-bit RGB565
/// (2 bytes, high byte first), 18-bit (3 bytes R, G, B, the upper 6 bits of each) and 12-bit (3 bytes per 2 pixels; ST7735 and ST7789).
/// The vendor tuning commands (gamma, power, frame rate) and any other command byte consume their data and are ignored; a byte
/// that is not a known command of the chip is written to <see cref="Trace"/> the first time each distinct value is seen by this instance.
/// No byte value throws.
/// </para>
/// <para>
/// The chip RAM holds the colours as written. The panel shows the RAM window of the size of the buffer that starts at
/// (<see cref="ColumnOffset"/>, <see cref="RowOffset"/>) in RAM addresses (MADCTL 0), wired as the common modules are: the picture is
/// upright and not mirrored after the MADCTL of the libraries' rotation 0 (Adafruit setRotation(0): MX | MY on the ST7735 and ST7789,
/// MX on the ILI9341). So an ST7735 or ST7789 panel shows the window rotated by 180 degrees from RAM order (the last RAM row of the
/// window at the top, the last column at the left), and an ILI9341 panel shows it mirrored left to right. The libraries' rotations reach the
/// panel through MADCTL, exactly as on the device. RAM outside the window is kept but not shown.
/// The colour shown has red and blue swapped when the MADCTL BGR bit differs from <see cref="IsPanelBgr"/>, and is inverted when the
/// chip's inversion (0x20 / 0x21) differs from <see cref="IsPanelInverted"/>.
/// Vertical scrolling (0x33 / 0x37) changes which RAM rows the panel shows; the RAM is unchanged.
/// Inversion (0x20 / 0x21), sleep (0x10 / 0x11), display on / off (0x28 / 0x29) and the brightness 0x51 (ST7789 and ILI9341)
/// set the state of the buffer. When the buffer is resized the RAM is kept and drawn again for the new size.
/// </para>
/// <para>
/// A new instance and <see cref="Reset"/> (the reset pin) apply the chip's power-on state: sleep in, display off, inversion off,
/// MADCTL 0x00, 18-bit pixels, the full-RAM window and no scrolling. The software reset 0x01 does the same but keeps MADCTL and the
/// pixel format on the ST7789 and ILI9341 (the ST7735 clears MADCTL). A new instance starts with black RAM; a reset leaves the RAM as it is.
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
public sealed class TftController : IGraphicDisplayController
{
    private const int St7735RamWidth = 132;
    private const int St7735RamHeight = 162;
    private const int LargeRamWidth = 240;
    private const int LargeRamHeight = 320;
    private const int MaxParameters = 6;
    private const byte MirrorYBit = 0x80;
    private const byte MirrorXBit = 0x40;
    private const byte ExchangeBit = 0x20;
    private const byte BgrBit = 0x08;
    private const int PixelFormatMask = 0x07;
    private const int Format12Bit = 0x03;
    private const int Format16Bit = 0x05;
    private const int Format18Bit = 0x06;
    private const int RedMask = 0xFF0000;
    private const int GreenMask = 0x00FF00;
    private const int BlueMask = 0x0000FF;

    private readonly Lock sync = new();
    private readonly ColorFrameBuffer buffer;
    private readonly byte[] parameters = new byte[MaxParameters];
    private readonly byte[] pixelBytes = new byte[3];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private readonly bool isPanelMirroredX;
    private readonly bool isPanelMirroredY;
    private int[] ram;
    private int[] rowBuffer = [];
    private int ramWidth;
    private int ramHeight;
    private int columnOffset;
    private int rowOffset;
    private bool isPanelBgr;
    private bool isPanelInverted;
    private FeedMode mode;
    private byte pendingCommand;
    private int pendingCount;
    private int collectedCount;
    private int pixelByteCount;
    private int pixelFormat;
    private int columnStart;
    private int columnEnd;
    private int rowStart;
    private int rowEnd;
    private bool isDisplayOn;
    private bool isInverted;
    private bool isSleeping;
    private byte brightness;
    private int topFixedArea;
    private int scrollArea;
    private int bottomFixedArea;
    private int scrollStart;
    private bool isColmodTraced;
    private bool isFullRedrawNeeded;
    private int dirtyFirst = int.MaxValue;
    private int dirtyLast = -1;
    private int mappedWidth;
    private int mappedHeight;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TftController"/> class in the chip's power-on state, with black RAM.
    /// </summary>
    /// <param name="buffer">The buffer that shows the panel window of the chip's RAM.</param>
    /// <param name="chip">The controller chip to emulate.</param>
    /// <param name="columnOffset">The first RAM column of the window the panel shows; see <see cref="ColumnOffset"/>.</param>
    /// <param name="rowOffset">The first RAM row of the window the panel shows; see <see cref="RowOffset"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">An offset is negative or outside the chip's RAM.</exception>
    public TftController(
        ColorFrameBuffer buffer,
        TftChip chip,
        int columnOffset = 0,
        int rowOffset = 0)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        this.buffer = buffer;
        Chip = chip;
        (ramWidth, ramHeight) = MaxRamSize(chip);
        ValidateOffset(columnOffset, ramWidth);
        ValidateOffset(rowOffset, ramHeight);
        this.columnOffset = columnOffset;
        this.rowOffset = rowOffset;

        // ref: §P2.X item 1, §ST7735.3, §ST7789.3, §ILI9341.3: the colour order that shows the libraries' data right
        // (ST7735 green tabs and ILI9341: BGR; ST7789: RGB). ⚠️ derived from library behaviour.
        isPanelBgr = chip != TftChip.St7789;

        // ref: §ST7789.2 (correction), §LIB.AST7789.1: the IPS panels of the ST7789 modules are inverted, so the library's INVON shows true colours.
        isPanelInverted = chip == TftChip.St7789;

        // ref: §P2.X item 8 (library upright = Adafruit rotation 0), §ST7735.4, §ST7789.4, §LIB.AILI9341.2 (⚠️ derived): rotation 0 writes
        // logical (0, 0) at the window's last RAM column and last RAM row (MX | MY) on the ST7735 / ST7789, and at the last column, first row
        // (MX) on the ILI9341; the panel shows that address at its top-left.
        isPanelMirroredX = true;
        isPanelMirroredY = chip != TftChip.Ili9341;
        ram = new int[ramWidth * ramHeight];

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
        MemoryWrite,
    }

    /// <summary>
    /// Gets the emulated controller chip.
    /// </summary>
    public TftChip Chip { get; }

    /// <summary>
    /// Gets the number of RAM columns: the extent the MADCTL column mirror (MX) uses. The default is 132 for an ST7735 and 240 otherwise;
    /// see <see cref="SetRamSize"/>.
    /// </summary>
    public int RamWidth
    {
        get
        {
            lock (sync)
            {
                return ramWidth;
            }
        }
    }

    /// <summary>
    /// Gets the number of RAM rows: the extent the MADCTL row mirror (MY) and vertical scrolling use. The default is 162 for an ST7735
    /// and 320 otherwise; see <see cref="SetRamSize"/>.
    /// </summary>
    public int RamHeight
    {
        get
        {
            lock (sync)
            {
                return ramHeight;
            }
        }
    }

    /// <summary>
    /// Gets or sets the first RAM column of the window the panel shows, in RAM addresses (MADCTL 0; the panel's rightmost column, see the remarks of <see cref="TftController"/>).
    /// Common modules: ST7735 1.8 inch 128 x 160 and 1.44 inch 128 x 128 green tab 2, ST7735 0.96 inch 80 x 160 24 (the plug-in variant 26),
    /// ST7789 1.14 inch 135 x 240 52, others 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or not less than <see cref="RamWidth"/>.</exception>
    public int ColumnOffset
    {
        get
        {
            lock (sync)
            {
                return columnOffset;
            }
        }

        set
        {
            using var batch = BeginBatchOutsideLock();

            lock (sync)
            {
                ValidateOffset(value, ramWidth);

                if (isDisposed)
                {
                    return;
                }

                if (columnOffset == value)
                {
                    return;
                }

                columnOffset = value;
                isFullRedrawNeeded = true;
                Flush();
            }
        }
    }

    /// <summary>
    /// Gets or sets the first RAM row of the window the panel shows, in RAM addresses (MADCTL 0; the panel's bottom row on an ST7735 or ST7789,
    /// its top row on an ILI9341, see the remarks of <see cref="TftController"/>).
    /// Common modules: ST7735 1.8 inch 128 x 160 and 1.44 inch 128 x 128 green tab 1, ST7735 0.96 inch 80 x 160 0 (the plug-in variant 1),
    /// ST7789 1.14 inch 135 x 240 40, others 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or not less than <see cref="RamHeight"/>.</exception>
    public int RowOffset
    {
        get
        {
            lock (sync)
            {
                return rowOffset;
            }
        }

        set
        {
            using var batch = BeginBatchOutsideLock();

            lock (sync)
            {
                ValidateOffset(value, ramHeight);

                if (isDisposed)
                {
                    return;
                }

                if (rowOffset == value)
                {
                    return;
                }

                rowOffset = value;
                isFullRedrawNeeded = true;
                Flush();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the panel's colour filter is in BGR order: the colour shown has red and blue swapped
    /// when the MADCTL BGR bit differs from this value. The default is <see langword="true"/> for an ST7735 (green tab modules) and
    /// an ILI9341, <see langword="false"/> for an ST7789; set it to <see langword="false"/> for an ST7735 black tab or 0.96 inch 80 x 160 module.
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

    /// <summary>
    /// Gets or sets a value indicating whether the panel shows the colours inverted by nature: the colour shown is inverted when the chip's
    /// inversion (0x20 off / 0x21 on) differs from this value. The default is <see langword="true"/> for an ST7789, whose modules have
    /// IPS panels that need inversion on (the 0x21 that Adafruit_ST7789 sends in its init) for true colours, and
    /// <see langword="false"/> for an ST7735 and an ILI9341, whose TN panels show true colours with inversion off.
    /// </summary>
    public bool IsPanelInverted
    {
        get
        {
            lock (sync)
            {
                return isPanelInverted;
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

                if (isPanelInverted == value)
                {
                    return;
                }

                isPanelInverted = value;
                Flush();
            }
        }
    }

    internal byte Madctl { get; private set; }

    internal int ColumnPointer { get; private set; }

    internal int RowPointer { get; private set; }

    /// <summary>
    /// Sets the size of the chip RAM the module uses: the extents the MADCTL mirrors (MX, MY) and vertical scrolling use.
    /// The ST7735 has a 132 x 162 frame memory, but a module's wiring selects 132 x 162 (1.8 inch 128 x 160 green tab and the
    /// 0.96 inch plug-in variant), 132 x 132 (1.44 inch 128 x 128 green tab) or 128 x 160 (0.96 inch 80 x 160 mini).
    /// The ST7789 and ILI9341 use 240 x 320.
    /// </summary>
    /// <remarks>
    /// The RAM that fits in both sizes is kept. The column and row window and the vertical scroll area are set to their defaults for the new size.
    /// <see cref="ColumnOffset"/> and <see cref="RowOffset"/> are kept when they fit in the new size and are clamped to the last column / row otherwise;
    /// a window that reaches past the RAM shows black there. A command whose parameters have not all arrived and a memory write in progress end
    /// (a half pixel is dropped): data is ignored until the next command, so pixel data continues only after a new 0x2C.
    /// </remarks>
    /// <param name="width">The number of RAM columns, 1 to the chip's 132 (ST7735) or 240.</param>
    /// <param name="height">The number of RAM rows, 1 to the chip's 162 (ST7735) or 320.</param>
    /// <exception cref="ArgumentOutOfRangeException">The size is out of range for the chip.</exception>
    public void SetRamSize(
        int width,
        int height)
    {
        var (maxWidth, maxHeight) = MaxRamSize(Chip);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, maxWidth);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, maxHeight);

        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            if (width == ramWidth && height == ramHeight)
            {
                return;
            }

            var resized = new int[width * height];
            var keptWidth = System.Math.Min(width, ramWidth);
            var keptHeight = System.Math.Min(height, ramHeight);
            for (var row = 0; row < keptHeight; row++)
            {
                Array.Copy(ram, row * ramWidth, resized, row * width, keptWidth);
            }

            ram = resized;
            ramWidth = width;
            ramHeight = height;

            // The offsets stay inside the RAM, as the ColumnOffset / RowOffset setters require.
            columnOffset = System.Math.Min(columnOffset, width - 1);
            rowOffset = System.Math.Min(rowOffset, height - 1);

            // A command or memory write in progress ends, as on a reset: data is ignored until the next command.
            mode = FeedMode.Ignore;
            pixelByteCount = 0;
            collectedCount = 0;
            ApplyWindowAndScrollDefaults();
            isFullRedrawNeeded = true;
            Flush();
        }
    }

    /// <inheritdoc />
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
    /// <remarks>The bytes are the parameters of the last command, or pixels after 0x2C / 0x3C; any other data is ignored.</remarks>
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

            switch (mode)
            {
                case FeedMode.MemoryWrite:
                    foreach (var value in data)
                    {
                        AddPixelByte(value);
                    }

                    break;
                case FeedMode.Parameters:
                    foreach (var value in data)
                    {
                        if (mode != FeedMode.Parameters)
                        {
                            // Data after the last parameter is ignored (§TFT.2).
                            break;
                        }

                        AddParameter(value);
                    }

                    break;
                default:
                    break;
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The hardware reset: MADCTL and the pixel format return to their power-on values as well (unlike the software reset 0x01 on an
    /// ST7789 or ILI9341). The RAM is left as it is (its content after a reset is undefined on the chip).
    /// </remarks>
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

    // ref: §ST7735.1, §ST7789.1, §ILI9341.1.
    private static (int Width, int Height) MaxRamSize(TftChip chip)
        => chip == TftChip.St7735
            ? (St7735RamWidth, St7735RamHeight)
            : (LargeRamWidth, LargeRamHeight);

    private static void ValidateOffset(
        int value,
        int extent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, extent);
    }

    private static int Word(
        ReadOnlySpan<byte> arguments,
        int index)
        => (arguments[index] << 8) | arguments[index + 1];

    // ref: §TFT.6: 5 -> 8 and 6 -> 8 bits by bit replication (⚠️ unverified LUT; design choice).
    private static int Expand565(int value)
    {
        var r5 = (value >> 11) & 0x1F;
        var g6 = (value >> 5) & 0x3F;
        var b5 = value & 0x1F;
        return (((r5 << 3) | (r5 >> 2)) << 16) | (((g6 << 2) | (g6 >> 4)) << 8) | (b5 << 3) | (b5 >> 2);
    }

    // ref: §TFT.6 18-bit: the upper 6 bits of the byte carry the colour.
    private static int Expand6(byte value)
    {
        var v6 = value >> 2;
        return (v6 << 2) | (v6 >> 4);
    }

    private static int Expand4(int nibble)
        => (nibble << 4) | nibble;

    private static int SwapRedBlue(int rgb)
        => ((rgb & BlueMask) << 16) | (rgb & GreenMask) | ((rgb & RedMask) >> 16);

    // ref: §TFT.2, §ST7735.2, §ST7789.2, §LIB.AILI9341.1: commands with no visual model whose data is consumed silently: reads, partial / idle
    // mode, tearing line, gamma, and the vendor tuning commands the Adafruit init streams send. ST7789: the content adaptive brightness
    // commands 0x53 / 0x55 / 0x5E and the system function (command 2) set of DS-7789 §9.2 (RAMCTRL 0xB0, PORCTRL 0xB2, GCTRL 0xB7,
    // VCOMS 0xBB, LCMCTRL 0xC0, VDVVRHEN 0xC2, VRHS 0xC3, VDVS 0xC4, FRCTRL2 0xC6, PWCTRL1 0xD0 and the rest) that TFT_eSPI's ST7789 init
    // sends, plus 0xB6, which TFT_eSPI sends as well although DS-7789 does not list it (⚠️ derived from the library, not read byte for byte).
    private static bool IsConsumedCommand(
        TftChip chip,
        byte command)
    {
        switch (command)
        {
            case 0x00 or 0x04 or (>= 0x09 and <= 0x0F) or 0x12 or 0x13 or 0x26 or 0x2E or 0x30 or 0x34 or 0x35 or 0x38 or 0x39 or 0x3E or 0x45:
            case 0xDA or 0xDB or 0xDC or 0xE0 or 0xE1:
                return true;
        }

        return chip switch
        {
            TftChip.St7735 => command is 0xB1 or 0xB2 or 0xB3 or 0xB4 or 0xB6 or (>= 0xC0 and <= 0xC5) or 0xFC,
            TftChip.Ili9341 => command is 0xEF or 0xCF or 0xED or 0xE8 or 0xCB or 0xF7 or 0xEA or 0xC0 or 0xC1 or 0xC5 or 0xC7 or 0xB1 or 0xB6 or 0xF2 or 0x2D,
            TftChip.St7789 => command is 0x53 or 0x55 or 0x5E or (>= 0xB0 and <= 0xB3) or 0xB5 or 0xB6 or 0xB7 or 0xB8 or 0xBA or 0xBB
                or (>= 0xC0 and <= 0xC8) or 0xCA or 0xCC or 0xD0 or 0xD2 or 0xDF or 0xE2 or 0xE3 or 0xE4 or 0xE7 or 0xE8 or 0xE9 or 0xEC
                or 0xFA or 0xFC or 0xFE,
            _ => false,
        };
    }

    // Opened before the lock is taken and disposed after it is released: the buffer raises Changed when the outermost batch ends,
    // so it is never raised while the controller's lock is held, and a Changed subscriber that waits for another thread feeding
    // this controller cannot deadlock.
    private IDisposable BeginBatchOutsideLock()
        => buffer.BeginBatch();

    // ref: §TFT.2 parameter counts; Unknown (-1) for a byte that is not a modelled command.
    private int ParameterCount(byte command)
        => command switch
        {
            0x2A or 0x2B => 4,
            0x33 => 6,
            0x36 or 0x3A => 1,
            0x37 => 2,
            0x51 when Chip != TftChip.St7735 => 1,
            _ => -1,
        };

    private void ApplyCommand(byte command)
    {
        // ref: §TFT.2: a new command byte ends the previous command; parameters not yet received and a half pixel are dropped
        // (⚠️ derived: the chip keeps the parameter bytes received before the break; this emulator applies a command only when complete).
        mode = FeedMode.Ignore;
        pixelByteCount = 0;

        switch (command)
        {
            case 0x01:
                ApplySoftwareReset();
                return;
            case 0x10 or 0x11:
                isSleeping = command == 0x10;
                return;
            case 0x20 or 0x21:
                isInverted = command == 0x21;
                return;
            case 0x28 or 0x29:
                isDisplayOn = command == 0x29;
                return;
            case 0x2C:
                ColumnPointer = columnStart;
                RowPointer = rowStart;
                mode = FeedMode.MemoryWrite;
                return;
            case 0x3C when Chip != TftChip.St7735:
                // ref: §TFT.1 RAMWRC: continues from the pixel after the last one written.
                mode = FeedMode.MemoryWrite;
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

        if (!IsConsumedCommand(Chip, command))
        {
            TraceOnce(command);
        }
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

    private void Execute(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x2A:
                // ref: §TFT.1: CASET / RASET set the window; they do not move the pointer.
                columnStart = Word(arguments, 0);
                columnEnd = Word(arguments, 2);
                break;
            case 0x2B:
                rowStart = Word(arguments, 0);
                rowEnd = Word(arguments, 2);
                break;
            case 0x33:
                topFixedArea = Word(arguments, 0);
                scrollArea = Word(arguments, 2);
                bottomFixedArea = Word(arguments, 4);
                isFullRedrawNeeded = true;
                break;
            case 0x37:
                scrollStart = Word(arguments, 0);
                isFullRedrawNeeded = true;
                break;
            case 0x36:
                // ref: §TFT.1: MV / MX / MY act on the following writes only; BGR is a live mapping of the shown colour.
                // ML and MH (refresh order) are ignored (⚠️ design decision, §TFT.1).
                if (((Madctl ^ arguments[0]) & BgrBit) != 0)
                {
                    isFullRedrawNeeded = true;
                }

                Madctl = arguments[0];
                break;
            case 0x3A:
                SetPixelFormat(arguments[0]);
                break;
            case 0x51:
                // ref: §TFT.7.
                brightness = arguments[0];
                break;
        }
    }

    // ref: §TFT.6: COLMOD & 0x07; 12-bit is reserved on the ILI9341. Any other value keeps the previous format (⚠️ derived).
    private void SetPixelFormat(byte value)
    {
        var format = value & PixelFormatMask;
        if (format is Format16Bit or Format18Bit ||
            (format == Format12Bit && Chip != TftChip.Ili9341))
        {
            pixelFormat = format;
            return;
        }

        if (isColmodTraced)
        {
            return;
        }

        isColmodTraced = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(TftController)} ({Chip}): pixel format 0x{value:X2} (0x3A) is not supported and is ignored."));
    }

    // ref: §TFT.6 wire layouts.
    private void AddPixelByte(byte value)
    {
        pixelBytes[pixelByteCount++] = value;
        switch (pixelFormat)
        {
            case Format16Bit:
                if (pixelByteCount == 2)
                {
                    pixelByteCount = 0;
                    WritePixel(Expand565((pixelBytes[0] << 8) | pixelBytes[1]));
                }

                break;
            case Format18Bit:
                if (pixelByteCount == 3)
                {
                    pixelByteCount = 0;
                    WritePixel((Expand6(pixelBytes[0]) << 16) | (Expand6(pixelBytes[1]) << 8) | Expand6(pixelBytes[2]));
                }

                break;
            default:
                // 12-bit: R1G1, B1R2, G2B2 (§TFT.6, ⚠️ read from the figure).
                if (pixelByteCount == 2)
                {
                    WritePixel((Expand4(pixelBytes[0] >> 4) << 16) | (Expand4(pixelBytes[0] & 0x0F) << 8) | Expand4(pixelBytes[1] >> 4));
                }
                else if (pixelByteCount == 3)
                {
                    pixelByteCount = 0;
                    WritePixel((Expand4(pixelBytes[1] & 0x0F) << 16) | (Expand4(pixelBytes[2] >> 4) << 8) | Expand4(pixelBytes[2] & 0x0F));
                }

                break;
        }
    }

    // ref: §TFT.1 logical to physical (MV / MX / MY with the mirror extents RamWidth - 1 / RamHeight - 1) and the counter table.
    private void WritePixel(int rgb)
    {
        var isExchanged = (Madctl & ExchangeBit) != 0;
        var columnSource = isExchanged ? RowPointer : ColumnPointer;
        var rowSource = isExchanged ? ColumnPointer : RowPointer;
        var column = (Madctl & MirrorXBit) != 0 ? ramWidth - 1 - columnSource : columnSource;
        var row = (Madctl & MirrorYBit) != 0 ? ramHeight - 1 - rowSource : rowSource;

        // Out of range addresses are ignored (§TFT.1, DS-7789 §9.1.20).
        if (column >= 0 && column < ramWidth && row >= 0 && row < ramHeight)
        {
            ref var cell = ref ram[(row * ramWidth) + column];
            if (cell != rgb)
            {
                cell = rgb;
                MarkRowDirty(row);
            }
        }

        if (ColumnPointer < columnEnd)
        {
            ColumnPointer++;
            return;
        }

        ColumnPointer = columnStart;
        RowPointer = RowPointer < rowEnd
            ? RowPointer + 1
            : rowStart;
    }

    private void MarkRowDirty(int row)
    {
        if (row < dirtyFirst)
        {
            dirtyFirst = row;
        }

        if (row > dirtyLast)
        {
            dirtyLast = row;
        }
    }

    // ref: §TFT.5 power-on column: MADCTL 0x00, COLMOD 18-bit, then the reset defaults.
    private void ApplyPowerOnState()
    {
        Madctl = 0x00;
        pixelFormat = Format18Bit;
        ApplyResetDefaults();
    }

    // ref: §TFT.5: SWRESET leaves MADCTL and COLMOD unchanged on the ST7789VW and ILI9341, sets MADCTL to 0 on the ST7735S.
    // ST7735S COLMOD after SWRESET is the "NV value" (⚠️ unverified): kept.
    private void ApplySoftwareReset()
    {
        if (Chip == TftChip.St7735)
        {
            Madctl = 0x00;
        }

        ApplyResetDefaults();
    }

    // ref: §TFT.5: sleep in, display off, inversion off, normal mode, scroll and window defaults. The RAM is not touched.
    private void ApplyResetDefaults()
    {
        mode = FeedMode.Ignore;
        pixelByteCount = 0;
        collectedCount = 0;
        isSleeping = true;
        isDisplayOn = false;
        isInverted = false;

        // ⚠️ design: the WRDISBV register resets to 0x00 (§TFT.5), but most modules wire the backlight to a pin instead (§TFT.7)
        // and the libraries never send 0x51, so the emulator shows full brightness until 0x51 is received.
        brightness = byte.MaxValue;
        ApplyWindowAndScrollDefaults();
        isFullRedrawNeeded = true;
    }

    // ref: §TFT.5 CASET / RASET defaults (the RAM extents, exchanged with MV) and §TFT.4 scroll defaults (VSA = the RAM rows).
    private void ApplyWindowAndScrollDefaults()
    {
        var isExchanged = (Madctl & ExchangeBit) != 0;
        columnStart = 0;
        rowStart = 0;
        columnEnd = (isExchanged ? ramHeight : ramWidth) - 1;
        rowEnd = (isExchanged ? ramWidth : ramHeight) - 1;
        ColumnPointer = 0;
        RowPointer = 0;
        topFixedArea = 0;
        scrollArea = ramHeight;
        bottomFixedArea = 0;
        scrollStart = 0;
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
                $"{nameof(TftController)} ({Chip}): command byte 0x{value:X2} is not a command of the chip; it and its data are ignored."));
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
        buffer.IsInverted = isInverted != isPanelInverted;
        buffer.IsSleeping = isSleeping;
        buffer.Brightness = brightness;

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
        var isSwapped = ((Madctl & BgrBit) != 0) != isPanelBgr;
        for (var y = 0; y < height; y++)
        {
            var ramRow = PanelRowToRamRow(y, height);
            if (!isFullRedrawNeeded && (ramRow < dirtyFirst || ramRow > dirtyLast))
            {
                continue;
            }

            FillPanelRow(row, ramRow, isSwapped);

            // A resize between reading the size and this write is clipped by the buffer; its Changed draws again (OnBufferChanged).
            buffer.WriteRow(y, 0, row);
        }

        mappedWidth = width;
        mappedHeight = height;
        isFullRedrawNeeded = false;
        dirtyFirst = int.MaxValue;
        dirtyLast = -1;
    }

    // The RAM -> panel column mapping: window column k (RAM column ColumnOffset + k) is panel column k, or width - 1 - k on a panel wired
    // mirrored in X (every supported chip, see the constructor); black outside the RAM.
    private void FillPanelRow(
        Span<int> row,
        int ramRow,
        bool isSwapped)
    {
        var count = System.Math.Clamp(ramWidth - columnOffset, 0, row.Length);
        if (ramRow < 0 || count == 0)
        {
            row.Clear();
            return;
        }

        var source = ram.AsSpan((ramRow * ramWidth) + columnOffset, count);
        if (isSwapped)
        {
            for (var x = 0; x < count; x++)
            {
                row[x] = SwapRedBlue(source[x]);
            }
        }
        else
        {
            source.CopyTo(row);
        }

        row[count..].Clear();
        if (isPanelMirroredX)
        {
            row.Reverse();
        }
    }

    // The RAM -> panel row mapping: panel row y is display line RowOffset + y, or RowOffset + height - 1 - y on a panel wired mirrored in Y
    // (ST7735, ST7789; see the constructor), shown through the vertical
    // scroll (§TFT.4, ⚠️ derived, ML = 0): a fixed-area line shows its own RAM row; a scroll-area line d shows
    // TFA + ((VSP - TFA + (d - TFA)) mod VSA). Scrolling is off while TFA + VSA + BFA is not the RAM height ("undefined" on the chip).
    // Returns -1 for a line outside the RAM.
    private int PanelRowToRamRow(
        int y,
        int height)
    {
        var line = rowOffset + (isPanelMirroredY ? height - 1 - y : y);
        if (line >= ramHeight)
        {
            return -1;
        }

        if (scrollArea <= 0 ||
            topFixedArea + scrollArea + bottomFixedArea != ramHeight ||
            line < topFixedArea ||
            line >= ramHeight - bottomFixedArea)
        {
            return line;
        }

        var shift = (scrollStart - topFixedArea + (line - topFixedArea)) % scrollArea;
        if (shift < 0)
        {
            shift += scrollArea;
        }

        return topFixedArea + shift;
    }
}