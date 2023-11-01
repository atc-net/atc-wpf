namespace Atc.Wpf.Controls;

public static class Constants
{
    public const string DefaultLabelControlLabel = "-Label-";

    public const string JTokenHexColorString = "#4e9a06";
    public const string JTokenHexColorFloat = "#ad7fa8";
    public const string JTokenHexColorInteger = "#ad7fa8";
    public const string JTokenHexColorBoolean = "#c4a000";
    public const string JTokenHexColorDate = "#f4a000";
    public const string JTokenHexColorGuid = "#f4af00";
    public const string JTokenHexColorUri = "#f4a0f0";
    public const string JTokenHexColorNull = "#ff6e00";

    public static Brush JTokenColorString => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorString)!;

    public static Brush JTokenColorFloat => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorFloat)!;

    public static Brush JTokenColorInteger => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorInteger)!;

    public static Brush JTokenColorBoolean => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorBoolean)!;

    public static Brush JTokenColorDate => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorDate)!;

    public static Brush JTokenColorGuid => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorGuid)!;

    public static Brush JTokenColorUri => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorUri)!;

    public static Brush JTokenColorNull => (Brush)new BrushConverter().ConvertFrom(JTokenHexColorNull)!;
}