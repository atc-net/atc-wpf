// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Numeric comparison → Visibility (true → Collapsed).
/// </summary>
/// <remarks>
/// Inverse of <see cref="NumericComparisonToVisibilityVisibleValueConverter"/>. Returns
/// <see cref="Visibility.Collapsed"/> when the comparison succeeds, otherwise
/// <see cref="Visibility.Visible"/>. See <see cref="NumericComparisonToVisibilityVisibleValueConverter"/>
/// for parameter syntax and supported types.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Hide row when ItemCount is zero --&gt;
/// Visibility="{Binding ItemCount,
///     Converter={x:Static converters:NumericComparisonToVisibilityCollapsedValueConverter.Instance},
///     ConverterParameter='=0'}"
/// </code>
/// </example>
[ValueConversion(typeof(double), typeof(Visibility))]
public sealed class NumericComparisonToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly NumericComparisonToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => NumericComparison.Evaluate(value, parameter)
            ? Visibility.Collapsed
            : Visibility.Visible;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}