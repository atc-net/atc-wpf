namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the NavigationView sample page, invoking a pane item navigates and selects it,
/// and the back button returns to the previous page and selects its item.
/// </summary>
[Collection("SampleApp")]
public sealed class NavigationViewTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public NavigationViewTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Invoking_items_navigates_and_back_selects_the_previous_item()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfControls",
            parentTreeNodeName: "Navigation",
            leafTreeNodeName: "NavigationView");

        var shell = FindByAutomationId(mainWindow, "ShellNavigationView");
        var home = FindItem(shell, "Home");
        var settings = FindItem(shell, "Settings");
        WaitUntil(() => Status().Contains("HomeViewModel", StringComparison.Ordinal));
        Assert.True(home.Patterns.SelectionItem.Pattern.IsSelected.Value);

        settings.Patterns.Invoke.Pattern.Invoke();
        WaitUntil(() => Status().Contains("SettingsViewModel", StringComparison.Ordinal));
        Assert.True(settings.Patterns.SelectionItem.Pattern.IsSelected.Value);
        Assert.False(home.Patterns.SelectionItem.Pattern.IsSelected.Value);

        FindByName(shell, "Back").Patterns.Invoke.Pattern.Invoke();
        WaitUntil(() => Status().Contains("HomeViewModel", StringComparison.Ordinal));
        Assert.True(home.Patterns.SelectionItem.Pattern.IsSelected.Value);
    }

    private string Status()
        => FindByAutomationId(mainWindow, "TbNavigationStatus").Name;

    private static AutomationElement FindItem(
        AutomationElement shell,
        string name)
    {
        var item = Retry.WhileNull(
            () => shell.FindFirstDescendant(cf => cf.ByControlType(ControlType.ListItem).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(item);
        return item;
    }

    private static AutomationElement FindByName(
        AutomationElement parent,
        string name)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByName(name)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

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

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}