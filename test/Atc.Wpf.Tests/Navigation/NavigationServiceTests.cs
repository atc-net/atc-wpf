namespace Atc.Wpf.Tests.Navigation;

public sealed class NavigationServiceTests
{
    private readonly NavigationService sut;

    public NavigationServiceTests()
    {
        sut = new NavigationService(type => Activator.CreateInstance(type)!);
    }

    [Fact]
    public void NavigateTo_SetsCurrentViewModel()
    {
        sut.NavigateTo<SimpleViewModel>();

        Assert.NotNull(sut.CurrentViewModel);
        Assert.IsType<SimpleViewModel>(sut.CurrentViewModel);
    }

    [Fact]
    public void NavigateTo_RaisesNavigatedEvent()
    {
        NavigatedEventArgs? receivedArgs = null;
        sut.Navigated += (_, e) => receivedArgs = e;

        sut.NavigateTo<SimpleViewModel>();

        Assert.NotNull(receivedArgs);
        Assert.IsType<SimpleViewModel>(receivedArgs.CurrentViewModel);
    }

    [Fact]
    public void NavigateTo_WithParameters_PassesParameters()
    {
        NavigatedEventArgs? receivedArgs = null;
        sut.Navigated += (_, e) => receivedArgs = e;

        var parameters = new NavigationParameters()
            .WithParameter("Id", 42);

        sut.NavigateTo<SimpleViewModel>(parameters);

        Assert.NotNull(receivedArgs);
        Assert.Equal(42, receivedArgs.Parameters.GetValue<int>("Id"));
    }

    [Fact]
    public void NavigateTo_CallsOnNavigatedTo_WhenNavigationAware()
    {
        sut.NavigateTo<AwareViewModel>();

        var vm = Assert.IsType<AwareViewModel>(sut.CurrentViewModel);
        Assert.True(vm.NavigatedToCalled);
    }

    [Fact]
    public void NavigateTo_CallsOnNavigatedFrom_OnPreviousViewModel()
    {
        sut.NavigateTo<AwareViewModel>();
        var first = Assert.IsType<AwareViewModel>(sut.CurrentViewModel);

        sut.NavigateTo<SimpleViewModel>();

        Assert.True(first.NavigatedFromCalled);
    }

    [Fact]
    public void GoBack_ReturnsTrue_WhenHistoryExists()
    {
        sut.NavigateTo<SimpleViewModel>();
        sut.NavigateTo<AwareViewModel>();

        var result = sut.GoBack();

        Assert.True(result);
        Assert.IsType<SimpleViewModel>(sut.CurrentViewModel);
    }

    [Fact]
    public void GoBack_ReturnsFalse_WhenNoHistory()
    {
        sut.NavigateTo<SimpleViewModel>();

        var result = sut.GoBack();

        Assert.False(result);
    }

    [Fact]
    public void GoForward_ReturnsTrue_AfterGoBack()
    {
        sut.NavigateTo<SimpleViewModel>();
        sut.NavigateTo<AwareViewModel>();
        sut.GoBack();

        var result = sut.GoForward();

        Assert.True(result);
        Assert.IsType<AwareViewModel>(sut.CurrentViewModel);
    }

    [Fact]
    public void GoForward_ReturnsFalse_WhenNoForwardHistory()
    {
        sut.NavigateTo<SimpleViewModel>();

        var result = sut.GoForward();

        Assert.False(result);
    }

    [Fact]
    public void NavigateTo_ClearsForwardHistory()
    {
        sut.NavigateTo<SimpleViewModel>();
        sut.NavigateTo<AwareViewModel>();
        sut.GoBack();
        Assert.True(sut.CanGoForward);

        sut.NavigateTo<AwareViewModel>();

        Assert.False(sut.CanGoForward);
    }

    [Fact]
    public void Guard_BlocksNavigation_WhenCanNavigateAwayReturnsFalse()
    {
        sut.NavigateTo<GuardedViewModel>();
        var guarded = Assert.IsType<GuardedViewModel>(sut.CurrentViewModel);
        guarded.AllowNavigation = false;

        var result = sut.NavigateTo<SimpleViewModel>();

        Assert.False(result);
        Assert.IsType<GuardedViewModel>(sut.CurrentViewModel);
    }

    [Fact]
    public async Task GuardAsync_BlocksNavigation()
    {
        await sut.NavigateToAsync<GuardedViewModel>(
            cancellationToken: TestContext.Current.CancellationToken);
        var guarded = Assert.IsType<GuardedViewModel>(sut.CurrentViewModel);
        guarded.AllowNavigation = false;

        var result = await sut.NavigateToAsync<SimpleViewModel>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(result);
        Assert.IsType<GuardedViewModel>(sut.CurrentViewModel);
    }

    [Fact]
    public void ClearHistory_ResetsBackAndForward()
    {
        sut.NavigateTo<SimpleViewModel>();
        sut.NavigateTo<AwareViewModel>();
        sut.GoBack();

        sut.ClearHistory();

        Assert.False(sut.CanGoBack);
        Assert.False(sut.CanGoForward);
    }
}