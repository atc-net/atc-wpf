// ReSharper disable InconsistentNaming
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogCategoryType To Color.
/// </summary>
/// <remarks>
/// Source of truth for the log-category-type color palette.
/// <see cref="LogCategoryTypeToBrushValueConverter"/> wraps these same colors in frozen
/// <see cref="SolidColorBrush"/> instances.
/// <para>
/// Each per-category color is a mutable static property so consumers can override individual
/// entries at application startup, e.g.
/// <c>LogCategoryTypeToColorValueConverter.SecurityColor = Colors.Teal;</c>.
/// Call <see cref="ResetToDefaults"/> to restore the built-in palette.
/// </para>
/// <para>
/// Note: WPF bindings are not re-evaluated automatically when these static properties change —
/// set overrides during application startup before any binding fires.
/// </para>
/// </remarks>
[ValueConversion(typeof(LogCategoryType), typeof(Color))]
public sealed class LogCategoryTypeToColorValueConverter : IValueConverter
{
    public static readonly LogCategoryTypeToColorValueConverter Instance = new();

    public static readonly Color DefaultFallbackColor = BindingFallbacks.DefaultColor;
    public static readonly Color DefaultCriticalColor = Colors.Red;
    public static readonly Color DefaultErrorColor = Colors.Crimson;
    public static readonly Color DefaultWarningColor = Colors.Goldenrod;
    public static readonly Color DefaultSecurityColor = Colors.LightCyan;
    public static readonly Color DefaultAuditColor = Colors.AntiqueWhite;
    public static readonly Color DefaultServiceColor = Colors.BurlyWood;
    public static readonly Color DefaultUIColor = Colors.Aquamarine;
    public static readonly Color DefaultInformationColor = Colors.DodgerBlue;
    public static readonly Color DefaultDebugColor = Colors.CadetBlue;
    public static readonly Color DefaultTraceColor = Colors.Gray;

    /// <summary>
    /// Returned when the bound value is null, not a <see cref="LogCategoryType"/>,
    /// or an enum value not explicitly mapped.
    /// Delegates to <see cref="BindingFallbacks.Color"/> so the library-wide fallback stays in sync.
    /// </summary>
    public static Color FallbackColor
    {
        get => BindingFallbacks.Color;
        set => BindingFallbacks.Color = value;
    }

    public static Color CriticalColor { get; set; } = DefaultCriticalColor;

    public static Color ErrorColor { get; set; } = DefaultErrorColor;

    public static Color WarningColor { get; set; } = DefaultWarningColor;

    public static Color SecurityColor { get; set; } = DefaultSecurityColor;

    public static Color AuditColor { get; set; } = DefaultAuditColor;

    public static Color ServiceColor { get; set; } = DefaultServiceColor;

    public static Color UIColor { get; set; } = DefaultUIColor;

    public static Color InformationColor { get; set; } = DefaultInformationColor;

    public static Color DebugColor { get; set; } = DefaultDebugColor;

    public static Color TraceColor { get; set; } = DefaultTraceColor;

    /// <summary>
    /// Gets the <see cref="Color"/> currently configured for the given <paramref name="category"/>.
    /// </summary>
    public static Color GetColor(LogCategoryType category)
        => category switch
        {
            LogCategoryType.Critical => CriticalColor,
            LogCategoryType.Error => ErrorColor,
            LogCategoryType.Warning => WarningColor,
            LogCategoryType.Security => SecurityColor,
            LogCategoryType.Audit => AuditColor,
            LogCategoryType.Service => ServiceColor,
            LogCategoryType.UI => UIColor,
            LogCategoryType.Information => InformationColor,
            LogCategoryType.Debug => DebugColor,
            LogCategoryType.Trace => TraceColor,
            _ => FallbackColor,
        };

    /// <summary>
    /// Sets the <see cref="Color"/> override for a single <paramref name="category"/>.
    /// </summary>
    public static void SetColor(
        LogCategoryType category,
        Color color)
    {
        switch (category)
        {
            case LogCategoryType.Critical: CriticalColor = color; break;
            case LogCategoryType.Error: ErrorColor = color; break;
            case LogCategoryType.Warning: WarningColor = color; break;
            case LogCategoryType.Security: SecurityColor = color; break;
            case LogCategoryType.Audit: AuditColor = color; break;
            case LogCategoryType.Service: ServiceColor = color; break;
            case LogCategoryType.UI: UIColor = color; break;
            case LogCategoryType.Information: InformationColor = color; break;
            case LogCategoryType.Debug: DebugColor = color; break;
            case LogCategoryType.Trace: TraceColor = color; break;
            default: FallbackColor = color; break;
        }
    }

    /// <summary>
    /// Restores the built-in default palette (clears all consumer overrides).
    /// </summary>
    public static void ResetToDefaults()
    {
        FallbackColor = DefaultFallbackColor;
        CriticalColor = DefaultCriticalColor;
        ErrorColor = DefaultErrorColor;
        WarningColor = DefaultWarningColor;
        SecurityColor = DefaultSecurityColor;
        AuditColor = DefaultAuditColor;
        ServiceColor = DefaultServiceColor;
        UIColor = DefaultUIColor;
        InformationColor = DefaultInformationColor;
        DebugColor = DefaultDebugColor;
        TraceColor = DefaultTraceColor;
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is LogCategoryType category
            ? GetColor(category)
            : FallbackColor;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}