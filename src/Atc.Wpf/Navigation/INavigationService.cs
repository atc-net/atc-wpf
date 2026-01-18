namespace Atc.Wpf.Navigation;

/// <summary>
/// Service interface for navigation in WPF applications.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets a value indicating whether back navigation is available.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether forward navigation is available.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Gets the current ViewModel.
    /// </summary>
    object? CurrentViewModel { get; }

    /// <summary>
    /// Occurs when the current ViewModel changes.
    /// </summary>
    event EventHandler<NavigatedEventArgs>? Navigated;

    /// <summary>
    /// Navigates to the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel to navigate to.</typeparam>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    bool NavigateTo<TViewModel>(NavigationParameters? parameters = null)
        where TViewModel : class;

    /// <summary>
    /// Navigates to the specified ViewModel type.
    /// </summary>
    /// <param name="viewModelType">The type of ViewModel to navigate to.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    bool NavigateTo(
        Type viewModelType,
        NavigationParameters? parameters = null);

    /// <summary>
    /// Navigates to the specified ViewModel type asynchronously.
    /// This allows for async navigation guards.
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel to navigate to.</typeparam>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    Task<bool> NavigateToAsync<TViewModel>(
        NavigationParameters? parameters = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class;

    /// <summary>
    /// Navigates to the specified ViewModel type asynchronously.
    /// </summary>
    /// <param name="viewModelType">The type of ViewModel to navigate to.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    Task<bool> NavigateToAsync(
        Type viewModelType,
        NavigationParameters? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates back to the previous ViewModel.
    /// </summary>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    bool GoBack();

    /// <summary>
    /// Navigates back to the previous ViewModel asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    Task<bool> GoBackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates forward to the next ViewModel.
    /// </summary>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    bool GoForward();

    /// <summary>
    /// Navigates forward to the next ViewModel asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    Task<bool> GoForwardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    void ClearHistory();
}