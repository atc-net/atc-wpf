namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: string-color-name To Color.
/// </summary>
[ValueConversion(typeof(string), typeof(Color))]
public sealed class ColorNameToColorValueConverter : IValueConverter
{
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

        if (!ColorUtil.KnownColors.TryGetValue(stringValue, out var color))
        {
            throw new InvalidCastException($"{stringValue} is not a valid color");
        }

        return color;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return "DeepPink";
        }

        if (value is not Color)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
        }

        var knownColor = ColorUtil.KnownColors.FirstOrDefault(x => x.Value.ToString(GlobalizationConstants.EnglishCultureInfo) == value.ToString());
        return string.IsNullOrEmpty(knownColor.Key)
            ? value.ToString()!
            : knownColor.Key;
    }
}