namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// Service interface for showing flyouts in an MVVM-friendly way.
/// </summary>
public interface IFlyoutService
{
    /// <summary>
    /// Gets the number of currently open flyouts.
    /// </summary>
    int OpenFlyoutCount { get; }

    /// <summary>
    /// Gets whether any flyout is currently open.
    /// </summary>
    bool IsAnyFlyoutOpen { get; }

    /// <summary>
    /// Shows a flyout with the specified content.
    /// </summary>
    /// <param name="header">The flyout header.</param>
    /// <param name="content">The content to display in the flyout.</param>
    /// <param name="options">Optional flyout configuration options.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when the flyout is closed, with the result value if any.</returns>
    Task<object?> ShowAsync(
        string header,
        object content,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a flyout with the specified ViewModel. The view is resolved automatically.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel.</typeparam>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="options">Optional flyout configuration options.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when the flyout is closed, with the result value if any.</returns>
    Task<object?> ShowAsync<TViewModel>(
        string header,
        TViewModel viewModel,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class;

    /// <summary>
    /// Shows a flyout with the specified ViewModel and returns a typed result.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel.</typeparam>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="options">Optional flyout configuration options.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when the flyout is closed, with the typed result.</returns>
    Task<TResult?> ShowAsync<TViewModel, TResult>(
        string header,
        TViewModel viewModel,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class;

    /// <summary>
    /// Closes the topmost flyout.
    /// </summary>
    /// <returns>True if a flyout was closed; otherwise, false.</returns>
    bool CloseTopFlyout();

    /// <summary>
    /// Closes all open flyouts.
    /// </summary>
    void CloseAllFlyouts();

    /// <summary>
    /// Closes the topmost flyout with a result value.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <returns>True if a flyout was closed; otherwise, false.</returns>
    bool CloseTopFlyoutWithResult(object? result);

    /// <summary>
    /// Registers a view type for a ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    void RegisterView<TViewModel, TView>()
        where TViewModel : class
        where TView : FrameworkElement, new();

    /// <summary>
    /// Registers a view factory for a ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="viewFactory">The factory function that creates the view.</param>
    void RegisterViewFactory<TViewModel>(Func<FrameworkElement> viewFactory)
        where TViewModel : class;
}