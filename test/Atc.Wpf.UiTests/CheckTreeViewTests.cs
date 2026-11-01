namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the CheckTreeView sample page. The rows are UI Automation tree items with ExpandCollapse and a
/// three-state Toggle: expanding "Settings" and ticking it ticks its children; unticking one child leaves "Settings"
/// indeterminate. The page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class CheckTreeViewTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public CheckTreeViewTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Ticking_a_parent_ticks_its_children_and_unticking_one_child_makes_the_parent_indeterminate()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CheckTreeView");

        var tree = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");

        var settings = FindTreeItem(tree, "Settings");
        settings.Patterns.ExpandCollapse.Pattern.Expand();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ExpandCollapseState.Expanded, settings.Patterns.ExpandCollapse.Pattern.ExpandCollapseState.Value);

        var settingsToggle = settings.Patterns.Toggle.Pattern;
        Assert.Equal(ToggleState.Off, settingsToggle.ToggleState.Value);

        settingsToggle.Toggle();
        Wait.UntilInputIsProcessed();

        Assert.Equal(ToggleState.On, settingsToggle.ToggleState.Value);
        var editSettings = FindTreeItem(tree, "Edit settings");
        Assert.Equal(ToggleState.On, editSettings.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindTreeItem(tree, "View settings").Patterns.Toggle.Pattern.ToggleState.Value);

        editSettings.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Assert.Equal(ToggleState.Off, editSettings.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Indeterminate, settingsToggle.ToggleState.Value);

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        var lines = Lines(log.Name);
        Assert.Equal(2, lines.Length);
        Assert.EndsWith("-> [admin.settings]", lines[0], StringComparison.Ordinal);
        Assert.EndsWith("[admin.settings] -> [admin.settings.view]", lines[1], StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Ticking_a_row_raises_toggle_state_changes_for_screen_readers()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CheckTreeView");

        var tree = FindByAutomationId(mainWindow, "InteractiveBox");
        var settings = FindTreeItem(tree, "Settings");
        settings.Patterns.ExpandCollapse.Pattern.Expand();
        Wait.UntilInputIsProcessed();
        var editSettings = FindTreeItem(tree, "Edit settings");

        var changedNames = new System.Collections.Concurrent.ConcurrentQueue<string>();
        using var handler = tree.RegisterPropertyChangedEvent(
            TreeScope.Subtree,
            (element, _, _) => changedNames.Enqueue(element.Properties.Name.ValueOrDefault ?? string.Empty),
            automation.PropertyLibrary.Toggle.ToggleState);

        editSettings.Patterns.Toggle.Pattern.Toggle();

        Retry.WhileFalse(
            () => changedNames.Contains("Edit settings", StringComparer.Ordinal) && changedNames.Contains("Settings", StringComparer.Ordinal),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Contains("Edit settings", changedNames, StringComparer.Ordinal);
        Assert.Contains("Settings", changedNames, StringComparer.Ordinal);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static AutomationElement FindTreeItem(
        AutomationElement tree,
        string name)
    {
        var item = Retry.WhileNull(
            () => tree.FindFirstDescendant(cf => cf.ByControlType(ControlType.TreeItem).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(item);
        return item;
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
}