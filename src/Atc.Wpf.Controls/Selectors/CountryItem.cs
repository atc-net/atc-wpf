namespace Atc.Wpf.Controls.Selectors;

/// <summary>A country entry in a <see cref="CountrySelector"/>.</summary>
/// <param name="Culture">The culture that represents the country.</param>
/// <param name="Image">The country flag image, if any.</param>
public record CountryItem(
    Culture Culture,
    BitmapImage? Image);