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