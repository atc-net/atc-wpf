namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Multi Bool To Bool.
/// </summary>
[ValueConversion(typeof(List<bool>), typeof(bool))]
public class MultiBoolToBoolValueConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        return values.All(value => value is not bool boolValue || boolValue);
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}