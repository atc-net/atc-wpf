namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Default implementation of <see cref="IFlyoutService"/> for showing flyouts in an MVVM-friendly way.
/// </summary>
public class FlyoutService : IFlyoutService
{
    private readonly Dictionary<Type, Func<FrameworkElement>> viewFactories = new();
    private readonly Stack<(Flyout Flyout, TaskCompletionSource<object?> Tcs)> openFlyouts = new();
    private FlyoutHost? flyoutHost;
    private Panel? hostPanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutService"/> class.
    /// </summary>
    public FlyoutService()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutService"/> class with a host panel.
    /// </summary>
    /// <param name="hostPanel">The panel that will host the flyouts (typically a Grid at the window root).</param>
    public FlyoutService(Panel hostPanel)
    {
        SetHostPanel(hostPanel);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutService"/> class with a FlyoutHost.
    /// </summary>
    /// <param name="flyoutHost">The FlyoutHost that will manage the flyouts.</param>
    public FlyoutService(FlyoutHost flyoutHost)
    {
        SetFlyoutHost(flyoutHost);
    }

    /// <inheritdoc />
    public int OpenFlyoutCount => openFlyouts.Count;

    /// <inheritdoc />
    public bool IsAnyFlyoutOpen => openFlyouts.Count > 0;

    /// <summary>
    /// Sets the host panel for flyouts. Call this before showing any flyouts.
    /// </summary>
    /// <param name="panel">The panel that will host the flyouts.</param>
    public void SetHostPanel(Panel panel)
    {
        ArgumentNullException.ThrowIfNull(panel);
        hostPanel = panel;
        flyoutHost = null;
    }

    /// <summary>
    /// Sets the FlyoutHost for managing flyouts. Call this before showing any flyouts.
    /// </summary>
    /// <param name="host">The FlyoutHost that will manage the flyouts.</param>
    public void SetFlyoutHost(FlyoutHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        flyoutHost = host;
        hostPanel = null;
    }

    /// <inheritdoc />
    public void RegisterView<TViewModel, TView>()
        where TViewModel : class
        where TView : FrameworkElement, new()
    {
        viewFactories[typeof(TViewModel)] = () => new TView();
    }

    /// <inheritdoc />
    public void RegisterViewFactory<TViewModel>(
        Func<FrameworkElement> viewFactory)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewFactory);
        viewFactories[typeof(TViewModel)] = viewFactory;
    }

    /// <inheritdoc />
    public Task<object?> ShowAsync(
        string header,
        object content,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        EnsureHostConfigured();

        options ??= FlyoutOptions.Default;
        var flyout = CreateFlyout(header, content, options);

        return ShowFlyoutAsync(flyout, cancellationToken);
    }

    /// <inheritdoc />
    public Task<object?> ShowAsync<TViewModel>(
        string header,
        TViewModel viewModel,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        EnsureHostConfigured();

        var view = ResolveView(viewModel);
        view.DataContext = viewModel;

        options ??= FlyoutOptions.Default;
        var flyout = CreateFlyout(header, view, options);

        return ShowFlyoutAsync(flyout, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TResult?> ShowAsync<TViewModel, TResult>(
        string header,
        TViewModel viewModel,
        FlyoutOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class
    {
        var result = await ShowAsync(header, viewModel, options, cancellationToken).ConfigureAwait(false);

        if (result is TResult typedResult)
        {
            return typedResult;
        }

        return default;
    }

    /// <inheritdoc />
    public bool CloseTopFlyout()
    {
        if (openFlyouts.Count == 0)
        {
            return false;
        }

        var (flyout, _) = openFlyouts.Peek();
        flyout.IsOpen = false;
        return true;
    }

    /// <inheritdoc />
    public void CloseAllFlyouts()
    {
        while (openFlyouts.Count > 0)
        {
            var (flyout, _) = openFlyouts.Peek();
            flyout.IsOpen = false;
        }
    }

    /// <inheritdoc />
    public bool CloseTopFlyoutWithResult(object? result)
    {
        if (openFlyouts.Count == 0)
        {
            return false;
        }

        var (flyout, _) = openFlyouts.Peek();
        flyout.Result = result;
        flyout.IsOpen = false;
        return true;
    }

    private void EnsureHostConfigured()
    {
        if (hostPanel is null && flyoutHost is null)
        {
            throw new InvalidOperationException(
                "FlyoutService is not configured. Call SetHostPanel or SetFlyoutHost before showing flyouts.");
        }
    }

    private FrameworkElement ResolveView<TViewModel>(TViewModel viewModel)
        where TViewModel : class
    {
        var viewModelType = viewModel.GetType();

        if (viewFactories.TryGetValue(viewModelType, out var factory))
        {
            return factory();
        }

        // Try to find a view by convention (ViewModelName -> ViewName)
        var viewTypeName = viewModelType.FullName?.Replace(
            "ViewModel",
            "View",
            StringComparison.Ordinal);
        if (viewTypeName is not null)
        {
            var viewType = viewModelType.Assembly.GetType(viewTypeName);
            if (viewType is not null && typeof(FrameworkElement).IsAssignableFrom(viewType))
            {
                return (FrameworkElement)Activator.CreateInstance(viewType)!;
            }
        }

        // Fallback: wrap in a ContentControl
        return new ContentControl { Content = viewModel };
    }

    private Flyout CreateFlyout(
        string header,
        object content,
        FlyoutOptions options)
    {
        var flyout = new Flyout
        {
            Header = header,
            Content = content,
            Position = options.Position,
            FlyoutWidth = options.Width,
            FlyoutHeight = options.Height,
            IsLightDismissEnabled = options.IsLightDismissEnabled,
            ShowOverlay = options.ShowOverlay,
            OverlayOpacity = options.OverlayOpacity,
            ShowCloseButton = options.ShowCloseButton,
            CloseOnEscape = options.CloseOnEscape,
            AnimationDuration = options.AnimationDuration,
            CornerRadius = options.CornerRadius,
            Padding = options.Padding,
        };

        return flyout;
    }

    private Task<object?> ShowFlyoutAsync(
        Flyout flyout,
        CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<object?>();

        // Handle cancellation
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                flyout.Dispatcher.Invoke(() =>
                {
                    if (flyout.IsOpen)
                    {
                        flyout.IsOpen = false;
                    }
                });

                tcs.TrySetCanceled(cancellationToken);
            });
        }

        // Handle flyout closed
        void OnClosed(
            object? sender,
            RoutedEventArgs e)
        {
            flyout.Closed -= OnClosed;

            // Remove from stack
            RemoveFlyoutFromStack(flyout);

            // Remove from host
            RemoveFlyoutFromHost(flyout);

            // Complete the task
            tcs.TrySetResult(flyout.Result);
        }

        flyout.Closed += OnClosed;

        // Add to host and show
        AddFlyoutToHost(flyout);
        openFlyouts.Push((flyout, tcs));
        flyout.IsOpen = true;

        return tcs.Task;
    }

    private void AddFlyoutToHost(Flyout flyout)
    {
        if (flyoutHost is not null)
        {
            flyoutHost.Items.Add(flyout);
        }
        else if (hostPanel is not null)
        {
            hostPanel.Children.Add(flyout);
        }
    }

    private void RemoveFlyoutFromHost(Flyout flyout)
    {
        if (flyoutHost is not null)
        {
            flyoutHost.Items.Remove(flyout);
        }
        else if (hostPanel is not null)
        {
            hostPanel.Children.Remove(flyout);
        }
    }

    private void RemoveFlyoutFromStack(Flyout flyout)
    {
        var tempStack = new Stack<(Flyout, TaskCompletionSource<object?>)>();

        while (openFlyouts.Count > 0)
        {
            var item = openFlyouts.Pop();
            if (item.Flyout == flyout)
            {
                break;
            }

            tempStack.Push(item);
        }

        while (tempStack.Count > 0)
        {
            openFlyouts.Push(tempStack.Pop());
        }
    }
}