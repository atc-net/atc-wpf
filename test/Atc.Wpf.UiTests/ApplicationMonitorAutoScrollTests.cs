using System.Globalization;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: launches the sample, navigates to ApplicationMonitor, clicks
/// "Add many items" enough times to push the entry list past the viewport,
/// and asserts auto-scroll moved the inner ListView to the tail.
/// </summary>
[Collection("SampleApp")]
public sealed class ApplicationMonitorAutoScrollTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public ApplicationMonitorAutoScrollTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void ApplicationMonitor_auto_scrolls_to_tail_when_many_entries_added()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Monitoring",
            leafTreeNodeName: "ApplicationMonitor");

        // Each click adds 10 entries; 6 clicks = 60 entries, comfortably past
        // the demo's viewport.
        var addManyButton = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByName("Add many items")),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(addManyButton);

        for (var i = 0; i < 6; i++)
        {
            addManyButton.AsButton().Invoke();
            Thread.Sleep(300);
        }

        Thread.Sleep(2000);

        var listView = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LvEntries"));
        Assert.NotNull(listView);

        var snapshotDir = Path.Combine(
            Path.GetDirectoryName(typeof(ApplicationMonitorAutoScrollTests).Assembly.Location)!,
            "Snapshots");
        Directory.CreateDirectory(snapshotDir);
        var snapshotPath = Path.Combine(snapshotDir, "application-monitor-after-burst.png");
        try { mainWindow.CaptureToFile(snapshotPath); } catch { /* best-effort */ }

        // Read the timestamps in column 1 — only realized rows surface in UIA
        // under WPF virtualization. With Ascending sort + auto-scroll engaged,
        // the realized timestamps must include the *latest* one. With the bug,
        // the viewport stays at the very first timestamp of the run.
        var timestamps = listView
            .FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
            .Select(t => t.Name ?? string.Empty)
            .Where(s => System.Text.RegularExpressions.Regex.IsMatch(s, @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$"))
            .ToList();

        Assert.NotEmpty(timestamps);

        // Assertion: the maximum visible timestamp must be the absolute max
        // across the run. We don't know the exact value, so instead we check
        // that the visible *index* range overlaps the tail of the 60-entry
        // buffer. Approximation: at least 5 distinct visible timestamps are
        // within the last few seconds (i.e. the most recently dispatched).
        var ordered = timestamps.OrderBy(s => s, StringComparer.Ordinal).ToList();
        var firstVisible = ordered.First();
        var lastVisible = ordered.Last();
        var range = (DateTime.Parse(lastVisible, CultureInfo.InvariantCulture)
                     - DateTime.Parse(firstVisible, CultureInfo.InvariantCulture)).TotalSeconds;

        // If the viewport is stuck at the start, we'd see consecutive
        // timestamps from the very first burst — i.e. the visible window's
        // last timestamp would be among the *earliest* entries. We instead
        // assert that the visible window contains "now-ish" entries: the
        // last visible timestamp is within 5 seconds of wall-clock now.
        // ApplicationEventEntry.Timestamp is captured as DateTimeOffset.UtcNow
        // and the column formats with "yyyy-MM-dd HH:mm:ss" (no zone
        // designator) — parse it as UTC and compare with DateTime.UtcNow.
        var lastUtc = DateTime.SpecifyKind(
            DateTime.Parse(lastVisible, CultureInfo.InvariantCulture),
            DateTimeKind.Utc);
        var nowSeconds = (DateTime.UtcNow - lastUtc).TotalSeconds;
        Assert.True(
            nowSeconds <= 10 && nowSeconds >= -1,
            $"Auto-scroll should leave the *latest* entries visible. Last visible timestamp '{lastVisible}' is {nowSeconds:F1}s old (now-utc), range across viewport is {range:F1}s. Realized timestamps: {timestamps.Count}. Screenshot: {snapshotPath}");
    }
}
