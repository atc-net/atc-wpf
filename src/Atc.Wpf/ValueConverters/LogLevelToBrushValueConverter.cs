namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogLevel To (SolidColor)Brush.
/// </summary>
/// <remarks>
/// Returns frozen <see cref="SolidColorBrush"/> instances built from the palette defined on
/// <see cref="LogLevelToColorValueConverter"/> (single source of truth for the colors).
/// To override a brush, set the corresponding color on the color converter — e.g.
/// <c>LogLevelToColorValueConverter.WarningColor = Colors.Orange;</c>.
/// The brush converter detects the change and rebuilds (and re-caches) the affected brush.
/// </remarks>
[ValueConversion(typeof(LogLevel), typeof(SolidColorBrush))]
public sealed class LogLevelToBrushValueConverter : IValueConverter
{
    public static readonly LogLevelToBrushValueConverter Instance = new();

    private static readonly ConcurrentDictionary<LogLevel, SolidColorBrush> Cache = new();

    private static SolidColorBrush? fallbackBrushCache;
    private static Color fallbackBrushCachedColor;

    /// <summary>
    /// Gets a frozen <see cref="SolidColorBrush"/> for the given <paramref name="level"/>.
    /// Brushes are cached and rebuilt automatically when their underlying color changes.
    /// </summary>
    public static SolidColorBrush GetBrush(LogLevel level)
    {
        var color = LogLevelToColorValueConverter.GetColor(level);
        return Cache.AddOrUpdate(
            level,
            _ => CreateFrozen(color),
            (_, existing) => existing.Color == color
                ? existing
                : CreateFrozen(color));
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is LogLevel level
            ? GetBrush(level)
            : GetFallbackBrush();

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    private static SolidColorBrush GetFallbackBrush()
    {
        var color = LogLevelToColorValueConverter.FallbackColor;
        if (fallbackBrushCache is null ||
            fallbackBrushCachedColor != color)
        {
            fallbackBrushCache = CreateFrozen(color);
            fallbackBrushCachedColor = color;
        }

        return fallbackBrushCache;
    }

    private static SolidColorBrush CreateFrozen(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}