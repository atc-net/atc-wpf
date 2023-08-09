namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Integer To TimeSpan.
/// </summary>
[ValueConversion(typeof(int), typeof(TimeSpan))]
public class IntegerToTimeSpanValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not int intValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(int)");
        }

        return TimeSpan.FromMilliseconds(intValue);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}