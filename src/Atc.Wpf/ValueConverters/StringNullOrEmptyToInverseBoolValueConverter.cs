namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String Null Or Empty To Inverse Bool.
/// </summary>
[ValueConversion(typeof(string), typeof(bool))]
public class StringNullOrEmptyToInverseBoolValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(value is null || string.IsNullOrEmpty(value.ToString()));
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}