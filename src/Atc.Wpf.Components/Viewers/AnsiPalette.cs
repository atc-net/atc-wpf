// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// 16-colour ANSI palette using the values shipped by Windows Terminal /
/// modern console hosts. All brushes are frozen for thread-safe sharing.
/// Index layout matches the standard SGR mapping: 0–7 are the base 8 colours
/// (codes 30/40), 8–15 are the bright variants (codes 90/100).
/// </summary>
public static class AnsiPalette
{
    public static readonly IReadOnlyList<Brush> Colors16 = CreateBrushes(
    [
        Color.FromRgb(0x00, 0x00, 0x00), // black
        Color.FromRgb(0xC5, 0x0F, 0x1F), // red
        Color.FromRgb(0x13, 0xA1, 0x0E), // green
        Color.FromRgb(0xC1, 0x9C, 0x00), // yellow
        Color.FromRgb(0x00, 0x37, 0xDA), // blue
        Color.FromRgb(0x88, 0x17, 0x98), // magenta
        Color.FromRgb(0x3A, 0x96, 0xDD), // cyan
        Color.FromRgb(0xCC, 0xCC, 0xCC), // white
        Color.FromRgb(0x76, 0x76, 0x76), // bright black (gray)
        Color.FromRgb(0xE7, 0x48, 0x56), // bright red
        Color.FromRgb(0x16, 0xC6, 0x0C), // bright green
        Color.FromRgb(0xF9, 0xF1, 0xA5), // bright yellow
        Color.FromRgb(0x3B, 0x78, 0xFF), // bright blue
        Color.FromRgb(0xB4, 0x00, 0x9E), // bright magenta
        Color.FromRgb(0x61, 0xD6, 0xD6), // bright cyan
        Color.FromRgb(0xF2, 0xF2, 0xF2), // bright white
    ]);

    private static IReadOnlyList<Brush> CreateBrushes(IReadOnlyList<Color> colors)
    {
        var result = new Brush[colors.Count];
        for (var i = 0; i < colors.Count; i++)
        {
            var brush = new SolidColorBrush(colors[i]);
            brush.Freeze();
            result[i] = brush;
        }

        return result;
    }
}
