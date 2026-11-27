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
                services.AddSingleton<MainWindow>();
            })
            .Build();
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