namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Decimal to Double.
/// </summary>
[ValueConversion(typeof(decimal), typeof(double))]
internal sealed class DecimalToDoubleValueConverter : IValueConverter
{
    public static readonly DecimalToDoubleValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            decimal d => d,
            _ => 0D,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            double d => d,
            _ => 0M,
        };
}