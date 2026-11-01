namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the ChipGroup sample page, the group is a UI Automation list whose chips are check boxes; toggling two
/// of the five chips through the Toggle pattern turns them on and leaves the others off, and toggling one again turns
/// it off. The page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class ChipGroupTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public ChipGroupTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Toggling_two_chips_turns_them_on_and_logs_one_change_each()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "ChipGroup");

        var group = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        Assert.Equal(ControlType.List, group.ControlType);
        Assert.Equal(ToggleState.Off, FindChip(group, "Apple").Patterns.Toggle.Pattern.ToggleState.Value);

        FindChip(group, "Apple").Patterns.Toggle.Pattern.Toggle();
        FindChip(group, "Cherry").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.On, FindChip(group, "Apple").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindChip(group, "Cherry").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindChip(group, "Banana").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindChip(group, "Date").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.EndsWith("-> [apple, cherry]", Lines(log.Name)[1], StringComparison.Ordinal);

        FindChip(group, "Apple").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.Off, FindChip(group, "Apple").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindChip(group, "Cherry").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.EndsWith("[apple, cherry] -> [cherry]", Lines(log.Name)[2], StringComparison.Ordinal);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static AutomationElement FindChip(
        AutomationElement group,
        string name)
    {
        var chip = Retry.WhileNull(
            () => group.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(chip);
        return chip;
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