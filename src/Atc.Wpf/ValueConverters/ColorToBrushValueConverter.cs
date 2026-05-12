namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Color To SolidColorBrush.
/// </summary>
/// <remarks>
/// <para>Supports two-way binding.</para>
/// <para>Convert: Color → SolidColorBrush (frozen)</para>
/// <para>ConvertBack: SolidColorBrush → Color</para>
/// </remarks>
[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public sealed class ColorToBrushValueConverter : IValueConverter
{
    public static readonly ColorToBrushValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Color color)
        {
            return BindingFallbacks.Brush;
        }

        var brush = new SolidColorBrush(color);
        brush.Freeze();

        return brush;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return BindingFallbacks.Color;
        }

        return brush.Color;
    }
}