namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

/// <summary>
/// A common monochrome OLED module: its size in pixels, its controller chip and its colour.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="Columns">The pixel columns.</param>
/// <param name="Rows">The pixel rows.</param>
/// <param name="Chip">The controller chip.</param>
/// <param name="Color">The pixel colour.</param>
public sealed record OledDisplayPreset(
    string Name,
    int Columns,
    int Rows,
    OledChip Chip,
    OledDisplayColor Color)
{
    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public static IReadOnlyList<OledDisplayPreset> All { get; } =
    [
        new("0.91\" 128 x 32 SSD1306 (white)", 128, 32, OledChip.Ssd1306, OledDisplayColor.White),
        new("0.96\" 128 x 64 SSD1306 (white)", 128, 64, OledChip.Ssd1306, OledDisplayColor.White),
        new("0.96\" 128 x 64 SSD1306 (blue)", 128, 64, OledChip.Ssd1306, OledDisplayColor.Blue),
        new("0.96\" 128 x 64 SSD1306 (yellow-blue)", 128, 64, OledChip.Ssd1306, OledDisplayColor.YellowBlue),
        new("1.3\" 128 x 64 SH1106 (white)", 128, 64, OledChip.Sh1106, OledDisplayColor.White),
        new("1.54\" 128 x 64 SSD1309 (white)", 128, 64, OledChip.Ssd1309, OledDisplayColor.White),
        new("2.42\" 128 x 64 SSD1309 (yellow)", 128, 64, OledChip.Ssd1309, OledDisplayColor.Yellow),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}