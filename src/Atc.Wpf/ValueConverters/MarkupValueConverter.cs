namespace Atc.Wpf.ValueConverters;

[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
[MarkupExtensionReturnType(typeof(IValueConverter))]
public abstract class MarkupValueConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    protected abstract object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    protected abstract object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return Convert(value, targetType, parameter, culture);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }
}