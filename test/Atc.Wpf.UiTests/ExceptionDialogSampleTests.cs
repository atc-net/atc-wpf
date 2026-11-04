namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test of the Exception dialog sample: the themed dialog has the copy button and the report, and exceptions
/// nobody catches reach the sample app's own handlers, which show the report and then end the app.
/// </summary>
[Collection("SampleApp")]
public sealed class ExceptionDialogSampleTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public ExceptionDialogSampleTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Dialogs",
            leafTreeNodeName: "Exception dialog");
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Show_exception_dialog_has_the_details_and_the_copy_button()
    {
        Invoke(mainWindow, "ExceptionSampleShowDialog");
        var dialog = WaitForModal("Exception dialog sample");
        try
        {
            Assert.NotNull(FindByAutomationId(dialog, "DialogBoxCopyButton"));
            Assert.NotNull(FindByAutomationId(dialog, "DialogBoxAffirmativeButton"));
            Assert.NotNull(dialog.FindAllDescendants().FirstOrDefault(x => x.Patterns.ExpandCollapse.IsSupported));

            Invoke(dialog, "DialogBoxAffirmativeButton");
            WaitUntil(() => mainWindow.ModalWindows.Length == 0);
        }
        finally
        {
            CloseModals();
        }

        WaitUntil(() => ResultText().Contains("closed", StringComparison.Ordinal));
        Assert.Contains("closed", ResultText(), StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Throw_on_the_ui_thread_shows_the_exception_dialog_and_then_ends_the_app()
    {
        Invoke(mainWindow, "ExceptionSampleThrowUi");
        Confirm();

        var dialog = WaitForModal("Dispatcher Unhandled Exception");
        Assert.NotNull(FindByAutomationId(dialog, "DialogBoxCopyButton"));
        Invoke(dialog, "DialogBoxAffirmativeButton");

        WaitUntil(() => application.HasExited, TimeSpan.FromSeconds(15));
        Assert.True(application.HasExited, "The app should close after the dispatcher handler.");
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Throw_on_a_background_thread_shows_the_native_message_box_and_then_ends_the_app()
    {
        var savedClipboard = ClipboardText.Read();
        Invoke(mainWindow, "ExceptionSampleThrowBackground");
        Confirm();

        // The native message box is a top-level window of the app.
        var messageBox = Retry.WhileNull(
            () => application
                .GetAllTopLevelWindows(automation)
                .FirstOrDefault(x => x.Title == "CurrentDomain Unhandled Exception"),
            timeout: TimeSpan.FromSeconds(15),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(messageBox);

        var copied = ClipboardText.Read();
        Assert.Contains("System.TimeoutException", copied, StringComparison.Ordinal);
        ClipboardText.Restore(savedClipboard);

        var ok = Retry.WhileNull(
            () => messageBox.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button)),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(ok);
        ok.Patterns.Invoke.Pattern.Invoke();

        WaitUntil(() => application.HasExited, TimeSpan.FromSeconds(15));
        Assert.True(application.HasExited, "The process should end after an exception on another thread.");
    }

    private void Confirm()
    {
        var question = WaitForModal("This closes the sample app");
        Invoke(question, "DialogBoxAffirmativeButton");
    }

    private Window WaitForModal(string title)
    {
        var dialog = Retry.WhileNull(
            () => mainWindow.ModalWindows.FirstOrDefault(x => x.Title == title),
            timeout: TimeSpan.FromSeconds(15),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(dialog);
        return dialog;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup.")]
    private void CloseModals()
    {
        try
        {
            foreach (var modalWindow in mainWindow.ModalWindows)
            {
                modalWindow.Close();
            }
        }
        catch (Exception)
        {
            // Best effort.
        }
    }

    private string ResultText()
        => FindByAutomationId(mainWindow, "ExceptionSampleResult").Name ?? string.Empty;

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

    /// <summary>
    /// Reads and restores the clipboard of whoever runs the tests (the clipboard needs an STA thread).
    /// </summary>
    private static class ClipboardText
    {
        public static string Read()
            => Run(() => System.Windows.Clipboard.ContainsText() ? System.Windows.Clipboard.GetText() : string.Empty) ?? string.Empty;

        public static void Restore(string text)
            => Run(
                () =>
                {
                    if (text.Length == 0)
                    {
                        System.Windows.Clipboard.Clear();
                    }
                    else
                    {
                        System.Windows.Clipboard.SetText(text);
                    }

                    return string.Empty;
                });

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The clipboard can be busy.")]
        private static string? Run(Func<string> action)
        {
            string? result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = action();
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }
    }
}