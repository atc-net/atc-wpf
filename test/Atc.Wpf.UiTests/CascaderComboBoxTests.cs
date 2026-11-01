namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the CascaderComboBox sample page through UI Automation patterns: the columns open by expanding rows,
/// a tick deep down makes its parent indeterminate, the filter lists matches from every level, and the summary
/// (the combo box value) shows the full paths of the ticked nodes.
/// </summary>
[Collection("SampleApp")]
public sealed class CascaderComboBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public CascaderComboBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Activating_columns_ticking_a_city_and_a_filter_result_shows_both_paths_in_the_summary()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CascaderComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox", ControlType.ComboBox);
        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;

        expandCollapse.Expand();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        var denmark = FindRow(box, "Column0", "Denmark");
        WaitForPopupAnimation(denmark);
        denmark.Patterns.ExpandCollapse.Pattern.Expand();

        var jutland = FindRow(box, "Column1", "Jutland");
        jutland.Patterns.ExpandCollapse.Pattern.Expand();

        var aarhus = FindRow(box, "Column2", "Aarhus");
        aarhus.Patterns.Toggle.Pattern.Toggle();
        WaitUntil(() => aarhus.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);

        Assert.Equal(ToggleState.Indeterminate, jutland.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Indeterminate, denmark.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        var filter = FindByAutomationId(box, "Filter", ControlType.Edit);
        filter.Patterns.Value.Pattern.SetValue("malm");

        var malmo = FindRow(box, "FilterResults", "Malmö");
        malmo.Patterns.Toggle.Pattern.Toggle();
        WaitUntil(() => malmo.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);

        expandCollapse.Collapse();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed);

        var summary = box.Patterns.Value.Pattern.Value.Value;
        Assert.Contains("Denmark / Jutland / Aarhus", summary, StringComparison.Ordinal);
        Assert.Contains("Sweden / Götaland / Malmö", summary, StringComparison.Ordinal);

        var log = FindByAutomationId(mainWindow, "SelectedKeysLog", ControlType.Text);
        Assert.Contains("[dk-jut-aar, se-got-mal]", log.Name, StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void A_click_on_the_text_activates_a_row_a_click_on_its_check_box_ticks_it_and_a_click_outside_closes()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CascaderComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox", ControlType.ComboBox);
        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;

        box.Click();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        var sweden = FindRow(box, "Column0", "Sweden");
        WaitForPopupAnimation(sweden);
        var swedenBounds = sweden.BoundingRectangle;
        Mouse.Click(new System.Drawing.Point(swedenBounds.Left + (swedenBounds.Width / 2), swedenBounds.Top + (swedenBounds.Height / 2)));
        WaitUntil(() => sweden.Patterns.ExpandCollapse.Pattern.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        Assert.Equal(ToggleState.Off, sweden.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.NotNull(FindRow(box, "Column1", "Svealand"));

        var norway = FindRow(box, "Column0", "Norway");
        var norwayBounds = norway.BoundingRectangle;
        Mouse.Click(new System.Drawing.Point(norwayBounds.Left + 10, norwayBounds.Top + (norwayBounds.Height / 2)));
        WaitUntil(() => norway.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On);

        Assert.Equal(ToggleState.On, norway.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ExpandCollapseState.Collapsed, norway.Patterns.ExpandCollapse.Pattern.ExpandCollapseState.Value);
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        FindByAutomationId(mainWindow, "SelectedKeysLog", ControlType.Text).Click();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed);

        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Rows_raise_toggle_and_expand_collapse_state_changes_for_screen_readers()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CascaderComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox", ControlType.ComboBox);
        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;
        expandCollapse.Expand();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded);

        var denmark = FindRow(box, "Column0", "Denmark");
        WaitForPopupAnimation(denmark);

        var toggled = new System.Collections.Concurrent.ConcurrentQueue<string>();
        var expanded = new System.Collections.Concurrent.ConcurrentQueue<string>();
        using var toggleHandler = denmark.RegisterPropertyChangedEvent(
            TreeScope.Element,
            (element, _, _) => toggled.Enqueue(element.Properties.Name.ValueOrDefault ?? string.Empty),
            automation.PropertyLibrary.Toggle.ToggleState);
        using var expandHandler = denmark.RegisterPropertyChangedEvent(
            TreeScope.Element,
            (element, _, _) => expanded.Enqueue(element.Properties.Name.ValueOrDefault ?? string.Empty),
            automation.PropertyLibrary.ExpandCollapse.ExpandCollapseState);

        denmark.Patterns.ExpandCollapse.Pattern.Expand();
        WaitUntil(() => !expanded.IsEmpty);

        denmark.Patterns.Toggle.Pattern.Toggle();
        WaitUntil(() => !toggled.IsEmpty);

        Assert.Contains("Denmark", expanded, StringComparer.Ordinal);
        Assert.Contains("Denmark", toggled, StringComparer.Ordinal);

        denmark.Patterns.Toggle.Pattern.Toggle();
        expandCollapse.Collapse();
        WaitUntil(() => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed);
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

    private static AutomationElement FindRow(
        AutomationElement box,
        string listAutomationId,
        string name)
    {
        var row = Retry.WhileNull(
            () => box
                .FindFirstDescendant(cf => cf.ByAutomationId(listAutomationId).And(cf.ByControlType(ControlType.List)))
                ?.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
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