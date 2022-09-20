namespace Atc.Wpf.ValueConverters;

[MarkupExtensionReturnType(typeof(MarkupMultiValueConverter))]
public abstract class MarkupMultiValueConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    public abstract object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    public abstract object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture);

    public abstract object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);

    public abstract object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture);

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}