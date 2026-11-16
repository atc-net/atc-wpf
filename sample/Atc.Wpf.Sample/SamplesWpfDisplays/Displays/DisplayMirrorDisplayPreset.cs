namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A display the mirror sample can add by hand before a device describes it: a common module as a Describe would name it.
/// </summary>
/// <param name="Name">The module name.</param>
/// <param name="Kind">The display kind.</param>
/// <param name="Chip">The controller chip.</param>
/// <param name="Width">The width in pixels (columns for a character display, digits for a seven-segment module).</param>
/// <param name="Height">The height in pixels (rows for a character display, 1 for a seven-segment module).</param>
public sealed record DisplayMirrorDisplayPreset(
    string Name,
    DisplayMirrorDisplayKind Kind,
    DisplayMirrorChip Chip,
    int Width,
    int Height)
{
    /// <summary>
    /// Gets the presets: the character sizes, then the modules of the OledDisplay, ColorGraphicDisplay, EPaperDisplay, LedMatrixDisplay,
    /// SevenSegmentDisplay and GraphicLcdDisplay samples.
    /// </summary>
    public static IReadOnlyList<DisplayMirrorDisplayPreset> All { get; } = CreateAll();

    /// <summary>
    /// Creates the Describe a device with this module would send.
    /// </summary>
    /// <returns>The description.</returns>
    public DisplayMirrorDescribe ToDescribe()
        => new(Kind, Chip, Width, Height, Name);

    /// <inheritdoc />
    public override string ToString()
        => Name;

    private static List<DisplayMirrorDisplayPreset> CreateAll()
    {
        var result = new List<DisplayMirrorDisplayPreset>();

        foreach (var (columns, rows) in new[] { (16, 2), (16, 4), (20, 2), (20, 4), (40, 2) })
        {
            result.Add(new DisplayMirrorDisplayPreset(
                string.Create(CultureInfo.InvariantCulture, $"Character LCD {columns} x {rows} (HD44780, PCF8574 backpack)"),
                DisplayMirrorDisplayKind.CharacterLcd,
                DisplayMirrorChip.Hd44780,
                columns,
                rows));
        }

        result.AddRange(OledDisplayPreset.All.Select(preset => new DisplayMirrorDisplayPreset(
            preset.Name,
            DisplayMirrorDisplayKind.MonoOled,
            DisplayMirrorCardFactory.ToMirrorChip(preset.Chip),
            preset.Columns,
            preset.Rows)));

        result.AddRange(ColorGraphicDisplayPreset.All.Select(preset => new DisplayMirrorDisplayPreset(
            preset.Name,
            DisplayMirrorDisplayKind.ColorPanel,
            DisplayMirrorCardFactory.ChipOf(preset),
            preset.Columns,
            preset.Rows)));

        result.AddRange(EPaperDisplayPreset.All.Select(preset => new DisplayMirrorDisplayPreset(
            preset.Name,
            DisplayMirrorDisplayKind.EPaper,
            DisplayMirrorChip.Ssd1680,
            preset.Columns,
            preset.Rows)));

        result.AddRange(LedMatrixDisplayPreset.All.Select(preset => new DisplayMirrorDisplayPreset(
            preset.Name,
            DisplayMirrorDisplayKind.LedMatrix,
            DisplayMirrorChip.Max7219,
            preset.ModuleCount * 8,
            8)));

        foreach (var digits in new[] { 4, 8 })
        {
            result.Add(new DisplayMirrorDisplayPreset(
                string.Create(CultureInfo.InvariantCulture, $"7-segment, {digits} digits (MAX7219)"),
                DisplayMirrorDisplayKind.SevenSegment,
                DisplayMirrorChip.Max7219,
                digits,
                1));
        }

        result.AddRange(GraphicLcdDisplayPreset.All.Select(preset => new DisplayMirrorDisplayPreset(
            preset.Name,
            DisplayMirrorDisplayKind.GraphicLcd,
            DisplayMirrorChip.St7920,
            GraphicLcdDisplay.PixelColumns,
            GraphicLcdDisplay.PixelRows)));

        return result;
    }
}