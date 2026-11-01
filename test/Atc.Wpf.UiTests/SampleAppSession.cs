namespace Atc.Wpf.UiTests;

/// <summary>
/// One sample app, with binding error tracing on, shared by all the samples; started again if a sample crashed it.
/// </summary>
public sealed class SampleAppSession : IDisposable
{
    private Application? application;
    private UIA3Automation? automation;
    private Window? mainWindow;

    public bool HasExited
        => application?.HasExited ?? true;

    public Window EnsureRunning()
    {
        if (application is null || application.HasExited || mainWindow is null)
        {
            Dispose();
            application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow, traceBindingErrors: true);

            // Errors raised while the app starts belong to no sample: drop them.
            _ = TakeProblemWindows(SampleSmokeTests.ProblemWindowTitles);
        }

        return mainWindow;
    }

    /// <summary>
    /// Closes the app's error dialogs with one of the given titles and returns their texts.
    /// </summary>
    /// <param name="titles">The dialog titles that mark a problem.</param>
    /// <returns>One line per dialog: its title and texts.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort close; the failing assertion carries the outcome.")]
    public IReadOnlyList<string> TakeProblemWindows(
        IReadOnlyCollection<string> titles)
    {
        var messages = new List<string>();
        if (application is null || automation is null || application.HasExited)
        {
            return messages;
        }

        var windows = new List<Window>();
        foreach (var topLevel in application.GetAllTopLevelWindows(automation))
        {
            windows.Add(topLevel);
            windows.AddRange(topLevel.ModalWindows);
        }

        foreach (var window in windows
                     .Where(x => titles.Contains(x.Title, StringComparer.Ordinal))
                     .GroupBy(x => x.Properties.NativeWindowHandle.ValueOrDefault)
                     .Select(x => x.First()))
        {
            var texts = window
                .FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
            messages.Add($"[{window.Title}] {string.Join(" | ", texts)}");
            try
            {
                window.Close();
            }
            catch (Exception)
            {
                // Best effort.
            }
        }

        return messages;
    }

    public void Dispose()
    {
        if (application is not null && automation is not null)
        {
            AutoScrollTestHelpers.CleanupApplication(application, automation);
        }

        application = null;
        automation = null;
        mainWindow = null;
    }
}