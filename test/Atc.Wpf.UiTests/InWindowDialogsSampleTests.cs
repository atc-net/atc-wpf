namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test of the In-window dialogs sample: the dialogs open inside the main window (no new top-level or modal
/// window), the view model gets the answer, and Escape cancels.
/// </summary>
[Collection("SampleApp")]
public sealed class InWindowDialogsSampleTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public InWindowDialogsSampleTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Dialogs",
            leafTreeNodeName: "In-window dialogs");
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Confirmation_OpensInsideTheMainWindow_AndYesAnswersTrue()
    {
        var windowsBefore = application.GetAllTopLevelWindows(automation).Length;

        Invoke(mainWindow, "InWindowShowConfirmation");
        var yes = FindByAutomationId(mainWindow, "DialogBoxAffirmativeButton");

        Assert.Empty(mainWindow.ModalWindows);
        Assert.Equal(windowsBefore, application.GetAllTopLevelWindows(automation).Length);

        // Wait until the open animation is over (the dialog then has the focus) before answering.
        WaitUntil(() => yes.Properties.HasKeyboardFocus.ValueOrDefault);
        yes.Patterns.Invoke.Pattern.Invoke();

        WaitUntil(() => ResultText() == "Confirmation: True");
        Assert.Equal("Confirmation: True", ResultText());
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Confirmation_WithNoAsTheDefault_EnterAnswersFalse()
    {
        Invoke(mainWindow, "InWindowShowConfirmationNoIsDefault");
        var no = FindByAutomationId(mainWindow, "DialogBoxNegativeButton");
        WaitUntil(() => no.Properties.HasKeyboardFocus.ValueOrDefault);

        Keyboard.Type(VirtualKeyShort.ENTER);

        WaitUntil(() => ResultText() == "Confirmation (No is default): False");
        Assert.Equal("Confirmation (No is default): False", ResultText());
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Escape_CancelsTheDialog()
    {
        Invoke(mainWindow, "InWindowShowOkCancel");
        var ok = FindByAutomationId(mainWindow, "DialogBoxAffirmativeButton");
        WaitUntil(() => ok.Properties.HasKeyboardFocus.ValueOrDefault);

        Keyboard.Type(VirtualKeyShort.ESCAPE);

        WaitUntil(() => ResultText() == "OK / Cancel: False");
        Assert.Equal("OK / Cancel: False", ResultText());
        WaitUntil(() => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DialogBoxAffirmativeButton")) is null);
        Assert.Null(mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DialogBoxAffirmativeButton")));
    }

    private string ResultText()
        => FindByAutomationId(mainWindow, "InWindowDialogResult").Name ?? string.Empty;

    private static void Invoke(
        AutomationElement parent,
        string automationId)
        => FindByAutomationId(parent, automationId).Patterns.Invoke.Pattern.Invoke();

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

    private static void WaitUntil(
        Func<bool> condition,
        TimeSpan? timeout = null)
        => Retry.WhileFalse(
            condition,
            timeout: timeout ?? TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}