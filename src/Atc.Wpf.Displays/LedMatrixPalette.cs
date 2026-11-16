namespace Atc.Wpf.Displays;

/// <summary>
/// The colours of the <see cref="LedMatrixDisplay"/> presets: the black module face, the colour of a lit LED and the dull colour of an unlit one.
/// </summary>
internal static class LedMatrixPalette
{
    /// <summary>The colour of the module face between the LEDs.</summary>
    internal static readonly Color Panel = FromRgb(0x0B0B0C);

    /// <summary>
    /// Gets the colours of a preset.
    /// </summary>
    /// <param name="color">The LED colour.</param>
    /// <returns>The panel colour, the lit LED colour and the unlit LED colour.</returns>
    public static (Color Panel, Color Lit, Color Unlit) Get(LedColor color)
        => color switch
        {
            LedColor.Green => (Panel, FromRgb(0x3CFF5A), FromRgb(0x123A18)),
            LedColor.Blue => (Panel, FromRgb(0x3C8CFF), FromRgb(0x10204A)),
            LedColor.Yellow => (Panel, FromRgb(0xFFD02A), FromRgb(0x4A3A0C)),
            LedColor.White => (Panel, FromRgb(0xF4F6FF), FromRgb(0x3A3C44)),
            _ => (Panel, FromRgb(0xFF2A1E), FromRgb(0x4A0E0A)),
        };

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

    private static Color FromRgb(int rgb)
        => Color.FromRgb(
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF));
}