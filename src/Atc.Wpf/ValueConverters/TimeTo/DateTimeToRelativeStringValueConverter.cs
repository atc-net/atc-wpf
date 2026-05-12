// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: DateTime To relative-time string (e.g. <c>"5 minutes ago"</c>).
/// </summary>
/// <remarks>
/// Delegates to <see cref="DateTimeExtensions.GetPrettyTimeDiff(DateTime, int)"/>, which compares the
/// bound value against <see cref="DateTime.UtcNow"/>. Optional <c>ConverterParameter</c> sets the
/// decimal precision (default 1).
/// <para>
/// Accepts <see cref="DateTime"/> and <see cref="DateTimeOffset"/> (the latter converted to
/// <see cref="DateTimeOffset.UtcDateTime"/>). Other types return <see cref="string.Empty"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding LastSeen,
///     Converter={x:Static converters:DateTimeToRelativeStringValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(DateTime), typeof(string))]
public sealed class DateTimeToRelativeStringValueConverter : IValueConverter
{
    public static readonly DateTimeToRelativeStringValueConverter Instance = new();

    /// <summary>Default decimal precision used when no <c>ConverterParameter</c> is provided.</summary>
    public static int DefaultDecimalPrecision { get; set; } = 1;

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var decimals = ParseDecimalPrecision(parameter);
        return value switch
        {
            DateTime dt => dt.GetPrettyTimeDiff(decimals),
            DateTimeOffset dto => dto.UtcDateTime.GetPrettyTimeDiff(decimals),
            _ => string.Empty,
        };
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