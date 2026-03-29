namespace Atc.Wpf.Components.ValueConverters;

/// <summary>
/// ValueConverter: LogCategoryTypeToResourceImage.
/// </summary>
[ValueConversion(typeof(LogCategoryType), typeof(BitmapImage))]
public class LogCategoryTypeToResourceImageValueConverter : IValueConverter
{
    [SuppressMessage("S1075", "S1075:URIs should not be hardcoded", Justification = "Pack URIs are required for WPF embedded resources.")]
    private const string ResourceBasePath = "pack://application:,,,/Atc.Wpf.Components;component/Resources/Images/";

    private static readonly Lazy<BitmapImage> CriticalImage = new(() => CreateFrozenBitmapImage("error.png"));
    private static readonly Lazy<BitmapImage> ErrorImage = new(() => CreateFrozenBitmapImage("error.png"));
    private static readonly Lazy<BitmapImage> WarningImage = new(() => CreateFrozenBitmapImage("warning.png"));
    private static readonly Lazy<BitmapImage> SecurityImage = new(() => CreateFrozenBitmapImage("security.png"));
    private static readonly Lazy<BitmapImage> AuditImage = new(() => CreateFrozenBitmapImage("audit.png"));
    private static readonly Lazy<BitmapImage> ServiceImage = new(() => CreateFrozenBitmapImage("service.png"));
    private static readonly Lazy<BitmapImage> UiImage = new(() => CreateFrozenBitmapImage("ui.png"));
    private static readonly Lazy<BitmapImage> InformationImage = new(() => CreateFrozenBitmapImage("information.png"));
    private static readonly Lazy<BitmapImage> DebugImage = new(() => CreateFrozenBitmapImage("debug.png"));
    private static readonly Lazy<BitmapImage> TraceImage = new(() => CreateFrozenBitmapImage("trace.png"));

    public static readonly LogCategoryTypeToResourceImageValueConverter Instance = new();

    public static BitmapImage GetImage(LogCategoryType logCategoryType)
        => logCategoryType switch
        {
            LogCategoryType.Critical => CriticalImage.Value,
            LogCategoryType.Error => ErrorImage.Value,
            LogCategoryType.Warning => WarningImage.Value,
            LogCategoryType.Security => SecurityImage.Value,
            LogCategoryType.Audit => AuditImage.Value,
            LogCategoryType.Service => ServiceImage.Value,
            LogCategoryType.UI => UiImage.Value,
            LogCategoryType.Information => InformationImage.Value,
            LogCategoryType.Debug => DebugImage.Value,
            LogCategoryType.Trace => TraceImage.Value,
            _ => throw new SwitchCaseDefaultException(logCategoryType),
        };

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not LogCategoryType logCategoryType)
        {
            throw new ArgumentException($"value is not {typeof(LogCategoryType)}", nameof(value));
        }

        return GetImage(logCategoryType);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();

    private static BitmapImage CreateFrozenBitmapImage(string fileName)
    {
        var bitmap = new BitmapImage(new Uri($"{ResourceBasePath}{fileName}", UriKind.Absolute));
        bitmap.Freeze();
        return bitmap;
    }
}