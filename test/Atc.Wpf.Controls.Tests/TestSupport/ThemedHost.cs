namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Shows a control in an off-screen window that carries a theme and the control's own style dictionary.
/// No Application is created; theme brushes are found through the window's resources, so this suits
/// templates that use <c>DynamicResource</c> for theme brushes.
/// </summary>
internal static class ThemedHost
{
    private static readonly string[] ThemeDictionaries =
    [
        "Styles/Bootstrap.xaml",
        "Styles/Themes/Light.Blue.xaml",
    ];

    public static Window Show(
        FrameworkElement content,
        string dictionaryPath,
        params (Type TargetType, string StyleKey)[] implicitStyles)
    {
        // The theme dictionary uses the StaticResource markup type from Atc.Wpf, and pack://application
        // URIs need the Application type initialized (not an instance).
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var resources = new ResourceDictionary();
        foreach (var themePath in ThemeDictionaries)
        {
            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{themePath}", UriKind.Absolute),
            });
        }

        var styles = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Controls;component/{dictionaryPath}", UriKind.Absolute),
        };
        resources.MergedDictionaries.Add(styles);
        foreach (var (targetType, styleKey) in implicitStyles)
        {
            resources[targetType] = styles[styleKey];
        }

        var window = new Window
        {
            Resources = resources,
            Content = content,
            Width = 600,
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

    public static T Part<T>(
        Control control,
        string name)
        where T : class
        => Assert.IsType<T>(control.Template.FindName(name, control), exactMatch: false);
}