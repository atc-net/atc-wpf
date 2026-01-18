namespace Atc.Wpf.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationService"/>.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Func<Type, object> viewModelFactory;
    private readonly NavigationHistory history = new();
    private object? currentViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="viewModelFactory">
    /// Factory function to create ViewModel instances.
    /// Typically this would be your DI container's resolve method.
    /// </param>
    public NavigationService(Func<Type, object> viewModelFactory)
    {
        this.viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
    }

    /// <inheritdoc />
    public bool CanGoBack => history.CanGoBack;

    /// <inheritdoc />
    public bool CanGoForward => history.CanGoForward;

    /// <inheritdoc />
    public object? CurrentViewModel => currentViewModel;

    /// <inheritdoc />
    public event EventHandler<NavigatedEventArgs>? Navigated;

    /// <inheritdoc />
    public bool NavigateTo<TViewModel>(NavigationParameters? parameters = null)
        where TViewModel : class
        => NavigateTo(typeof(TViewModel), parameters);

    /// <inheritdoc />
    public bool NavigateTo(
        Type viewModelType,
        NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        // Check navigation guard synchronously
        if (currentViewModel is INavigationGuard guard && !guard.CanNavigateAway())
        {
            return false;
        }

        return PerformNavigation(viewModelType, parameters);
    }

    /// <inheritdoc />
    public Task<bool> NavigateToAsync<TViewModel>(
        NavigationParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class
        => NavigateToAsync(typeof(TViewModel), parameters, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> NavigateToAsync(
        Type viewModelType,
        NavigationParameters? parameters = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        // Check navigation guard asynchronously
        if (currentViewModel is INavigationGuard guard &&
            !await guard.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        return PerformNavigation(viewModelType, parameters);
    }

    /// <inheritdoc />
    public bool GoBack()
    {
        if (!CanGoBack)
        {
            return false;
        }

        // Check navigation guard
        if (currentViewModel is INavigationGuard guard && !guard.CanNavigateAway())
        {
            return false;
        }

        return PerformGoBack();
    }

    /// <inheritdoc />
    public async Task<bool> GoBackAsync(
        CancellationToken cancellationToken = default)
    {
        if (!CanGoBack)
        {
            return false;
        }

        // Check navigation guard asynchronously
        if (currentViewModel is INavigationGuard guard &&
            !await guard.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        return PerformGoBack();
    }

    /// <inheritdoc />
    public bool GoForward()
    {
        if (!CanGoForward)
        {
            return false;
        }

        // Check navigation guard
        if (currentViewModel is INavigationGuard guard && !guard.CanNavigateAway())
        {
            return false;
        }

        return PerformGoForward();
    }

    /// <inheritdoc />
    public async Task<bool> GoForwardAsync(
        CancellationToken cancellationToken = default)
    {
        if (!CanGoForward)
        {
            return false;
        }

        // Check navigation guard asynchronously
        if (currentViewModel is INavigationGuard guard &&
            !await guard.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        return PerformGoForward();
    }

    /// <inheritdoc />
    public void ClearHistory()
    {
        history.Clear();
    }

    /// <summary>
    /// Raises the <see cref="Navigated"/> event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnNavigated(NavigatedEventArgs e)
    {
        Navigated?.Invoke(this, e);
    }

    private bool PerformNavigation(
        Type viewModelType,
        NavigationParameters? parameters)
    {
        var previousViewModel = currentViewModel;

        // Notify the previous ViewModel that it's being navigated away from
        if (previousViewModel is INavigationAware previousAware)
        {
            previousAware.OnNavigatedFrom();
        }

        // Create the new ViewModel
        var newViewModel = viewModelFactory(viewModelType);

        // Update current and history
        currentViewModel = newViewModel;
        history.Navigate(new NavigationEntry(viewModelType, parameters));

        // Notify the new ViewModel that it's been navigated to
        if (newViewModel is INavigationAware newAware)
        {
            newAware.OnNavigatedTo(parameters);
        }

        // Raise the Navigated event
        OnNavigated(new NavigatedEventArgs(previousViewModel, newViewModel, parameters));

        return true;
    }

    private bool PerformGoBack()
    {
        var entry = history.GoBack();
        if (entry is null)
        {
            return false;
        }

        return NavigateToEntry(entry);
    }

    private bool PerformGoForward()
    {
        var entry = history.GoForward();
        if (entry is null)
        {
            return false;
        }

        return NavigateToEntry(entry);
    }

    private bool NavigateToEntry(NavigationEntry entry)
    {
        var previousViewModel = currentViewModel;

        // Notify the previous ViewModel that it's being navigated away from
        if (previousViewModel is INavigationAware previousAware)
        {
            previousAware.OnNavigatedFrom();
        }

        // Create the new ViewModel
        var newViewModel = viewModelFactory(entry.ViewModelType);

        // Update current
        currentViewModel = newViewModel;

        // Notify the new ViewModel that it's been navigated to
        if (newViewModel is INavigationAware newAware)
        {
            newAware.OnNavigatedTo(entry.Parameters);
        }

        // Raise the Navigated event
        OnNavigated(new NavigatedEventArgs(previousViewModel, newViewModel, entry.Parameters));

        return true;
    }
}