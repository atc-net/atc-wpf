namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To DependencyProperty.UnsetValue.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]
public sealed class NullToUnsetValueConverter : MarkupValueConverterBase
{
    public static readonly NullToUnsetValueConverter Instance = new();

    protected override object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value ?? DependencyProperty.UnsetValue;

    protected override object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}