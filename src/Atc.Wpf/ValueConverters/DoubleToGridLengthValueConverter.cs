namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Double To GridLength.
/// </summary>
/// <remarks>
/// Converts a double value to a <see cref="GridLength"/>.
/// Supports pixel, star, and auto values via the converter parameter.
/// <list type="bullet">
/// <item><description>No parameter or "Pixel": Creates a pixel-based GridLength</description></item>
/// <item><description>"Star" or "*": Creates a star-based GridLength</description></item>
/// <item><description>"Auto": Creates an auto GridLength (ignores the value)</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Pixel width from binding --&gt;
/// Width="{Binding ColumnWidth, Converter={x:Static converters:DoubleToGridLengthValueConverter.Instance}}"
///
/// &lt;!-- Star width from binding --&gt;
/// Width="{Binding StarRatio, Converter={x:Static converters:DoubleToGridLengthValueConverter.Instance}, ConverterParameter=Star}"
/// </code>
/// </example>
[ValueConversion(typeof(double), typeof(GridLength))]
public sealed class DoubleToGridLengthValueConverter : IValueConverter
{
    public static readonly DoubleToGridLengthValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not double doubleValue)
        {
            if (value is int intValue)
            {
                doubleValue = intValue;
            }
            else if (value is float floatValue)
            {
                doubleValue = floatValue;
            }
            else
            {
                return new GridLength(1, GridUnitType.Auto);
            }
        }

        var parameterString = parameter?.ToString()?.ToUpperInvariant();

        return parameterString switch
        {
            "AUTO" => new GridLength(1, GridUnitType.Auto),
            "STAR" or "*" => new GridLength(doubleValue, GridUnitType.Star),
            _ => new GridLength(doubleValue, GridUnitType.Pixel),
        };
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not GridLength gridLength)
        {
            return 0.0;
        }

        return gridLength.GridUnitType == GridUnitType.Auto
            ? double.NaN
            : gridLength.Value;
    }
}