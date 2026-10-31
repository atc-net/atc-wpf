namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the Segmented sample page, the weekday picker is a multi-select segmented control whose segments are
/// toggle buttons; toggling Mon and Wed through the Toggle pattern turns them on and leaves the other days off, and
/// toggling Mon again turns it off. The page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class SegmentedMultipleTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public SegmentedMultipleTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Toggling_Mon_and_Wed_turns_them_on_and_logs_one_change_each()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "Segmented");

        var weekdays = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        Assert.Equal(ControlType.List, weekdays.ControlType);
        Assert.Equal(ControlType.Button, FindDay(weekdays, "Mon").ControlType);
        Assert.Equal(ToggleState.Off, ToggleStateOf(weekdays, "Mon"));

        FindDay(weekdays, "Mon").Patterns.Toggle.Pattern.Toggle();
        FindDay(weekdays, "Wed").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.On, ToggleStateOf(weekdays, "Mon"));
        Assert.Equal(ToggleState.On, ToggleStateOf(weekdays, "Wed"));
        Assert.Equal(ToggleState.Off, ToggleStateOf(weekdays, "Tue"));
        Assert.Equal(ToggleState.Off, ToggleStateOf(weekdays, "Thu"));
        Assert.Equal(ToggleState.Off, ToggleStateOf(weekdays, "Sun"));
        Assert.EndsWith("-> [mon, wed]", Lines(log.Name)[1], StringComparison.Ordinal);

        FindDay(weekdays, "Mon").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.Off, ToggleStateOf(weekdays, "Mon"));
        Assert.Equal(ToggleState.On, ToggleStateOf(weekdays, "Wed"));
        Assert.EndsWith("[mon, wed] -> [wed]", Lines(log.Name)[2], StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void In_a_single_mode_arrows_move_the_focus_with_the_selection_and_Tab_leaves_the_control()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "Segmented");

        var switcher = FindByAutomationId(mainWindow, "ViewSwitcher");
        var day = FindDay(switcher, "Day");
        day.Click();
        day.Focus();
        Wait.UntilInputIsProcessed();
        Retry.WhileFalse(
            () => day.Patterns.SelectionItem.Pattern.IsSelected.Value,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));

        Keyboard.Type(VirtualKeyShort.RIGHT);
        Keyboard.Type(VirtualKeyShort.RIGHT);
        Keyboard.Type(VirtualKeyShort.SPACE);
        Wait.UntilInputIsProcessed();

        var month = FindDay(switcher, "Month");
        Retry.WhileFalse(
            () => month.Properties.HasKeyboardFocus.Value,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.True(month.Patterns.SelectionItem.Pattern.IsSelected.Value);
        Assert.True(month.Properties.HasKeyboardFocus.Value);

        Keyboard.Type(VirtualKeyShort.TAB);
        Wait.UntilInputIsProcessed();

        var names = new[] { "Day", "Week", "Month", "Year" };
        Assert.DoesNotContain(names, name => FindDay(switcher, name).Properties.HasKeyboardFocus.Value);
        Assert.True(month.Patterns.SelectionItem.Pattern.IsSelected.Value);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static ToggleState ToggleStateOf(
        AutomationElement weekdays,
        string name)
        => FindDay(weekdays, name).Patterns.Toggle.Pattern.ToggleState.Value;

    private static AutomationElement FindDay(
        AutomationElement weekdays,
        string name)
    {
        var day = Retry.WhileNull(
            () => weekdays.FindFirstDescendant(cf => cf.ByName(name).And(cf.ByClassName("SegmentedItem"))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(day);
        return day;
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