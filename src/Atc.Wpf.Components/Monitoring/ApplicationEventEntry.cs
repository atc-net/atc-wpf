// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Represents a single event entry shown in the application monitor.
/// </summary>
public class ApplicationEventEntry
{
    private readonly LogCategoryType logCategoryType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationEventEntry"/> class with
    /// <see cref="LogCategoryType.Information"/> as category.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationEventEntry"/> class with the given category.
    /// </summary>
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

    /// <summary>
    /// Gets the UTC time at which the entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the log category (severity) of the entry.
    /// </summary>
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

    /// <summary>
    /// Gets the icon representing the <see cref="LogCategoryType"/>.
    /// </summary>
    public BitmapSource? LogCategoryTypeImage { get; private init; }

    /// <summary>
    /// Gets the tooltip text describing the <see cref="LogCategoryType"/>.
    /// </summary>
    public string? LogCategoryTypeToolTip { get; private init; }

    /// <summary>
    /// Gets the area (source) the event belongs to.
    /// </summary>
    public string Area { get; }

    /// <summary>
    /// Gets the event message.
    /// </summary>
    public string Message { get; }
}