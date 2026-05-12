namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogCategoryType To (SolidColor)Brush.
/// </summary>
/// <remarks>
/// Returns frozen <see cref="SolidColorBrush"/> instances built from the palette defined on
/// <see cref="LogCategoryTypeToColorValueConverter"/> (single source of truth for the colors).
/// To override a brush, set the corresponding color on the color converter — e.g.
/// <c>LogCategoryTypeToColorValueConverter.SecurityColor = Colors.Teal;</c>.
/// The brush converter detects the change and rebuilds (and re-caches) the affected brush.
/// </remarks>
[ValueConversion(typeof(LogCategoryType), typeof(SolidColorBrush))]
public sealed class LogCategoryTypeToBrushValueConverter : IValueConverter
{
    public static readonly LogCategoryTypeToBrushValueConverter Instance = new();

    private static readonly ConcurrentDictionary<LogCategoryType, SolidColorBrush> Cache = new();

    /// <summary>
    /// Gets a frozen <see cref="SolidColorBrush"/> for the given <paramref name="category"/>.
    /// Brushes are cached and rebuilt automatically when their underlying color changes.
    /// </summary>
    public static SolidColorBrush GetBrush(LogCategoryType category)
    {
        var color = LogCategoryTypeToColorValueConverter.GetColor(category);
        return Cache.AddOrUpdate(
            category,
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
        => value is LogCategoryType category
            ? GetBrush(category)
            : BindingFallbacks.Brush;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    private static SolidColorBrush CreateFrozen(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}