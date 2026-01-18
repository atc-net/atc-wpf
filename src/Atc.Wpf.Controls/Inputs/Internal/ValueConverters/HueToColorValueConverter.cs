namespace Atc.Wpf.Controls.Inputs.Internal.ValueConverters;

/// <summary>
/// Converts hue [0 - 360] to color.
/// </summary>
internal class HueToColorValueConverter : IValueConverter
{
    public static readonly HueToColorValueConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double d)
        {
            return ColorHelper.GetColorFromHsv(d, 1, 1);
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is Color color)
        {
            return color.GetHue();
        }

        return null;
    }
}