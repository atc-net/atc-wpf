namespace Atc.Wpf.Theming.Tests.Controls.Windows;

public sealed class NiceWindowCultureTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    // An application that sets its culture at startup must keep it when a window shows.
    [StaFact]
    public void ContentRendered_KeepsTheCurrentCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var originalDefaultCulture = CultureInfo.DefaultThreadCurrentCulture;
        var originalDefaultUiCulture = CultureInfo.DefaultThreadCurrentUICulture;
        var danish = new CultureInfo("da-DK");

        try
        {
            CultureInfo.CurrentCulture = danish;
            CultureInfo.CurrentUICulture = danish;
            var sut = CreateWindow();

            RaiseContentRendered(sut);

            Assert.Equal(danish.Name, CultureInfo.CurrentCulture.Name);
            Assert.Equal(danish.Name, CultureInfo.CurrentUICulture.Name);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
            CultureInfo.DefaultThreadCurrentCulture = originalDefaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = originalDefaultUiCulture;
        }
    }

    // Window.OnContentRendered raises ContentRendered; it is protected, so it is invoked by reflection
    // instead of putting a window on screen.
    private static void RaiseContentRendered(Window window)
        => typeof(Window)
            .GetMethod("OnContentRendered", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(window, [EventArgs.Empty]);

    private static NiceWindow CreateWindow()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var window = new NiceWindow
        {
            OverridesDefaultStyle = true,
        };

        window.BeginInit();
        window.EndInit();
        return window;
    }
}