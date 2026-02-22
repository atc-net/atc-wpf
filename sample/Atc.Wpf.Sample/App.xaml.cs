// ReSharper disable once UnusedParameter.Local
namespace Atc.Wpf.Sample;

public partial class App
{
    private readonly IHost host;
    private IConfiguration? configuration;

    public App()
    {
        EnsureAppSettingsInCommonDataDirectory();

        host = Host
            .CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configuration = configurationBuilder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        Path.Combine(
                            AppConstants.DataDirectory.FullName,
                            AtcFileNameConstants.AppSettings),
                        optional: false,
                        reloadOnChange: true)
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

                services.AddSingleton<IToastNotificationService, ToastNotificationService>();
                services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    /// <summary>
    /// Raises the Startup event.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
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

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
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

        ThemeManagerHelper.SetThemeAndAccent(
            Current,
            applicationOptions.Theme);

        ColorHelper.InitializeWithSupportedLanguages();
        SolidColorBrushHelper.InitializeWithSupportedLanguages();

        var mainWindow = host
            .Services
            .GetService<MainWindow>()!;

        var isScreenshotMode = e.Args.Contains(
            "--generate-screenshots",
            StringComparer.OrdinalIgnoreCase);

        if (isScreenshotMode)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }

        mainWindow.Show();

        if (isScreenshotMode)
        {
            mainWindow.Loaded += (_, _) =>
            {
                _ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, () =>
                {
                    RunScreenshotGeneration(mainWindow);
                });
            };
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Top-level handler for screenshot generation.")]
    private static void RunScreenshotGeneration(MainWindow mainWindow)
    {
        try
        {
            var solutionRoot = FindSolutionRoot();
            System.Console.WriteLine($"[Screenshots] Solution root: {solutionRoot}");

            var generator = new ScreenshotGenerator(solutionRoot);
            generator.GenerateAll(mainWindow.SampleTreeViews);

            Application.Current.Shutdown(0);
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"[Screenshots] Fatal error: {ex.Message}");
            System.Console.Error.WriteLine(ex.StackTrace);
            Application.Current.Shutdown(1);
        }
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory is not null)
        {
            if (directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        // Fallback: navigate up from bin/Debug/net10.0-windows
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        return Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", ".."));
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

    private static void EnsureAppSettingsInCommonDataDirectory()
    {
        var dataDirectoryFile = AppConstants
            .DataDirectory
            .CombineFileInfo(AtcFileNameConstants.AppSettings);
        if (dataDirectoryFile.Exists)
        {
            return;
        }

        if (!Directory.Exists(AppConstants.DataDirectory.FullName))
        {
            Directory.CreateDirectory(AppConstants.DataDirectory.FullName);
        }

        var deployedFile = new FileInfo(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AtcFileNameConstants.AppSettings));

        File.Copy(
            deployedFile.FullName,
            dataDirectoryFile.FullName,
            overwrite: true);
    }
}