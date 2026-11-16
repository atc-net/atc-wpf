namespace Atc.Wpf.Displays;

/// <summary>
/// Emulates the command set of an SSD1306, SSD1309 or SH1106 monochrome OLED controller on a <see cref="MonochromeFrameBuffer"/>:
/// the command and data bytes a microcontroller sends to the chip are applied to the chip's own display RAM, and the part
/// of the RAM the panel shows is drawn in the buffer, so a desktop app can mirror a real module byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// A new instance and <see cref="Reset"/> apply the chip's power-on state: display off, page addressing, contrast 0x7F (0x80 on an SH1106),
/// normal (not inverted) mode, display start line 0, display offset 0, multiplex ratio 63 and no segment remap or COM scan reversal.
/// A new instance starts with all RAM bits cleared, so the buffer is cleared; <see cref="Reset"/> leaves the RAM as it is.
/// </para>
/// <para>
/// The buffer shows the panel of a typical module: the picture is upright after the segment remap 0xA1 and COM scan 0xC8 that
/// Adafruit_SSD1306 and U8g2 send, and the power-on 0xA0 and 0xC0 show it rotated by 180 degrees. The panel rows are wired as the
/// common modules are: a buffer taller than 32 rows is an alternative-COM panel (upright with 0xDA 0x12), a buffer of 32 rows or
/// fewer is a sequential-COM panel (upright with 0xDA 0x02). The display start line, display offset, multiplex ratio, COM scan
/// direction and COM pins configuration change the picture at once, as on the chip; the segment remap acts on the data written after it.
/// When the buffer is resized the RAM is kept and drawn again for the new size.
/// </para>
/// <para>
/// A command with parameters takes them from the following command bytes, also across calls, as on the chip.
/// A command byte the chosen chip does not have is ignored, takes no parameters and is written to <see cref="Trace"/> the first
/// time each distinct byte value is seen by this instance. No byte value throws.
/// </para>
/// <para>
/// Continuous scrolling (0x26, 0x27, 0x29, 0x2A, then 0x2F) moves the picture one column per step on a timer from the
/// <see cref="TimeProvider"/> given to the constructor, at the step time the chip's interval code and frame period give.
/// 0x2E stops it and leaves the picture where it is. Only the horizontal part is emulated: the vertical offset of 0x29/0x2A and the
/// vertical scroll area 0xA3 are recorded but move nothing (the chip's vertical movement is not confirmed by the datasheet). Dispose the controller to stop the timer and detach it from the buffer; a disposed controller ignores everything sent to it.
/// </para>
/// <para>
/// Each call that changes something visible ends in one update of the buffer, so the buffer raises
/// <see cref="MonochromeFrameBuffer.Changed"/> at most once per call, and not at all when nothing visible changed.
/// </para>
/// <para>
/// Feed the controller from one thread at a time: bytes fed from several threads at once arrive in no defined order.
/// The calls and the scroll timer are serialized by an internal lock, so a scroll step never runs in the middle of a call;
/// the buffer raises <see cref="MonochromeFrameBuffer.Changed"/> after that lock is released, on the thread that made the change
/// (a scroll step: a timer thread).
/// The buffer it writes to is thread-safe.
/// </para>
/// </remarks>
public sealed class Ssd1306Controller : IGraphicDisplayController
{
    private const int Unknown = -1;
    private const int PageCount = 8;
    private const int PageHeight = 8;
    private const int LastPage = PageCount - 1;
    private const int ComCount = 64;
    private const int HalfComCount = ComCount / 2;
    private const int MaxSequentialPanelHeight = 32;
    private const int Sh1106RamWidth = 132;
    private const int Ssd130XRamWidth = 128;
    private const int Sh1106DefaultColumnOffset = 2;
    private const int MaxParameters = 7;
    private const int NibbleBits = 4;
    private const int NibbleMask = 0x0F;
    private const int ColumnMask = 0x7F;
    private const int PageMask = 0x07;
    private const int SixBitMask = 0x3F;
    private const int AddressingModeMask = 0x03;
    private const int AlternativeComBit = 0x10;
    private const int LeftRightRemapBit = 0x20;
    private const int MinSsd130XMultiplexRatio = 15;
    private const byte DefaultContrast = 0x7F;
    private const byte Sh1106DefaultContrast = 0x80;
    private const byte DefaultMultiplexRatio = 63;
    private const int DefaultScrollRows = 64;
    private const byte DefaultComPins = 0x12;
    private const byte DefaultPreCharge = 0x22;
    private const byte Ssd1306DefaultClock = 0x80;
    private const byte Ssd1309DefaultClock = 0x70;
    private const byte Sh1106DefaultClock = 0x50;
    private const long Ssd1306OscillatorHz = 370_000;
    private const long Ssd1309OscillatorHz = 450_000;
    private const int Ssd1306BankPulseWidth = 50;
    private const int Ssd1309BankPulseWidth = 65;

    // ref: §SSD1306.8 (Table 9-1) and §SSD1309.3 (Table 9-2): frames per scroll step for the interval code C[2:0].
    // ⚠️ unverified: 110b = 25 frames on the SSD1306 is taken as printed; the two chips' tables differ in 100b-111b.
    private static readonly int[] Ssd1306ScrollFrames = [5, 64, 128, 256, 3, 4, 25, 2];
    private static readonly int[] Ssd1309ScrollFrames = [5, 64, 128, 256, 2, 3, 4, 1];

    private readonly Lock sync = new();
    private readonly MonochromeFrameBuffer buffer;
    private readonly byte[] ram;
    private readonly byte[] parameters = new byte[MaxParameters];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private byte[] visible = [];
    private int mappedWidth;
    private int mappedHeight;
    private int columnOffset;
    private bool hasPendingCommand;
    private byte pendingCommand;
    private int pendingCount;
    private int collectedCount;
    private AddressingMode addressingMode;
    private int columnStart;
    private int columnEnd;
    private int pageStart;
    private int pageEnd;
    private int columnLowNibble;
    private int columnHighNibble;
    private int? readModifyWriteColumn;
    private bool isDisplayOn;
    private bool isInverted;
    private bool isEntireDisplayOn;
    private byte contrast;
    private bool isDirty;
    private ITimer? scrollTimer;
    private int scrollGeneration;
    private bool isVerticalScrollTraced;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ssd1306Controller"/> class in the chip's power-on state.
    /// </summary>
    /// <param name="buffer">The buffer that shows the visible part of the chip's display RAM.</param>
    /// <param name="chip">The controller chip to emulate.</param>
    /// <param name="timeProvider">The clock that drives continuous scrolling; <see langword="null"/> uses the system clock.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    public Ssd1306Controller(
        MonochromeFrameBuffer buffer,
        OledChip chip = OledChip.Ssd1306,
        TimeProvider? timeProvider = null)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        this.buffer = buffer;
        Chip = chip;
        TimeProvider = timeProvider ?? TimeProvider.System;
        RamWidth = chip == OledChip.Sh1106
            ? Sh1106RamWidth
            : Ssd130XRamWidth;
        columnOffset = chip == OledChip.Sh1106
            ? Sh1106DefaultColumnOffset
            : 0;
        ram = new byte[RamWidth * PageCount];

        ResetState();
        Flush();

        buffer.Changed += OnBufferChanged;
    }

    private enum AddressingMode
    {
        Horizontal = 0,
        Vertical = 1,
        Page = 2,
    }

    /// <summary>
    /// Gets the emulated controller chip.
    /// </summary>
    public OledChip Chip { get; }

    /// <summary>
    /// Gets the number of columns of the chip's display RAM: 132 for an SH1106, otherwise 128.
    /// </summary>
    public int RamWidth { get; }

    /// <summary>
    /// Gets or sets the first RAM column shown in the leftmost column of the buffer under the segment remap 0xA1 the libraries send:
    /// 2 for an SH1106 by default (a 128-wide SH1106 module shows RAM columns 2 to 129), otherwise 0.
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
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, RamWidth);

            using var batch = BeginBatchOutsideLock();

            lock (sync)
            {
                if (isDisposed)
                {
                    return;
                }

                if (columnOffset == value)
                {
                    return;
                }

                columnOffset = value;
                isDirty = true;
                Flush();
            }
        }
    }

    internal TimeProvider TimeProvider { get; }

    internal int ColumnPointer { get; private set; }

    internal int PagePointer { get; private set; }

    internal int StartLine { get; private set; }

    internal int DisplayOffset { get; private set; }

    internal int MultiplexRatio { get; private set; }

    // ref: §SSD1306.6 / ruling R3: A0/A1 applies to subsequent data writes (WriteData mirrors the column), not to the stored picture.
    internal bool IsSegmentRemapped { get; private set; }

    internal bool IsComScanReversed { get; private set; }

    internal byte ComPinsConfiguration { get; private set; }

    internal byte ClockDivideOscillator { get; private set; }

    internal byte PreChargePeriod { get; private set; }

    internal int ScrollTopFixedRows { get; private set; }

    internal int ScrollRows { get; private set; }

    internal byte[] ScrollSetup { get; private set; } = [];

    internal bool IsScrollActive { get; private set; }

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
                WriteRam(value);
                AdvancePointers();
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>The display RAM is left as it is (its content after a reset is undefined on the chip). A running scroll stops.</remarks>
    public void Reset()
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ResetState();
            Flush();
        }
    }

    /// <summary>
    /// Stops the scroll timer, detaches the controller from the buffer and makes it inert: the buffer keeps the last picture, and every
    /// later write, reset or setting is ignored (argument checks still apply).
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
            StopScrollTimer();
            buffer.Changed -= OnBufferChanged;
        }
    }

    internal byte[] GetRam()
    {
        lock (sync)
        {
            return (byte[])ram.Clone();
        }
    }

    // ref: §SSD1306.2, §SSD1309.2, §SH1106.2: the number of parameter bytes, or Unknown for a byte that is not a command of the chip.
    private static int CommonParameterCount(byte command)
        => command switch
        {
            <= 0x1F => 0,
            >= 0x40 and <= 0x7F => 0,
            >= 0xB0 and <= 0xB7 => 0,
            0x81 or 0xA8 or 0xD3 or 0xD5 or 0xD9 or 0xDA or 0xDB => 1,
            0xA0 or 0xA1 or 0xA4 or 0xA5 or 0xA6 or 0xA7 or 0xAE or 0xAF or 0xC0 or 0xC8 or 0xE3 => 0,
            _ => Unknown,
        };

    private static int Ssd130XParameterCount(byte command)
        => command switch
        {
            0x20 => 1,
            0x21 or 0x22 or 0xA3 => 2,
            0x2E or 0x2F => 0,
            _ => Unknown,
        };

    private static int Ssd1306ParameterCount(byte command)
        => command switch
        {
            0x26 or 0x27 => 6,
            0x29 or 0x2A => 5,
            0x8D => 1,
            _ => Ssd130XParameterCount(command),
        };

    // ref: §SSD1309.2: 7-byte scroll setups, 0x2C/0x2D content scroll, 0xFD command lock, 0xDC GPIO; no 0x8D (§SSD1309.5).
    private static int Ssd1309ParameterCount(byte command)
        => command switch
        {
            0x26 or 0x27 or 0x29 or 0x2A or 0x2C or 0x2D => 7,
            0xFD or 0xDC => 1,
            _ => Ssd130XParameterCount(command),
        };

    // ref: §SH1106.2, §SH1106.4: 0x30-0x33 and 0xE0/0xEE take no parameter, 0xAD takes one; 0x20-0x22, 0x8D and scrolling do not exist.
    private static int Sh1106ParameterCount(byte command)
        => command switch
        {
            >= 0x30 and <= 0x33 => 0,
            0xE0 or 0xEE => 0,
            0xAD => 1,
            _ => Unknown,
        };

    private static void RotatePageColumns(
        Span<byte> columns,
        bool isRight)
    {
        if (columns.Length < 2)
        {
            return;
        }

        // The RAM is kept in SEG order and panel column q shows SEG (last - q), so a move to the right on the panel
        // takes each SEG from the next one.
        if (isRight)
        {
            var first = columns[0];
            columns[1..].CopyTo(columns);
            columns[^1] = first;
        }
        else
        {
            var last = columns[^1];
            columns[..^1].CopyTo(columns[1..]);
            columns[0] = last;
        }
    }

    // Opened before the lock is taken and disposed after it is released: the buffer raises Changed when the outermost batch ends,
    // so it is never raised while the controller's lock is held, and a Changed subscriber that waits for another thread feeding
    // this controller (for example a Dispatcher.Invoke to a UI thread inside WriteData) cannot deadlock.
    private IDisposable BeginBatchOutsideLock()
        => buffer.BeginBatch();

    private int ParameterCount(byte command)
    {
        var count = CommonParameterCount(command);
        if (count != Unknown)
        {
            return count;
        }

        return Chip switch
        {
            OledChip.Sh1106 => Sh1106ParameterCount(command),
            OledChip.Ssd1309 => Ssd1309ParameterCount(command),
            _ => Ssd1306ParameterCount(command),
        };
    }

    private void ApplyCommand(byte value)
    {
        if (hasPendingCommand)
        {
            parameters[collectedCount++] = value;
            if (collectedCount == pendingCount)
            {
                hasPendingCommand = false;
                Execute(pendingCommand, parameters.AsSpan(0, pendingCount));
            }

            return;
        }

        var count = ParameterCount(value);
        if (count == Unknown)
        {
            TraceOnce(value, "is not a command of the chip and is ignored");
            return;
        }

        if (count == 0)
        {
            Execute(value, []);
            return;
        }

        hasPendingCommand = true;
        pendingCommand = value;
        pendingCount = count;
        collectedCount = 0;
    }

    private void Execute(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case <= 0x1F:
                SetColumnNibble(command);
                break;
            case >= 0x40 and <= 0x7F:
                StartLine = command & SixBitMask;
                isDirty = true;
                break;
            case >= 0xB0 and <= 0xB7:
                // ref: §LIB.U8G2.2, ruling R4: honoured in every addressing mode (⚠️ unverified for horizontal / vertical mode).
                PagePointer = command & PageMask;
                break;
            case (>= 0x20 and <= 0x2F) or 0xA3:
                ExecuteAddressingOrScroll(command, arguments);
                break;
            default:
                ExecuteDisplayCommand(command, arguments);
                break;
        }
    }

    private void ExecuteAddressingOrScroll(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x20:
                // ref: §SSD1306.9 item 5: 11b is invalid and ignored; the pointers are not moved (⚠️ unverified).
                var mode = arguments[0] & AddressingModeMask;
                if (mode != AddressingModeMask)
                {
                    addressingMode = (AddressingMode)mode;
                }

                break;
            case 0x21:
                // ref: §X item 2: masked to the bit field. The datasheet says 0x21 is for horizontal / vertical mode only (§SSD1306.2);
                // in page mode the window and the pointer are still set (⚠️ unverified: the datasheet does not say what the chip does, §SSD1306.4).
                columnStart = arguments[0] & ColumnMask;
                columnEnd = arguments[1] & ColumnMask;
                ColumnPointer = columnStart;
                break;
            case 0x22:
                // ref: §SSD1306.2: bits 7:3 are don't care, so Adafruit's end 0xFF is page 7 (§LIB.A1306.2). As 0x21, also applied in
                // page mode (⚠️ unverified, §SSD1306.4).
                pageStart = arguments[0] & PageMask;
                pageEnd = arguments[1] & PageMask;
                PagePointer = pageStart;
                break;
            case 0x2E:
                // ref: §SSD1306.8: the design keeps the picture where it is (⚠️ unverified against hardware); nothing visible changes.
                IsScrollActive = false;
                StopScrollTimer();
                break;
            case 0x2F:
                IsScrollActive = true;
                StartScrollTimer();
                break;
            case 0xA3:
                ScrollTopFixedRows = arguments[0] & SixBitMask;
                ScrollRows = arguments[1] & ColumnMask;
                break;
            case 0x2C or 0x2D:
                // ref: §SSD1309.2: content scroll, one column per command, right (2C) or left (2D).
                ScrollOneStep([command, .. arguments]);
                break;
            default:
                // 0x26/0x27/0x29/0x2A: the last setup wins (§SSD1306.2). A setup must be preceded by 2E on the chip (§SSD1306.2);
                // the emulator stops a running scroll when its parameters change (⚠️ unverified what the chip does).
                ScrollSetup = [command, .. arguments];
                IsScrollActive = false;
                StopScrollTimer();
                break;
        }
    }

    private void ExecuteDisplayCommand(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x81:
                contrast = arguments[0];
                break;
            case 0xA0 or 0xA1:
                // ref: §SSD1306.6, ruling R3: affects the following data writes only, so nothing visible changes now.
                IsSegmentRemapped = command == 0xA1;
                return;
            case 0xA4 or 0xA5:
                isEntireDisplayOn = command == 0xA5;
                break;
            case 0xA6 or 0xA7:
                isInverted = command == 0xA7;
                break;
            case 0xAE or 0xAF:
                isDisplayOn = command == 0xAF;
                break;
            case 0xA8:
                SetMultiplexRatio(arguments[0] & SixBitMask);
                RestartScrollTimer();
                break;
            case 0xC0 or 0xC8:
                IsComScanReversed = command == 0xC8;
                break;
            case 0xD3:
                DisplayOffset = arguments[0] & SixBitMask;
                break;
            case 0xDA:
                ComPinsConfiguration = arguments[0];
                break;
            case 0xD5:
                ClockDivideOscillator = arguments[0];
                RestartScrollTimer();
                return;
            case 0xD9:
                PreChargePeriod = arguments[0];
                RestartScrollTimer();
                return;
            case 0xE0:
                // ref: §SH1106.3: read-modify-write; the column returns to this address on 0xEE (⚠️ derived).
                readModifyWriteColumn = ColumnPointer;
                return;
            case 0xEE:
                ColumnPointer = readModifyWriteColumn ?? ColumnPointer;
                readModifyWriteColumn = null;
                return;
            default:
                // 0xDB, 0xE3, 0x8D, 0xFD, 0xDC, 0xAD (with its 0x8A/0x8B parameter), 0x30-0x33: consumed, nothing visible changes
                // (§SSD1306.2, §SSD1309.2, §SH1106.2, §SH1106.4).
                return;
        }

        isDirty = true;
    }

    private void SetColumnNibble(byte command)
    {
        if (command <= NibbleMask)
        {
            columnLowNibble = command & NibbleMask;
        }
        else
        {
            columnHighNibble = command & NibbleMask;
        }

        // ref: §LIB.U8G2.2, ruling R4: honoured in every addressing mode. The SH1106 counter is 8 bits (writes past column 131
        // are dropped, §SH1106.3); the SSD130x column is kept inside its 128 columns (⚠️ unverified for high nibbles 8-F).
        var column = (columnHighNibble << NibbleBits) | columnLowNibble;
        ColumnPointer = Chip == OledChip.Sh1106
            ? column
            : column & ColumnMask;
    }

    private void SetMultiplexRatio(int ratio)
    {
        // ref: §SSD1306.2: N = 0-14 is an invalid entry on the SSD130x and is ignored (⚠️ unverified); the SH1106 takes 1-64 MUX (§SH1106.2).
        if (Chip != OledChip.Sh1106 && ratio < MinSsd130XMultiplexRatio)
        {
            return;
        }

        MultiplexRatio = ratio;
    }

    // ref: §SSD1306.6, ruling R3: the segment remap acts at write time. The RAM is kept in SEG order: with A0 column x drives SEG x,
    // with A1 it drives SEG (last - x) (§SH1106.1: SEG 131 - x on the SH1106).
    private void WriteRam(byte value)
    {
        if (ColumnPointer >= RamWidth)
        {
            return;
        }

        var segment = IsSegmentRemapped
            ? RamWidth - 1 - ColumnPointer
            : ColumnPointer;
        ref var cell = ref ram[(PagePointer * RamWidth) + segment];
        if (cell == value)
        {
            return;
        }

        cell = value;

        // Data in a column the panel does not show (the SH1106 columns outside the 128-wide window) changes nothing visible.
        var panelColumn = RamWidth - 1 - columnOffset - segment;
        if (panelColumn >= 0 && panelColumn < mappedWidth)
        {
            isDirty = true;
        }
    }

    // ref: §SSD1306.4 (wrap rules per addressing mode), §SH1106.3 (no wrap, no page advance).
    private void AdvancePointers()
    {
        var lastColumn = RamWidth - 1;

        if (Chip == OledChip.Sh1106)
        {
            if (ColumnPointer < RamWidth)
            {
                ColumnPointer++;
            }

            return;
        }

        switch (addressingMode)
        {
            case AddressingMode.Horizontal:
                if (ColumnPointer != columnEnd && ColumnPointer < lastColumn)
                {
                    ColumnPointer++;
                    return;
                }

                ColumnPointer = columnStart;
                PagePointer = PagePointer == pageEnd || PagePointer >= LastPage
                    ? pageStart
                    : PagePointer + 1;
                break;
            case AddressingMode.Vertical:
                if (PagePointer != pageEnd && PagePointer < LastPage)
                {
                    PagePointer++;
                    return;
                }

                PagePointer = pageStart;
                ColumnPointer = ColumnPointer == columnEnd || ColumnPointer >= lastColumn
                    ? columnStart
                    : ColumnPointer + 1;
                break;
            default:
                // ref: §SSD1306.4: the page never changes; the column wraps to 0 (⚠️ unverified: 0 or the nibble start).
                ColumnPointer = ColumnPointer >= lastColumn
                    ? 0
                    : ColumnPointer + 1;
                break;
        }
    }

    // ref: §SSD1306.1, §SSD1309.1, §SH1106.2 (POR column): the power-on state. The RAM is not touched.
    private void ResetState()
    {
        hasPendingCommand = false;
        collectedCount = 0;
        addressingMode = AddressingMode.Page;
        columnStart = 0;
        columnEnd = Ssd130XRamWidth - 1;
        pageStart = 0;
        pageEnd = LastPage;
        columnLowNibble = 0;
        columnHighNibble = 0;
        ColumnPointer = 0;
        PagePointer = 0;
        readModifyWriteColumn = null;
        isDisplayOn = false;
        isInverted = false;
        isEntireDisplayOn = false;
        contrast = Chip == OledChip.Sh1106
            ? Sh1106DefaultContrast
            : DefaultContrast;
        StartLine = 0;
        DisplayOffset = 0;
        MultiplexRatio = DefaultMultiplexRatio;
        IsSegmentRemapped = false;
        IsComScanReversed = false;
        ComPinsConfiguration = DefaultComPins;
        ClockDivideOscillator = Chip switch
        {
            OledChip.Sh1106 => Sh1106DefaultClock,
            OledChip.Ssd1309 => Ssd1309DefaultClock,
            _ => Ssd1306DefaultClock,
        };
        PreChargePeriod = DefaultPreCharge;
        ScrollTopFixedRows = 0;
        ScrollRows = DefaultScrollRows;
        ScrollSetup = [];

        // ⚠️ unverified (§SSD1306.1): the reset state of the scroll flag is not stated; assumed deactivated.
        IsScrollActive = false;
        StopScrollTimer();
        isDirty = true;
    }

    private void StartScrollTimer()
    {
        StopScrollTimer();

        if (isDisposed || !TryGetScrollStepTime(out var stepTime))
        {
            return;
        }

        var generation = scrollGeneration;
        scrollTimer = TimeProvider.CreateTimer(
            _ => OnScrollTimer(generation),
            state: null,
            stepTime,
            stepTime);
    }

    private void RestartScrollTimer()
    {
        if (IsScrollActive)
        {
            StartScrollTimer();
        }
    }

    private void StopScrollTimer()
    {
        // A callback that is already queued sees a newer generation and does nothing.
        scrollGeneration++;
        scrollTimer?.Dispose();
        scrollTimer = null;
    }

    private void OnScrollTimer(int generation)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (generation != scrollGeneration || isDisposed)
            {
                return;
            }

            ScrollOneStep(ScrollSetup);
            Flush();
        }
    }

    // ref: §SSD1306.8: step time = interval frames x frame period, frame period = D x K x MUX / Fosc with D = 0xD5 A[3:0] + 1,
    // K = P1 + P2 + 50 (0xD9 nibbles) and Fosc = 370 kHz typical; §SSD1309.4: K = P1 + P2 + 65, Fosc = 450 kHz.
    // ⚠️ derived (datasheet formula with the typical oscillator; the oscillator for other 0xD5 A[7:4] values is not tabled, §SSD1306.8).
    private bool TryGetScrollStepTime(out TimeSpan stepTime)
    {
        stepTime = TimeSpan.Zero;
        if (ScrollSetup.Length < 4 || Chip == OledChip.Sh1106)
        {
            return false;
        }

        var intervalCode = ScrollSetup[3] & PageMask;
        var isSsd1309 = Chip == OledChip.Ssd1309;
        var frames = isSsd1309
            ? Ssd1309ScrollFrames[intervalCode]
            : Ssd1306ScrollFrames[intervalCode];
        var divideRatio = (ClockDivideOscillator & NibbleMask) + 1;
        var phases = (PreChargePeriod & NibbleMask) + (PreChargePeriod >> NibbleBits);
        var clocksPerRow = phases + (isSsd1309 ? Ssd1309BankPulseWidth : Ssd1306BankPulseWidth);
        var oscillatorHz = isSsd1309 ? Ssd1309OscillatorHz : Ssd1306OscillatorHz;
        var ticks = (long)frames * divideRatio * clocksPerRow * (MultiplexRatio + 1) * TimeSpan.TicksPerSecond / oscillatorHz;

        stepTime = TimeSpan.FromTicks(System.Math.Max(1, ticks));
        return true;
    }

    // ref: §SSD1306.8: pages B..D move one column per step, wrapping around all 128 columns (SSD1306); §SSD1309.2: columns F..G.
    // Right (26/29/2C) is towards higher panel columns under the upright A1 + C8 (⚠️ derived from the library names; the datasheet
    // figures are drawn in SEG terms). The vertical part of 29/2A is not emulated (§SSD1306.8 allows this; ⚠️ unverified direction).
    private void ScrollOneStep(ReadOnlySpan<byte> setup)
    {
        if (setup.Length < 5 || Chip == OledChip.Sh1106)
        {
            return;
        }

        var command = setup[0];
        var isSsd1309 = Chip == OledChip.Ssd1309;
        var isCombined = command is 0x29 or 0x2A;
        var isRight = command is 0x26 or 0x29 or 0x2C;
        var startPage = setup[2] & PageMask;
        var endPage = setup[4] & PageMask;

        if (isCombined && setup.Length > 5 && (setup[5] & SixBitMask) != 0)
        {
            TraceVerticalScroll();
        }

        // ref: §SSD1309.2: on the SSD1309, 29/2A A[0] = 0 means no column scroll.
        if (isSsd1309 && isCombined && (setup[1] & 1) == 0)
        {
            return;
        }

        var firstColumn = 0;
        var lastColumn = Ssd130XRamWidth - 1;
        if (isSsd1309 && setup.Length >= 8)
        {
            // ⚠️ unverified: F..G are taken as column addresses and mapped to SEG order with the current segment remap.
            var start = setup[6] & ColumnMask;
            var end = setup[7] & ColumnMask;
            (firstColumn, lastColumn) = IsSegmentRemapped
                ? (Ssd130XRamWidth - 1 - end, Ssd130XRamWidth - 1 - start)
                : (start, end);
        }

        if (lastColumn <= firstColumn || endPage < startPage)
        {
            // ⚠️ unverified: an end before its start is not described by the datasheet; nothing moves.
            return;
        }

        for (var page = startPage; page <= endPage; page++)
        {
            RotatePageColumns(
                ram.AsSpan((page * RamWidth) + firstColumn, lastColumn - firstColumn + 1),
                isRight);
        }

        isDirty = true;
    }

    private void TraceVerticalScroll()
    {
        if (isVerticalScrollTraced)
        {
            return;
        }

        isVerticalScrollTraced = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1306Controller)} ({Chip}): the vertical part of a continuous scroll is not emulated; only the horizontal part moves."));
    }

    private void TraceOnce(
        byte value,
        string reason)
    {
        if (tracedCommands[value])
        {
            return;
        }

        tracedCommands[value] = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1306Controller)} ({Chip}): command byte 0x{value:X2} {reason}."));
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

            // The buffer was resized: the RAM is kept and drawn again for the new size and panel wiring.
            isDirty = true;
            Flush();
        }
    }

    // Must be called with the lock held, inside a BeginBatchOutsideLock scope, so the buffer raises Changed after the lock is released.
    private void Flush()
    {
        if (!isDirty)
        {
            return;
        }

        using var batch = buffer.BeginBatch();

        buffer.IsDisplayOn = isDisplayOn;
        buffer.IsInverted = isInverted;
        buffer.IsEntireDisplayOn = isEntireDisplayOn;
        buffer.Contrast = contrast;

        // A resize of the buffer between reading its size and loading the frame makes the length wrong; try once more.
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var width = buffer.Width;
            var height = buffer.Height;
            var length = width * ((height + PageHeight - 1) / PageHeight);
            if (visible.Length != length)
            {
                visible = new byte[length];
            }

            MapToVisible(visible, width, height);

            try
            {
                buffer.LoadFrame(visible);
                mappedWidth = width;
                mappedHeight = height;
                isDirty = false;
                return;
            }
            catch (ArgumentException)
            {
                // The buffer was resized meanwhile.
            }
        }

        // Both attempts lost a race with resizes: isDirty stays set, so the next call (or the resize's Changed) draws again.
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Ssd1306Controller)} ({Chip}): the buffer was resized while drawing; the picture is drawn on the next update."));
    }

    // The RAM -> panel mapping. Columns: panel column q shows SEG (RamWidth - 1 - ColumnOffset - q) (§SSD1306.6, §SH1106.1: the module
    // is wired so that A1 shows RAM column q + offset at panel column q). Rows: see PanelRowToRamRow.
    private void MapToVisible(
        Span<byte> target,
        int width,
        int height)
    {
        target.Clear();

        var multiplex = MultiplexRatio + 1;
        var isAlternative = (ComPinsConfiguration & AlternativeComBit) != 0;

        // ref: §SH1106.2: the SH1106 has no left/right remap bit.
        var isLeftRightRemapped = Chip != OledChip.Sh1106 && (ComPinsConfiguration & LeftRightRemapBit) != 0;
        var firstSegment = RamWidth - 1 - columnOffset;
        var columns = System.Math.Min(width, firstSegment + 1);

        for (var y = 0; y < height; y++)
        {
            var ramRow = PanelRowToRamRow(y, height, multiplex, isAlternative, isLeftRightRemapped);
            if (ramRow < 0)
            {
                continue;
            }

            var source = ram.AsSpan((ramRow / PageHeight) * RamWidth, RamWidth);
            var bit = ramRow % PageHeight;
            var row = target.Slice((y / PageHeight) * width, width);
            var mask = (byte)(1 << (y % PageHeight));
            for (var q = 0; q < columns; q++)
            {
                if (((source[firstSegment - q] >> bit) & 1) != 0)
                {
                    row[q] |= mask;
                }
            }
        }
    }

    // Returns the RAM row panel row y shows, or -1 for a dark row.
    // 1. Panel wiring (§SSD1306.5.3, ⚠️ derived): a panel taller than 32 rows is alternative wired (even y: pin 63 - y/2, odd y: pin 31 - (y-1)/2),
    //    a panel of 32 rows or fewer is sequential wired (pin height - 1 - y; the note gives 31 - y for 32 rows, ⚠️ generalised to other heights).
    // 2. COM pins (§SSD1306.5.2, ⚠️ derived): L/R remap moves the pin by 32; alternative: pins 0-31 carry the even scan slots, 32-63 the odd ones.
    // 3. Row addressing (§SSD1306.5.1, ⚠️ derived formula reproducing Tables 10-1 and 10-2): C0 j = (slot + offset) mod 64, C8 j = (slot - offset) mod 64;
    //    lit if j < MUX; sigma = j (C0) or MUX - 1 - j (C8); RAM row = (sigma + start line) mod 64 (rows beyond MUX are dark, §SSD1306.9 item 4).
    private int PanelRowToRamRow(
        int y,
        int height,
        int multiplex,
        bool isAlternative,
        bool isLeftRightRemapped)
    {
        int pin;
        if (height > MaxSequentialPanelHeight)
        {
            pin = y % 2 == 0
                ? ComCount - 1 - (y / 2)
                : HalfComCount - 1 - ((y - 1) / 2);
        }
        else
        {
            pin = height - 1 - y;
        }

        if (isLeftRightRemapped)
        {
            pin = (pin + HalfComCount) % ComCount;
        }

        int slot;
        if (!isAlternative)
        {
            slot = pin;
        }
        else
        {
            slot = pin < HalfComCount
                ? 2 * pin
                : (2 * (pin - HalfComCount)) + 1;
        }

        var scan = IsComScanReversed
            ? (slot - DisplayOffset + ComCount) % ComCount
            : (slot + DisplayOffset) % ComCount;
        if (scan >= multiplex)
        {
            return -1;
        }

        var sigma = IsComScanReversed
            ? multiplex - 1 - scan
            : scan;
        return (sigma + StartLine) % ComCount;
    }
}