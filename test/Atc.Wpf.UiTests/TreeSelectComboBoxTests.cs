namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the TreeSelectComboBox sample page with real mouse clicks: a click on the box opens the pop-up, a click
/// on a row's text toggles the node and keeps the pop-up open, a click on a row's arrow only expands it, and a click
/// outside closes the pop-up.
/// </summary>
[Collection("SampleApp")]
public sealed class TreeSelectComboBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public TreeSelectComboBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Clicking_rows_toggles_them_the_arrow_only_expands_and_a_click_outside_closes()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "TreeSelectComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox", ControlType.ComboBox);
        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;

        box.Click();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        var norway = FindTreeItem(box, "Norway");
        WaitForPopupAnimation(norway);
        norway.Click();
        WaitUntil(() => norway.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);
        Assert.Equal(ToggleState.On, norway.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        var sweden = FindTreeItem(box, "Sweden");
        var swedenState = sweden.Patterns.Toggle.Pattern.ToggleState.Value;
        var arrow = sweden.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button));
        Assert.NotNull(arrow);
        arrow.Click();
        WaitUntil(() => sweden.Patterns.ExpandCollapse.Pattern.ExpandCollapseState.Value == ExpandCollapseState.Expanded);
        Assert.Equal(swedenState, sweden.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        var malmo = FindTreeItem(box, "Malmö");
        malmo.Click();
        WaitUntil(() => malmo.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        FindByAutomationId(mainWindow, "SelectedKeysLog", ControlType.Text).Click();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed);
        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);
    }

    /// <summary>
    /// Waits until the pop-up's opening (slide) animation is over, so a click lands on the row: the row's bounds are
    /// final before the animation ends, so the wait also gives the animation a second.
    /// </summary>
    private static void WaitForPopupAnimation(AutomationElement element)
    {
        var last = element.BoundingRectangle;
        Retry.WhileFalse(
            () =>
            {
                var current = element.BoundingRectangle;
                var isStable = current == last;
                last = current;
                return isStable;
            },
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(250));
        Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(1));
    }

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));

    private static AutomationElement FindTreeItem(
        AutomationElement parent,
        string name)
    {
        var item = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByControlType(ControlType.TreeItem).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(item);
        return item;
    }

    private static AutomationElement FindByAutomationId(
        AutomationElement parent,
        string automationId,
        ControlType controlType)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByAutomationId(automationId).And(cf.ByControlType(controlType))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }
}