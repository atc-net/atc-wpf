namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

/// <summary>
/// A common colour display module: its size in pixels, its panel kind and the chip emulator that drives it.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="Columns">The pixel columns (the width with the library's rotation 0).</param>
/// <param name="Rows">The pixel rows.</param>
/// <param name="Kind">The panel kind.</param>
/// <param name="CreateController">Creates the chip emulator on a buffer, configured for the module (panel window, RAM size, colour order).</param>
public sealed record ColorGraphicDisplayPreset(
    string Name,
    int Columns,
    int Rows,
    ColorPanelKind Kind,
    Func<ColorFrameBuffer, IGraphicDisplayController> CreateController)
{
    /// <summary>The 0.96 inch 80 x 160 ST7735 module (Adafruit INITR_MINI160x80).</summary>
    public const string St7735Mini = "0.96\" 80 x 160 ST7735";

    /// <summary>The 1.44 inch 128 x 128 ST7735 module (Adafruit INITR_144GREENTAB).</summary>
    public const string St7735Green144 = "1.44\" 128 x 128 ST7735";

    /// <summary>The 1.8 inch 128 x 160 ST7735 module (Adafruit INITR_GREENTAB).</summary>
    public const string St7735Green = "1.8\" 128 x 160 ST7735";

    /// <summary>The 1.14 inch 135 x 240 ST7789 module.</summary>
    public const string St7789Small = "1.14\" 135 x 240 ST7789";

    /// <summary>The 1.3 inch 240 x 240 ST7789 module.</summary>
    public const string St7789Square = "1.3\" 240 x 240 ST7789";

    /// <summary>The 2.0 inch 240 x 320 ST7789 module.</summary>
    public const string St7789Large = "2.0\" 240 x 320 ST7789";

    /// <summary>The 2.4 inch / 2.8 inch 240 x 320 ILI9341 module.</summary>
    public const string Ili9341 = "2.4\" / 2.8\" 240 x 320 ILI9341";

    /// <summary>The 0.95 inch 96 x 64 SSD1331 colour OLED module.</summary>
    public const string Ssd1331 = "0.95\" 96 x 64 SSD1331";

    /// <summary>The 1.27 inch 128 x 96 SSD1351 colour OLED module.</summary>
    public const string Ssd1351Small = "1.27\" 128 x 96 SSD1351";

    /// <summary>The 1.5 inch 128 x 128 SSD1351 colour OLED module.</summary>
    public const string Ssd1351Large = "1.5\" 128 x 128 SSD1351";

    /// <summary>
    /// Gets the presets of the common modules. The panel windows, RAM sizes and colour orders follow the Adafruit libraries
    /// (ST7735: the green tabs are BGR, the 0.96 inch mini is RGB with the 128 x 160 RAM; ST7789: RGB; ILI9341: BGR;
    /// SSD1331 / SSD1351: the default C-B-A order).
    /// </summary>
    public static IReadOnlyList<ColorGraphicDisplayPreset> All { get; } =
    [
        new(St7735Mini, 80, 160, ColorPanelKind.Tft, CreateSt7735Mini),
        new(St7735Green144, 128, 128, ColorPanelKind.Tft, CreateSt7735Green144),
        new(St7735Green, 128, 160, ColorPanelKind.Tft, buffer => new TftController(buffer, TftChip.St7735, columnOffset: 2, rowOffset: 1)),
        new(St7789Small, 135, 240, ColorPanelKind.Tft, buffer => new TftController(buffer, TftChip.St7789, columnOffset: 52, rowOffset: 40)),
        new(St7789Square, 240, 240, ColorPanelKind.Tft, buffer => new TftController(buffer, TftChip.St7789)),
        new(St7789Large, 240, 320, ColorPanelKind.Tft, buffer => new TftController(buffer, TftChip.St7789)),
        new(Ili9341, 240, 320, ColorPanelKind.Tft, buffer => new TftController(buffer, TftChip.Ili9341)),
        new(Ssd1331, 96, 64, ColorPanelKind.Oled, buffer => new Ssd1331Controller(buffer)),
        new(Ssd1351Small, 128, 96, ColorPanelKind.Oled, buffer => new Ssd1351Controller(buffer)),
        new(Ssd1351Large, 128, 128, ColorPanelKind.Oled, buffer => new Ssd1351Controller(buffer)),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;

    // The 0.96 inch mini uses the 128 x 160 RAM mode (the mirror extents), shows RAM columns 24 to 103 and is wired RGB.
    private static TftController CreateSt7735Mini(ColorFrameBuffer buffer)
    {
        var controller = new TftController(buffer, TftChip.St7735, columnOffset: 24, rowOffset: 0);
        controller.SetRamSize(128, 160);
        controller.IsPanelBgr = false;
        return controller;
    }

    // The 1.44 inch green tab uses the 132 x 132 RAM mode and shows RAM columns 2 to 129, rows 1 to 128.
    private static TftController CreateSt7735Green144(ColorFrameBuffer buffer)
    {
        var controller = new TftController(buffer, TftChip.St7735, columnOffset: 2, rowOffset: 1);
        controller.SetRamSize(132, 132);
        return controller;
    }
}