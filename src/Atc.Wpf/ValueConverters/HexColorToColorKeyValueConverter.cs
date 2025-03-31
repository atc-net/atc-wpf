namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Hex-Color To Color/Brush-Key.
/// </summary>
/// <remarks>
/// Convert back is from: name/key to Color.
/// </remarks>
[ValueConversion(typeof(string), typeof(string))]
public sealed class HexColorToColorKeyValueConverter : IValueConverter
{
    public static readonly HexColorToColorKeyValueConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null ||
            !value.ToString()!.StartsWith('#'))
        {
            throw new UnexpectedTypeException("Type is not a Hex-Color");
        }

        var str = value.ToString()!;

        return ColorHelper.GetColorKeyFromHex(str);
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            throw new UnexpectedTypeException("Type is not a Color-Name");
        }

        var str = value.ToString()!;

        return ColorHelper.GetColorFromString(str, GlobalizationConstants.EnglishCultureInfo);
    }
}