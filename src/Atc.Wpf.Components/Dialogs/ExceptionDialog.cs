namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// Shows an unhandled (or any) exception in the themed error dialog, from any thread: the exception messages, with
/// the full report (stack traces, app, OS and .NET versions) in the collapsed details area. Ctrl+C copies everything,
/// for a bug report or an AI agent. The report is also saved to a file first.
/// </summary>
/// <remarks>
/// When the themed dialog cannot be shown (no application, the dispatcher is shutting down, or the dialog itself
/// fails), it falls back to <see cref="ExceptionReport.ShowMessageBox"/>, the native Windows message box. Deciding
/// whether the app shuts down, goes on or restarts stays with the caller.
/// </remarks>
public static class ExceptionDialog
{
    /// <summary>
    /// Gets or sets the action used when the themed dialog cannot be shown (exception, title); replaced in tests.
    /// </summary>
    internal static Action<Exception, string> Fallback { get; set; }
        = (exception, title) => ExceptionReport.ShowMessageBox(exception, title);

    /// <summary>
    /// Shows the exception and waits until the dialog is closed.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="title">The title, such as "Dispatcher Unhandled Exception".</param>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Any failure of the themed dialog falls back to the native message box.")]
    public static void Show(
        Exception exception,
        string title)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var application = Application.Current;
        var dispatcher = application?.Dispatcher;
        if (application is null ||
            dispatcher is null ||
            dispatcher.HasShutdownStarted ||
            dispatcher.HasShutdownFinished)
        {
            Fallback(exception, title);
            return;
        }

        if (!dispatcher.CheckAccess())
        {
            try
            {
                dispatcher.Invoke(() => Show(exception, title));
            }
            catch (Exception)
            {
                Fallback(exception, title);
            }

            return;
        }

        try
        {
            var report = ExceptionReport.CreateText(exception, title);
            var file = ExceptionReport.TryWriteToFile(report);

            var owner = application.Windows
                .OfType<Window>()
                .FirstOrDefault(x => x.IsActive) ?? application.MainWindow;

            var settings = new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Error)
            {
                TitleBarText = title,
                ShowCopyButton = true,
                DetailsText = file is null
                    ? report
                    : report + Environment.NewLine + Environment.NewLine + file,
            };

            new InfoDialogBox(owner!, settings, CreateMessage(exception)).ShowDialog();
        }
        catch (Exception)
        {
            Fallback(exception, title);
        }
    }

    /// <summary>
    /// Creates the message shown above the details: the messages of the exception and its inner exceptions.
    /// </summary>
    internal static string CreateMessage(Exception exception)
    {
        var lines = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!lines.Contains(current.Message, StringComparer.Ordinal))
            {
                lines.Add(current.Message);
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}