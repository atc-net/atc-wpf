namespace Atc.Wpf.Controls.Tests.Navigation;

/// <summary>
/// Applies the shipped NavigationView style and checks the template parts the control relies on.
/// Theme brushes are DynamicResources and simply stay unset here.
/// </summary>
public sealed class NavigationViewTemplateTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Template_HostsMenuAndFooterItemsInThePane()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var settings = new NavigationViewItem { Content = "Settings" };
        var sut = CreateView(home);
        sut.FooterMenuItems.Add(settings);

        Layout(sut);

        Assert.IsType<StackPanel>(VisualTreeHelper.GetParent(home));
        Assert.Equal("PART_MenuItemsHost", ((FrameworkElement)VisualTreeHelper.GetParent(home)).Name);
        Assert.Equal("PART_FooterMenuItemsHost", ((FrameworkElement)VisualTreeHelper.GetParent(settings)).Name);
    }

    [StaFact]
    public void ItemAddedAfterTheTemplate_IsHostedToo()
    {
        var sut = CreateView();
        Layout(sut);
        var home = new NavigationViewItem { Content = "Home" };

        sut.MenuItems.Add(home);

        Assert.Equal("PART_MenuItemsHost", ((FrameworkElement)VisualTreeHelper.GetParent(home)).Name);
    }

    [StaFact]
    public void PaneWidth_FollowsIsPaneOpen()
    {
        var sut = CreateView(new NavigationViewItem { Content = "Home" });
        sut.OpenPaneLength = 200;
        sut.CompactPaneLength = 50;
        Layout(sut);
        var pane = (FrameworkElement)sut.Template.FindName("PART_Pane", sut);
        var openWidth = pane.ActualWidth;

        sut.IsPaneOpen = false;
        Layout(sut);

        Assert.Equal(200, openWidth);
        Assert.Equal(50, pane.ActualWidth);
    }

    [StaFact]
    public void PaneToggleButton_TogglesIsPaneOpen()
    {
        var sut = CreateView(new NavigationViewItem { Content = "Home" });
        Layout(sut);
        var toggle = (ToggleButton)sut.Template.FindName("PART_PaneToggleButton", sut);

        toggle.IsChecked = false;

        Assert.False(sut.IsPaneOpen);
    }

    [StaFact]
    public void BackButton_NavigatesBack()
    {
        var service = new NavigationService(type => Activator.CreateInstance(type)!);
        var sut = CreateView();
        sut.NavigationService = service;
        Layout(sut);
        service.NavigateTo<HomeViewModel>();
        service.NavigateTo<SettingsViewModel>();
        var back = (ButtonBase)sut.Template.FindName("PART_BackButton", sut);

        back.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, back));

        Assert.IsType<HomeViewModel>(service.CurrentViewModel);
    }

    [StaFact]
    public void IconColumnWidth_IsTheCompactPaneMinusTheItemMargins()
    {
        var converter = NavigationViewIconColumnWidthValueConverter.Instance;

        var column = converter.Convert(48d, typeof(GridLength), null, CultureInfo.InvariantCulture);
        var width = converter.Convert(48d, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(new GridLength(40), column);
        Assert.Equal(40d, width);
    }

    private static NavigationView CreateView(params NavigationViewItem[] items)
    {
        var resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Navigation/NavigationView.xaml"),
        };

        var view = new NavigationView
        {
            Style = (Style)resources["AtcApps.Styles.NavigationView"],
        };
        view.Resources.MergedDictionaries.Add(resources);
        view.Resources[typeof(NavigationViewItem)] = resources["AtcApps.Styles.NavigationViewItem"];

        foreach (var item in items)
        {
            view.MenuItems.Add(item);
        }

        return view;
    }

    private static void Layout(NavigationView view)
    {
        view.Measure(new Size(800, 600));
        view.Arrange(new Rect(0, 0, 800, 600));
        view.UpdateLayout();
    }

    private sealed class HomeViewModel;

    private sealed class SettingsViewModel;
}