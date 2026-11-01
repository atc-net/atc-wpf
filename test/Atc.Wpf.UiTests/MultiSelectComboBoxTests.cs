namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E tests on the MultiSelectComboBox sample page. Opening the drop-down through the UI Automation ExpandCollapse
/// pattern, ticking two of the three rows through Toggle and closing it again leaves the summary "Orders, Payments"
/// as the control's value; the page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// In chips mode, ticking three rows and invoking one chip's "Remove …" button leaves the other two chips.
/// </summary>
[Collection("SampleApp")]
public sealed class MultiSelectComboBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public MultiSelectComboBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Opening_ticking_two_of_three_and_closing_shows_their_names_as_the_summary()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "MultiSelectComboBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        Assert.Equal(ControlType.ComboBox, box.ControlType);
        Assert.Equal(string.Empty, box.Patterns.Value.Pattern.Value.Value);

        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;
        expandCollapse.Expand();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);

        FindRow(box, "Orders").Patterns.Toggle.Pattern.Toggle();
        FindRow(box, "Payments").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState.Value);
        Assert.Equal(ToggleState.Off, FindRow(box, "Shipping").Patterns.Toggle.Pattern.ToggleState.Value);

        expandCollapse.Collapse();
        Wait.UntilInputIsProcessed();

        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);
        Retry.WhileFalse(
            () => string.Equals(box.Patterns.Value.Pattern.Value.Value, "Orders, Payments", StringComparison.Ordinal),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal("Orders, Payments", box.Patterns.Value.Pattern.Value.Value);

        var lines = Lines(log.Name);
        Assert.Equal(2, lines.Length);
        Assert.EndsWith("-> [orders, payments]", lines[1], StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Chips_mode_removing_a_chip_through_its_button_leaves_the_other_chips()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "MultiSelectComboBox");

        var box = FindByAutomationId(mainWindow, "ChipsBox");
        var log = FindByAutomationId(mainWindow, "ChipsLog");

        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;
        expandCollapse.Expand();
        Wait.UntilInputIsProcessed();

        FindRow(box, "Orders").Patterns.Toggle.Pattern.Toggle();
        FindRow(box, "Payments").Patterns.Toggle.Pattern.Toggle();
        FindRow(box, "Shipping").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        expandCollapse.Collapse();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);
        Retry.WhileFalse(
            () => ChipNames(box).Count == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(["Orders", "Payments", "Shipping"], ChipNames(box));

        var removePayments = box.FindFirstDescendant(cf => cf
            .ByControlType(ControlType.Button)
            .And(cf.ByName("Remove Payments")));
        Assert.NotNull(removePayments);
        removePayments.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => ChipNames(box).Count == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(["Orders", "Shipping"], ChipNames(box));
        Assert.Equal("Orders, Shipping", box.Patterns.Value.Pattern.Value.Value);
        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);

        var lines = Lines(log.Name);
        Assert.Equal(4, lines.Length);
        Assert.EndsWith("[orders, payments, shipping] -> [orders, shipping]", lines[3], StringComparison.Ordinal);
    }

    private static List<string> ChipNames(AutomationElement box)
        => box
            .FindAllChildren(cf => cf.ByControlType(ControlType.Button))
            .Select(x => x.Name)
            .Where(x => x.StartsWith("Remove ", StringComparison.Ordinal))
            .Select(x => x["Remove ".Length..])
            .ToList();

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static AutomationElement FindRow(
        AutomationElement box,
        string name)
    {
        var row = Retry.WhileNull(
            () => box.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(row);
        return row;
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