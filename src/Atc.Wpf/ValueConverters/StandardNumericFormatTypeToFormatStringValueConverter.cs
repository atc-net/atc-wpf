namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: DoubleLongFormatType To FormatString
/// </summary>
[ValueConversion(typeof(StandardNumericFormatType), typeof(string))]
public sealed class StandardNumericFormatTypeToFormatStringValueConverter : IValueConverter
{
    public static readonly StandardNumericFormatTypeToFormatStringValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        var formatType = Enum<StandardNumericFormatType>.Parse(value.ToString()!, ignoreCase: false);

        return formatType switch
        {
            StandardNumericFormatType.General => "G",
            StandardNumericFormatType.Currency => "C",
            StandardNumericFormatType.Decimal1 => "D1",
            StandardNumericFormatType.Decimal2 => "D2",
            StandardNumericFormatType.Decimal3 => "D3",
            StandardNumericFormatType.Decimal4 => "D4",
            StandardNumericFormatType.FixedPoint1 => "F1",
            StandardNumericFormatType.FixedPoint2 => "F2",
            StandardNumericFormatType.FixedPoint3 => "F3",
            StandardNumericFormatType.FixedPoint4 => "F4",
            StandardNumericFormatType.Number1 => "N1",
            StandardNumericFormatType.Number2 => "N2",
            StandardNumericFormatType.Number3 => "N3",
            StandardNumericFormatType.Number4 => "N4",
            StandardNumericFormatType.Percent => "P",
            _ => throw new SwitchCaseDefaultException(formatType),
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}