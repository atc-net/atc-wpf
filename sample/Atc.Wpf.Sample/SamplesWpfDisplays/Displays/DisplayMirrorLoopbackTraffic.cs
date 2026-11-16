namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Builds the traffic of the e-paper, MAX7219 matrix and ST7920 displays of the loopback demo, as the device libraries send it.
/// </summary>
public static class DisplayMirrorLoopbackTraffic
{
    /// <summary>The native width of the loopback e-paper (2.9").</summary>
    public const int EPaperWidth = 128;

    /// <summary>The native height of the loopback e-paper (2.9").</summary>
    public const int EPaperHeight = 296;

    /// <summary>The number of modules of the loopback matrix.</summary>
    public const int MatrixModules = 4;

    private const byte InstructionStart = 0xF8;
    private const byte DataStart = 0xFA;
    private const int LcdHalfRows = 32;
    private const int LcdRowBytes = 16;

    /// <summary>
    /// Creates an EPaperFrame payload: two lines of text, upright in landscape (GxEPD2 <c>setRotation(1)</c>, the control's default
    /// rotation 90), and a red bar under them.
    /// </summary>
    /// <param name="lines">The text lines, drawn twice the size of the 5 x 8 font.</param>
    /// <param name="isPartial">Whether the refresh is partial.</param>
    /// <returns>The payload.</returns>
    public static byte[] CreateEPaperFrame(
        IReadOnlyList<string> lines,
        bool isPartial)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var (black, red) = EPaperDisplayCaptures.CreatePicture(EPaperWidth, EPaperWidth, EPaperHeight, hasRed: true, lines);
        return DisplayMirrorProtocol.CreateEPaperFramePayload(EPaperWidth, EPaperHeight, black, red, isPartial);
    }

    /// <summary>
    /// Creates the latch frames of MD_MAX72XX <c>begin()</c> for the chain (§LIB.MDMAX72XX.1): test off, scan limit 7, intensity 7,
    /// decode off, then shutdown off; each a full-length frame with the same pair for every device.
    /// </summary>
    /// <returns>The frames.</returns>
    public static IReadOnlyList<byte[]> CreateMatrixBegin()
    {
        byte[][] pairs = [[0x0F, 0x00], [0x0B, 0x07], [0x0A, 0x07], [0x09, 0x00], [0x0C, 0x01]];
        return pairs.Select(pair => Enumerable.Repeat(pair, MatrixModules).SelectMany(p => p).ToArray()).ToList();
    }

    /// <summary>
    /// Creates the eight latch frames (one per row) that show a window of scrolling text on an FC-16 chain: digit = row, bit 7 = leftmost
    /// LED of a module, the first pair for the leftmost (farthest) module (§MAX7219.6, §MAX7219.7).
    /// </summary>
    /// <param name="text">The text, at most 22 characters.</param>
    /// <param name="offset">The scroll position in pixels.</param>
    /// <returns>The frames.</returns>
    public static IReadOnlyList<byte[]> CreateMatrixScroll(
        string text,
        int offset)
    {
        ArgumentNullException.ThrowIfNull(text);

        var scratch = new MonochromeFrameBuffer(132, 8);
        scratch.DrawText(0, 0, text);
        var textWidth = System.Math.Max(1, System.Math.Min(132, text.Length * 6));

        var frames = new List<byte[]>();
        for (var row = 0; row < 8; row++)
        {
            var frame = new byte[MatrixModules * 2];
            for (var module = 0; module < MatrixModules; module++)
            {
                byte value = 0;
                for (var x = 0; x < 8; x++)
                {
                    var column = ((module * 8) + x + offset) % textWidth;
                    if (scratch.GetPixel(column, row))
                    {
                        value |= (byte)(0x80 >> x);
                    }
                }

                frame[module * 2] = (byte)(row + 1);
                frame[(module * 2) + 1] = value;
            }

            frames.Add(frame);
        }

        return frames;
    }

    /// <summary>
    /// Creates the serial (SPI) bytes that set up an ST7920 and write two text lines, as U8g2 and ESPHome frame them (§ST7920.6):
    /// basic set, display on, entry mode, the lines at DDRAM 0x80 and 0x90, then the extended set with graphics on.
    /// </summary>
    /// <param name="line1">The first text line (16 characters are shown).</param>
    /// <param name="line2">The second text line.</param>
    /// <returns>The serial bytes.</returns>
    public static byte[] CreateLcdText(
        string line1,
        string line2)
    {
        ArgumentNullException.ThrowIfNull(line1);
        ArgumentNullException.ThrowIfNull(line2);

        var bytes = new List<byte>();
        AddInstructions(bytes, 0x30, 0x0C, 0x06, 0x80);
        AddData(bytes, Encoding.ASCII.GetBytes(line1.PadRight(16)[..16]));
        AddInstructions(bytes, 0x90);
        AddData(bytes, Encoding.ASCII.GetBytes(line2.PadRight(16)[..16]));

        // 0x34 sets RE (basic form), 0x36 in the extended form sets G: graphics on (§ST7920.3).
        AddInstructions(bytes, 0x34, 0x36);
        return [.. bytes];
    }

    /// <summary>
    /// Creates the serial bytes that write the bottom half of the glass (rows 32 to 63) into GDRAM, as U8g2 draws a tile row:
    /// per row the vertical address, the horizontal address 8 (the 128 x 64 fold, §ST7920.5) and one data run of 16 bytes.
    /// </summary>
    /// <param name="picture">A 128 x 32 picture.</param>
    /// <returns>The serial bytes.</returns>
    public static byte[] CreateLcdGraphics(MonochromeFrameBuffer picture)
    {
        ArgumentNullException.ThrowIfNull(picture);

        var bytes = new List<byte>();
        AddInstructions(bytes, 0x34, 0x36);
        var row = new byte[LcdRowBytes];
        for (var y = 0; y < LcdHalfRows; y++)
        {
            Array.Clear(row);
            for (var x = 0; x < 128; x++)
            {
                if (picture.GetPixel(x, y))
                {
                    row[x / 8] |= (byte)(0x80 >> (x % 8));
                }
            }

            AddInstructions(bytes, (byte)(0x80 | y), 0x80 | 8);
            AddData(bytes, row);
        }

        return [.. bytes];
    }

    /// <summary>
    /// Adds instruction bytes, each with its own start byte (§LIB.U8G2ST7920: <c>F8, c &amp; F0, c &lt;&lt; 4</c>).
    /// </summary>
    /// <param name="bytes">The stream.</param>
    /// <param name="instructions">The instructions.</param>
    private static void AddInstructions(
        ICollection<byte> bytes,
        params byte[] instructions)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(instructions);

        foreach (var instruction in instructions)
        {
            bytes.Add(InstructionStart);
            bytes.Add((byte)(instruction & 0xF0));
            bytes.Add((byte)((instruction << 4) & 0xF0));
        }
    }

    /// <summary>
    /// Adds a data run: one start byte, then each byte as two nibble bytes (§LIB.U8G2ST7920).
    /// </summary>
    /// <param name="bytes">The stream.</param>
    /// <param name="data">The data bytes.</param>
    private static void AddData(
        ICollection<byte> bytes,
        IReadOnlyList<byte> data)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(data);

        bytes.Add(DataStart);
        foreach (var value in data)
        {
            bytes.Add((byte)(value & 0xF0));
            bytes.Add((byte)((value << 4) & 0xF0));
        }
    }
}