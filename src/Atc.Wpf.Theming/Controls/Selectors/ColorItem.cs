namespace Atc.Wpf.Theming.Controls.Selectors;

public record ColorItem
{
    public ColorItem(
        string name,
        string displayName,
        string displayHexCode,
        Brush borderColorBrush,
        Brush colorBrush)
    {
        Name = name;
        DisplayName = displayName;
        DisplayHexCode = displayHexCode;
        BorderColorBrush = borderColorBrush;
        ColorBrush = colorBrush;
    }

    public string Name { get; init; }

    public string DisplayName { get; set; }

    public string DisplayHexCode { get; init; }

    public Brush BorderColorBrush { get; init; }

    public Brush ColorBrush { get; init; }
}