namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// ValueConverter: Integer to Double.
/// </summary>
[ValueConversion(typeof(int), typeof(double))]
[ValueConversion(typeof(decimal), typeof(double))]
public sealed class IntegerToDoubleValueConverter : IValueConverter
{
    /// <summary>The shared instance of the converter.</summary>
    public static readonly IntegerToDoubleValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            int i => i,
            decimal d => d,
            _ => 0D,
        };

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    => value switch
        {
            double d => d,
            _ => 0,
        };
}