namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Base class for converters that act as both a <see cref="MarkupExtension"/> and an
/// <see cref="IValueConverter"/> / <see cref="IMultiValueConverter"/>, so they can be used inline in XAML.
/// </summary>
[MarkupExtensionReturnType(typeof(MarkupMultiValueConverterBase))]
public abstract class MarkupMultiValueConverterBase : MarkupExtension, IValueConverter, IMultiValueConverter
{
    /// <inheritdoc />
    public abstract object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    /// <inheritdoc />
    public abstract object? Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    /// <inheritdoc />
    public abstract object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    /// <inheritdoc />
    public abstract object[]? ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture);

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}