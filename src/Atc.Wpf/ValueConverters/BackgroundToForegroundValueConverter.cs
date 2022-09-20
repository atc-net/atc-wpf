namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Background To Foreground.
/// </summary>
[ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
public sealed class BackgroundToForegroundValueConverter : IValueConverter, IMultiValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="BackgroundToForegroundValueConverter"/>.
    /// </summary>
    public static readonly BackgroundToForegroundValueConverter Instance = new();

    /// <summary>
    /// Determining Ideal Text Color Based on Specified Background Color
    /// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
    /// </summary>
    /// <param name = "background">The background color.</param>
    private static Color IdealTextColor(Color background)
    {
        const int nThreshold = 86;
        var backgroundDelta = System.Convert.ToInt32((background.R * 0.299) + (background.G * 0.587) + (background.B * 0.114));
        var foregroundColor = (255 - backgroundDelta < nThreshold)
            ? Colors.Black
            : Colors.White;

        return foregroundColor;
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush backgroundBrush)
        {
            return Brushes.White;
        }

        var idealForegroundColor = IdealTextColor(backgroundBrush.Color);
        var foregroundBrush = new SolidColorBrush(idealForegroundColor);
        foregroundBrush.Freeze();

        return foregroundBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }

    public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        Brush? titleBrush = null;
        if (values is not null)
        {
            titleBrush = values.Length > 1
                ? values[1] as Brush
                : null;
        }

        if (titleBrush is not null)
        {
            return titleBrush;
        }

        Brush? backgroundBrush = null;
        if (values is not null)
        {
            backgroundBrush = values.Length > 0
                ? values[0] as Brush
                : null;
        }

        return Convert(backgroundBrush, targetType, parameter, culture);
    }

    public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
    }
}