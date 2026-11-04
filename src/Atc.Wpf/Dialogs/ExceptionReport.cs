namespace Atc.Wpf.Dialogs;

/// <summary>
/// Builds, saves and shows a report of an unhandled exception with as few dependencies as possible (.NET and WPF only),
/// so it still works when the rest of the UI is broken. Nothing here throws.
/// </summary>
/// <remarks>
/// Use <see cref="ShowMessageBox"/> as the last resort in <see cref="AppDomain.UnhandledException"/> and similar
/// handlers. Atc.Wpf.Components has <c>ExceptionDialog</c>, which shows the same report in the themed dialog and falls
/// back to this class.
/// </remarks>
public static class ExceptionReport
{
    private const string HarmlessComboBoxItemBindingError =
        "BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'ComboBoxItem'";

    private static readonly TimeSpan ClipboardTimeout = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets the action that shows the native message box (message, title); replaced in tests.
    /// </summary>
    internal static Action<string, string> MessageBoxShower { get; set; }
        = (message, title) => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>
    /// Creates the report text: the title, the time, the app, OS and .NET versions and the full exception with its
    /// inner exceptions and stack traces (<see cref="Exception.ToString"/>).
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="title">A title, such as "Dispatcher Unhandled Exception".</param>
    /// <returns>The report text.</returns>
    public static string CreateText(
        Exception exception,
        string title)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var entryAssembly = System.Reflection.Assembly.GetEntryAssembly()?.GetName();
        var lines = new List<string>
        {
            title,
            $"Time: {DateTimeOffset.Now:O}",
            $"App: {entryAssembly?.Name ?? AppDomain.CurrentDomain.FriendlyName} {entryAssembly?.Version}",
            $"OS: {Environment.OSVersion}, .NET {Environment.Version}, {(Environment.Is64BitProcess ? "64" : "32")}-bit",
            string.Empty,
            exception.ToString(),
        };

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Gets the folder crash reports are written to: <c>%LOCALAPPDATA%\&lt;app&gt;\Crashes</c>.
    /// </summary>
    /// <returns>The folder path.</returns>
    public static string GetDefaultFolder()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName,
            "Crashes");

    /// <summary>
    /// Writes the report to a new <c>crash-&lt;time&gt;.txt</c> file.
    /// </summary>
    /// <param name="text">The report text.</param>
    /// <param name="folder">The folder; <see cref="GetDefaultFolder"/> when <see langword="null"/>.</param>
    /// <returns>The file path, or <see langword="null"/> when the file could not be written.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A crash report must never throw; null tells the caller it was not saved.")]
    public static string? TryWriteToFile(
        string text,
        string? folder = null)
    {
        try
        {
            var directory = folder ?? GetDefaultFolder();
            Directory.CreateDirectory(directory);
            var file = Path.Combine(directory, $"crash-{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt");
            File.WriteAllText(file, text);
            return file;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Puts the report on the clipboard, from any thread (the clipboard is used from its own STA thread, for at most
    /// two seconds).
    /// </summary>
    /// <param name="text">The report text.</param>
    /// <returns><see langword="true"/> when the text was copied.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The clipboard can be held by another process; copying is best effort.")]
    public static bool TryCopyToClipboard(string text)
    {
        var copied = false;
        try
        {
            var thread = new Thread(() =>
            {
                try
                {
                    System.Windows.Clipboard.SetDataObject(text, copy: true);
                    copied = true;
                }
                catch (Exception)
                {
                    // Clipboard busy or unavailable.
                }
            })
            {
                IsBackground = true,
            };

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(ClipboardTimeout);
        }
        catch (Exception)
        {
            return false;
        }

        return copied;
    }

    /// <summary>
    /// Saves the report, copies it to the clipboard and shows the native Windows message box with the exception
    /// message and where the details are. Ctrl+C in the message box copies its text as well.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="title">The title, such as "CurrentDomain Unhandled Exception".</param>
    /// <returns>The path of the saved report, or <see langword="null"/> when it could not be saved.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The last resort must never throw.")]
    public static string? ShowMessageBox(
        Exception exception,
        string title)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var text = CreateText(exception, title);
        var file = TryWriteToFile(text);
        var copied = TryCopyToClipboard(text);

        try
        {
            MessageBoxShower(CreateMessageBoxText(exception, file, copied), title);
        }
        catch (Exception)
        {
            // Nothing more can be shown; the file has the report.
        }

        return file;
    }

    /// <summary>
    /// Gets a value indicating whether the exception is a known, harmless WPF binding error that apps ignore (a
    /// <c>ComboBoxItem</c> that loses its <c>HorizontalContentAlignment</c> binding while its items change).
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns><see langword="true"/> for the harmless binding error.</returns>
    public static bool IsKnownHarmlessWpfBindingError(Exception? exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current.Message.Contains(HarmlessComboBoxItemBindingError, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Creates the short text of the message box: the messages of the exception and its inner exceptions, then where
    /// the full report is.
    /// </summary>
    internal static string CreateMessageBoxText(
        Exception exception,
        string? file,
        bool copied)
    {
        var lines = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            lines.Add(current.Message);
        }

        lines.Add(string.Empty);
        if (copied)
        {
            lines.Add("The full details (with the stack trace) were copied to the clipboard.");
        }
        else
        {
            lines.Add("Press Ctrl+C to copy this message.");
        }

        if (file is not null)
        {
            lines.Add($"Saved to: {file}");
        }

        return string.Join(Environment.NewLine, lines);
    }
}