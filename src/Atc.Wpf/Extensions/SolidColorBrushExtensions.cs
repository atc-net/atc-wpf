namespace Atc.Wpf.Extensions;

/// <summary>
/// Extension methods for <see cref="SolidColorBrush"/>.
/// </summary>
public static class SolidColorBrushExtensions
{
    /// <summary>
    /// Gets the brush resource key that matches the brush, if any.
    /// </summary>
    public static string? GetBrushKey(this SolidColorBrush brush)
        => SolidColorBrushHelper.GetBrushKeyFromBrush(brush);

    /// <summary>
    /// Gets the known name of the brush, if any.
    /// </summary>
    public static string? GetBrushName(this SolidColorBrush brush)
        => SolidColorBrushHelper.GetBrushNameFromBrush(brush);

    /// <summary>
    /// Gets the localized known name of the brush, optionally including its hex value.
    /// </summary>
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