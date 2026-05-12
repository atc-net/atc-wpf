// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: unit fraction (0–1) ↔ formatted percent string.
/// </summary>
/// <remarks>
/// Convert: <c>0.42 → "42 %"</c> (or culture-specific equivalent) using
/// <see cref="NumberFormatInfo.PercentSymbol"/> from the supplied culture (falls back to
/// <see cref="CultureInfo.CurrentCulture"/>). Optional <c>ConverterParameter</c> is the number
/// of decimal places (default 0).
/// <para>
/// ConvertBack accepts strings with or without the percent symbol, surrounding whitespace,
/// and culture-specific decimal separators. Falls back to <see cref="Binding.DoNothing"/> if
/// the input cannot be parsed.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding Progress,
///     Converter={x:Static converters:NumberToPercentStringValueConverter.Instance}}" /&gt;
///
/// &lt;!-- One decimal place --&gt;
/// &lt;TextBlock Text="{Binding Accuracy,
///     Converter={x:Static converters:NumberToPercentStringValueConverter.Instance},
///     ConverterParameter=1}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(double), typeof(string))]
public sealed class NumberToPercentStringValueConverter : IValueConverter
{
    public static readonly NumberToPercentStringValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (!TryToDouble(value, out var fraction))
        {
            return string.Empty;
        }

        var decimals = ParseDecimals(parameter);
        var resolved = culture ?? CultureInfo.CurrentCulture;
        return fraction.ToString("P" + decimals.ToString(CultureInfo.InvariantCulture), resolved);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return Binding.DoNothing;
        }

        var resolved = culture ?? CultureInfo.CurrentCulture;
        var trimmed = text
            .Replace(resolved.NumberFormat.PercentSymbol, string.Empty, StringComparison.Ordinal)
            .Trim();

        if (double.TryParse(trimmed, NumberStyles.Float, resolved, out var n))
        {
            return n / 100.0;
        }

        return Binding.DoNothing;
    }

    private static int ParseDecimals(object? parameter)
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

        return 0;
    }

    private static bool TryToDouble(
        object? value,
        out double result)
    {
        switch (value)
        {
            case double d when !double.IsNaN(d) && !double.IsInfinity(d):
                result = d;
                return true;
            case float f when !float.IsNaN(f) && !float.IsInfinity(f):
                result = f;
                return true;
            case decimal dec:
                result = (double)dec;
                return true;
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            default:
                result = 0;
                return false;
        }
    }
}