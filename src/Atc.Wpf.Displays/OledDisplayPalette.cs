namespace Atc.Wpf.Displays;

/// <summary>
/// The colours of the <see cref="OledDisplay"/> presets: the near-black panel and the colour of the lit pixels.
/// </summary>
/// <remarks>
/// The values are sampled from photos of real modules. An OLED pixel that is off emits no light, so the panel is the same near-black glass
/// for every preset. White modules look slightly bluish, blue modules sky-blue, yellow modules warm. The two-colour
/// <see cref="OledDisplayColor.YellowBlue"/> module uses the blue ink for its main area and <see cref="YellowZoneInk"/> for the top rows.
/// </remarks>
internal static class OledDisplayPalette
{
    /// <summary>The colour of the panel: the near-black glass of an OLED.</summary>
    internal static readonly Color Panel = FromRgb(0x050607);

    /// <summary>The colour of the top rows of a <see cref="OledDisplayColor.YellowBlue"/> module.</summary>
    internal static readonly Color YellowZoneInk = FromRgb(0xFFD23C);

    /// <summary>
    /// Gets the colours of a preset.
    /// </summary>
    /// <param name="color">The colour preset.</param>
    /// <returns>The panel colour and the lit pixel colour (for <see cref="OledDisplayColor.YellowBlue"/> the blue of the main area).</returns>
    public static (Color Panel, Color Ink) Get(OledDisplayColor color)
        => (Panel, GetInk(color));

    /// <summary>
    /// Creates a frozen brush.
    /// </summary>
    /// <param name="color">The colour.</param>
    /// <returns>The frozen brush.</returns>
    public static SolidColorBrush CreateBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    private static Color GetInk(OledDisplayColor color)
        => color switch
        {
            OledDisplayColor.Blue or OledDisplayColor.YellowBlue => FromRgb(0x3CC8FF),
            OledDisplayColor.Yellow => YellowZoneInk,
            OledDisplayColor.Green => FromRgb(0x5AFF8C),
            _ => FromRgb(0xE6F2FF),
        };

    private static Color FromRgb(int rgb)
        => Color.FromRgb(
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF));
}