namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the CardSelector sample page, the interactive selector is a UI Automation list whose cards are check
/// boxes named from the card template; toggling two of the five cards through the Toggle pattern turns them on and
/// leaves the others off, and toggling one again turns it off. The page writes each <c>SelectedKeysChanged</c> event
/// to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class CardSelectorTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public CardSelectorTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Toggling_two_cards_turns_them_on_and_logs_one_change_each()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CardSelector");

        var selector = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        Assert.Equal(ControlType.List, selector.ControlType);
        Assert.Equal(ToggleState.Off, FindCard(selector, "Reports").Patterns.Toggle.Pattern.ToggleState.Value);

        FindCard(selector, "Reports").Patterns.Toggle.Pattern.Toggle();
        FindCard(selector, "Maps").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.On, FindCard(selector, "Reports").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindCard(selector, "Maps").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindCard(selector, "Alarms").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.Off, FindCard(selector, "Export").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.EndsWith("-> [reports, maps]", Lines(log.Name)[1], StringComparison.Ordinal);

        FindCard(selector, "Reports").Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ToggleState.Off, FindCard(selector, "Reports").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.Equal(ToggleState.On, FindCard(selector, "Maps").Patterns.Toggle.Pattern.ToggleState.Value);
        Assert.EndsWith("[reports, maps] -> [maps]", Lines(log.Name)[2], StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Ctrl_A_respects_the_limit_and_single_select_cards_are_radio_buttons()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "CardSelector");

        // Plans (SelectionMode="Single" in the page XAML): radio buttons, "Team" preselected through SelectedKeys.
        var plans = FindByAutomationId(mainWindow, "PlanPicker");
        var team = Retry.WhileNull(
            () => plans.FindFirstChild(cf => cf.ByControlType(ControlType.RadioButton).And(cf.ByName("Team"))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(team);
        Assert.True(team.Patterns.SelectionItem.Pattern.IsSelected.Value);

        // Devices: three online cards, a limit of two, so Ctrl+A has no room and changes nothing; offline cards stay off.
        var devices = FindByAutomationId(mainWindow, "DevicePicker");
        Assert.False(FindCard(devices, "Yard camera").IsEnabled);
        FindCard(devices, "Entrance camera").Focus();
        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
        Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(500));

        foreach (var name in new[] { "Entrance camera", "Yard camera", "Hall sensor", "Roof sensor", "Gateway" })
        {
            Assert.Equal(ToggleState.Off, FindCard(devices, name).Patterns.Toggle.Pattern.ToggleState.Value);
        }

        // Without a limit, Ctrl+A selects every card in one change.
        var selector = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        FindCard(selector, "Alarms").Focus();
        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 1,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.EndsWith("[] -> [reports, alarms, maps, audit, export]", Lines(log.Name)[0], StringComparison.Ordinal);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static AutomationElement FindCard(
        AutomationElement selector,
        string name)
    {
        var card = Retry.WhileNull(
            () => selector.FindFirstChild(cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(card);
        return card;
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