namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A common SSD1680 e-paper module: its native (portrait) size, whether it is black/white/red and the GxEPD2 class that drives it.
/// </summary>
/// <param name="Name">The name shown in the sample.</param>
/// <param name="Columns">The native pixel columns of the RAM and of the frames the libraries send (128 for both sizes).</param>
/// <param name="VisibleColumns">The native columns the glass shows: 122 on the 2.13" panels (bits 122-127 of a row are padding), 128 on the 2.9".</param>
/// <param name="Rows">The native pixel rows.</param>
/// <param name="HasRed">Whether the panel is black/white/red.</param>
/// <param name="LibraryClass">The GxEPD2 class whose traffic the replay shows.</param>
public sealed record EPaperDisplayPreset(
    string Name,
    int Columns,
    int VisibleColumns,
    int Rows,
    bool HasRed,
    string LibraryClass)
{
    /// <summary>
    /// Gets the presets of the common modules (§SSD1680.1, §LIB.GXEPD2).
    /// </summary>
    public static IReadOnlyList<EPaperDisplayPreset> All { get; } =
    [
        new("2.13\" 122 x 250 SSD1680 (black/white)", 128, 122, 250, HasRed: false, "GxEPD2_213_B74"),
        new("2.13\" 122 x 250 SSD1680 (black/white/red)", 128, 122, 250, HasRed: true, "GxEPD2_213_Z98c"),
        new("2.9\" 128 x 296 SSD1680 (black/white)", 128, 128, 296, HasRed: false, "GxEPD2_290_T94"),
        new("2.9\" 128 x 296 SSD1680 (black/white/red)", 128, 128, 296, HasRed: true, "GxEPD2_290_C90c"),
    ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}