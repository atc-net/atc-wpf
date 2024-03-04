// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: string-color-hex To Color.
/// </summary>
[ValueConversion(typeof(string), typeof(Color))]
public sealed class ColorHexToColorValueConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => ConvertValue(value);

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => ConvertValue(value);

    private static object ConvertValue(object? value)
    {
        if (value is null)
        {
            return Binding.DoNothing;
        }

        if (value is Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        var hexValue = value.ToString()!;
        if (!hexValue.StartsWith('#'))
        {
            hexValue = "#" + hexValue;
        }

        return hexValue.Length is not (9 or 7 or 4)
            ? Binding.DoNothing
            : ColorHelper.GetColorFromHex(hexValue)!;
    }
}