namespace Atc.Wpf.ValueConverters;

[MarkupExtensionReturnType(typeof(MarkupMultiValueConverterBase))]
public abstract class MarkupMultiValueConverterBase : MarkupExtension, IValueConverter, IMultiValueConverter
{
    public abstract object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    public abstract object? Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    public abstract object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    public abstract object[]? ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture);

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}