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
    /// <summary>
    /// Gets a static default instance of <see cref="LogCategoryTypeToColorValueConverter"/>.
    /// </summary>
    public static readonly LogCategoryTypeToColorValueConverter Instance = new();

    /// <summary>The built-in default fallback color, used when a value cannot be mapped.</summary>
    public static readonly Color DefaultFallbackColor = BindingFallbacks.DefaultColor;

    /// <summary>The built-in default color for the <c>Critical</c> log category type.</summary>
    public static readonly Color DefaultCriticalColor = Colors.Red;

    /// <summary>The built-in default color for the <c>Error</c> log category type.</summary>
    public static readonly Color DefaultErrorColor = Colors.Crimson;

    /// <summary>The built-in default color for the <c>Warning</c> log category type.</summary>
    public static readonly Color DefaultWarningColor = Colors.Goldenrod;

    /// <summary>The built-in default color for the <c>Security</c> log category type.</summary>
    public static readonly Color DefaultSecurityColor = Colors.LightCyan;

    /// <summary>The built-in default color for the <c>Audit</c> log category type.</summary>
    public static readonly Color DefaultAuditColor = Colors.AntiqueWhite;

    /// <summary>The built-in default color for the <c>Service</c> log category type.</summary>
    public static readonly Color DefaultServiceColor = Colors.BurlyWood;

    /// <summary>The built-in default color for the <c>UI</c> log category type.</summary>
    public static readonly Color DefaultUIColor = Colors.Aquamarine;

    /// <summary>The built-in default color for the <c>Information</c> log category type.</summary>
    public static readonly Color DefaultInformationColor = Colors.DodgerBlue;

    /// <summary>The built-in default color for the <c>Debug</c> log category type.</summary>
    public static readonly Color DefaultDebugColor = Colors.CadetBlue;

    /// <summary>The built-in default color for the <c>Trace</c> log category type.</summary>
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

    /// <summary>Gets or sets the color used for the <c>Critical</c> log category type.</summary>
    public static Color CriticalColor { get; set; } = DefaultCriticalColor;

    /// <summary>Gets or sets the color used for the <c>Error</c> log category type.</summary>
    public static Color ErrorColor { get; set; } = DefaultErrorColor;

    /// <summary>Gets or sets the color used for the <c>Warning</c> log category type.</summary>
    public static Color WarningColor { get; set; } = DefaultWarningColor;

    /// <summary>Gets or sets the color used for the <c>Security</c> log category type.</summary>
    public static Color SecurityColor { get; set; } = DefaultSecurityColor;

    /// <summary>Gets or sets the color used for the <c>Audit</c> log category type.</summary>
    public static Color AuditColor { get; set; } = DefaultAuditColor;

    /// <summary>Gets or sets the color used for the <c>Service</c> log category type.</summary>
    public static Color ServiceColor { get; set; } = DefaultServiceColor;

    /// <summary>Gets or sets the color used for the <c>UI</c> log category type.</summary>
    public static Color UIColor { get; set; } = DefaultUIColor;

    /// <summary>Gets or sets the color used for the <c>Information</c> log category type.</summary>
    public static Color InformationColor { get; set; } = DefaultInformationColor;

    /// <summary>Gets or sets the color used for the <c>Debug</c> log category type.</summary>
    public static Color DebugColor { get; set; } = DefaultDebugColor;

    /// <summary>Gets or sets the color used for the <c>Trace</c> log category type.</summary>
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