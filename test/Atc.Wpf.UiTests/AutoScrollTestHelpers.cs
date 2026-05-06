namespace Atc.Wpf.UiTests;

/// <summary>
/// Shared launch / navigation / scroll-readback helpers for the auto-scroll
/// E2E tests.
/// </summary>
internal static class AutoScrollTestHelpers
{
    public const int LaunchTimeoutMilliseconds = 30_000;

    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "FlaUI E2E tests must yield real wall time for the WPF UI thread to render between actions.")]
    public static Application LaunchSample(
        out UIA3Automation automation,
        out Window mainWindow)
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup; process may already be gone, transient FlaUI navigation errors must not mask the test outcome.")]
    public static void CleanupApplication(
        Application application,
        UIA3Automation automation)
    {
        try
        {
            application.Close();
            application.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5));
        }
        catch (Exception)
        {
            // Best-effort cleanup; process may already be gone.
        }

        automation.Dispose();
        application.Dispose();
    }

    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "FlaUI E2E tests must yield real wall time for the WPF UI thread to render between actions.")]
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort screenshot capture must not fail the test on transient I/O errors.")]
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