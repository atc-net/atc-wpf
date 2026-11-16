namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Captures of an ESP32 sketch that initializes an ST7920 128 x 64 module over its serial (SPI) interface and draws one scene
/// (a border, "Hello ESP32" and a filled box), as U8g2 and ESPHome send it: the bytes as they go over the wire, start bytes and nibble pairs.
/// </summary>
/// <remarks>
/// The instruction bytes and the framing are the libraries', restated from §LIB.U8G2ST7920 and §LIB.ESPHOMEST7920 (no library code is copied).
/// </remarks>
public static class GraphicLcdDisplayCaptures
{
    private const byte InstructionStart = 0xF8;
    private const byte DataStart = 0xFA;
    private const int Width = 128;
    private const int Height = 64;
    private const int HalfHeight = 32;
    private const int RowBytes = Width / 8;

    /// <summary>
    /// Draws the scene: a border, "Hello ESP32" and a filled box.
    /// </summary>
    /// <returns>The scene, 128 x 64.</returns>
    public static MonochromeFrameBuffer CreateScene()
    {
        var scene = new MonochromeFrameBuffer(Width, Height);
        scene.DrawRectangle(0, 0, Width, Height);
        scene.DrawText(8, 8, "Hello ESP32");
        scene.DrawText(8, 20, "ST7920 12864");
        scene.FillRectangle(88, 36, 28, 18);
        return scene;
    }

    /// <summary>
    /// Creates the U8g2 capture (§LIB.U8G2ST7920): init, power save off, then one transfer per tile row of 8 glass rows:
    /// <c>3E</c> twice, then per row the vertical address, the horizontal address and one data run of the row.
    /// </summary>
    /// <returns>The serial transfers in the order they are sent.</returns>
    public static IReadOnlyList<byte[]> CreateU8g2Capture()
    {
        var transfers = new List<byte[]>
        {
            Instructions(0x38, 0x08, 0x06, 0x02, 0x01),
            Instructions(0x38, 0x0C),
        };

        var scene = CreateScene();
        for (var tileY = 0; tileY < Height / 8; tileY++)
        {
            // The 128 x 64 fold: rows 32 to 63 are GDRAM rows 0 to 31, words 8 to 15 (§ST7920.5).
            var y = tileY * 8;
            var x = 0;
            if (y >= HalfHeight)
            {
                y -= HalfHeight;
                x += 8;
            }

            var bytes = new List<byte>();
            AddInstructions(bytes, 0x3E, 0x3E);
            for (var i = 0; i < 8; i++)
            {
                AddInstructions(bytes, (byte)(0x80 | (y + i)), (byte)(0x80 | x));
                AddData(bytes, Row(scene, (tileY * 8) + i), isStartPerByte: false);
            }

            transfers.Add([.. bytes]);
        }

        return transfers;
    }

    /// <summary>
    /// Creates the ESPHome capture (§LIB.ESPHOMEST7920): init, then per GDRAM row the address and the bytes of glass rows
    /// <c>j</c> and <c>j + 32</c>, every byte with its own start byte.
    /// </summary>
    /// <returns>The serial transfers in the order they are sent.</returns>
    public static IReadOnlyList<byte[]> CreateEspHomeCapture()
    {
        var transfers = new List<byte[]>
        {
            Instructions(0x30, 0x30, 0x01),
            Instructions(0x06, 0x0C, 0x34, 0x36),
        };

        var scene = CreateScene();
        for (var j = 0; j < HalfHeight; j++)
        {
            var bytes = new List<byte>();
            AddInstructions(bytes, (byte)(0x80 | j), 0x80);
            AddData(bytes, [.. Row(scene, j), .. Row(scene, j + HalfHeight)], isStartPerByte: true);
            transfers.Add([.. bytes]);
        }

        return transfers;
    }

    private static byte[] Row(
        MonochromeFrameBuffer scene,
        int y)
    {
        var row = new byte[RowBytes];
        for (var x = 0; x < Width; x++)
        {
            if (scene.GetPixel(x, y))
            {
                row[x / 8] |= (byte)(0x80 >> (x % 8));
            }
        }

        return row;
    }

    private static byte[] Instructions(params byte[] instructions)
    {
        var bytes = new List<byte>();
        AddInstructions(bytes, instructions);
        return [.. bytes];
    }

    // Every instruction with its own start byte: F8, high nibble, low nibble (§ST7920.6).
    private static void AddInstructions(
        ICollection<byte> bytes,
        params byte[] instructions)
    {
        foreach (var instruction in instructions)
        {
            bytes.Add(InstructionStart);
            bytes.Add((byte)(instruction & 0xF0));
            bytes.Add((byte)((instruction << 4) & 0xF0));
        }
    }

    // U8g2 sends one FA for the run; ESPHome repeats FA before every byte.
    private static void AddData(
        ICollection<byte> bytes,
        byte[] data,
        bool isStartPerByte)
    {
        if (!isStartPerByte)
        {
            bytes.Add(DataStart);
        }

        foreach (var value in data)
        {
            if (isStartPerByte)
            {
                bytes.Add(DataStart);
            }

            bytes.Add((byte)(value & 0xF0));
            bytes.Add((byte)((value << 4) & 0xF0));
        }
    }
}