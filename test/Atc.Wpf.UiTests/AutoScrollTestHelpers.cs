using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

namespace Atc.Wpf.UiTests;

/// <summary>
/// Shared launch / navigation / scroll-readback helpers for the auto-scroll
/// E2E tests.
/// </summary>
internal static class AutoScrollTestHelpers
{
    public const int LaunchTimeoutMilliseconds = 30_000;

    public static Application LaunchSample(out UIA3Automation automation, out Window mainWindow)
    {
        var exePath = SampleAppPath.Resolve();
        var startInfo = new ProcessStartInfo(exePath)
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(exePath)!,
        };

        var application = Application.Launch(startInfo);
        automation = new UIA3Automation();
        mainWindow = application.GetMainWindow(
            automation,
            TimeSpan.FromMilliseconds(LaunchTimeoutMilliseconds));

        // Ensure the test window is visible and foregrounded — without this,
        // off-screen tree-view items can be enumerated but mouse clicks land
        // on whatever owns those screen coordinates instead.
        var windowPattern = mainWindow.Patterns.Window.PatternOrDefault;
        if (windowPattern is not null &&
            windowPattern.CanMaximize.Value)
        {
            windowPattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Maximized);
            Thread.Sleep(300);
        }

        mainWindow.Focus();
        return application;
    }

    public static void CleanupApplication(Application application, UIA3Automation automation)
    {
#pragma warning disable CA1031
        try
        {
            application.Close();
            application.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5));
        }
        catch (Exception)
        {
            // Best-effort cleanup; process may already be gone.
        }
#pragma warning restore CA1031

        automation.Dispose();
        application.Dispose();
    }

    /// <summary>
    /// Routes navigation through the sample app's filter textbox: typing the
    /// leaf name into <c>TbSampleFilter</c> hides everything else, after which
    /// the lone surviving <see cref="ControlType.TreeItem"/> match is reliably
    /// hit-tested by a real mouse click. This avoids the WPF tree-view's
    /// virtualization corner-cases that prevent clicks against off-screen
    /// items.
    /// </summary>
    public static void NavigateToSample(
        Window mainWindow,
        string tabAutomationId,
        string parentTreeNodeName,
        string leafTreeNodeName)
    {
        // tabAutomationId/parentTreeNodeName retained for back-compat with
        // existing test signatures; the filter approach doesn't need them.
        _ = tabAutomationId;
        _ = parentTreeNodeName;

        var filterBox = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("TbSampleFilter")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(filterBox);

        var textBox = filterBox.AsTextBox();
        textBox.Focus();
        textBox.Text = leafTreeNodeName;

        // FilterOnTextChanged in MainWindow.xaml.cs runs synchronously on UI
        // thread; give layout a couple of ticks to settle.
        Thread.Sleep(500);

        var leaf = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.TreeItem).And(cf.ByName(leafTreeNodeName))),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(leaf);

        var scrollItem = leaf.Patterns.ScrollItem.PatternOrDefault;
        scrollItem?.ScrollIntoView();
        Thread.Sleep(200);

        // SampleTreeViewItem overrides OnMouseLeftButtonDown to set IsSelected
        // and load the sample. SelectionItemPattern.Select() times out against
        // the custom class — a real mouse click fires the override correctly.
        leaf.Click();

        // Sample loads via SelectedItemChanged + reflection — give it time.
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Resolves an <see cref="FlaUI.Core.Patterns.IScrollPattern"/> on the
    /// supplied list-view, or on the first scrollable descendant if the
    /// list-view itself doesn't expose one (depends on the WPF list's content
    /// overflowing — the pattern only surfaces when there's actually something
    /// to scroll). Captures a screenshot for diagnostics.
    /// </summary>
    public static double ReadVerticalScrollPercent(
        Window mainWindow,
        AutomationElement listView,
        string snapshotName)
    {
        // Best-effort capture for after-the-fact debugging.
        var snapshotDir = Path.Combine(
            Path.GetDirectoryName(typeof(AutoScrollTestHelpers).Assembly.Location)!,
            "Snapshots");
        Directory.CreateDirectory(snapshotDir);
        try
        {
            mainWindow.CaptureToFile(Path.Combine(snapshotDir, snapshotName));
        }
        catch (Exception)
        {
            // Screenshot is optional; ignore failures.
        }

        var scrollPattern = listView.Patterns.Scroll.PatternOrDefault;
        if (scrollPattern is null)
        {
            var scrollHost = listView.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Pane));
            if (scrollHost is not null)
            {
                scrollPattern = scrollHost.Patterns.Scroll.PatternOrDefault;
            }
        }

        Assert.NotNull(scrollPattern);
        return scrollPattern.VerticalScrollPercent.Value;
    }
}
