namespace Atc.Wpf.Components.Monitoring;

public static class ApplicationEventEntryFactory
{
    public static ApplicationEventEntry CreateInformation(
        string area,
        string message)
        => new(LogCategoryType.Information, area, message);

    public static ApplicationEventEntry CreateWarning(
        string area,
        string message)
        => new(LogCategoryType.Warning, area, message);

    public static ApplicationEventEntry CreateError(
        string area,
        string message)
        => new(LogCategoryType.Error, area, message);
}