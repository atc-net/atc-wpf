namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Message sent when new entries are added, asking the application monitor view to scroll to the newest entry.
/// </summary>
/// <param name="Direction">The current sort direction, which determines whether the newest entry is at the top or the bottom.</param>
public record ApplicationMonitorScrollEvent(
    ListSortDirection Direction);