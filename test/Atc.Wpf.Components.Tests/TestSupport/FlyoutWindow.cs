namespace Atc.Wpf.Components.Tests.TestSupport;

/// <summary>
/// Shows a <see cref="FlyoutHost"/> over some page content in an off-screen window that carries the theme,
/// the Bootstrap brushes and the flyout styles. No Application is created; theme brushes are found through the
/// window's resources.
/// </summary>
internal sealed class FlyoutWindow : IDisposable
{
    private FlyoutWindow(
        Window window,
        FlyoutHost host,
        Button pageButton)
    {
        Window = window;
        Host = host;
        PageButton = pageButton;
    }

    public Window Window { get; }

    public FlyoutHost Host { get; }

    /// <summary>
    /// Gets a focusable button on the page under the host.
    /// </summary>
    public Button PageButton { get; }

    public static FlyoutWindow Show(
        double width = 800,
        double height = 600,
        FlowDirection flowDirection = FlowDirection.LeftToRight)
    {
        // The theme dictionary uses the StaticResource markup type from Atc.Wpf, and pack://application
        // URIs need the Application type initialized (not an instance).
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var resources = new ResourceDictionary();
        foreach (var source in new[]
                 {
                     "pack://application:,,,/Atc.Wpf.Theming;component/Styles/Bootstrap.xaml",
                     "pack://application:,,,/Atc.Wpf.Theming;component/Styles/Themes/Light.Blue.xaml",
                 })
        {
            resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(source, UriKind.Absolute) });
        }

        var flyoutStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Components;component/Flyouts/Flyout.xaml", UriKind.Absolute),
        };
        var hostStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Components;component/Flyouts/FlyoutHost.xaml", UriKind.Absolute),
        };
        resources.MergedDictionaries.Add(flyoutStyles);
        resources.MergedDictionaries.Add(hostStyles);
        resources[typeof(Flyout)] = flyoutStyles["AtcApps.Styles.Flyout"];
        resources[typeof(FlyoutHost)] = hostStyles["AtcApps.Styles.FlyoutHost"];

        var pageButton = new Button
        {
            Content = "Page",
            Width = 120,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        var host = new FlyoutHost();
        var root = new Grid();
        root.Children.Add(pageButton);
        root.Children.Add(host);

        var window = new Window
        {
            Resources = resources,
            Content = root,
            Width = width,
            Height = height,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
            FlowDirection = flowDirection,
        };

        window.Show();
        Pump();
        return new FlyoutWindow(window, host, pageButton);
    }

    /// <summary>Runs the dispatcher until the layout, data binding and render work queued so far is done.</summary>
    public static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    // Runs the dispatcher (animations included) until the condition holds or the time-out guard ends.
    public static bool WaitUntil(Func<bool> condition)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(10) };
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        timer.Tick += (_, _) =>
        {
            if (condition() || DateTime.UtcNow > deadline)
            {
                timer.Stop();
                frame.Continue = false;
            }
        };

        timer.Start();
        Dispatcher.PushFrame(frame);
        return condition();
    }

    public void Dispose()
        => Window.Close();
}