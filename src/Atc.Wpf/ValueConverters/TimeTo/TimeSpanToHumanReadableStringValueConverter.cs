// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: TimeSpan To human-readable string (e.g. <c>"1h 15m"</c>).
/// </summary>
/// <remarks>
/// Delegates to <see cref="TimeSpanExtensions.GetPrettyTime(TimeSpan, int)"/>.
/// Optional <c>ConverterParameter</c> sets the decimal precision (default 1):
/// <code>ConverterParameter=2</code> → two decimals on the largest unit.
/// Non-<see cref="TimeSpan"/> values return <see cref="string.Empty"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding Elapsed,
///     Converter={x:Static converters:TimeSpanToHumanReadableStringValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(TimeSpan), typeof(string))]
public sealed class TimeSpanToHumanReadableStringValueConverter : IValueConverter
{
    public static readonly TimeSpanToHumanReadableStringValueConverter Instance = new();

    /// <summary>Default decimal precision used when no <c>ConverterParameter</c> is provided.</summary>
    public static int DefaultDecimalPrecision { get; set; } = 1;

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not TimeSpan timeSpan)
        {
            return string.Empty;
        }

        var decimals = ParseDecimalPrecision(parameter);
        return timeSpan.GetPrettyTime(decimals);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    private static int ParseDecimalPrecision(object? parameter)
    {
        if (parameter is int i)
        {
            return System.Math.Max(0, i);
        }

        if (parameter is string s &&
            int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return System.Math.Max(0, parsed);
        }

        return DefaultDecimalPrecision;
    }
}