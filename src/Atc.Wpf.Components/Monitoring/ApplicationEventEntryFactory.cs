namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Factory methods for creating <see cref="ApplicationEventEntry"/> instances.
/// </summary>
public static class ApplicationEventEntryFactory
{
    /// <summary>
    /// Creates an information entry.
    /// </summary>
    public static ApplicationEventEntry CreateInformation(
        string area,
        string message)
        => new(LogCategoryType.Information, area, message);

    /// <summary>
    /// Creates a warning entry.
    /// </summary>
    public static ApplicationEventEntry CreateWarning(
        string area,
        string message)
        => new(LogCategoryType.Warning, area, message);

    /// <summary>
    /// Creates an error entry.
    /// </summary>
    public static ApplicationEventEntry CreateError(
        string area,
        string message)
        => new(LogCategoryType.Error, area, message);
}