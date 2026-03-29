// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Components.Monitoring;

public class ApplicationEventEntry
{
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
            LogCategoryTypeImage = LogCategoryTypeToResourceImageValueConverter.GetImage(value);
            LogCategoryTypeToolTip = value.GetDescription();
        }
    }

    public BitmapSource? LogCategoryTypeImage { get; private init; }

    public string? LogCategoryTypeToolTip { get; private init; }

    public string Area { get; }

    public string Message { get; }
}