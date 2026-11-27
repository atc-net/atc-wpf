namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The application base of a sample app. At start-up it merges the Atc style dictionaries the app has not
/// merged itself, sets the <see cref="Culture"/> and <see cref="Theme"/>, traces data binding errors (with a
/// debugger, or when <see cref="TraceBindingErrorsVariable"/> is <c>1</c>) and shows unhandled exceptions in the
/// <see cref="ExceptionDialog"/>.
/// </summary>
public class SampleApplication : Application
{
    private readonly SampleApplicationRules.ExceptionDialogGate exceptionDialogGate = new();

    /// <summary>
    /// The title of the dialog for an unhandled exception on the UI thread.
    /// </summary>
    public const string DispatcherUnhandledExceptionTitle = "Dispatcher Unhandled Exception";

    /// <summary>
    /// The title of the message box for an unhandled exception on another thread.
    /// </summary>
    public const string CurrentDomainUnhandledExceptionTitle = "CurrentDomain Unhandled Exception";

    /// <summary>
    /// Gets or sets the culture set at start-up (UI and formatting), e.g. <c>en-US</c>;
    /// <see langword="null"/> leaves the culture as it is.
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets the theme and accent set at start-up, e.g. <c>Dark.Blue</c>;
    /// <see langword="null"/> leaves the theme as it is.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Gets or sets the name of an environment variable that, set to <c>1</c>, traces data binding errors (shown
    /// in a "Binding Error" dialog) without a debugger, e.g. for UI tests. With a debugger they are always traced.
    /// </summary>
    public string? TraceBindingErrorsVariable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the app closes after showing an unhandled exception on the UI
    /// thread. The default, <see langword="false"/>, keeps the app running, so one broken sample does not end it.
    /// </summary>
    public bool ShutdownOnUnhandledException { get; set; }

    /// <summary>
    /// Raises the Startup event, after the start-up work of the sample app and <see cref="OnSampleStartup"/>.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        var missing = SampleApplicationRules.MissingStyleDictionaries(SampleApplicationRules.MergedSources(Resources));
        for (var i = 0; i < missing.Count; i++)
        {
            Resources.MergedDictionaries.Insert(i, new ResourceDictionary { Source = missing[i] });
        }

        if (!string.IsNullOrEmpty(Culture))
        {
            CultureManager.SetCultures(Culture);
        }

        if (!string.IsNullOrEmpty(Theme))
        {
            ThemeManagerHelper.SetThemeAndAccent(this, Theme);
        }

        if (SampleApplicationRules.ShouldTraceBindingErrors(
                Debugger.IsAttached,
                TraceBindingErrorsVariable,
                Environment.GetEnvironmentVariable))
        {
            if (!Debugger.IsAttached)
            {
                // Without a debugger WPF does not trace data binding until the trace sources are refreshed and given a level.
                PresentationTraceSources.Refresh();
                PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            }

            BindingErrorTraceListener.StartTrace();
        }

        OnSampleStartup(e);

        base.OnStartup(e);
    }

    /// <summary>
    /// Called at start-up, after the style dictionaries, culture and theme are set and before the Startup event
    /// and the <see cref="Application.StartupUri"/> window: the place for the app's own start-up work.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
    protected virtual void OnSampleStartup(StartupEventArgs e)
    {
    }

    private static void OnCurrentDomainUnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            _ = ExceptionReport.ShowMessageBox(exception, CurrentDomainUnhandledExceptionTitle);
        }
    }

    private void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        if (ExceptionReport.IsKnownHarmlessWpfBindingError(e.Exception))
        {
            return;
        }

        // An exception raised again while the dialog's message loop runs (e.g. in a page's layout) is not shown again.
        if (!exceptionDialogGate.TryEnter())
        {
            return;
        }

        try
        {
            ExceptionDialog.Show(e.Exception, DispatcherUnhandledExceptionTitle);
        }
        finally
        {
            exceptionDialogGate.Exit();
        }

        if (ShutdownOnUnhandledException)
        {
            Shutdown(-1);
        }
    }
}