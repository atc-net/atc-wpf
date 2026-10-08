namespace Atc.Wpf.Theming.Tests.Controls.Windows;

public sealed class NiceWindowBackdropTests : IDisposable
{
    private static readonly SolidColorBrush ThemeBackground = Brushes.White;
    private static readonly SolidColorBrush WindowTitle = Brushes.Blue;
    private static readonly SolidColorBrush ThemeForeground = Brushes.Black;
    private static readonly SolidColorBrush IdealForeground = Brushes.Yellow;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void BackdropType_Default_IsNone()
    {
        var sut = CreateWindow();

        Assert.Equal(WindowBackdropType.None, sut.BackdropType);
    }

    [StaFact]
    public void BackdropType_Set_IsPassedToTheWindowBackdropManager()
    {
        var sut = CreateWindow();

        sut.BackdropType = WindowBackdropType.Mica;

        Assert.Equal(WindowBackdropType.Mica, WindowBackdropManager.GetBackdropType(sut));
    }

    [StaFact]
    public void IsBackdropActive_NoBackdrop_IsFalseAndTheThemeBrushesAreUsed()
    {
        var sut = CreateWindow();

        Assert.False(sut.IsBackdropActive);
        Assert.Same(ThemeBackground, sut.Background);
        Assert.Same(WindowTitle, sut.WindowTitleBrush);
        Assert.Same(IdealForeground, sut.TitleForeground);
    }

    // Windows 10, AllowsTransparency or a refused DWM call: ControlzEx leaves CurrentBackdropType at None.
    [StaFact]
    public void IsBackdropActive_BackdropRequestedButNotApplied_StaysFalseAndKeepsTheThemeBrushes()
    {
        var sut = CreateWindow();

        sut.BackdropType = WindowBackdropType.Mica;

        Assert.False(sut.IsBackdropActive);
        Assert.Same(ThemeBackground, sut.Background);
        Assert.Same(WindowTitle, sut.WindowTitleBrush);
    }

    [StaFact]
    public void IsBackdropActive_BackdropApplied_IsTrueAndTheWindowSurfacesAreTransparent()
    {
        SkipInHighContrast();
        var sut = CreateWindow();
        sut.BackdropType = WindowBackdropType.Mica;

        ReportAppliedBackdrop(sut, WindowBackdropType.Mica);

        Assert.True(sut.IsBackdropActive);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(sut.Background).Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(sut.WindowTitleBrush).Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(sut.NonActiveWindowTitleBrush).Color);
        Assert.Same(ThemeForeground, sut.TitleForeground);
    }

    // The window buttons are styled for the accent title bar; on a backdrop they use the theme foreground.
    [StaFact]
    public void IsBackdropActive_BackdropApplied_WindowButtonsUseTheThemeForeground()
    {
        SkipInHighContrast();
        var sut = CreateWindow();
        sut.BackdropType = WindowBackdropType.Mica;

        ReportAppliedBackdrop(sut, WindowBackdropType.Mica);

        Assert.Same(ThemeForeground, sut.OverrideDefaultWindowCommandsBrush);
    }

    [StaFact]
    public void IsBackdropActive_BackdropRemoved_RestoresTheThemeBrushes()
    {
        SkipInHighContrast();
        var sut = CreateWindow();
        sut.BackdropType = WindowBackdropType.Mica;
        ReportAppliedBackdrop(sut, WindowBackdropType.Mica);

        ReportAppliedBackdrop(sut, WindowBackdropType.None);

        Assert.False(sut.IsBackdropActive);
        Assert.Same(ThemeBackground, sut.Background);
        Assert.Same(WindowTitle, sut.WindowTitleBrush);
        Assert.Same(IdealForeground, sut.TitleForeground);
    }

    // .NET's Application.ThemeMode gives every window without a Style the Fluent Window style
    // (SetResourceReference(StyleProperty, typeof(Window))) when the window is created; its template replaces NiceWindow's.
    [StaFact]
    public void FluentWindowStyleFromThemeMode_AssignedBeforeTheWindowIsCreated_IsRemoved()
    {
        var sut = CreateWindow();
        sut.Resources[typeof(Window)] = new Style(typeof(Window));
        sut.SetResourceReference(FrameworkElement.StyleProperty, typeof(Window));

        CreateHandle(sut);

        Assert.Equal(DependencyProperty.UnsetValue, sut.ReadLocalValue(FrameworkElement.StyleProperty));
    }

    // WPF assigns it again when the application's ThemeMode changes while the window is open.
    [StaFact]
    public void FluentWindowStyleFromThemeMode_AssignedAfterTheWindowIsCreated_IsRemoved()
    {
        var sut = CreateWindow();
        sut.Resources[typeof(Window)] = new Style(typeof(Window));
        CreateHandle(sut);

        sut.SetResourceReference(FrameworkElement.StyleProperty, typeof(Window));

        Assert.Equal(DependencyProperty.UnsetValue, sut.ReadLocalValue(FrameworkElement.StyleProperty));
    }

    [StaFact]
    public void StyleSetByTheApplication_IsKept()
    {
        var sut = CreateWindow();
        var style = new Style(typeof(Window));
        sut.Style = style;

        CreateHandle(sut);

        Assert.Same(style, sut.Style);
    }

    [StaFact]
    public void DynamicStyleSetByTheApplication_IsKept()
    {
        var sut = CreateWindow();
        var style = new Style(typeof(NiceWindow));
        sut.Resources["AppWindowStyle"] = style;
        sut.SetResourceReference(FrameworkElement.StyleProperty, "AppWindowStyle");

        CreateHandle(sut);

        Assert.Same(style, sut.Style);
    }

    // Creates the native window without showing it, which runs OnSourceInitialized.
    private static void CreateHandle(Window window)
        => new WindowInteropHelper(window).EnsureHandle();

    // The NiceWindow style is applied straight from its own dictionary: the theme dictionary (Generic.xaml)
    // registers the ATC themes at the process-wide ThemeManager, which other tests use from other threads.
    private static NiceWindow CreateWindow()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        var styles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Themes/Windows/NiceWindow.xaml", UriKind.Absolute),
        };

        var window = new NiceWindow
        {
            OverridesDefaultStyle = true,
            Style = (Style)styles[typeof(NiceWindow)],
        };
        window.Resources["AtcApps.Brushes.ThemeBackground"] = ThemeBackground;
        window.Resources["AtcApps.Brushes.WindowTitle"] = WindowTitle;
        window.Resources["AtcApps.Brushes.WindowTitle.NonActive"] = WindowTitle;
        window.Resources["AtcApps.Brushes.ThemeForeground"] = ThemeForeground;
        window.Resources["AtcApps.Brushes.IdealForeground"] = IdealForeground;

        // A window created in code gets its default style when it is initialized.
        window.BeginInit();
        window.EndInit();
        return window;
    }

    // ControlzEx sets the read-only CurrentBackdropType once DWM accepts the backdrop, which needs a shown
    // window; the tests set it the same way instead of putting a window on screen.
    private static void ReportAppliedBackdrop(
        Window window,
        WindowBackdropType appliedType)
    {
        var key = (DependencyPropertyKey)typeof(WindowBackdropManager)
            .GetField("CurrentBackdropTypePropertyKey", BindingFlags.NonPublic | BindingFlags.Static)!
            .GetValue(obj: null)!;
        window.SetValue(key, appliedType);
    }

    private static void SkipInHighContrast()
    {
        if (SystemParameters.HighContrast)
        {
            Assert.Skip("Window backdrops are not used in high contrast.");
        }
    }
}