namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Captures of an ESP32 sketch that initializes a 128-column monochrome OLED module and draws one scene
/// (a border, "Hello ESP32" and a filled box), as the common libraries send it:
/// Adafruit_SSD1306 for the SSD1306 and SSD1309 modules, U8g2 for the SH1106 module.
/// </summary>
/// <remarks>
/// The command bytes and their grouping into transactions are the libraries' (restated as facts, no library code is copied).
/// The frame bytes are drawn with a scratch <see cref="MonochromeFrameBuffer"/>, which has the same layout as the libraries' buffers:
/// byte <c>x + (y / 8) * width</c>, the least significant bit on top.
/// </remarks>
public static class OledDisplayCaptures
{
    private const byte CommandStream = 0x00;
    private const byte DataStream = 0x40;
    private const int Width = 128;
    private const int PageHeight = 8;
    private const string SceneText = "Hello ESP32";

    // Adafruit_SSD1306 flushes a data transaction at WIRE_MAX bytes (32 on the classic Arduino cores): the control byte and 31 data bytes.
    private const int AdafruitDataBytesPerTransaction = 31;

    // U8g2's I2C byte procedure sends at most 24 data bytes after the control byte.
    private const int U8g2DataBytesPerTransaction = 24;

    // A 128-wide SH1106 module shows RAM columns 2 to 129; U8g2 writes each page from column 2.
    private const int Sh1106ColumnOffset = 2;

    // U8g2 SSD1306/SH1106 128 x 64 "noname" init sequence; on the SH1106 the 0x8D and 0x20 bytes are ignored.
    private static readonly byte[] U8g2Init =
    [
        0xAE,
        0xD5, 0x80,
        0xA8, 0x3F,
        0xD3, 0x00,
        0x40,
        0x8D, 0x14,
        0x20, 0x00,
        0xA1,
        0xC8,
        0xDA, 0x12,
        0x81, 0xCF,
        0xD9, 0xF1,
        0xDB, 0x40,
        0x2E,
        0xA4,
        0xA6,
    ];

    /// <summary>
    /// Creates the I2C capture: the write transactions after the address byte, each starting with its control byte.
    /// </summary>
    /// <param name="chip">The chip of the module: an SH1106 gets the U8g2 page-by-page stream, the others the Adafruit_SSD1306 stream.</param>
    /// <param name="rows">The pixel rows of the module, 32 or 64 (an SH1106 module has 64).</param>
    /// <returns>The transactions in the order they are sent.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rows"/> is not 32 or 64, or is 32 for an SH1106.</exception>
    public static IReadOnlyList<byte[]> CreateI2cCapture(
        OledChip chip,
        int rows)
    {
        if (rows is not (32 or 64) ||
            (chip == OledChip.Sh1106 && rows != 64))
        {
            throw new ArgumentOutOfRangeException(nameof(rows), rows, "The captures cover 128 x 32 and 128 x 64 modules (SH1106: 128 x 64).");
        }

        var frame = CreateSceneFrame(rows);
        return chip == OledChip.Sh1106
            ? CreateU8g2Capture(frame)
            : CreateAdafruitCapture(frame, rows);
    }

    /// <summary>
    /// Creates the same capture as 4-wire SPI steps: the command bytes of each I2C transaction become one command step,
    /// and the display data is split into one step per page (128 bytes), so a replay shows the picture fill page by page.
    /// </summary>
    /// <param name="chip">The chip of the module.</param>
    /// <param name="rows">The pixel rows of the module, 32 or 64 (an SH1106 module has 64).</param>
    /// <returns>The steps in the order they are sent.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rows"/> is not 32 or 64, or is 32 for an SH1106.</exception>
    public static IReadOnlyList<GraphicDisplayCaptureStep> CreateSpiCapture(
        OledChip chip,
        int rows)
    {
        var steps = new List<GraphicDisplayCaptureStep>();
        var data = new List<byte>();

        foreach (var transaction in CreateI2cCapture(chip, rows))
        {
            if ((transaction[0] & DataStream) != 0)
            {
                data.AddRange(transaction.AsSpan(1));
                continue;
            }

            FlushData(steps, data);
            steps.Add(new GraphicDisplayCaptureStep(IsData: false, transaction[1..]));
        }

        FlushData(steps, data);
        return steps;
    }

    private static byte[] CreateSceneFrame(int rows)
    {
        var scratch = new MonochromeFrameBuffer(Width, rows);
        scratch.DrawRectangle(0, 0, Width, rows);
        scratch.DrawText(6, 6, SceneText);
        scratch.FillRectangle(92, rows - 20, 28, 14);
        return scratch.GetFrame();
    }

    // Adafruit_SSD1306 begin() then display(), with the transaction grouping of the library.
    private static List<byte[]> CreateAdafruitCapture(
        byte[] frame,
        int rows)
    {
        var multiplex = (byte)(rows - 1);
        byte comPins = rows == 32 ? (byte)0x02 : (byte)0x12;
        byte contrast = rows == 32 ? (byte)0x8F : (byte)0xCF;

        List<byte[]> transactions =
        [
            // begin(): display off, clock, multiplex (its parameter in its own transaction).
            [CommandStream, 0xAE, 0xD5, 0x80, 0xA8],
            [CommandStream, multiplex],

            // Display offset 0, start line 0, charge pump (its parameter: 0x14 = internal charge pump).
            [CommandStream, 0xD3, 0x00, 0x40, 0x8D],
            [CommandStream, 0x14],

            // Horizontal addressing, segment remap, COM scan reversed.
            [CommandStream, 0x20, 0x00, 0xA1, 0xC8],
            [CommandStream, 0xDA],
            [CommandStream, comPins],
            [CommandStream, 0x81],
            [CommandStream, contrast],
            [CommandStream, 0xD9],
            [CommandStream, 0xF1],

            // VCOMH, resume to RAM, normal, scroll off, display on.
            [CommandStream, 0xDB, 0x40, 0xA4, 0xA6, 0x2E, 0xAF],

            // display(): page window 0 to 0xFF (masked to 7 by the chip), then the column window 0 to 127.
            [CommandStream, 0x22, 0x00, 0xFF, 0x21],
            [CommandStream, 0x00],
            [CommandStream, Width - 1],
        ];

        AddData(transactions, frame, AdafruitDataBytesPerTransaction);
        return transactions;
    }

    // U8g2 begin() (init, clear display, power save off) then sendBuffer(), in the classic I2C framing:
    // every command byte is its own transaction.
    private static List<byte[]> CreateU8g2Capture(byte[] frame)
    {
        var transactions = new List<byte[]>();
        foreach (var command in U8g2Init)
        {
            transactions.Add([CommandStream, command]);
        }

        AddU8g2Frame(transactions, new byte[frame.Length]);
        transactions.Add([CommandStream, 0xAF]);
        AddU8g2Frame(transactions, frame);
        return transactions;
    }

    // One page-mode write per page: start line 0, column high and low nibble (with the column offset), page, then 128 data bytes.
    private static void AddU8g2Frame(
        List<byte[]> transactions,
        byte[] frame)
    {
        for (var page = 0; page < frame.Length / Width; page++)
        {
            transactions.Add([CommandStream, 0x40]);
            transactions.Add([CommandStream, (byte)(0x10 | (Sh1106ColumnOffset >> 4))]);
            transactions.Add([CommandStream, (byte)(Sh1106ColumnOffset & 0x0F)]);
            transactions.Add([CommandStream, (byte)(0xB0 | page)]);
            AddData(transactions, frame.AsSpan(page * Width, Width), U8g2DataBytesPerTransaction);
        }
    }

    private static void AddData(
        List<byte[]> transactions,
        ReadOnlySpan<byte> data,
        int bytesPerTransaction)
    {
        for (var offset = 0; offset < data.Length; offset += bytesPerTransaction)
        {
            var chunk = data.Slice(offset, System.Math.Min(bytesPerTransaction, data.Length - offset));
            var transaction = new byte[chunk.Length + 1];
            transaction[0] = DataStream;
            chunk.CopyTo(transaction.AsSpan(1));
            transactions.Add(transaction);
        }
    }

    private static void FlushData(
        List<GraphicDisplayCaptureStep> steps,
        List<byte> data)
    {
        for (var offset = 0; offset < data.Count; offset += Width)
        {
            var count = System.Math.Min(Width, data.Count - offset);
            steps.Add(new GraphicDisplayCaptureStep(IsData: true, data.GetRange(offset, count).ToArray()));
        }

        data.Clear();
    }
}