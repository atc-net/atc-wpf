namespace Atc.Wpf.Extensions;

public static class SolidColorBrushExtensions
{
    public static string? GetBaseBrushName(
        this SolidColorBrush brush)
        => SolidColorBrushHelper.GetBaseBrushNameFromBrush(brush);

    public static string? GetBrushName(
        this SolidColorBrush brush)
        => SolidColorBrushHelper.GetBrushNameFromBrush(brush);

    public static string? GetBrushName(
        this SolidColorBrush brush,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
        => SolidColorBrushHelper.GetBrushNameFromBrush(
            brush,
            culture,
            includeColorHex,
            useAlphaChannel);
}