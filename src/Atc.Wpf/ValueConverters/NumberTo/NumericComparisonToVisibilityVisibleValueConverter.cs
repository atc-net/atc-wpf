// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Numeric comparison → Visibility (true → Visible).
/// </summary>
/// <remarks>
/// Parses a comparison expression from <c>ConverterParameter</c> and applies it to the bound
/// numeric value. Returns <see cref="Visibility.Visible"/> when the comparison succeeds, otherwise
/// <see cref="Visibility.Collapsed"/>.
/// <para>
/// Supported operators: <c>&gt;</c>, <c>&gt;=</c>, <c>&lt;</c>, <c>&lt;=</c>, <c>=</c>, <c>&lt;&gt;</c>,
/// plus the range form <c>between:min,max</c> (inclusive).
/// </para>
/// <para>
/// Accepts <see cref="int"/>, <see cref="long"/>, <see cref="double"/>, <see cref="decimal"/>,
/// <see cref="float"/>, <see cref="short"/>, <see cref="byte"/>, <see cref="uint"/>, <see cref="ulong"/>.
/// Invalid input value or a malformed parameter expression throws <see cref="FormatException"/> —
/// fail-fast so typos are caught early.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Show warning when 10 or more items --&gt;
/// Visibility="{Binding ItemCount,
///     Converter={x:Static converters:NumericComparisonToVisibilityVisibleValueConverter.Instance},
///     ConverterParameter='&gt;=10'}"
///
/// &lt;!-- Show message when value is in a range --&gt;
/// Visibility="{Binding Temperature,
///     Converter={x:Static converters:NumericComparisonToVisibilityVisibleValueConverter.Instance},
///     ConverterParameter='between:18,22'}"
/// </code>
/// </example>
[ValueConversion(typeof(double), typeof(Visibility))]
public sealed class NumericComparisonToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly NumericComparisonToVisibilityVisibleValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => NumericComparison.Evaluate(value, parameter)
            ? Visibility.Visible
            : Visibility.Collapsed;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}