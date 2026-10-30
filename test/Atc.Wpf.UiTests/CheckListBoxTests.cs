namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the CheckListBox sample page, ticking two of the five rows through the UI Automation Toggle pattern
/// leaves the "Select all" check box indeterminate, and toggling "Select all" ticks every row in one change.
/// The page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class CheckListBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public CheckListBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Ticking_two_of_five_then_select_all_ticks_every_row_in_one_change()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CheckListBox");

        var list = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        var selectAll = FindByAutomationId(list, "SelectAll");
        var count = FindByAutomationId(list, "SelectionCount");
        Assert.Equal(ToggleState.Off, selectAll.Patterns.Toggle.Pattern.ToggleState.Value);

        FindRow(list, "Apple").Patterns.Toggle.Pattern.Toggle();
        FindRow(list, "Cherry").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => selectAll.Patterns.Toggle.Pattern.ToggleState.Value == ToggleState.Indeterminate,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.Indeterminate, selectAll.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindRow(list, "Apple").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindRow(list, "Banana").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(["2", "5"], Numbers(count.Name));
        Assert.Equal(2, Lines(log.Name).Length);

        selectAll.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.On, selectAll.Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(["5", "5"], Numbers(count.Name));
        foreach (var name in new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry" })
        {
            Assert.Equal(ToggleState.On, FindRow(list, name).Patterns.Toggle.Pattern.ToggleState.Value);
        }

        var lines = Lines(log.Name);
        Assert.Equal(3, lines.Length);
        Assert.EndsWith("-> [apple, cherry, banana, date, elderberry]", lines[2], StringComparison.Ordinal);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static string[] Numbers(string text)
        => text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x.All(char.IsDigit))
            .ToArray();

    private static AutomationElement FindRow(
        AutomationElement list,
        string name)
    {
        var row = Retry.WhileNull(
            () => list.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
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