namespace Atc.Wpf.Theming.Controls.Selectors;

public record ColorItem(
    string Key,
    string DisplayName,
    string DisplayHexCode,
    Brush BorderColorBrush,
    Brush ColorBrush)
{
    public string DisplayName { get; set; } = DisplayName;
}