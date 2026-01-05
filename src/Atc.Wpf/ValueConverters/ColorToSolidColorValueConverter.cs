namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Color To Solid Color (sets Alpha to 255).
/// </summary>
/// <remarks>
/// <para>One-way binding only. ConvertBack is not supported because the conversion is lossy (alpha channel is discarded).</para>
/// </remarks>
[ValueConversion(typeof(Color), typeof(Color))]
public sealed class ColorToSolidColorValueConverter : IValueConverter
{
    public static readonly ColorToSolidColorValueConverter Instance = new();

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