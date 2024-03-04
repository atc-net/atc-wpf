namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Color To Solid-Color.
/// </summary>
[ValueConversion(typeof(Color), typeof(Color))]
public sealed class ColorToSolidColorValueConverter : IValueConverter
{
    public static readonly ColorToSolidColorValueConverter DefaultInstance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return Brushes.DeepPink;
        }

        if (value is not Color color)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
        }

        color.A = 255;

        return color;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}