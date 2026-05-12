namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Background brush → ideal foreground brush (Black or White) for readable
/// contrast, computed from perceptual luminance of the background.
/// </summary>
/// <remarks>
/// Algorithm:
/// <c>luminance = round(R·0.299 + G·0.587 + B·0.114)</c>; if
/// <c>(255 − luminance) &lt; <see cref="LuminanceThreshold"/></c>, returns
/// <see cref="DarkForegroundColor"/>, otherwise <see cref="LightForegroundColor"/>.
/// <para>
/// Threshold and the two foreground colors are mutable static properties so consumers can
/// retheme this library-wide. Defaults: threshold <c>86</c>, dark <c>Colors.Black</c>,
/// light <c>Colors.White</c>. Call <see cref="ResetToDefaults"/> to restore the built-ins.
/// </para>
/// <para>
/// Source: <a href="http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx">Determining
/// Ideal Text Color Based on Specified Background Color</a>.
/// </para>
/// </remarks>
[ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
public sealed class BackgroundToForegroundValueConverter : IValueConverter, IMultiValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="BackgroundToForegroundValueConverter"/>.
    /// </summary>
    public static readonly BackgroundToForegroundValueConverter Instance = new();

    /// <summary>The built-in default luminance threshold (<c>86</c>).</summary>
    public static readonly int DefaultLuminanceThreshold = 86;

    /// <summary>The built-in default dark foreground (<see cref="Colors.Black"/>).</summary>
    public static readonly Color DefaultDarkForegroundColor = Colors.Black;

    /// <summary>The built-in default light foreground (<see cref="Colors.White"/>).</summary>
    public static readonly Color DefaultLightForegroundColor = Colors.White;

    /// <summary>
    /// Luminance threshold used to decide between <see cref="DarkForegroundColor"/> and
    /// <see cref="LightForegroundColor"/>. Higher values bias toward the light foreground.
    /// </summary>
    public static int LuminanceThreshold { get; set; } = DefaultLuminanceThreshold;

    /// <summary>Foreground color returned for light backgrounds. Defaults to <see cref="Colors.Black"/>.</summary>
    public static Color DarkForegroundColor { get; set; } = DefaultDarkForegroundColor;

    /// <summary>Foreground color returned for dark backgrounds. Defaults to <see cref="Colors.White"/>.</summary>
    public static Color LightForegroundColor { get; set; } = DefaultLightForegroundColor;

    /// <summary>Restores <see cref="LuminanceThreshold"/>, <see cref="DarkForegroundColor"/>, and <see cref="LightForegroundColor"/> to their built-in defaults.</summary>
    public static void ResetToDefaults()
    {
        LuminanceThreshold = DefaultLuminanceThreshold;
        DarkForegroundColor = DefaultDarkForegroundColor;
        LightForegroundColor = DefaultLightForegroundColor;
    }

    private static Color IdealTextColor(Color background)
    {
        var backgroundDelta = System.Convert.ToInt32((background.R * 0.299) + (background.G * 0.587) + (background.B * 0.114));
        return (255 - backgroundDelta < LuminanceThreshold)
            ? DarkForegroundColor
            : LightForegroundColor;
    }

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
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

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;

    public object? Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
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

    public object[]? ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
    {
        if (targetTypes is null || targetTypes.Length == 0)
        {
            return [];
        }

        var result = new object[targetTypes.Length];
        Array.Fill(result, DependencyProperty.UnsetValue);
        return result;
    }
}