namespace Atc.Wpf.Navigation;

/// <summary>
/// Event arguments for navigation events.
/// </summary>
public sealed class NavigatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatedEventArgs"/> class.
    /// </summary>
    /// <param name="previousViewModel">The previous ViewModel, if any.</param>
    /// <param name="currentViewModel">The current ViewModel.</param>
    /// <param name="parameters">The navigation parameters.</param>
    public NavigatedEventArgs(
        object? previousViewModel,
        object currentViewModel,
        NavigationParameters? parameters)
    {
        PreviousViewModel = previousViewModel;
        CurrentViewModel = currentViewModel ?? throw new ArgumentNullException(nameof(currentViewModel));
        Parameters = parameters ?? new NavigationParameters();
    }

    /// <summary>
    /// Gets the previous ViewModel, if any.
    /// </summary>
    public object? PreviousViewModel { get; }

    /// <summary>
    /// Gets the current (new) ViewModel.
    /// </summary>
    public object CurrentViewModel { get; }

    /// <summary>
    /// Gets the navigation parameters.
    /// </summary>
    public NavigationParameters Parameters { get; }
}