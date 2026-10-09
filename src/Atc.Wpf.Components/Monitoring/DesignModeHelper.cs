namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Provides sample data for the application monitor at design time.
/// </summary>
public static class DesignModeHelper
{
    /// <summary>
    /// Creates a small list of sample <see cref="ApplicationEventEntry"/> items.
    /// </summary>
    public static IEnumerable<ApplicationEventEntry> CreateApplicationEventEntryList()
    {
        var list = new List<ApplicationEventEntry>
        {
            new(LogCategoryType.Information, "Area1", "Hello world 1"),
            new(LogCategoryType.Information, "Area1", "Hello world 2"),
            new(LogCategoryType.Warning, "Area1", "Hello world 3"),
            new(LogCategoryType.Error, "Area1", "Hello world 4"),
        };

        return list;
    }
}