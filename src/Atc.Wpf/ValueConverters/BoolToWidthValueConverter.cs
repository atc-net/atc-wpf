namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Width.
/// </summary>
[ValueConversion(typeof(bool), typeof(LengthConverter))]
public sealed class BoolToWidthValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not bool boolValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
        }

        var width = double.NaN;
        if (parameter is null || "Auto".IsEqual(parameter.ToString()!, StringComparison.OrdinalIgnoreCase))
        {
            return boolValue
                ? width
                : 0;
        }

        var lengthConverter = new LengthConverter();
        var s = parameter.ToString();
        if (s is null)
        {
            return 0;
        }

        var convertFromString = lengthConverter.ConvertFromString(s);
        width = convertFromString is not null &&
                double.TryParse(convertFromString.ToString(), NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo, out var result)
            ? result
            : 0;

        return boolValue
            ? width
            : 0;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}