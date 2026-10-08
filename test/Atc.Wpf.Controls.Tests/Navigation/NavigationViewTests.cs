namespace Atc.Wpf.Controls.Tests.Navigation;

public sealed class NavigationViewTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void InvokingAnItem_SelectsItAndRaisesSelectionChanged()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var sut = CreateView(home);
        NavigationViewSelectionChangedEventArgs? raised = null;
        sut.SelectionChanged += (_, e) => raised = e;

        Invoke(home);

        Assert.Same(home, sut.SelectedItem);
        Assert.True(home.IsSelected);
        Assert.NotNull(raised);
        Assert.Null(raised.OldItem);
        Assert.Same(home, raised.NewItem);
    }

    [StaFact]
    public void InvokingAnItem_RaisesItemInvoked()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var sut = CreateView(home);
        NavigationViewItem? invoked = null;
        sut.ItemInvoked += (_, e) => invoked = e.Item;

        Invoke(home);

        Assert.Same(home, invoked);
    }

    [StaFact]
    public void SelectingAnotherItem_DeselectsThePreviousOneAcrossMenuAndFooter()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var settings = new NavigationViewItem { Content = "Settings" };
        var sut = CreateView(home);
        sut.FooterMenuItems.Add(settings);
        Invoke(home);

        Invoke(settings);

        Assert.Same(settings, sut.SelectedItem);
        Assert.False(home.IsSelected);
        Assert.True(settings.IsSelected);
    }

    [StaFact]
    public void ItemThatDoesNotSelectOnInvoked_KeepsTheSelectionButRaisesItemInvoked()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var about = new NavigationViewItem { Content = "About", SelectsOnInvoked = false };
        var sut = CreateView(home, about);
        Invoke(home);
        NavigationViewItem? invoked = null;
        sut.ItemInvoked += (_, e) => invoked = e.Item;

        Invoke(about);

        Assert.Same(home, sut.SelectedItem);
        Assert.False(about.IsSelected);
        Assert.Same(about, invoked);
    }

    [StaFact]
    public void InvokingAnItemWithATarget_NavigatesAndShowsTheViewModel()
    {
        var service = CreateService();
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(settings);
        sut.NavigationService = service;

        Invoke(settings);

        Assert.IsType<SettingsViewModel>(service.CurrentViewModel);
        Assert.Same(service.CurrentViewModel, sut.Content);
    }

    [StaFact]
    public void InvokingAnItemWithATarget_PassesItsNavigationParameters()
    {
        var service = CreateService();
        var parameters = new NavigationParameters().WithParameter("Id", 42);
        var details = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel), NavigationParameters = parameters };
        var sut = CreateView(details);
        sut.NavigationService = service;
        NavigationParameters? received = null;
        service.Navigated += (_, e) => received = e.Parameters;

        Invoke(details);

        Assert.NotNull(received);
        Assert.Equal(42, received.GetValue<int>("Id"));
    }

    [StaFact]
    public void NavigationBlockedByAGuard_KeepsThePreviousSelection()
    {
        var service = CreateService();
        var home = new NavigationViewItem { TargetViewModelType = typeof(GuardedViewModel) };
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(home, settings);
        sut.NavigationService = service;
        Invoke(home);
        var selectionChanges = 0;
        sut.SelectionChanged += (_, _) => selectionChanges++;

        Invoke(settings);

        Assert.IsType<GuardedViewModel>(service.CurrentViewModel);
        Assert.Same(home, sut.SelectedItem);
        Assert.True(home.IsSelected);
        Assert.False(settings.IsSelected);
        Assert.Equal(0, selectionChanges);
    }

    [StaFact]
    public void NavigationFromCode_SelectsTheMatchingItem()
    {
        var service = CreateService();
        var home = new NavigationViewItem { TargetViewModelType = typeof(HomeViewModel) };
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(home, settings);
        sut.NavigationService = service;

        service.NavigateTo<SettingsViewModel>();

        Assert.Same(settings, sut.SelectedItem);
        Assert.Same(service.CurrentViewModel, sut.Content);
    }

    [StaFact]
    public void NavigationToAViewModelWithoutAnItem_ClearsTheSelection()
    {
        var service = CreateService();
        var home = new NavigationViewItem { TargetViewModelType = typeof(HomeViewModel) };
        var sut = CreateView(home);
        sut.NavigationService = service;
        Invoke(home);

        service.NavigateTo<SettingsViewModel>();

        Assert.Null(sut.SelectedItem);
        Assert.False(home.IsSelected);
    }

    [StaFact]
    public void GoBack_NavigatesBackAndSelectsThePreviousItem()
    {
        var service = CreateService();
        var home = new NavigationViewItem { TargetViewModelType = typeof(HomeViewModel) };
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(home, settings);
        sut.NavigationService = service;
        Invoke(home);
        Invoke(settings);

        var result = sut.GoBack();

        Assert.True(result);
        Assert.IsType<HomeViewModel>(service.CurrentViewModel);
        Assert.Same(home, sut.SelectedItem);
    }

    [StaFact]
    public void IsBackEnabled_FollowsTheServiceHistory()
    {
        var service = CreateService();
        var home = new NavigationViewItem { TargetViewModelType = typeof(HomeViewModel) };
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(home, settings);
        sut.NavigationService = service;

        Invoke(home);
        var afterFirst = sut.IsBackEnabled;
        Invoke(settings);
        var afterSecond = sut.IsBackEnabled;

        Assert.False(afterFirst);
        Assert.True(afterSecond);
    }

    [StaFact]
    public void NavigationServiceSetAfterNavigating_ShowsItsCurrentViewModel()
    {
        var service = CreateService();
        service.NavigateTo<SettingsViewModel>();
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(settings);

        sut.NavigationService = service;

        Assert.Same(service.CurrentViewModel, sut.Content);
        Assert.Same(settings, sut.SelectedItem);
    }

    [StaFact]
    public void NavigationServiceReplaced_StopsFollowingTheOldService()
    {
        var oldService = CreateService();
        var settings = new NavigationViewItem { TargetViewModelType = typeof(SettingsViewModel) };
        var sut = CreateView(settings);
        sut.NavigationService = oldService;
        sut.NavigationService = CreateService();

        oldService.NavigateTo<SettingsViewModel>();

        Assert.Null(sut.Content);
        Assert.Null(sut.SelectedItem);
    }

    [StaFact]
    public void ItemRemoved_IsNoLongerInvokable()
    {
        var home = new NavigationViewItem { Content = "Home" };
        var sut = CreateView(home);
        sut.MenuItems.Remove(home);

        Invoke(home);

        Assert.Null(sut.SelectedItem);
    }

    private static NavigationView CreateView(params NavigationViewItem[] items)
    {
        var view = new NavigationView();
        foreach (var item in items)
        {
            view.MenuItems.Add(item);
        }

        return view;
    }

    private static NavigationService CreateService()
        => new(type => Activator.CreateInstance(type)!);

    private static void Invoke(NavigationViewItem item)
        => item.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, item));

    private sealed class HomeViewModel;

    private sealed class SettingsViewModel;

    private sealed class GuardedViewModel : INavigationGuard
    {
        public bool CanNavigateAway()
            => false;

        public Task<bool> CanNavigateAwayAsync(
            CancellationToken cancellationToken = default)
            => Task.FromResult(false);
    }
}