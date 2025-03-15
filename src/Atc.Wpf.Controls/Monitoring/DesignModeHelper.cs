namespace Atc.Wpf.Controls.Monitoring;

public static class DesignModeHelper
{
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