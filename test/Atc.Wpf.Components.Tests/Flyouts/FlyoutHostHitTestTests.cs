namespace Atc.Wpf.Components.Tests.Flyouts;

public sealed class FlyoutHostHitTestTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    // A FlyoutHost is placed over the page content; with no flyout open, clicks must reach the content below it.
    [StaFact]
    public void Click_OverAnEmptyFlyoutHost_ReachesTheContentBelowIt()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        var styles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Components;component/Flyouts/FlyoutHost.xaml", UriKind.Absolute),
        };

        var button = new Button { Content = "Open", Width = 120, Height = 40 };
        var host = new FlyoutHost { Style = (Style)styles["AtcApps.Styles.FlyoutHost"] };
        host.Items.Add(new Flyout { Visibility = Visibility.Collapsed });

        var root = new Grid { Width = 400, Height = 300 };
        root.Children.Add(button);
        root.Children.Add(host);
        using var source = new HwndSource(new HwndSourceParameters("FlyoutHostHitTestTests") { WindowStyle = 0 })
        {
            SizeToContent = SizeToContent.WidthAndHeight,
            RootVisual = root,
        };
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        var hit = root.InputHitTest(new Point(200, 150)) as DependencyObject;

        Assert.True(IsInside(hit, button), $"The click hit {hit?.GetType().Name ?? "nothing"} instead of the button.");
    }

    private static bool IsInside(
        DependencyObject? element,
        DependencyObject ancestor)
    {
        for (var current = element; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }
}