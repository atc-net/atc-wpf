using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: launches the sample, navigates to TerminalViewer, fires a burst
/// of incoming lines, and asserts that the inner ListView's
/// <see cref="FlaUI.Core.Patterns.IScrollPattern.VerticalScrollPercent"/> ends
/// at (or near) 100% — i.e. auto-scroll engaged.
/// </summary>
[Collection("SampleApp")]
public sealed class TerminalViewerAutoScrollTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public TerminalViewerAutoScrollTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void TerminalViewer_auto_scrolls_to_tail_when_burst_arrives()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Viewers",
            leafTreeNodeName: "TerminalViewer");

        // Diagnostic: capture what we see right after navigation.
        var snapshotDir = Path.Combine(
            Path.GetDirectoryName(typeof(TerminalViewerAutoScrollTests).Assembly.Location)!,
            "Snapshots");
        Directory.CreateDirectory(snapshotDir);
        try
        {
            mainWindow.CaptureToFile(Path.Combine(snapshotDir, "terminal-after-nav.png"));
        }
        catch
        {
            // Best-effort.
        }

        // Five bursts of 100 lines each = 500 lines streamed through the
        // channel. With the default viewport that's well past overflow, so
        // auto-scroll has to actually do its job.
        var burstButton = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByName("Send burst (100 lines)")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(300)).Result;

        if (burstButton is null)
        {
            // Diagnostic: list all buttons on screen.
            var allButtons = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
            var names = string.Join(" | ", allButtons.Select(b => $"\"{b.Name}\""));
            Assert.Fail($"'Send burst (100 lines)' button not found. Visible buttons: {names}");
        }

        for (var i = 0; i < 5; i++)
        {
            burstButton.AsButton().Invoke();
            Thread.Sleep(300);
        }

        // Drain loop runs in the background; let it settle.
        Thread.Sleep(2500);

        var listView = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ListViewTerminal"));
        Assert.NotNull(listView);

        // Best-effort screenshot for diagnostics.
        var snapshotPath = Path.Combine(snapshotDir, "terminal-after-burst.png");
        try { mainWindow.CaptureToFile(snapshotPath); } catch { /* ignore */ }

        // Under WPF virtualization the only ListItems exposed to UIA are the
        // ones currently realized in the viewport. If auto-scroll worked, the
        // viewport contains items near the very end of the burst; if it
        // didn't, the viewport is stuck at the top.
        const int totalLines = 500; // 5 bursts × 100 lines
        var realized = listView.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
        Assert.NotEmpty(realized);

        // The DataTemplate populates TextBlock.Inlines via the
        // TerminalLineHighlight attached property; UIA's ListItem.Name may be
        // empty, so look at descendant Text elements instead.
        var allTexts = listView
            .FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
            .Select(t => t.Name ?? string.Empty)
            .Where(s => s.StartsWith("burst ", StringComparison.Ordinal))
            .ToList();

        var maxLineNumber = allTexts
            .Select(ExtractBurstNumber)
            .DefaultIfEmpty(0)
            .Max();

        Assert.True(
            maxLineNumber >= totalLines - 30,
            $"Auto-scroll should leave the tail in view. Highest realized burst-line was {maxLineNumber} (expected near {totalLines}). " +
            $"Realized 'burst' lines: {allTexts.Count}, sample names: [{string.Join(", ", allTexts.Take(5))}]. " +
            $"Screenshot: {snapshotPath}");
    }

    private static int ExtractBurstNumber(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        // Lines look like "burst 00432 – <guid>".
        var space = text.IndexOf(' ', StringComparison.Ordinal);
        if (space < 0 || space + 1 >= text.Length)
        {
            return 0;
        }

        var rest = text[(space + 1)..];
        var end = 0;
        while (end < rest.Length && char.IsDigit(rest[end]))
        {
            end++;
        }

        return end > 0 && int.TryParse(rest[..end], out var n) ? n : 0;
    }
}
