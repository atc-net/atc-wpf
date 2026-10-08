namespace Atc.Wpf.Theming.Theming;

/// <summary>
/// The Windows high-contrast system colors a high-contrast theme is built from.
/// </summary>
/// <param name="Window">The background of windows and controls.</param>
/// <param name="WindowText">Text and borders.</param>
/// <param name="Highlight">Selection, focus and accent.</param>
/// <param name="HighlightText">Text on <paramref name="Highlight"/>.</param>
/// <param name="GrayText">Disabled text and controls.</param>
internal sealed record HighContrastPalette(
    Color Window,
    Color WindowText,
    Color Highlight,
    Color HighlightText,
    Color GrayText)
{
    /// <summary>
    /// Reads the palette from the current Windows system colors.
    /// </summary>
    public static HighContrastPalette FromSystemColors()
        => new(
            SystemColors.WindowColor,
            SystemColors.WindowTextColor,
            SystemColors.HighlightColor,
            SystemColors.HighlightTextColor,
            SystemColors.GrayTextColor);
}