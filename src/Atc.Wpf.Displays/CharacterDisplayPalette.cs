namespace Atc.Wpf.Displays;

/// <summary>
/// The colours of the <see cref="CharacterDisplay"/> presets: the panel, the lit dots ("ink") and the unlit ghost dots,
/// for an LCD with the backlight on, an LCD with the backlight off (reflective) and a VFD.
/// </summary>
/// <remarks>
/// The values are sampled from photos of real modules. An LCD with the backlight off shows the same grey-green
/// reflective panel for every preset. A VFD has no backlight, so <c>isBacklightOn</c> does not change it, and an LCD-only preset
/// (<see cref="CharacterDisplayColor.YellowGreen"/>, <see cref="CharacterDisplayColor.Blue"/>, <see cref="CharacterDisplayColor.WhiteOnBlack"/>)
/// falls back to <see cref="CharacterDisplayColor.BlueGreen"/>; on an LCD the VFD-only <see cref="CharacterDisplayColor.BlueGreen"/>
/// falls back to <see cref="CharacterDisplayColor.YellowGreen"/>.
/// </remarks>
internal static class CharacterDisplayPalette
{
    /// <summary>The ghost dot opacity of a backlit LCD.</summary>
    internal const double LcdGhostOpacity = 0.08;

    /// <summary>The ghost dot opacity of an LCD with the backlight off: weaker than backlit.</summary>
    internal const double ReflectiveGhostOpacity = 0.04;

    /// <summary>The ghost dot opacity of a VFD: very faint.</summary>
    internal const double VfdGhostOpacity = 0.06;

    private static readonly Color Black = Color.FromRgb(0x00, 0x00, 0x00);
    private static readonly Color ReflectivePanel = Color.FromRgb(0x8E, 0x9A, 0x7A);
    private static readonly Color ReflectiveInk = Color.FromRgb(0x22, 0x2A, 0x1C);
    private static readonly Color VfdPanel = Color.FromRgb(0x05, 0x08, 0x07);

    /// <summary>
    /// Gets the colours of a preset.
    /// </summary>
    /// <param name="kind">The display technology.</param>
    /// <param name="color">The colour preset.</param>
    /// <param name="isBacklightOn">Whether the LCD backlight is on; ignored for a VFD.</param>
    /// <returns>The panel, lit dot and unlit dot colours and the ghost dot opacity.</returns>
    public static (Color Panel, Color Ink, Color Unlit, double GhostOpacity) Get(
        CharacterDisplayKind kind,
        CharacterDisplayColor color,
        bool isBacklightOn)
    {
        if (kind == CharacterDisplayKind.Vfd)
        {
            var ink = GetVfdInk(color);
            return (VfdPanel, ink, ink, VfdGhostOpacity);
        }

        if (!isBacklightOn)
        {
            return (ReflectivePanel, ReflectiveInk, ReflectiveInk, ReflectiveGhostOpacity);
        }

        return color switch
        {
            // Negative modes (light ink): the unlit dots show as slightly darker squares on the panel.
            CharacterDisplayColor.Blue => Lcd(0x1E4FD8, 0xE8F0FF, isNegative: true),
            CharacterDisplayColor.WhiteOnBlack => Lcd(0x0B0B0B, 0xF2F2F2, isNegative: false),
            CharacterDisplayColor.Amber => Lcd(0xF2A43A, 0x2A1500, isNegative: false),
            CharacterDisplayColor.Red => Lcd(0xC8102E, 0xFFE0E0, isNegative: true),
            CharacterDisplayColor.Green => Lcd(0x3BAA3B, 0x08240A, isNegative: false),
            _ => Lcd(0xB5D13A, 0x1B2A10, isNegative: false),
        };
    }

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

    private static Color GetVfdInk(CharacterDisplayColor color)
        => color switch
        {
            CharacterDisplayColor.Green => FromRgb(0x66FF66),
            CharacterDisplayColor.Amber => FromRgb(0xFFB000),
            CharacterDisplayColor.Red => FromRgb(0xFF3B3B),
            _ => FromRgb(0x4DFFD0),
        };

    private static (Color Panel, Color Ink, Color Unlit, double GhostOpacity) Lcd(
        int panel,
        int ink,
        bool isNegative)
    {
        var inkColor = FromRgb(ink);
        return (FromRgb(panel), inkColor, isNegative ? Black : inkColor, LcdGhostOpacity);
    }

    private static Color FromRgb(int rgb)
        => Color.FromRgb(
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF));
}