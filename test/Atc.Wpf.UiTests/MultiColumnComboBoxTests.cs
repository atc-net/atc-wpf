namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the MultiColumnComboBox sample page through UI Automation patterns: the pop-up grid is filtered from
/// the filter box, two rows are ticked through their check boxes, a column header sorts the grid without changing the
/// ticks, and the summary (the combo box value) and the event log show the two customers.
/// </summary>
[Collection("SampleApp")]
public sealed class MultiColumnComboBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public MultiColumnComboBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Filtering_ticking_two_rows_and_sorting_keeps_the_ticks_and_shows_both_in_the_summary()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "MultiColumnComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox", ControlType.ComboBox);
        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;

        expandCollapse.Expand();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        var filter = FindByAutomationId(box, "Filter", ControlType.Edit);
        WaitForPopupAnimation(filter);
        filter.Patterns.Value.Pattern.SetValue("Berlin");

        var grid = FindByAutomationId(box, "Items", ControlType.DataGrid);
        WaitUntil(() => grid.FindAllChildren(cf => cf.ByControlType(ControlType.DataItem)).Length == 3);
        Assert.Equal(3, grid.FindAllChildren(cf => cf.ByControlType(ControlType.DataItem)).Length);

        var bravo = FindRowCheckBox(grid, "Bravo Foods");
        bravo.Patterns.Toggle.Pattern.Toggle();
        WaitUntil(() => bravo.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);

        var lima = FindRowCheckBox(grid, "Lima Robotics");
        lima.Patterns.Toggle.Pattern.Toggle();
        WaitUntil(() => lima.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);

        var nameHeader = Retry.WhileNull(
            () => grid.FindFirstDescendant(cf => cf.ByControlType(ControlType.HeaderItem).And(cf.ByName("Name"))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(nameHeader);
        nameHeader.Patterns.Invoke.Pattern.Invoke();
        nameHeader.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(1));

        var firstRowName = grid
            .FindAllChildren(cf => cf.ByControlType(ControlType.DataItem))[0]
            .FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox))
            ?.Name;
        Assert.Equal("Victor Motors", firstRowName);
        Assert.Equal(ToggleState.On, FindRowCheckBox(grid, "Bravo Foods").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindRowCheckBox(grid, "Lima Robotics").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindRowCheckBox(grid, "Victor Motors").Patterns.Toggle.Pattern.ToggleState.Value);

        expandCollapse.Collapse();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed);

        Assert.Equal("Bravo Foods, Lima Robotics", box.Patterns.Value.Pattern.Value.Value);

        var log = FindByAutomationId(mainWindow, "SelectedKeysLog", ControlType.Text);
        Assert.Contains("[] -> [1002]", log.Name, StringComparison.Ordinal);
        Assert.Contains("[1002] -> [1002, 1012]", log.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Waits until the pop-up's opening (slide) animation is over: the row's bounds are final before the animation
    /// ends, so the wait also gives the animation a second.
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

    private static AutomationElement FindRowCheckBox(
        AutomationElement grid,
        string name)
    {
        var row = Retry.WhileNull(
            () => grid.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(row);
        return row;
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