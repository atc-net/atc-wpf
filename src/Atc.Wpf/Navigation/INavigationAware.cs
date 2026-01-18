namespace Atc.Wpf.Navigation;

/// <summary>
/// Interface for ViewModels that want to be notified about navigation events.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Called when navigating to this ViewModel.
    /// </summary>
    /// <param name="parameters">The navigation parameters passed to this ViewModel.</param>
    void OnNavigatedTo(NavigationParameters? parameters);

    /// <summary>
    /// Called when navigating away from this ViewModel.
    /// </summary>
    void OnNavigatedFrom();
}