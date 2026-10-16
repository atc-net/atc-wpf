namespace Atc.Wpf.Tests.Navigation;

public sealed class NavigationServiceAsyncGuardTests
{
    [WpfFact]
    public async Task NavigateToAsync_WithGuardThatCompletesOnAnotherThread_RaisesNavigatedOnTheCallingThread()
    {
        var sut = new NavigationService(type => Activator.CreateInstance(type)!);
        await sut.NavigateToAsync<ThreadHoppingGuardViewModel>();
        var callingThreadId = Environment.CurrentManagedThreadId;
        var navigatedThreadId = -1;
        sut.Navigated += (_, _) => navigatedThreadId = Environment.CurrentManagedThreadId;

        var result = await sut.NavigateToAsync<SimpleViewModel>();

        Assert.True(result);
        Assert.Equal(callingThreadId, navigatedThreadId);
    }

    [WpfFact]
    public async Task GoBackAsync_WithGuardThatCompletesOnAnotherThread_RaisesNavigatedOnTheCallingThread()
    {
        var sut = new NavigationService(type => Activator.CreateInstance(type)!);
        await sut.NavigateToAsync<SimpleViewModel>();
        await sut.NavigateToAsync<ThreadHoppingGuardViewModel>();
        var callingThreadId = Environment.CurrentManagedThreadId;
        var navigatedThreadId = -1;
        sut.Navigated += (_, _) => navigatedThreadId = Environment.CurrentManagedThreadId;

        var result = await sut.GoBackAsync();

        Assert.True(result);
        Assert.Equal(callingThreadId, navigatedThreadId);
    }

    [WpfFact]
    public async Task GoForwardAsync_WithGuardThatCompletesOnAnotherThread_RaisesNavigatedOnTheCallingThread()
    {
        var sut = new NavigationService(type => Activator.CreateInstance(type)!);
        await sut.NavigateToAsync<ThreadHoppingGuardViewModel>();
        await sut.NavigateToAsync<SimpleViewModel>();
        await sut.GoBackAsync();
        var callingThreadId = Environment.CurrentManagedThreadId;
        var navigatedThreadId = -1;
        sut.Navigated += (_, _) => navigatedThreadId = Environment.CurrentManagedThreadId;

        var result = await sut.GoForwardAsync();

        Assert.True(result);
        Assert.Equal(callingThreadId, navigatedThreadId);
    }

    [WpfFact]
    public async Task NavigateToAsync_WithGuardThatCompletesOnAnotherThread_CanUpdateADependencyObjectFromNavigated()
    {
        var sut = new NavigationService(type => Activator.CreateInstance(type)!);
        await sut.NavigateToAsync<ThreadHoppingGuardViewModel>();
        var target = new ContentControl();
        sut.Navigated += (_, e) => target.SetCurrentValue(ContentControl.ContentProperty, e.CurrentViewModel);

        await sut.NavigateToAsync<SimpleViewModel>();

        Assert.IsType<SimpleViewModel>(target.Content);
    }

    /// <summary>
    /// A guard whose task completes on a thread-pool thread, like a guard that awaits a dialog or an I/O call.
    /// </summary>
    private sealed class ThreadHoppingGuardViewModel : INavigationGuard
    {
        public bool CanNavigateAway()
            => true;

        public async Task<bool> CanNavigateAwayAsync(
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(20, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}