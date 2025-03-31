namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: string-color-name To Color.
/// </summary>
[ValueConversion(typeof(string), typeof(Color))]
public sealed class ColorNameToColorValueConverter : IValueConverter
{
    public static readonly ColorNameToColorValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Colors.DeepPink;
        }

        if (value is not string stringValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(string)");
        }

        var color = ColorHelper.GetColorFromString(stringValue, GlobalizationConstants.EnglishCultureInfo);
        return color ?? throw new InvalidCastException($"{stringValue} is not a valid color");
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return "DeepPink";
        }

        if (value is not Color color)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
        }

        var colorName = ColorHelper.GetColorNameFromColor(color);

        return string.IsNullOrEmpty(colorName)
            ? value.ToString()!
            : colorName;
    }
}