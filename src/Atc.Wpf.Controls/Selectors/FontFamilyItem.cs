namespace Atc.Wpf.Controls.Selectors;

/// <summary>A font family entry in a <see cref="FontFamilySelector"/>.</summary>
/// <param name="Key">The key that identifies the item.</param>
/// <param name="FontFamily">The font family name used to render the item.</param>
/// <param name="DisplayName">The text shown for the item.</param>
public record FontFamilyItem(
    string Key,
    string FontFamily,
    string DisplayName);