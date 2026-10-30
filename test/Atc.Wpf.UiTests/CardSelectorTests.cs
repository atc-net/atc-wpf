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