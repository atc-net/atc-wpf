namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To DependencyProperty.UnsetValue.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]
public sealed class NullToUnsetValueConverter : MarkupValueConverterBase
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