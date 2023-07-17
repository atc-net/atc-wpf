namespace Atc.Wpf.Theming.Controls.Selectors;

public record ColorItem(
    string Name,
    string DisplayName,
    string DisplayHexCode,
    Brush BorderColorBrush,
    Brush ColorBrush)
{
    public string DisplayName { get; set; } = DisplayName;
}