namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: SolidColorBrush To Color.
/// </summary>
/// <remarks>
/// <para>Supports two-way binding.</para>
/// <para>Convert: SolidColorBrush → Color</para>
/// <para>ConvertBack: Color → SolidColorBrush</para>
/// </remarks>
[ValueConversion(typeof(SolidColorBrush), typeof(Color))]
public sealed class BrushToColorValueConverter : IValueConverter
{
    public static readonly BrushToColorValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Colors.DeepPink;
        }

        if (value is not SolidColorBrush brush)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(SolidColorBrush)");
        }

        return brush.Color;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Brushes.DeepPink;
        }

        if (value is not Color color)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
        }

        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    }
}