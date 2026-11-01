namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the DockPanelPro sample page, invoking the float button of the right region floats its active
/// pane into a separate tool window, invoking that window's Dock button docks it back and closes the window, and
/// selecting a tab of the bottom region through SelectionItem activates it. The page writes each pane event to its
/// <c>DockLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class DockPanelProTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DockPanelProTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Floating_a_pane_opens_a_tool_window_docking_it_closes_the_window_and_tabs_select()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "DockPanelPro");

        var log = FindByAutomationId(mainWindow, "DockLog");
        var floatButton = FindByAutomationId(mainWindow, "FloatPane_Right");
        Assert.Equal(ControlType.Button, floatButton.ControlType);

        floatButton.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        var floatingWindow = Retry.WhileNull(
            () => FindFloatingWindow("FloatingPane_properties"),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(floatingWindow);
        Assert.Contains("Properties: Docked -> Floating", log.Name, StringComparison.Ordinal);

        var dockButton = Retry.WhileNull(
            () => floatingWindow.FindFirstDescendant(cf => cf.ByAutomationId("RedockPane")),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(dockButton);
        dockButton.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => FindFloatingWindow("FloatingPane_properties") is null,
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Null(FindFloatingWindow("FloatingPane_properties"));
        Assert.Contains("Properties: Floating -> Docked", log.Name, StringComparison.Ordinal);

        var errorsTab = FindByAutomationId(mainWindow, "errors");
        Assert.Equal(ControlType.TabItem, errorsTab.ControlType);
        errorsTab.Patterns.SelectionItem.Pattern.Select();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => errorsTab.Patterns.SelectionItem.Pattern.IsSelected.Value,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.True(errorsTab.Patterns.SelectionItem.Pattern.IsSelected.Value);
        Assert.Contains("Active: Errors", log.Name, StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Collapsing_a_region_through_ExpandCollapse_raises_the_state_change()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "DockPanelPro");

        var region = FindByAutomationId(mainWindow, "Left");
        var states = new System.Collections.Concurrent.ConcurrentQueue<object>();
        using var handler = region.RegisterPropertyChangedEvent(
            TreeScope.Element,
            (_, _, value) => states.Enqueue(value),
            automation.PropertyLibrary.ExpandCollapse.ExpandCollapseState);

        region.Patterns.ExpandCollapse.Pattern.Collapse();
        Retry.WhileFalse(
            () => !states.IsEmpty,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        region.Patterns.ExpandCollapse.Pattern.Expand();

        Assert.False(states.IsEmpty);
        Assert.Equal(ExpandCollapseState.Collapsed, (ExpandCollapseState)Convert.ToInt32(states.First(), CultureInfo.InvariantCulture));
    }

    [Trait("Category", "UI")]
    [Fact]
    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "The system move loop follows real mouse moves over wall time.")]
    public void Dragging_a_floating_window_onto_the_bottom_guide_docks_it_there()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "DockPanelPro");

        var log = FindByAutomationId(mainWindow, "DockLog");
        var panel = FindByAutomationId(mainWindow, "InteractiveDockPanel");

        // The splitter is SplitterThickness (5) device-independent units high: it gives the DPI scale.
        var scale = FindByAutomationId(mainWindow, "Splitter_Bottom").BoundingRectangle.Height / 5d;

        FindByAutomationId(mainWindow, "FloatPane_Right").Patterns.Invoke.Pattern.Invoke();
        var floatingWindow = Retry.WhileNull(
            () => FindFloatingWindow("FloatingPane_properties"),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(floatingWindow);
        Wait.UntilInputIsProcessed();

        var windowBounds = floatingWindow.BoundingRectangle;
        var start = new System.Drawing.Point(windowBounds.Left + (windowBounds.Width / 3), windowBounds.Top + (int)(12 * scale));
        var panelBounds = panel.BoundingRectangle;
        var target = new System.Drawing.Point(
            panelBounds.Left + (panelBounds.Width / 2),
            panelBounds.Top + (panelBounds.Height / 2) + (int)(36 * scale));

        Mouse.MoveTo(start);
        Mouse.Down(MouseButton.Left);
        try
        {
            for (var i = 1; i <= 10; i++)
            {
                Mouse.MoveTo(
                    start.X + ((target.X - start.X) * i / 10),
                    start.Y + ((target.Y - start.Y) * i / 10));
                Thread.Sleep(30);
            }

            Mouse.MoveTo(target);
            Thread.Sleep(200);
        }
        finally
        {
            Mouse.Up(MouseButton.Left);
        }

        Retry.WhileFalse(
            () => FindFloatingWindow("FloatingPane_properties") is null,
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Null(FindFloatingWindow("FloatingPane_properties"));
        Assert.Contains("Properties: Floating -> Docked", log.Name, StringComparison.Ordinal);
        var bottomRegion = FindByAutomationId(mainWindow, "Bottom");
        Assert.NotNull(bottomRegion.FindFirstDescendant(cf => cf.ByAutomationId("properties")));
    }

    private AutomationElement? FindFloatingWindow(string automationId)
        => mainWindow.FindFirstChild(cf => cf.ByAutomationId(automationId))
           ?? automation.GetDesktop().FindFirstChild(cf => cf.ByAutomationId(automationId));

    private static AutomationElement FindByAutomationId(
        AutomationElement parent,
        string automationId)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }
}