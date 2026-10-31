namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the DualListSelector sample page, the check-box style selector filters its available list,
/// "Select all" checks only the shown rows, and moving them puts exactly those items in the selected list.
/// The page writes the selected list to its <c>SelectedItemsLog</c> text after each transfer.
/// </summary>
[Collection("SampleApp")]
public sealed class DualListSelectorTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DualListSelectorTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow, traceBindingErrors: true);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void CheckBox_style_select_all_on_a_filtered_list_moves_only_the_matches()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfComponents",
            parentTreeNodeName: "Selectors",
            leafTreeNodeName: "DualListSelector");

        var selector = FindByAutomationId(mainWindow, "CheckBoxDualList");
        var log = FindByAutomationId(mainWindow, "SelectedItemsLog");
        var filter = FindByAutomationId(selector, "AvailableFilter");
        var selectAll = FindByAutomationId(selector, "AvailableSelectAll");

        filter.Patterns.Value.Pattern.SetValue("ap");
        Wait.UntilInputIsProcessed();
        Retry.WhileFalse(
            () => selectAll.HelpText == "0/3",
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal("0/3", selectAll.HelpText);

        selectAll.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Retry.WhileFalse(
            () => selectAll.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.On,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal("3/3", selectAll.HelpText);

        FindByAutomationId(selector, "MoveToSelectedButton").Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => log.Name.Length > 0,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal("Apple, Apricot, Grape", log.Name);

        var selectedList = FindByAutomationId(selector, "SelectedList");
        var rows = selectedList
            .FindAllDescendants(cf => cf.ByControlType(ControlType.CheckBox))
            .Select(x => x.Name)
            .ToArray();
        Assert.Equal(["Apple", "Apricot", "Grape"], rows);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Opening_the_page_and_moving_items_shows_no_binding_errors()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfComponents",
            parentTreeNodeName: "Selectors",
            leafTreeNodeName: "DualListSelector");

        var selector = FindByAutomationId(mainWindow, "CheckBoxDualList");
        Wait.UntilInputIsProcessed();
        BindingErrorWindows.AssertNone(application, automation);

        FindByAutomationId(selector, "AvailableSelectAll").Patterns.Toggle.Pattern.Toggle();
        FindByAutomationId(selector, "MoveToSelectedButton").Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();
        BindingErrorWindows.AssertNone(application, automation);

        // Bulk moves reach the lists as one Reset, which throws the rows away.
        FindByAutomationId(selector, "MoveAllToAvailableButton").Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();
        FindByAutomationId(selector, "MoveAllToSelectedButton").Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();
        BindingErrorWindows.AssertNone(application, automation);
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