// ReSharper disable once UnusedParameter.Local
namespace Atc.Wpf.Sample;

public partial class App
{
    private readonly IHost host;
    private IConfiguration? configuration;

    public App()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configuration = configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(AtcFileNameConstants.AppSettings, optional: false, reloadOnChange: true)
                    .AddJsonFile(AtcFileNameConstants.AppSettingsCustom, optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            })
            .ConfigureServices((_, services) =>
            {
                services
                    .AddOptions<BasicApplicationOptions>()
                    .Bind(configuration!.GetRequiredSection(BasicApplicationOptions.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    /// <summary>
    /// Raises the Startup event.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
    protected override void OnStartup(
        StartupEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Hook on error before app really starts
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        Current.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
        base.OnStartup(e);

        if (Debugger.IsAttached)
        {
            BindingErrorTraceListener.StartTrace();
        }
    }

    /// <summary>
    /// Currents the domain unhandled exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
    private static void CurrentDomainUnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        if (!(e is { ExceptionObject: Exception ex }))
        {
            return;
        }

        var exceptionMessage = ex.GetMessage(includeInnerMessage: true);
        _ = MessageBox.Show(
            exceptionMessage,
            "CurrentDomain Unhandled Exception",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <summary>
    /// Applications the on dispatcher unhandled exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
    private void ApplicationOnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        var exceptionMessage = e.Exception.GetMessage(includeInnerMessage: true);
        if (exceptionMessage.Contains(
                "BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'ComboBoxItem'",
                StringComparison.Ordinal))
        {
            e.Handled = true;
            return;
        }

        _ = MessageBox.Show(
            exceptionMessage,
            "Dispatcher Unhandled Exception",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
        Shutdown(-1);
    }

    private async void ApplicationStartup(
        object sender,
        StartupEventArgs e)
    {
        await host
            .StartAsync()
            .ConfigureAwait(false);

        var applicationOptions = new BasicApplicationOptions();
        configuration!
            .GetRequiredSection(BasicApplicationOptions.SectionName)
            .Bind(applicationOptions);

        CultureManager.SetCultures(applicationOptions.Language);

        ThemeManagerHelper.SetThemeAndAccent(Current, applicationOptions.Theme);

        ColorHelper.InitializeWithSupportedLanguages();
        SolidColorBrushHelper.InitializeWithSupportedLanguages();

        var mainWindow = host
            .Services
            .GetService<MainWindow>()!;

        mainWindow.Show();
    }

    private async void ApplicationExit(
        object sender,
        ExitEventArgs e)
    {
        await host
            .StopAsync()
            .ConfigureAwait(false);

        host.Dispose();
    }
}