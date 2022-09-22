namespace Atc.Wpf.ValueConverters;

[ValueConversion(typeof(object), typeof(object))]
public class NullToUnsetValueConverter : MarkupValueConverter
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value ?? DependencyProperty.UnsetValue;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}