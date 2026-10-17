namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// Every flyout listens for Escape on its window, and the first listener used to win, so Escape closed the
/// oldest open flyout. Escape must act on the topmost (most recently opened) flyout only.
/// </summary>
public sealed class FlyoutEscapeTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Escape_TwoOpenFlyouts_ClosesOnlyTheNewerOne()
    {
        var older = new Flyout();
        var newer = new Flyout();
        var window = CreateWindow(older, newer);

        try
        {
            older.IsOpen = true;
            newer.IsOpen = true;

            PressEscape(window);

            Assert.True(older.IsOpen);
            Assert.False(newer.IsOpen);

            PressEscape(window);

            Assert.False(older.IsOpen);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Escape_TopmostFlyoutIsPinned_ClosesNoFlyout()
    {
        var older = new Flyout();
        var newer = new Flyout { IsPinned = true };
        var window = CreateWindow(older, newer);

        try
        {
            older.IsOpen = true;
            newer.IsOpen = true;

            PressEscape(window);

            Assert.True(older.IsOpen);
            Assert.True(newer.IsOpen);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window CreateWindow(
        Flyout older,
        Flyout newer)
    {
        // The older flyout is loaded first, so it subscribes to the window's key events first.
        var root = new Grid();
        root.Children.Add(older);
        root.Children.Add(newer);

        var window = new Window
        {
            Content = root,
            Width = 200,
            Height = 200,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        return window;
    }

    private static void PressEscape(Window window)
        => window.RaiseEvent(
            new System.Windows.Input.KeyEventArgs(
                System.Windows.Input.Keyboard.PrimaryDevice,
                PresentationSource.FromVisual(window)!,
                timestamp: 0,
                System.Windows.Input.Key.Escape)
            {
                RoutedEvent = System.Windows.Input.Keyboard.PreviewKeyDownEvent,
            });
}