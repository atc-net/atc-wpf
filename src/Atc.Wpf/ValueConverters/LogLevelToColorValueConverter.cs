namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogLevel To Color.
/// </summary>
/// <remarks>
/// Source of truth for the log-level color palette.
/// <see cref="LogLevelToBrushValueConverter"/> wraps these same colors in frozen
/// <see cref="SolidColorBrush"/> instances.
/// <para>
/// Each per-level color is a mutable static property so consumers can override individual
/// entries at application startup, e.g.
/// <c>LogLevelToColorValueConverter.WarningColor = Colors.Orange;</c>.
/// Call <see cref="ResetToDefaults"/> to restore the built-in palette.
/// </para>
/// <para>
/// Note: WPF bindings are not re-evaluated automatically when these static properties change —
/// set overrides during application startup before any binding fires.
/// </para>
/// </remarks>
[ValueConversion(typeof(LogLevel), typeof(Color))]
public sealed class LogLevelToColorValueConverter : IValueConverter
{
    public static readonly LogLevelToColorValueConverter Instance = new();

    public static readonly Color DefaultFallbackColor = BindingFallbacks.DefaultColor;
    public static readonly Color DefaultTraceColor = Colors.Gray;
    public static readonly Color DefaultDebugColor = Colors.CadetBlue;
    public static readonly Color DefaultInformationColor = Colors.DodgerBlue;
    public static readonly Color DefaultWarningColor = Colors.Goldenrod;
    public static readonly Color DefaultErrorColor = Colors.Crimson;
    public static readonly Color DefaultCriticalColor = Colors.Red;

    /// <summary>
    /// Returned when the bound value is null, not a <see cref="LogLevel"/>,
    /// or an enum value not explicitly mapped (e.g. <see cref="LogLevel.None"/>).
    /// Delegates to <see cref="BindingFallbacks.Color"/> so the library-wide fallback stays in sync.
    /// </summary>
    public static Color FallbackColor
    {
        get => BindingFallbacks.Color;
        set => BindingFallbacks.Color = value;
    }

    public static Color TraceColor { get; set; } = DefaultTraceColor;

    public static Color DebugColor { get; set; } = DefaultDebugColor;

    public static Color InformationColor { get; set; } = DefaultInformationColor;

    public static Color WarningColor { get; set; } = DefaultWarningColor;

    public static Color ErrorColor { get; set; } = DefaultErrorColor;

    public static Color CriticalColor { get; set; } = DefaultCriticalColor;

    /// <summary>
    /// Gets the <see cref="Color"/> currently configured for the given <paramref name="level"/>.
    /// Returns <see cref="FallbackColor"/> for unmapped values (e.g. <see cref="LogLevel.None"/>).
    /// </summary>
    public static Color GetColor(LogLevel level)
        => level switch
        {
            LogLevel.Trace => TraceColor,
            LogLevel.Debug => DebugColor,
            LogLevel.Information => InformationColor,
            LogLevel.Warning => WarningColor,
            LogLevel.Error => ErrorColor,
            LogLevel.Critical => CriticalColor,
            _ => FallbackColor,
        };

    /// <summary>
    /// Sets the <see cref="Color"/> override for a single <paramref name="level"/>.
    /// Setting an unmapped level (e.g. <see cref="LogLevel.None"/>) updates
    /// <see cref="FallbackColor"/> instead.
    /// </summary>
    public static void SetColor(
        LogLevel level,
        Color color)
    {
        switch (level)
        {
            case LogLevel.Trace: TraceColor = color; break;
            case LogLevel.Debug: DebugColor = color; break;
            case LogLevel.Information: InformationColor = color; break;
            case LogLevel.Warning: WarningColor = color; break;
            case LogLevel.Error: ErrorColor = color; break;
            case LogLevel.Critical: CriticalColor = color; break;
            default: FallbackColor = color; break;
        }
    }

    /// <summary>
    /// Restores the built-in default palette (clears all consumer overrides).
    /// </summary>
    public static void ResetToDefaults()
    {
        FallbackColor = DefaultFallbackColor;
        TraceColor = DefaultTraceColor;
        DebugColor = DefaultDebugColor;
        InformationColor = DefaultInformationColor;
        WarningColor = DefaultWarningColor;
        ErrorColor = DefaultErrorColor;
        CriticalColor = DefaultCriticalColor;
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is LogLevel level
            ? GetColor(level)
            : FallbackColor;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}