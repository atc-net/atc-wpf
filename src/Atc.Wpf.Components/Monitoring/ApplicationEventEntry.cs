// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable S1075
namespace Atc.Wpf.Components.Monitoring;

public class ApplicationEventEntry
{
    private static readonly ConcurrentDictionary<string, BitmapSource> ImageCache = new(StringComparer.Ordinal);
    private readonly LogCategoryType logCategoryType;

    public ApplicationEventEntry(
        string area,
        string message)
    {
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(message);

        LogCategoryType = LogCategoryType.Information;
        Area = area;
        Message = message;
    }

    public ApplicationEventEntry(
        LogCategoryType logCategoryType,
        string area,
        string message)
        : this(
            area,
            message)
    {
        LogCategoryType = logCategoryType;
    }

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    public LogCategoryType LogCategoryType
    {
        get => logCategoryType;

        private init
        {
            logCategoryType = value;
            LogCategoryTypeImage = GetImageByLogCategoryType(value);
            LogCategoryTypeToolTip = value.GetDescription();
        }
    }

    public BitmapSource? LogCategoryTypeImage { get; private init; }

    public string? LogCategoryTypeToolTip { get; private init; }

    public string Area { get; }

    public string Message { get; }

    private static BitmapSource GetImageByLogCategoryType(
        LogCategoryType logCategory)
    {
        var imagePath = logCategory switch
        {
            LogCategoryType.Critical or LogCategoryType.Error
                => "pack://application:,,,/Atc.Wpf.Components;component/Resources/Images/error.png",
            LogCategoryType.Warning
                => "pack://application:,,,/Atc.Wpf.Components;component/Resources/Images/warning.png",
            LogCategoryType.Security or
                LogCategoryType.Audit or
                LogCategoryType.Service or
                LogCategoryType.UI or
                LogCategoryType.Information or
                LogCategoryType.Debug or
                LogCategoryType.Trace
                => "pack://application:,,,/Atc.Wpf.Components;component/Resources/Images/information.png",
            _ => throw new SwitchCaseDefaultException(logCategory),
        };

        return ImageCache.GetOrAdd(
            imagePath,
            path =>
            {
                var uri = new Uri(
                    path,
                    UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            });
    }
}