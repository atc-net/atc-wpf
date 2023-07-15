namespace Atc.Wpf.Controls;

public static class Constants
{
    public const string DefaultLabelControlLabel = "-Label-";

    public const string JTokenHexColorString = "#4e9a06";
    public const string JTokenHexColorFloat = "#ad7fa8";
    public const string JTokenHexColorInteger = "#ad7fa8";
    public const string JTokenHexColorBoolean = "#c4a000";
    public const string JTokenHexColorNull = "#ff6e00";

    public static Brush JTokenColorString => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorString)!;

    public static Brush JTokenColorFloat => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorFloat)!;

    public static Brush JTokenColorInteger => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorInteger)!;

    public static Brush JTokenColorBoolean => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorBoolean)!;

    public static Brush JTokenColorNull => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorNull)!;
}