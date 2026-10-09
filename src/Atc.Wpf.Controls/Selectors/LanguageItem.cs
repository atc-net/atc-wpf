namespace Atc.Wpf.Controls.Selectors;

/// <summary>A language entry in a <see cref="LanguageSelector"/>.</summary>
/// <param name="Culture">The culture that represents the language.</param>
/// <param name="Image">The flag image for the language, if any.</param>
public record LanguageItem(
    Culture Culture,
    BitmapImage? Image);