namespace Atc.Wpf.UiTests;

/// <summary>
/// Finds the "Binding Error" dialogs the sample app shows for data binding errors when it is launched with
/// <c>traceBindingErrors: true</c> (see <see cref="AutoScrollTestHelpers.LaunchSample"/>).
/// </summary>
internal static class BindingErrorWindows
{
    /// <summary>
    /// The environment variable the sample app reads (mirrors <c>App.TraceBindingErrorsVariable</c>).
    /// </summary>
    public const string TraceBindingErrorsVariable = "ATC_WPF_SAMPLE_TRACE_BINDING_ERRORS";

    private const string Title = "Binding Error";

    /// <summary>
    /// Asserts that the app shows no "Binding Error" dialog; any found are closed and their text is reported.
    /// </summary>
    /// <param name="application">The launched sample app.</param>
    /// <param name="automation">The automation that found the app's main window.</param>
    public static void AssertNone(
        Application application,
        UIA3Automation automation)
    {
        var messages = new List<string>();
        foreach (var window in Find(application, automation))
        {
            messages.Add(string.Join(
                " | ",
                window
                    .FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
                    .Select(x => x.Name)
                    .Where(x => !string.IsNullOrWhiteSpace(x))));
            Close(window);
        }

        var report = $"The sample app showed {messages.Count} '{Title}' window(s):{Environment.NewLine}" +
                     string.Join(Environment.NewLine, messages);
        Assert.True(messages.Count == 0, report);
    }

    private static List<Window> Find(
        Application application,
        UIA3Automation automation)
    {
        var windows = new List<Window>();
        foreach (var topLevel in application.GetAllTopLevelWindows(automation))
        {
            if (IsBindingErrorWindow(topLevel))
            {
                windows.Add(topLevel);
            }

            // A message box owned by the main window is also listed as its modal window.
            windows.AddRange(topLevel.ModalWindows.Where(IsBindingErrorWindow));
        }

        return windows
            .GroupBy(x => x.Properties.NativeWindowHandle.ValueOrDefault)
            .Select(x => x.First())
            .ToList();
    }

    private static bool IsBindingErrorWindow(Window window)
        => string.Equals(window.Title, Title, StringComparison.Ordinal);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort close; the failing assertion carries the outcome.")]
    private static void Close(Window window)
    {
        try
        {
            window.Close();
        }
        catch (Exception)
        {
            // Best effort.
        }
    }
}