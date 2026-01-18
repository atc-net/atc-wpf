namespace Atc.Wpf.Navigation;

/// <summary>
/// Interface for ViewModels that want to control whether navigation away is allowed.
/// </summary>
public interface INavigationGuard
{
    /// <summary>
    /// Determines whether navigation away from this ViewModel is allowed.
    /// </summary>
    /// <returns>True if navigation is allowed; otherwise, false.</returns>
    bool CanNavigateAway();

    /// <summary>
    /// Asynchronously determines whether navigation away from this ViewModel is allowed.
    /// Override this for async validation (e.g., showing a confirmation dialog).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if navigation is allowed; otherwise, false.</returns>
    Task<bool> CanNavigateAwayAsync(
        CancellationToken cancellationToken = default);
}