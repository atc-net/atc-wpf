namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the sample page of each number box with lost-focus events, typing a new value raises nothing
/// while the input box keeps focus, leaving it raises the event once with the edited value, and leaving the
/// next input box without a change raises nothing. The page writes each event to its <c>LostFocusLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class NumberBoxLostFocusEventTests : IDisposable
{
    private const string TypedValue = "37";

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public NumberBoxLostFocusEventTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Theory]
    [InlineData("TabWpfControls", "DecimalXyBox", "ValueXLostFocus")]
    [InlineData("TabWpfControls", "IntegerXyBox", "ValueXLostFocus")]
    [InlineData("TabWpfControls", "PixelSizeBox", "ValueWidthLostFocus")]
    [InlineData("TabWpfForms", "LabelDecimalXyBox", "ValueXLostFocus")]
    [InlineData("TabWpfForms", "LabelIntegerBox", "ValueLostFocus")]
    [InlineData("TabWpfForms", "LabelIntegerXyBox", "ValueXLostFocus")]
    [InlineData("TabWpfForms", "LabelPixelSizeBox", "ValueWidthLostFocus")]
    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "FlaUI E2E tests must yield real wall time for the WPF UI thread to render between actions.")]
    public void Editing_the_first_input_box_raises_its_lost_focus_event_once_when_focus_leaves(
        string tabAutomationId,
        string sampleName,
        string expectedEventName)
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId,
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: sampleName);

        var interactiveBox = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "LostFocusLog");
        var firstInput = Retry.WhileNull(
            () => interactiveBox.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(firstInput);
        Assert.Equal(string.Empty, log.Name);

        firstInput.Click();
        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
        Keyboard.Type(TypedValue);
        Wait.UntilInputIsProcessed();
        Thread.Sleep(500);

        Assert.Equal(string.Empty, log.Name);

        Keyboard.Type(VirtualKeyShort.TAB);
        Retry.WhileTrue(
            () => string.IsNullOrEmpty(log.Name),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));

        AssertSingleEvent(log.Name, expectedEventName);

        Keyboard.Type(VirtualKeyShort.TAB);
        Wait.UntilInputIsProcessed();
        Thread.Sleep(500);

        AssertSingleEvent(log.Name, expectedEventName);
    }

    private static void AssertSingleEvent(
        string logText,
        string expectedEventName)
    {
        var line = Assert.Single(logText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
        Assert.StartsWith(expectedEventName + ": ", line, StringComparison.Ordinal);

        var newValue = line[(line.LastIndexOf("-> ", StringComparison.Ordinal) + 3)..];
        Assert.Equal(
            decimal.Parse(TypedValue, CultureInfo.InvariantCulture),
            decimal.Parse(newValue, CultureInfo.InvariantCulture));
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