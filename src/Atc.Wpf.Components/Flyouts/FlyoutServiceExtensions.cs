namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// Extension methods for configuring flyout services with dependency injection.
/// </summary>
public static class FlyoutServiceExtensions
{
    /// <summary>
    /// Registers a view type for a ViewModel type using fluent configuration.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <returns>The flyout service for chaining.</returns>
    public static IFlyoutService WithView<TViewModel, TView>(
        this IFlyoutService service)
        where TViewModel : class
        where TView : FrameworkElement, new()
    {
        ArgumentNullException.ThrowIfNull(service);
        service.RegisterView<TViewModel, TView>();
        return service;
    }

    /// <summary>
    /// Registers a view factory for a ViewModel type using fluent configuration.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="viewFactory">The factory function that creates the view.</param>
    /// <returns>The flyout service for chaining.</returns>
    public static IFlyoutService WithViewFactory<TViewModel>(
        this IFlyoutService service,
        Func<FrameworkElement> viewFactory)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(viewFactory);
        service.RegisterViewFactory<TViewModel>(viewFactory);
        return service;
    }

    /// <summary>
    /// Shows a flyout and returns the result as a form result.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="getModel">Function to extract the model from the ViewModel.</param>
    /// <param name="options">Optional flyout configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The form result.</returns>
    public static async Task<FlyoutFormResult<TModel>> ShowFormAsync<TViewModel, TModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        Func<TViewModel, TModel> getModel,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(getModel);

        var result = await service.ShowAsync(header, viewModel, options, cancellationToken)
            .ConfigureAwait(false);

        var model = getModel(viewModel);

        if (result is null)
        {
            return FlyoutFormResultFactory.Cancelled(model);
        }

        if (result is bool success && success)
        {
            return FlyoutFormResultFactory.Success(model);
        }

        if (result is IReadOnlyList<string> errors)
        {
            return FlyoutFormResultFactory.ValidationFailed(model, errors);
        }

        return FlyoutFormResultFactory.Success(model);
    }

    /// <summary>
    /// Shows a flyout with a wide width preset.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The flyout result.</returns>
    public static Task<object?> ShowWideAsync<TViewModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        return service.ShowAsync(header, viewModel, FlyoutOptions.Wide, cancellationToken);
    }

    /// <summary>
    /// Shows a flyout with a narrow width preset.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The flyout result.</returns>
    public static Task<object?> ShowNarrowAsync<TViewModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        return service.ShowAsync(header, viewModel, FlyoutOptions.Narrow, cancellationToken);
    }

    /// <summary>
    /// Shows a modal flyout (no light dismiss).
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The flyout result.</returns>
    public static Task<object?> ShowModalAsync<TViewModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        return service.ShowAsync(header, viewModel, FlyoutOptions.Modal, cancellationToken);
    }

    /// <summary>
    /// Shows a centered flyout (dialog-like).
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The flyout result.</returns>
    public static Task<object?> ShowCenteredAsync<TViewModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        return service.ShowAsync(header, viewModel, FlyoutOptions.Center, cancellationToken);
    }

    /// <summary>
    /// Shows a flyout from the left side.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
    /// <param name="service">The flyout service.</param>
    /// <param name="header">The flyout header.</param>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The flyout result.</returns>
    public static Task<object?> ShowFromLeftAsync<TViewModel>(
        this IFlyoutService service,
        string header,
        TViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(service);
        return service.ShowAsync(header, viewModel, FlyoutOptions.Left, cancellationToken);
    }
}