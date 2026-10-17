namespace Atc.Wpf.Theming.Tests.TestSupport;

/// <summary>
/// Shows content in an off-screen window that carries the ATC text-input styles and a theme.
/// The window gets the resources itself: no Application is created, and Default.xaml / Controls.xaml
/// are left out because their NiceWindow style pulls in Generic.xaml, which registers the ATC themes
/// at the process-wide ThemeManager.
/// </summary>
internal static class ThemedWindow
{
    private static readonly string[] StyleDictionaries =
    [
        "Styles/BaseControls/ValidationError.xaml",
        "Styles/BaseControls/TextBlock.xaml",
        "Styles/BaseControls/TextBox.xaml",
        "Styles/BaseControls/ComboBox.xaml",
    ];

    public static ResourceDictionary CreateResources(
        string theme = "Light.Blue")
    {
        // The theme dictionaries use the StaticResource markup type from Atc.Wpf, and pack://application
        // URIs need the Application type initialized (not an instance).
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var resources = new ResourceDictionary();
        foreach (var path in StyleDictionaries)
        {
            resources.MergedDictionaries.Add(Load(path));
        }

        resources.MergedDictionaries.Add(Load($"Styles/Themes/{theme}.xaml"));
        return resources;
    }

    public static Window Show(
        ResourceDictionary resources,
        UIElement content)
    {
        var window = new Window
        {
            Resources = resources,
            Content = content,
            Width = 400,
            Height = 300,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };

        window.Show();
        Pump();
        return window;
    }

    /// <summary>Runs the dispatcher until the layout, data binding and render work queued so far is done.</summary>
    public static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static ResourceDictionary Load(string path)
        => new()
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{path}", UriKind.Absolute),
        };
}