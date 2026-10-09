namespace Atc.Wpf.Data.Models;

/// <summary>
/// Represents a named color with brushes for display in color selectors.
/// </summary>
/// <param name="Key">The key (name) that identifies the color.</param>
/// <param name="DisplayName">The name shown for the color.</param>
/// <param name="DisplayHexCode">The hex code shown for the color.</param>
/// <param name="BorderColorBrush">The brush used for the border of the color swatch.</param>
/// <param name="ColorBrush">The brush that renders the color.</param>
public record ColorItem(
    string Key,
    string DisplayName,
    string DisplayHexCode,
    Brush BorderColorBrush,
    Brush ColorBrush)
{
    /// <summary>
    /// Gets or sets the name shown for the color.
    /// </summary>
    public string DisplayName { get; set; } = DisplayName;
}