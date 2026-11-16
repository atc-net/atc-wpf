namespace Atc.Wpf.Displays;

/// <summary>
/// Emulates the instruction set of a Sitronix ST7920 graphic LCD controller (a 128 x 64 module) on a <see cref="MonochromeFrameBuffer"/>:
/// the instruction and data bytes a microcontroller sends to the chip are applied to its text RAM (DDRAM), user font RAM (CGRAM) and
/// graphic RAM (GDRAM), and the picture the module shows is drawn in the buffer.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="WriteCommand"/> and <see cref="WriteCommands"/> take instruction bytes (RS = 0), <see cref="WriteData"/> takes data
/// bytes (RS = 1); use <see cref="St7920SerialDecoder"/> for the bytes of the serial (SPI) interface. Instructions are decoded strictly
/// by the current instruction set: the basic set while RE = 0, the extended set while RE = 1 (function set bit 2). Each RAM cell is a
/// 16-bit word written as two data bytes, high byte first; the address counter moves after the second byte. A pending first byte is
/// dropped by the next instruction (⚠️ unverified).
/// </para>
/// <para>
/// The text layer shows four lines of 16 half-width characters, at the DDRAM addresses 0x80, 0x90, 0x88 and 0x98 from the top
/// (⚠️ derived: the order the common libraries use on 128 x 64 modules). A DDRAM word holds two half-width codes (0x02 to 0x7F), one
/// user font (0x0000, 0x0002, 0x0004, 0x0006: CGRAM fonts 0 to 3) or one full-width code pair (0xA1 or more), which is drawn blank.
/// The half-width glyphs are ⚠️ approximate: a public-domain 8 x 16 font stands in for the chip's ROM, and codes 0x02 to 0x1F and 0x7F
/// are blank. The graphic layer is the GDRAM folded onto the module: words 0 to 7 of GDRAM rows 0 to 31 are the top half, words 8 to 15
/// the bottom half (⚠️ derived from the libraries). The graphics show only while the graphic display bit G is set (extended function
/// set). Where both layers have a pixel they are combined by XOR (⚠️ unverified: the datasheet gives no rule).
/// </para>
/// <para>
/// A new instance resizes the buffer to 128 x 64 and applies the power-on state: display, cursor and blink off, graphics off, basic
/// instruction set, AC + 1 entry mode; DDRAM holds spaces (0x20), CGRAM and GDRAM zeros (⚠️ the datasheet does not state the RAM content
/// at power-on). <see cref="Reset"/> applies the same state. The display on/off bit D and standby (⚠️ derived) switch
/// <see cref="MonochromeFrameBuffer.IsDisplayOn"/> for both layers.
/// </para>
/// <para>
/// Not emulated (⚠️): the cursor and its blink (the C and B bits are stored but not drawn), display shift, vertical scroll and the
/// line reverse instruction (stored only). Address codes above 0xBF and bytes that are not instructions of the current set are
/// written to <see cref="Trace"/> the first time each distinct byte value is seen by this instance; no byte value throws.
/// When the buffer is resized the RAM is kept and drawn again for the new size.
/// </para>
/// <para>
/// Each call that changes something visible ends in one update of the buffer, so the buffer raises
/// <see cref="MonochromeFrameBuffer.Changed"/> at most once per call. Feed the controller from one thread at a time: the calls are
/// serialized by an internal lock, and the buffer raises <see cref="MonochromeFrameBuffer.Changed"/> after that lock is released.
/// A disposed controller ignores everything sent to it.
/// </para>
/// </remarks>
public sealed class St7920Controller : IGraphicDisplayController
{
    private const int GlassWidth = 128;
    private const int GlassHeight = 64;
    private const int GlassRowBytes = GlassWidth / 8;
    private const int HalfGlassHeight = GlassHeight / 2;
    private const int PageHeight = 8;
    private const int DdramWords = 64;
    private const int CgramWords = 64;
    private const int GdramRows = 64;
    private const int GdramRowWords = 16;
    private const int WordsPerTextLine = 8;
    private const int GlyphRows = 16;
    private const int AddressMask = 0x3F;
    private const int HorizontalMask = 0x0F;
    private const ushort SpacePair = 0x2020;
    private const byte FirstHalfWidthCode = 0x02;
    private const byte LastHalfWidthCode = 0x7F;

    // ref: §ST7920.4 (⚠️ derived): the first DDRAM word of text lines 1 to 4, top to bottom (addresses 0x80, 0x90, 0x88, 0x98).
    private static readonly int[] TextLineWords = [0, 16, 8, 24];

    private readonly Lock sync = new();
    private readonly MonochromeFrameBuffer buffer;
    private readonly ushort[] ddram = new ushort[DdramWords];
    private readonly ushort[] cgram = new ushort[CgramWords];
    private readonly ushort[] gdram = new ushort[GdramRows * GdramRowWords];
    private readonly byte[] glass = new byte[GlassRowBytes * GlassHeight];
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private byte[] visible = [];
    private int mappedWidth;
    private int mappedHeight;
    private bool isDirty;
    private bool isDisposed;
    private RamTarget target;
    private int addressCounter;
    private int gdramVertical;
    private int gdramHorizontal;
    private bool isGdramHorizontalNext;
    private bool hasPendingByte;
    private byte pendingByte;

    /// <summary>
    /// Initializes a new instance of the <see cref="St7920Controller"/> class: the buffer is resized to 128 x 64 and the chip is put
    /// in its power-on state.
    /// </summary>
    /// <param name="buffer">The buffer that shows the picture of the module.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    public St7920Controller(MonochromeFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        this.buffer = buffer;

        using (buffer.BeginBatch())
        {
            lock (sync)
            {
                buffer.Resize(GlassWidth, GlassHeight);
                ResetState();
                Flush();
            }
        }

        buffer.Changed += OnBufferChanged;
    }

    private enum RamTarget
    {
        Ddram,
        Cgram,
        Gdram,
    }

    internal bool IsExtendedInstructionSet { get; private set; }

    internal bool IsEightBitInterface { get; private set; }

    internal bool IsGraphicDisplayOn { get; private set; }

    internal bool IsDisplayOn { get; private set; }

    internal bool IsCursorOn { get; private set; }

    internal bool IsBlinkOn { get; private set; }

    internal bool IsAddressIncrement { get; private set; }

    internal bool IsDisplayShiftOnEntry { get; private set; }

    internal bool IsScrollSelected { get; private set; }

    internal bool IsStandby { get; private set; }

    internal int ReversedLines { get; private set; }

    internal int ScrollAddress { get; private set; }

    /// <inheritdoc />
    /// <remarks>The byte is an instruction (RS = 0) of the current instruction set.</remarks>
    public void WriteCommand(byte command)
    {
        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            ApplyInstruction(command);
            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>The bytes are instructions (RS = 0), applied in order.</remarks>
    public void WriteCommands(ReadOnlySpan<byte> commands)
    {
        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var command in commands)
            {
                ApplyInstruction(command);
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The bytes go to the RAM whose address was set last (DDRAM, CGRAM or GDRAM), two bytes per 16-bit word, high byte first.
    /// An odd last byte waits for the next data byte, also across calls.
    /// </remarks>
    public void WriteData(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var value in data)
            {
                WriteDataByte(value);
            }

            Flush();
        }
    }

    /// <inheritdoc />
    /// <remarks>Applies the power-on state, including the RAM content (⚠️ not stated by the datasheet: spaces and zeros).</remarks>
    public void Reset()
    {
        using var batch = buffer.BeginBatch();

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
    /// Detaches the controller from the buffer and makes it inert: the buffer keeps the last picture, and every later write or reset
    /// is ignored.
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

    /// <summary>
    /// Starts a batch on the buffer, so several calls raise <see cref="MonochromeFrameBuffer.Changed"/> once.
    /// </summary>
    /// <returns>The scope to dispose to end the batch.</returns>
    internal IDisposable BeginBatch()
        => buffer.BeginBatch();

    private static bool IsHalfWidth(int code)
        => code is >= FirstHalfWidthCode and <= LastHalfWidthCode;

    // Must be called with the lock held.
    private void ResetState()
    {
        // ref: §ST7920.1 "Initial Setting" (P 18).
        Array.Fill(ddram, SpacePair);
        Array.Clear(cgram);
        Array.Clear(gdram);
        IsExtendedInstructionSet = false;
        IsEightBitInterface = true;
        IsGraphicDisplayOn = false;
        IsDisplayOn = false;
        IsCursorOn = false;
        IsBlinkOn = false;
        IsAddressIncrement = true;
        IsDisplayShiftOnEntry = false;
        IsScrollSelected = false;
        IsStandby = false;
        ReversedLines = 0;
        ScrollAddress = 0;
        target = RamTarget.Ddram;
        addressCounter = 0;
        gdramVertical = 0;
        gdramHorizontal = 0;
        isGdramHorizontalNext = false;
        hasPendingByte = false;
        pendingByte = 0;
        isDirty = true;
    }

    private void ApplyInstruction(byte value)
    {
        // ⚠️ unverified (§ST7920.2): a pending first data byte is dropped by any instruction.
        hasPendingByte = false;

        // ref: §ST7920.3: any instruction ends standby.
        if (IsStandby)
        {
            IsStandby = false;
            isDirty = true;
        }

        if (IsExtendedInstructionSet)
        {
            // ⚠️ unverified (§ST7920.3): any other instruction resets the GDRAM address pair to "vertical next".
            if (value < 0x80)
            {
                isGdramHorizontalNext = false;
            }

            ExecuteExtended(value);
        }
        else
        {
            isGdramHorizontalNext = false;
            ExecuteBasic(value);
        }
    }

    // ref: §ST7920.2 (DS-7920 P 16, P 19-22): decoded by the highest set bit.
    private void ExecuteBasic(byte value)
    {
        switch (value)
        {
            case >= 0x80:
                if (value >= 0xC0)
                {
                    TraceOnce(value, "is a DDRAM address above 0xBF; it is masked with 0x3F");
                }

                addressCounter = value & AddressMask;
                target = RamTarget.Ddram;
                break;
            case >= 0x40:
                if (IsScrollSelected)
                {
                    // ⚠️ unverified: Set CGRAM Address needs SR = 0 (§ST7920.2).
                    TraceOnce(value, "is ignored: Set CGRAM Address needs the scroll/RAM select bit SR = 0");
                    break;
                }

                addressCounter = value & AddressMask;
                target = RamTarget.Cgram;
                break;
            case >= 0x20:
                IsEightBitInterface = (value & 0x10) != 0;
                IsExtendedInstructionSet = (value & 0x04) != 0;
                break;
            case >= 0x10:
                // S/C = 0 moves the cursor (AC); S/C = 1 shifts the display (⚠️ not emulated).
                if ((value & 0x08) == 0)
                {
                    addressCounter = (addressCounter + ((value & 0x04) != 0 ? 1 : -1)) & AddressMask;
                }

                break;
            case >= 0x08:
                IsDisplayOn = (value & 0x04) != 0;
                IsCursorOn = (value & 0x02) != 0;
                IsBlinkOn = (value & 0x01) != 0;
                isDirty = true;
                break;
            case >= 0x04:
                IsAddressIncrement = (value & 0x02) != 0;
                IsDisplayShiftOnEntry = (value & 0x01) != 0;
                break;
            case >= 0x02:
                addressCounter = 0;
                target = RamTarget.Ddram;
                break;
            case 0x01:
                Array.Fill(ddram, SpacePair);
                addressCounter = 0;
                target = RamTarget.Ddram;
                IsAddressIncrement = true;
                isDirty = true;
                break;
            default:
                TraceOnce(value, "is not an instruction of the basic set and is ignored");
                break;
        }
    }

    // ref: §ST7920.3 (DS-7920 P 17, P 23-24).
    private void ExecuteExtended(byte value)
    {
        switch (value)
        {
            case >= 0x80:
                if (value >= 0xC0)
                {
                    TraceOnce(value, "is a GDRAM address above 0xBF; it is masked");
                }

                if (isGdramHorizontalNext)
                {
                    gdramHorizontal = value & HorizontalMask;
                    isGdramHorizontalNext = false;
                }
                else
                {
                    gdramVertical = value & AddressMask;
                    isGdramHorizontalNext = true;
                }

                target = RamTarget.Gdram;
                break;
            case >= 0x40:
                // ⚠️ unverified: the vertical scroll address is stored but not emulated.
                ScrollAddress = value & AddressMask;
                break;
            case >= 0x20:
                IsEightBitInterface = (value & 0x10) != 0;
                IsExtendedInstructionSet = (value & 0x04) != 0;
                IsGraphicDisplayOn = (value & 0x02) != 0;
                isDirty = true;
                break;
            case >= 0x08:
                TraceOnce(value, "is not an instruction of the extended set and is ignored");
                break;
            case >= 0x04:
                // ⚠️ unverified (§ST7920.7): the line reverse toggles and is stored, not drawn.
                ReversedLines ^= 1 << (value & 0x03);
                break;
            case >= 0x02:
                IsScrollSelected = (value & 0x01) != 0;
                break;
            case 0x01:
                // ⚠️ derived (§ST7920.3): standby shows nothing until the next instruction.
                IsStandby = true;
                isDirty = true;
                break;
            default:
                TraceOnce(value, "is not an instruction of the extended set and is ignored");
                break;
        }
    }

    // ref: §ST7920.2 (P 22: two bytes per word, high byte first, AC moves after the second), §ST7920.5 (GDRAM horizontal wrap).
    private void WriteDataByte(byte value)
    {
        if (!hasPendingByte)
        {
            pendingByte = value;
            hasPendingByte = true;
            return;
        }

        hasPendingByte = false;
        var word = (ushort)((pendingByte << 8) | value);
        var step = IsAddressIncrement ? 1 : -1;

        switch (target)
        {
            case RamTarget.Ddram:
                ddram[addressCounter] = word;

                // ⚠️ unverified: the 6-bit AC wraps.
                addressCounter = (addressCounter + step) & AddressMask;
                break;
            case RamTarget.Cgram:
                cgram[addressCounter] = word;
                addressCounter = (addressCounter + step) & AddressMask;
                break;
            default:
                gdram[(gdramVertical * GdramRowWords) + gdramHorizontal] = word;
                gdramHorizontal = (gdramHorizontal + 1) & HorizontalMask;
                break;
        }

        isDirty = true;
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
                $"{nameof(St7920Controller)}: instruction byte 0x{value:X2} {reason}."));
    }

    private void OnBufferChanged(
        object? sender,
        EventArgs e)
    {
        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed ||
                (buffer.Width == mappedWidth && buffer.Height == mappedHeight))
            {
                return;
            }

            // The buffer was resized: the RAM is kept and drawn again for the new size.
            isDirty = true;
            Flush();
        }
    }

    // Must be called with the lock held, inside a buffer batch, so the buffer raises Changed after the lock is released.
    private void Flush()
    {
        if (!isDirty)
        {
            return;
        }

        using var batch = buffer.BeginBatch();

        buffer.IsDisplayOn = IsDisplayOn && !IsStandby;
        buffer.IsInverted = false;
        buffer.IsEntireDisplayOn = false;

        ComposeGlass();

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

        Trace.TraceWarning($"{nameof(St7920Controller)}: the buffer was resized while drawing; the picture is drawn on the next update.");
    }

    // Draws the module picture into the glass: 64 rows of 16 bytes, bit 7 = leftmost pixel.
    private void ComposeGlass()
    {
        Array.Clear(glass);

        // ref: §ST7920.4: the text layer.
        for (var line = 0; line < TextLineWords.Length; line++)
        {
            for (var w = 0; w < WordsPerTextLine; w++)
            {
                DrawTextWord(ddram[TextLineWords[line] + w], line * GlyphRows, w * 2);
            }
        }

        if (!IsGraphicDisplayOn)
        {
            return;
        }

        // ref: §ST7920.5 (⚠️ derived fold) and §ST7920.7 (⚠️ unverified XOR combination).
        for (var y = 0; y < GlassHeight; y++)
        {
            var row = y < HalfGlassHeight ? y : y - HalfGlassHeight;
            var firstWord = y < HalfGlassHeight ? 0 : WordsPerTextLine;
            for (var w = 0; w < WordsPerTextLine; w++)
            {
                var word = gdram[(row * GdramRowWords) + firstWord + w];
                var index = (y * GlassRowBytes) + (w * 2);
                glass[index] ^= (byte)(word >> 8);
                glass[index + 1] ^= (byte)word;
            }
        }
    }

    private void DrawTextWord(
        ushort word,
        int top,
        int byteColumn)
    {
        var high = word >> 8;
        var low = word & 0xFF;

        if (high == 0x00 && (low & 0xF9) == 0)
        {
            // ref: §ST7920.4: a CGRAM font, code bits 2..1 select the font; each CGRAM word is a 16-pixel row, high byte on the left.
            var font = (low >> 1) & 0x03;
            for (var r = 0; r < GlyphRows; r++)
            {
                var pattern = cgram[(font * GlyphRows) + r];
                var index = ((top + r) * GlassRowBytes) + byteColumn;
                glass[index] = (byte)(pattern >> 8);
                glass[index + 1] = (byte)pattern;
            }

            return;
        }

        // ref: §ST7920.4: full-width codes (0xA1 or more) draw blank; other mixes are ⚠️ unverified and draw blank too.
        if (!IsHalfWidth(high) || !IsHalfWidth(low))
        {
            return;
        }

        DrawGlyph((byte)high, top, byteColumn);
        DrawGlyph((byte)low, top, byteColumn + 1);
    }

    private void DrawGlyph(
        byte code,
        int top,
        int byteColumn)
    {
        var glyph = St7920RomTables.GetGlyph(code);
        for (var r = 0; r < glyph.Length; r++)
        {
            glass[((top + r) * GlassRowBytes) + byteColumn] = glyph[r];
        }
    }

    // Copies the glass into the page layout of the buffer, clipped to its size.
    private void MapToVisible(
        Span<byte> pages,
        int width,
        int height)
    {
        pages.Clear();

        var rows = System.Math.Min(height, GlassHeight);
        var columns = System.Math.Min(width, GlassWidth);
        for (var y = 0; y < rows; y++)
        {
            var source = glass.AsSpan(y * GlassRowBytes, GlassRowBytes);
            var row = pages.Slice((y / PageHeight) * width, width);
            var mask = (byte)(1 << (y % PageHeight));
            for (var x = 0; x < columns; x++)
            {
                if ((source[x >> 3] & (0x80 >> (x & 7))) != 0)
                {
                    row[x] |= mask;
                }
            }
        }
    }
}