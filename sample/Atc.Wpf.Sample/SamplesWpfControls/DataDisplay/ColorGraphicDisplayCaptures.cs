namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

/// <summary>
/// 4-wire SPI captures of an ESP32 sketch that initializes a colour display module with its Adafruit library
/// (Adafruit_ST7735 / Adafruit_ST7789, Adafruit_ILI9341, Adafruit_SSD1331, Adafruit_SSD1351), sets rotation 0 and sends one scene:
/// colour bars, "Hello ESP32", the module size, a filled rectangle and a filled circle.
/// </summary>
/// <remarks>
/// The command bytes and their D/C framing are the libraries' (restated as facts, no library code is copied); the library delays put no bytes
/// on the wire and are left out. The scene is drawn with a scratch <see cref="ColorFrameBuffer"/> and sent as one address window over the
/// panel followed by every pixel as RGB565, high byte first (the libraries' <c>writePixels</c> order), split into bands of rows so a replay
/// shows the picture fill from the top.
/// </remarks>
public static class ColorGraphicDisplayCaptures
{
    private const string SceneText = "Hello ESP32";
    private const int BandCount = 16;

    // The CASET / RASET of each ST7735 tab's Rcmd2 list.
    private static readonly byte[][] St7735GreenWindow = [[0x00, 0x02, 0x00, 0x81], [0x00, 0x01, 0x00, 0xA0]];
    private static readonly byte[][] St7735Green144Window = [[0x00, 0x00, 0x00, 0x7F], [0x00, 0x00, 0x00, 0x7F]];
    private static readonly byte[][] St7735MiniWindow = [[0x00, 0x00, 0x00, 0x4F], [0x00, 0x00, 0x00, 0x9F]];

    /// <summary>
    /// Creates the capture of a preset: the library init, rotation 0 and the scene.
    /// </summary>
    /// <param name="presetName">The <see cref="ColorGraphicDisplayPreset.Name"/> of one of <see cref="ColorGraphicDisplayPreset.All"/>.</param>
    /// <returns>The steps in the order they are sent.</returns>
    /// <exception cref="ArgumentException"><paramref name="presetName"/> is not the name of a preset.</exception>
    public static IReadOnlyList<GraphicDisplayCaptureStep> Create(
        string presetName)
    {
        var preset = ColorGraphicDisplayPreset.All.FirstOrDefault(p => p.Name == presetName)
            ?? throw new ArgumentException($"'{presetName}' is not a preset.", nameof(presetName));

        var pixels = ToRgb565BigEndian(CreateScene(preset.Columns, preset.Rows));
        var steps = new List<GraphicDisplayCaptureStep>();

        switch (presetName)
        {
            case ColorGraphicDisplayPreset.St7735Mini:
                AddSt7735Init(steps, St7735MiniWindow, isMini: true);
                AddTftFrame(steps, preset, pixels, xStart: 24, yStart: 0);
                break;
            case ColorGraphicDisplayPreset.St7735Green144:
                AddSt7735Init(steps, St7735Green144Window, isMini: false);

                // Rotation 0 of the 1.44 inch tab adds row 3 (the RAM is 132 rows, the panel window starts at row 1 in native order).
                AddTftFrame(steps, preset, pixels, xStart: 2, yStart: 3);
                break;
            case ColorGraphicDisplayPreset.St7735Green:
                AddSt7735Init(steps, St7735GreenWindow, isMini: false);
                AddTftFrame(steps, preset, pixels, xStart: 2, yStart: 1);
                break;
            case ColorGraphicDisplayPreset.St7789Small:
                AddSt7789Init(steps);
                AddTftFrame(steps, preset, pixels, xStart: 53, yStart: 40);
                break;
            case ColorGraphicDisplayPreset.St7789Square:
                AddSt7789Init(steps);
                AddTftFrame(steps, preset, pixels, xStart: 0, yStart: 80);
                break;
            case ColorGraphicDisplayPreset.St7789Large:
                AddSt7789Init(steps);
                AddTftFrame(steps, preset, pixels, xStart: 0, yStart: 0);
                break;
            case ColorGraphicDisplayPreset.Ili9341:
                AddIli9341Init(steps);
                AddTftFrame(steps, preset, pixels, xStart: 0, yStart: 0);
                break;
            case ColorGraphicDisplayPreset.Ssd1331:
                AddSsd1331(steps, preset, pixels);
                break;
            default:
                AddSsd1351(steps, preset, pixels);
                break;
        }

        return steps;
    }

    /// <summary>
    /// Draws the scene of the captures: colour bars (white on the left to blue on the right) over the top quarter, "Hello ESP32" and
    /// the size below them, a green filled square at the bottom left and a red filled circle at the bottom right, on black.
    /// </summary>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>A buffer with the scene.</returns>
    public static ColorFrameBuffer CreateScene(
        int width,
        int height)
    {
        Color[] bars = [Colors.White, Colors.Yellow, Colors.Cyan, Colors.Lime, Colors.Magenta, Colors.Red, Colors.Blue];
        var scene = new ColorFrameBuffer(width, height);
        var barHeight = height / 4;
        var scale = width >= 200 ? 2 : 1;
        var radius = System.Math.Min(width, height) / 6;

        for (var i = 0; i < bars.Length; i++)
        {
            var left = i * width / bars.Length;
            var right = (i + 1) * width / bars.Length;
            scene.FillRectangle(left, 0, right - left, barHeight, bars[i]);
        }

        scene.DrawText(2, barHeight + 3, SceneText, Colors.White, scale, CharacterRom.A00);
        scene.DrawText(
            2,
            barHeight + 3 + (9 * scale),
            string.Create(CultureInfo.InvariantCulture, $"{width}x{height}"),
            Colors.Yellow,
            scale,
            CharacterRom.A00);

        scene.FillRectangle(2, height - (2 * radius) - 3, 2 * radius, 2 * radius, Colors.Lime);

        var centerX = width - radius - 3;
        var centerY = height - radius - 3;
        for (var dy = -radius; dy <= radius; dy++)
        {
            var half = (int)System.Math.Sqrt((radius * radius) - (dy * dy));
            scene.DrawLine(centerX - half, centerY + dy, centerX + half, centerY + dy, Colors.Red);
        }

        return scene;
    }

    /// <summary>
    /// Converts a buffer to RGB565 bytes, high byte first, row by row from the top left.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Two bytes per pixel.</returns>
    public static byte[] ToRgb565BigEndian(ColorFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        var bytes = new byte[buffer.Width * buffer.Height * 2];
        var index = 0;
        for (var y = 0; y < buffer.Height; y++)
        {
            for (var x = 0; x < buffer.Width; x++)
            {
                var color = buffer.GetPixel(x, y);
                var word = ((color.R >> 3) << 11) | ((color.G >> 2) << 5) | (color.B >> 3);
                bytes[index++] = (byte)(word >> 8);
                bytes[index++] = (byte)word;
            }
        }

        return bytes;
    }

    // Adafruit_ST7735 initR(): Rcmd1, the tab's Rcmd2, Rcmd3; the mini tab then sends MADCTL 0xC0; setRotation(0) sends the MADCTL of rotation 0.
    private static void AddSt7735Init(
        List<GraphicDisplayCaptureStep> steps,
        byte[][] window,
        bool isMini)
    {
        Send(steps, 0x01);
        Send(steps, 0x11);
        Send(steps, 0xB1, 0x01, 0x2C, 0x2D);
        Send(steps, 0xB2, 0x01, 0x2C, 0x2D);
        Send(steps, 0xB3, 0x01, 0x2C, 0x2D, 0x01, 0x2C, 0x2D);
        Send(steps, 0xB4, 0x07);
        Send(steps, 0xC0, 0xA2, 0x02, 0x84);
        Send(steps, 0xC1, 0xC5);
        Send(steps, 0xC2, 0x0A, 0x00);
        Send(steps, 0xC3, 0x8A, 0x2A);
        Send(steps, 0xC4, 0x8A, 0xEE);
        Send(steps, 0xC5, 0x0E);
        Send(steps, 0x20);
        Send(steps, 0x36, 0xC8);
        Send(steps, 0x3A, 0x05);

        Send(steps, 0x2A, window[0]);
        Send(steps, 0x2B, window[1]);

        Send(steps, 0xE0, 0x02, 0x1C, 0x07, 0x12, 0x37, 0x32, 0x29, 0x2D, 0x29, 0x25, 0x2B, 0x39, 0x00, 0x01, 0x03, 0x10);
        Send(steps, 0xE1, 0x03, 0x1D, 0x07, 0x06, 0x2E, 0x2C, 0x29, 0x2D, 0x2E, 0x2E, 0x37, 0x3F, 0x00, 0x00, 0x02, 0x10);
        Send(steps, 0x13);
        Send(steps, 0x29);

        if (isMini)
        {
            Send(steps, 0x36, 0xC0);
        }

        // setRotation(0): MX | MY, with the BGR bit for the green tabs and without it for the mini.
        Send(steps, 0x36, isMini ? (byte)0xC0 : (byte)0xC8);
    }

    // Adafruit_ST7789 init(): generic_st7789, then setRotation(0) = MX | MY (RGB).
    private static void AddSt7789Init(List<GraphicDisplayCaptureStep> steps)
    {
        Send(steps, 0x01);
        Send(steps, 0x11);
        Send(steps, 0x3A, 0x55);
        Send(steps, 0x36, 0x08);
        Send(steps, 0x2A, 0x00, 0x00, 0x00, 0xF0);
        Send(steps, 0x2B, 0x00, 0x00, 0x01, 0x40);
        Send(steps, 0x21);
        Send(steps, 0x13);
        Send(steps, 0x29);
        Send(steps, 0x36, 0xC0);
    }

    // Adafruit_ILI9341 begin() without a reset pin: software reset, then initcmd (MADCTL 0x48, MX | BGR, is rotation 0), then the sketch's setRotation(0).
    private static void AddIli9341Init(List<GraphicDisplayCaptureStep> steps)
    {
        Send(steps, 0x01);
        Send(steps, 0xEF, 0x03, 0x80, 0x02);
        Send(steps, 0xCF, 0x00, 0xC1, 0x30);
        Send(steps, 0xED, 0x64, 0x03, 0x12, 0x81);
        Send(steps, 0xE8, 0x85, 0x00, 0x78);
        Send(steps, 0xCB, 0x39, 0x2C, 0x00, 0x34, 0x02);
        Send(steps, 0xF7, 0x20);
        Send(steps, 0xEA, 0x00, 0x00);
        Send(steps, 0xC0, 0x23);
        Send(steps, 0xC1, 0x10);
        Send(steps, 0xC5, 0x3E, 0x28);
        Send(steps, 0xC7, 0x86);
        Send(steps, 0x36, 0x48);

        // The library sends one parameter byte; the chip defines two.
        Send(steps, 0x37, 0x00);
        Send(steps, 0x3A, 0x55);
        Send(steps, 0xB1, 0x00, 0x18);
        Send(steps, 0xB6, 0x08, 0x82, 0x27);
        Send(steps, 0xF2, 0x00);
        Send(steps, 0x26, 0x01);
        Send(steps, 0xE0, 0x0F, 0x31, 0x2B, 0x0C, 0x0E, 0x08, 0x4E, 0xF1, 0x37, 0x07, 0x10, 0x03, 0x0E, 0x09, 0x00);
        Send(steps, 0xE1, 0x00, 0x0E, 0x14, 0x03, 0x11, 0x07, 0x31, 0xC1, 0x48, 0x08, 0x0F, 0x0C, 0x31, 0x36, 0x0F);
        Send(steps, 0x11);
        Send(steps, 0x29);

        // The sketch's setRotation(0): MX | BGR, the same value begin() left.
        Send(steps, 0x36, 0x48);
    }

    // setAddrWindow(0, 0, width, height) plus the rotation's (xstart, ystart): CASET, RASET, RAMWR, then the pixels.
    private static void AddTftFrame(
        List<GraphicDisplayCaptureStep> steps,
        ColorGraphicDisplayPreset preset,
        byte[] pixels,
        int xStart,
        int yStart)
    {
        var xEnd = xStart + preset.Columns - 1;
        var yEnd = yStart + preset.Rows - 1;
        Send(steps, 0x2A, (byte)(xStart >> 8), (byte)xStart, (byte)(xEnd >> 8), (byte)xEnd);
        Send(steps, 0x2B, (byte)(yStart >> 8), (byte)yStart, (byte)(yEnd >> 8), (byte)yEnd);
        Send(steps, 0x2C);
        AddPixels(steps, preset, pixels);
    }

    // Adafruit_SSD1331: every byte, parameters included, is sent with D/C low.
    private static void AddSsd1331(
        List<GraphicDisplayCaptureStep> steps,
        ColorGraphicDisplayPreset preset,
        byte[] pixels)
    {
        // begin() sends SETREMAP without its parameter, so the chip takes 0xA1 (meant as STARTLINE) as the remap and 0x00 as an unknown command.
        byte[] init =
        [
            0xAE, 0xA0, 0xA1, 0x00, 0xA2, 0x00, 0xA4, 0xA8, 0x3F, 0xAD, 0x8E, 0xB0, 0x0B, 0xB1, 0x31, 0xB3, 0xF0,
            0x8A, 0x64, 0x8B, 0x78, 0x8C, 0x64, 0xBB, 0x3A, 0xBE, 0x3E, 0x87, 0x06, 0x81, 0x91, 0x82, 0x50, 0x83, 0x7D, 0xAF,
        ];

        foreach (var command in init)
        {
            steps.Add(new GraphicDisplayCaptureStep(IsData: false, [command]));
        }

        // setRotation(0): remap 0x76 (65k, COM split, C-B-A order, scan bottom-up, column remap).
        steps.Add(new GraphicDisplayCaptureStep(IsData: false, [0xA0, 0x76]));

        // setAddrWindow(0, 0, 96, 64): column and row windows in command mode, then the pixels with D/C high (no RAM-write command).
        steps.Add(new GraphicDisplayCaptureStep(
            IsData: false,
            [0x15, 0x00, (byte)(preset.Columns - 1), 0x75, 0x00, (byte)(preset.Rows - 1)]));
        AddPixels(steps, preset, pixels);
    }

    // Adafruit_SSD1351: the command byte with D/C low, its parameters with D/C high; pixels only after 0x5C.
    private static void AddSsd1351(
        List<GraphicDisplayCaptureStep> steps,
        ColorGraphicDisplayPreset preset,
        byte[] pixels)
    {
        Send(steps, 0xFD, 0x12);
        Send(steps, 0xFD, 0xB1);
        Send(steps, 0xAE);
        Send(steps, 0xB3, 0xF1);
        Send(steps, 0xCA, 0x7F);
        Send(steps, 0xA2, 0x00);
        Send(steps, 0xB5, 0x00);
        Send(steps, 0xAB, 0x01);
        Send(steps, 0xB1, 0x32);
        Send(steps, 0xBE, 0x05);
        Send(steps, 0xA6);
        Send(steps, 0xC1, 0xC8, 0x80, 0xC8);
        Send(steps, 0xC7, 0x0F);
        Send(steps, 0xB4, 0xA0, 0xB5, 0x55);
        Send(steps, 0xB6, 0x01);
        Send(steps, 0xAF);

        // setRotation(0): remap 0x74 and the start line = the panel height (128 or 96).
        Send(steps, 0xA0, 0x74);
        Send(steps, 0xA1, (byte)preset.Rows);

        Send(steps, 0x15, 0x00, (byte)(preset.Columns - 1));
        Send(steps, 0x75, 0x00, (byte)(preset.Rows - 1));
        Send(steps, 0x5C);
        AddPixels(steps, preset, pixels);
    }

    private static void AddPixels(
        List<GraphicDisplayCaptureStep> steps,
        ColorGraphicDisplayPreset preset,
        byte[] pixels)
    {
        var rowBytes = preset.Columns * 2;
        var bandRows = System.Math.Max(1, preset.Rows / BandCount);
        for (var row = 0; row < preset.Rows; row += bandRows)
        {
            var rows = System.Math.Min(bandRows, preset.Rows - row);
            steps.Add(new GraphicDisplayCaptureStep(IsData: true, pixels.AsSpan(row * rowBytes, rows * rowBytes).ToArray()));
        }
    }

    // One command byte (D/C low), then its parameters (D/C high) as a second step.
    private static void Send(
        List<GraphicDisplayCaptureStep> steps,
        byte command,
        params byte[] parameters)
    {
        steps.Add(new GraphicDisplayCaptureStep(IsData: false, [command]));
        if (parameters.Length > 0)
        {
            steps.Add(new GraphicDisplayCaptureStep(IsData: true, parameters));
        }
    }
}