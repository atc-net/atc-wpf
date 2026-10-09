namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To DependencyProperty.UnsetValue.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]
public sealed class NullToUnsetValueConverter : MarkupValueConverterBase
{
    /// <summary>
    /// Gets a static default instance of <see cref="NullToUnsetValueConverter"/>.
    /// </summary>
    public static readonly NullToUnsetValueConverter Instance = new();

    /// <inheritdoc />
    protected override object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value ?? DependencyProperty.UnsetValue;

    /// <inheritdoc />
    protected override object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}