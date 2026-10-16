namespace Atc.Wpf.Theming.Tests.Controls.Windows;

public sealed class WindowButtonCommandsTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    // Outside a NiceWindow nothing sets ParentWindow, so the control has to find its window itself once it is loaded.
    [StaFact]
    public void ParentWindow_InsideAPlainWindow_IsResolvedWhenLoaded()
    {
        var sut = CreateWithoutThemeStyle();
        var window = new Window { Content = sut };

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.Same(window, sut.ParentWindow);
    }

    [StaFact]
    public void ParentWindow_AlreadySet_IsKeptWhenLoaded()
    {
        var sut = CreateWithoutThemeStyle();
        var owner = new Window();
        _ = new Window { Content = sut };
        sut.SetValue(WindowButtonCommands.ParentWindowPropertyKey, owner);

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.Same(owner, sut.ParentWindow);
    }

    // The theme style lives in Generic.xaml, whose library theme provider registers at the ThemeManager; that is owned by
    // whichever test thread touched ThemeManager first. The Loaded handling under test does not need the style.
    private static WindowButtonCommands CreateWithoutThemeStyle()
        => new() { OverridesDefaultStyle = true };
}