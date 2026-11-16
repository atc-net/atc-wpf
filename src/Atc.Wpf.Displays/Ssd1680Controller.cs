namespace Atc.Wpf.Displays;

/// <summary>
/// Emulates the command set of an SSD1680 e-paper controller (176 sources x 296 gates, black/white or black/white/red) on an
/// <see cref="EPaperFrameBuffer"/>: the command and data bytes a microcontroller sends to the chip (as GxEPD2 and Adafruit_EPD do) are
/// applied to the chip's own RAM, and the part of the RAM the panel shows is kept in the buffer's RAM; a display update (0x20) refreshes
/// the buffer, so a desktop app can mirror a real module byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// A command byte is written with <see cref="WriteCommand"/> or <see cref="WriteCommands"/> (D/C low); its parameters follow as data bytes
/// (<see cref="WriteData"/>, D/C high), also across calls. A new command byte ends the previous command and drops the parameters not yet
/// received. After 0x24 (black/white RAM: bit 0 = black, 1 = white) or 0x26 (red RAM: bit 1 = red) every data byte goes into that RAM
/// at the X (byte) and Y (row) counters set by 0x4E / 0x4F, inside the window set by 0x44 / 0x45, until the next command byte.
/// The counters move as the data entry mode (0x11) says: X or Y first (AM), each axis up or down (ID); at the window edge an axis
/// returns to the other edge of the window and steps the other axis. Bytes outside the window or the RAM are dropped.
/// </para>
/// <para>
/// 0x20 (master activation) refreshes the buffer as the last 0x22 value selects: F7, F4 and C7 are a full refresh; FC, CC, 0C, CF and
/// the power-on FF a partial refresh; the power, clock and LUT-load values (F8, E0, 83, 80, 01, C0, 03, 91, 99, B1, B9) refresh nothing.
/// Any other value is taken as a full refresh (⚠️ a design choice: the bit meaning of 0x22 is derived, not stated by the datasheet) and
/// written to <see cref="Trace"/> once. The refresh shows the whole panel and is instant; the BUSY pin is not emulated.
/// </para>
/// <para>
/// The panel shows <see cref="EPaperFrameBuffer.Width"/> x <see cref="EPaperFrameBuffer.Height"/> pixels of the RAM, starting at the RAM
/// byte column <see cref="RamXOffsetBytes"/> and row 0; the size and offset are properties of the panel and are never taken from 0x01 or
/// 0x21. On a black/white buffer the red RAM is kept but not shown (GxEPD2 keeps the previous picture there).
/// 0x10 with a non-zero parameter puts the chip in deep sleep. The chip leaves deep sleep only on its RST pin, which a mirror usually
/// never sees, so the emulator wakes leniently (⚠️ a design choice, not chip behaviour): while asleep, data bytes and every command
/// except 0x12, 0x20, 0x22, 0x24 and 0x26 are ignored; one of those five commands wakes the emulator and is then applied as usual, and
/// <see cref="Reset"/> always wakes it. 0x12 (software reset) restores the registers and leaves the RAM. The temperature, voltage, LUT, OTP, border
/// and display option commands consume their parameters and are kept or ignored; the read commands consume nothing. A byte that is not
/// a command of the chip is written to <see cref="Trace"/> the first time each distinct value is seen by this instance. No byte value throws.
/// </para>
/// <para>
/// A new instance and <see cref="Reset"/> apply the chip's power-on state: data entry mode 0x03, the full RAM window, counters 0,
/// 0x22 = 0xFF, border 0xC0, not asleep. A new instance starts with white RAM and no red (⚠️ the RAM at power-on is not stated);
/// a reset keeps the RAM. When the buffer is resized or <see cref="RamXOffsetBytes"/> changes, the RAM is mapped into the buffer again.
/// </para>
/// <para>
/// Feed the controller from one thread at a time: bytes fed from several threads at once arrive in no defined order.
/// The calls are serialized by an internal lock; the buffer raises <see cref="EPaperFrameBuffer.Changed"/> and
/// <see cref="EPaperFrameBuffer.Refreshed"/> after that lock is released, on the thread that made the change. Dispose the controller to
/// detach it from the buffer; a disposed controller ignores everything sent to it.
/// </para>
/// </remarks>
public sealed class Ssd1680Controller : IGraphicDisplayController
{
    // ref: §SSD1680.1: 176 sources (22 bytes) x 296 gates, for each plane.
    private const int RamBytesPerRow = 22;
    private const int RamRows = 296;

    private const int MaxParameters = 153;
    private const int Unknown = -1;
    private const byte WhiteByte = 0xFF;
    private const byte NoRedByte = 0x00;
    private const byte PowerOnDataEntryMode = 0x03;
    private const byte XIncrementBit = 0x01;
    private const byte YIncrementBit = 0x02;
    private const byte YFirstBit = 0x04;
    private const byte DeepSleepCommand = 0x10;
    private const byte SoftwareResetCommand = 0x12;
    private const byte MasterActivationCommand = 0x20;
    private const byte WriteBlackRamCommand = 0x24;
    private const byte WriteRedRamCommand = 0x26;

    private readonly Lock sync = new();
    private readonly EPaperFrameBuffer buffer;
    private readonly byte[] ramBlack = new byte[RamBytesPerRow * RamRows];
    private readonly byte[] ramRed = new byte[RamBytesPerRow * RamRows];
    private readonly byte[] parameters = new byte[MaxParameters];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private readonly bool[] tracedUpdateValues = new bool[byte.MaxValue + 1];
    private FeedMode mode;
    private EPaperPlane ramPlane;
    private byte pendingCommand;
    private int pendingCount;
    private int collectedCount;
    private int ramXOffsetBytes;
    private bool isRemapNeeded = true;
    private int mappedWidth;
    private int mappedHeight;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ssd1680Controller"/> class in the chip's power-on state, with white RAM and no red.
    /// </summary>
    /// <param name="buffer">The buffer that shows the panel: 128 x 296 for the 2.9" panel, 128 x 250 for the 2.13" panel (122 columns visible).</param>
    /// <param name="ramXOffsetBytes">The first RAM byte column the panel shows, 0 to 21; see <see cref="RamXOffsetBytes"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ramXOffsetBytes"/> is out of range.</exception>
    public Ssd1680Controller(
        EPaperFrameBuffer buffer,
        int ramXOffsetBytes = 0)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ValidateOffset(ramXOffsetBytes);

        this.buffer = buffer;
        this.ramXOffsetBytes = ramXOffsetBytes;
        Array.Fill(ramBlack, WhiteByte);

        using (BeginBatchOutsideLock())
        {
            lock (sync)
            {
                ApplyPowerOnState();
                IsDeepSleep = false;
                EnsureMapped();
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
    /// Gets or sets the first RAM byte column (8 pixels) the panel shows, 0 to 21; the default is 0.
    /// GxEPD2 always uses 0; Adafruit_EPD uses 1 for some panels (for example ThinkInk_290_Mono_BN and ThinkInk_213_Mono_BN) and 0
    /// for others. A mirror of traffic for an offset-1 panel shown with offset 0 is 8 pixels off.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is out of range.</exception>
    public int RamXOffsetBytes
    {
        get
        {
            lock (sync)
            {
                return ramXOffsetBytes;
            }
        }

        set
        {
            ValidateOffset(value);

            using var batch = BeginBatchOutsideLock();

            lock (sync)
            {
                if (isDisposed ||
                    ramXOffsetBytes == value)
                {
                    return;
                }

                ramXOffsetBytes = value;
                isRemapNeeded = true;
                EnsureMapped();
            }
        }
    }

    internal byte DataEntryMode { get; private set; }

    internal int XStart { get; private set; }

    internal int XEnd { get; private set; }

    internal int YStart { get; private set; }

    internal int YEnd { get; private set; }

    internal int XCounter { get; private set; }

    internal int YCounter { get; private set; }

    // The 0x01 MUX setting (A[8:0] + 1); kept, never used to size the panel (§SSD1680.1).
    internal int MultiplexLines { get; private set; }

    internal byte GateScan { get; private set; }

    internal byte UpdateControl1A { get; private set; }

    internal byte UpdateControl1B { get; private set; }

    internal byte UpdateControl2 { get; private set; }

    internal byte BorderWaveform { get; private set; }

    internal byte TemperatureSensor { get; private set; }

    internal bool IsDeepSleep { get; private set; }

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
            EnsureMapped();
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

            EnsureMapped();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The bytes are the parameters of the last command while it waits for them, then RAM bytes after 0x24 or 0x26; other data is ignored.
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
            if (isDisposed || IsDeepSleep)
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
                        WriteRamByte(value);
                        break;

                    // FeedMode.Ignore: data that is not a pending parameter and not RAM data.
                }
            }

            EnsureMapped();
        }
    }

    /// <inheritdoc />
    /// <remarks>The hardware reset (RST pin): always wakes the chip from deep sleep and applies the power-on registers; the RAM is kept (⚠️).</remarks>
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
            IsDeepSleep = false;
            EnsureMapped();
        }
    }

    /// <summary>
    /// Detaches the controller from the buffer and makes it inert: the buffer keeps its RAM and picture, and every later write, reset or
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

    internal byte[] GetRam(EPaperPlane plane)
    {
        lock (sync)
        {
            return plane == EPaperPlane.Red
                ? (byte[])ramRed.Clone()
                : (byte[])ramBlack.Clone();
        }
    }

    private static void ValidateOffset(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, RamBytesPerRow - 1);
    }

    // ref: §SSD1680.2 parameter counts; 0 for a known command without parameters (the read commands included: the emulator returns
    // nothing), Unknown for a byte that is not a command.
    private static int ParameterCount(byte command)
        => command switch
        {
            0x32 => 153,
            0x37 or 0x38 => 10,
            0x09 or 0x0C or 0x45 => 4,
            0x01 or 0x04 or 0x1C => 3,
            0x1A or 0x21 or 0x44 or 0x4F => 2,
            0x03 or DeepSleepCommand or 0x11 or 0x14 or 0x15 or 0x18 or 0x22 or 0x29 or 0x2C or 0x39 or 0x3C or 0x3F or 0x41 or 0x46 or 0x47 or 0x4E => 1,
            0x08 or SoftwareResetCommand or MasterActivationCommand or WriteBlackRamCommand or WriteRedRamCommand => 0,
            0x28 or 0x2A or 0x30 or 0x31 or 0x34 or 0x36 or 0x7F => 0,
            0x0A or 0x1B or 0x27 or 0x2D or 0x2E or 0x2F or 0x35 => 0,
            _ => Unknown,
        };

    private static bool IsWakeCommand(byte command)
        => command is SoftwareResetCommand or 0x22 or MasterActivationCommand or WriteBlackRamCommand or WriteRedRamCommand;

    // ref: §SSD1680.6 (the library values and the 12 datasheet values; the bit decoding is ⚠️ derived). Null = no refresh.
    private static EPaperRefreshKind? RefreshKindOf(
        byte value,
        out bool isKnown)
    {
        isKnown = true;
        switch (value)
        {
            case 0xF7 or 0xF4 or 0xC7:
                return EPaperRefreshKind.Full;
            case 0xFC or 0xCC or 0x0C or 0xCF or 0xFF:
                return EPaperRefreshKind.Partial;
            case 0xF8 or 0xE0 or 0x83 or 0x80 or 0x01 or 0xC0 or 0x03 or 0x91 or 0x99 or 0xB1 or 0xB9:
                return null;
            default:
                // ⚠️ design: a value neither library nor the datasheet table uses is taken as a full refresh.
                isKnown = false;
                return EPaperRefreshKind.Full;
        }
    }

    // One step of a counter along its axis, inside the window [min(start, end), max(start, end)]: past the edge it returns to the other
    // edge and reports the wrap. ref: §SSD1680.5 (⚠️ unverified: the recommended model, the datasheet does not state the wrap).
    private static int Step(
        int counter,
        int start,
        int end,
        bool isIncrement,
        out bool isWrapped)
    {
        var low = System.Math.Min(start, end);
        var high = System.Math.Max(start, end);
        isWrapped = false;
        if (isIncrement)
        {
            if (counter >= high || counter < low)
            {
                isWrapped = true;
                return low;
            }

            return counter + 1;
        }

        if (counter <= low || counter > high)
        {
            isWrapped = true;
            return high;
        }

        return counter - 1;
    }

    private static bool IsInside(
        int value,
        int start,
        int end)
        => value >= System.Math.Min(start, end) && value <= System.Math.Max(start, end);

    // Opened before the lock is taken and disposed after it is released: the buffer raises Changed and Refreshed when the outermost batch
    // ends, so they are never raised while the controller's lock is held.
    private IDisposable BeginBatchOutsideLock()
        => buffer.BeginBatch();

    private void ApplyCommand(byte command)
    {
        // ref: §SSD1680.7, §P3.X item 5: the chip leaves deep sleep only by HWRESET, which a mirror usually never sees (Adafruit_EPD
        // pulses RST before every display()). ⚠️ design (the note's lenient recommendation): a software reset, a refresh command
        // (0x22, 0x20) or a RAM write command (0x24, 0x26) wakes the emulator; other commands are ignored while asleep.
        if (IsDeepSleep)
        {
            if (!IsWakeCommand(command))
            {
                return;
            }

            IsDeepSleep = false;
        }

        // ref: §SSD1680.2: a command byte ends a RAM write and drops the parameters not yet received.
        mode = FeedMode.Ignore;

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

    // ref: §SSD1680.2: the commands without parameters.
    private void ExecuteSimple(byte command)
    {
        switch (command)
        {
            case WriteBlackRamCommand:
                mode = FeedMode.WriteRam;
                ramPlane = EPaperPlane.Black;
                break;
            case WriteRedRamCommand:
                mode = FeedMode.WriteRam;
                ramPlane = EPaperPlane.Red;
                break;
            case SoftwareResetCommand:
                // ref: §SSD1680.2 0x12: the registers go to their S/W reset values (⚠️ taken as the POR values, §SSD1680.3) except R10h;
                // "RAM are unaffected".
                ApplyPowerOnState();
                break;
            case MasterActivationCommand:
                Activate();
                break;

            // 0x08, 0x28, 0x2A, 0x30, 0x31, 0x34, 0x36 (OTP, VCOM, CRC), 0x7F (NOP) and the read commands: nothing to emulate.
        }
    }

    private void Execute(
        byte command,
        ReadOnlySpan<byte> arguments)
    {
        switch (command)
        {
            case 0x01:
                MultiplexLines = (arguments[0] | ((arguments[1] & 0x01) << 8)) + 1;
                GateScan = (byte)(arguments[2] & 0x07);
                break;
            case DeepSleepCommand:
                // ref: §SSD1680.2 0x10: A[1:0] 00 normal, 01 mode 1, 11 mode 2.
                IsDeepSleep = (arguments[0] & 0x03) != 0;
                break;
            case 0x11:
                DataEntryMode = (byte)(arguments[0] & 0x07);
                break;
            case 0x18:
                TemperatureSensor = arguments[0];
                break;
            case 0x21:
                // ref: §SSD1680.4 item 5: the bypass / inverse options are not honoured (neither library sends them; ⚠️ optional).
                UpdateControl1A = arguments[0];
                UpdateControl1B = arguments[1];
                break;
            case 0x22:
                UpdateControl2 = arguments[0];
                break;
            case 0x3C:
                // ref: §SSD1680.7: the border is not drawn; the value is kept.
                BorderWaveform = arguments[0];
                break;
            case 0x44:
                // ref: §SSD1680.2 0x44: A[5:0] (§8.3 writes 5 bits: ⚠️, 6 bits are kept).
                XStart = arguments[0] & 0x3F;
                XEnd = arguments[1] & 0x3F;
                break;
            case 0x45:
                YStart = arguments[0] | ((arguments[1] & 0x01) << 8);
                YEnd = arguments[2] | ((arguments[3] & 0x01) << 8);
                break;
            case 0x4E:
                XCounter = arguments[0] & 0x3F;
                break;
            case 0x4F:
                YCounter = arguments[0] | ((arguments[1] & 0x01) << 8);
                break;

            // 0x03, 0x04, 0x09, 0x0C, 0x14, 0x15, 0x1A, 0x1C, 0x29, 0x2C, 0x32, 0x37, 0x38, 0x39, 0x3F, 0x41: voltages, LUT, OTP, options.
            // 0x46 / 0x47 (auto write of a regular pattern) are consumed and not drawn: neither library uses them.
        }
    }

    // ref: §SSD1680.6: 0x20 runs the sequence the last 0x22 value selected.
    private void Activate()
    {
        var kind = RefreshKindOf(UpdateControl2, out var isKnown);
        if (!isKnown && !tracedUpdateValues[UpdateControl2])
        {
            tracedUpdateValues[UpdateControl2] = true;
            Trace.TraceWarning(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"{nameof(Ssd1680Controller)}: display update control 2 value 0x{UpdateControl2:X2} is not a known sequence; it is taken as a full refresh."));
        }

        if (kind is { } refreshKind)
        {
            buffer.Refresh(refreshKind);
        }
    }

    // ref: §SSD1680.5: the byte goes to the RAM at the counters when they are inside the window and the RAM, then the counters move.
    private void WriteRamByte(byte value)
    {
        var x = XCounter;
        var y = YCounter;
        if (IsInside(x, XStart, XEnd) &&
            IsInside(y, YStart, YEnd) &&
            x < RamBytesPerRow &&
            y < RamRows)
        {
            var ram = ramPlane == EPaperPlane.Red ? ramRed : ramBlack;
            ram[(y * RamBytesPerRow) + x] = value;

            // The buffer ignores a byte outside the panel.
            buffer.WriteRam(ramPlane, x - ramXOffsetBytes, y, value);
        }

        var isXIncrement = (DataEntryMode & XIncrementBit) != 0;
        var isYIncrement = (DataEntryMode & YIncrementBit) != 0;
        bool isWrapped;
        if ((DataEntryMode & YFirstBit) == 0)
        {
            XCounter = Step(x, XStart, XEnd, isXIncrement, out isWrapped);
            if (isWrapped)
            {
                YCounter = Step(y, YStart, YEnd, isYIncrement, out _);
            }

            return;
        }

        YCounter = Step(y, YStart, YEnd, isYIncrement, out isWrapped);
        if (isWrapped)
        {
            XCounter = Step(x, XStart, XEnd, isXIncrement, out _);
        }
    }

    // ref: §SSD1680.3 power-on state (the deep sleep state is handled by the caller). The RAM is not touched.
    private void ApplyPowerOnState()
    {
        mode = FeedMode.Ignore;
        pendingCount = 0;
        collectedCount = 0;
        MultiplexLines = RamRows;
        GateScan = 0;
        DataEntryMode = PowerOnDataEntryMode;
        XStart = 0;
        XEnd = RamBytesPerRow - 1;
        YStart = 0;
        YEnd = RamRows - 1;
        XCounter = 0;
        YCounter = 0;
        UpdateControl1A = 0x00;
        UpdateControl1B = 0x00;
        UpdateControl2 = 0xFF;
        BorderWaveform = 0xC0;
        TemperatureSensor = 0x48;
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
                $"{nameof(Ssd1680Controller)}: command byte 0x{value:X2} is not a command of the chip and is ignored."));
    }

    private void OnBufferChanged(
        object? sender,
        EventArgs e)
    {
        using var batch = BeginBatchOutsideLock();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            EnsureMapped();
        }
    }

    // Must be called with the lock held, inside a BeginBatchOutsideLock scope. Maps the panel window of the RAM into the buffer's RAM
    // when the buffer size or the offset changed (RAM writes in between go to the buffer as they happen).
    private void EnsureMapped()
    {
        var width = buffer.Width;
        var height = buffer.Height;
        if (!isRemapNeeded &&
            width == mappedWidth &&
            height == mappedHeight)
        {
            return;
        }

        var bytesPerRow = (width + 7) / 8;
        for (var y = 0; y < height; y++)
        {
            for (var bx = 0; bx < bytesPerRow; bx++)
            {
                var ramX = bx + ramXOffsetBytes;
                var isInRam = ramX < RamBytesPerRow && y < RamRows;
                var index = (y * RamBytesPerRow) + ramX;

                // A resize between reading the size and these writes is clipped by the buffer; its Changed maps again.
                buffer.WriteRam(EPaperPlane.Black, bx, y, isInRam ? ramBlack[index] : WhiteByte);
                buffer.WriteRam(EPaperPlane.Red, bx, y, isInRam ? ramRed[index] : NoRedByte);
            }
        }

        mappedWidth = width;
        mappedHeight = height;
        isRemapNeeded = false;
    }
}