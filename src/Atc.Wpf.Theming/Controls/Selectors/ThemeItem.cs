namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Represents a theme listed in the <see cref="ThemeSelector"/>.
/// </summary>
/// <param name="Name">The base color scheme name of the theme.</param>
/// <param name="DisplayName">The localized name shown to the user.</param>
/// <param name="BorderColorBrush">The brush used for the color indicator border.</param>
/// <param name="ColorBrush">The brush used to fill the color indicator.</param>
public record ThemeItem(
    string Name,
    string DisplayName,
    Brush BorderColorBrush,
    Brush ColorBrush);