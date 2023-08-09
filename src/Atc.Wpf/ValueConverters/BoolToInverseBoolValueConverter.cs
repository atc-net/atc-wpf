namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Inverse Bool.
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public sealed class BoolToInverseBoolValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value);
    }

    private static bool Convert(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not bool boolValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
        }

        return !boolValue;
    }
}