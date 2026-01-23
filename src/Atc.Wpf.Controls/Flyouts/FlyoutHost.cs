namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// A container control that manages multiple flyouts with support for nesting.
/// Place this control at the root level of your window/page to enable flyout functionality.
/// </summary>
[ContentProperty(nameof(Items))]
public partial class FlyoutHost : ItemsControl
{
    private readonly Stack<Flyout> openFlyouts = new();

    /// <summary>
    /// Gets or sets the maximum nesting depth allowed.
    /// </summary>
    [DependencyProperty(DefaultValue = 5)]
    private int maxNestingDepth;

    /// <summary>
    /// Gets the number of currently open flyouts.
    /// </summary>
    [DependencyProperty(DefaultValue = 0)]
    private int openFlyoutCount;

    /// <summary>
    /// Gets whether any flyout is currently open.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isAnyFlyoutOpen;

    static FlyoutHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FlyoutHost),
            new FrameworkPropertyMetadata(typeof(FlyoutHost)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutHost"/> class.
    /// </summary>
    public FlyoutHost()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets the stack of currently open flyouts.
    /// </summary>
    public IReadOnlyCollection<Flyout> OpenFlyouts => openFlyouts;

    /// <summary>
    /// Gets the topmost open flyout, if any.
    /// </summary>
    public Flyout? TopFlyout
        => openFlyouts.Count > 0 ? openFlyouts.Peek() : null;

    /// <summary>
    /// Opens the specified flyout.
    /// </summary>
    /// <param name="flyout">The flyout to open.</param>
    /// <returns>True if the flyout was opened; otherwise, false.</returns>
    public bool OpenFlyout(Flyout flyout)
    {
        ArgumentNullException.ThrowIfNull(flyout);

        if (openFlyouts.Count >= MaxNestingDepth)
        {
            return false;
        }

        if (openFlyouts.Contains(flyout))
        {
            return false;
        }

        // Subscribe to closing event
        flyout.Closed += OnFlyoutClosed;

        openFlyouts.Push(flyout);
        UpdateState();

        flyout.IsOpen = true;
        return true;
    }

    /// <summary>
    /// Closes the topmost flyout.
    /// </summary>
    /// <returns>True if a flyout was closed; otherwise, false.</returns>
    public bool CloseTopFlyout()
    {
        if (openFlyouts.Count == 0)
        {
            return false;
        }

        var topFlyout = openFlyouts.Peek();
        topFlyout.IsOpen = false;
        return true;
    }

    /// <summary>
    /// Closes all open flyouts.
    /// </summary>
    public void CloseAllFlyouts()
    {
        while (openFlyouts.Count > 0)
        {
            var flyout = openFlyouts.Peek();
            flyout.IsOpen = false;
        }
    }

    /// <summary>
    /// Closes a specific flyout and all flyouts opened after it.
    /// </summary>
    /// <param name="flyout">The flyout to close.</param>
    public void CloseFlyoutAndDescendants(Flyout flyout)
    {
        ArgumentNullException.ThrowIfNull(flyout);

        var flyoutsToClose = new List<Flyout>();
        foreach (var openFlyout in openFlyouts)
        {
            flyoutsToClose.Add(openFlyout);
            if (openFlyout == flyout)
            {
                break;
            }
        }

        foreach (var flyoutToClose in flyoutsToClose)
        {
            flyoutToClose.IsOpen = false;
        }
    }

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is Flyout;

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new Flyout();

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        // Subscribe to child flyout events
        foreach (var item in Items)
        {
            if (item is Flyout flyout)
            {
                SubscribeToFlyout(flyout);
            }
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        // Unsubscribe from child flyout events
        foreach (var item in Items)
        {
            if (item is Flyout flyout)
            {
                UnsubscribeFromFlyout(flyout);
            }
        }

        openFlyouts.Clear();
        UpdateState();
    }

    private void SubscribeToFlyout(Flyout flyout)
    {
        flyout.Opening += OnFlyoutOpening;
        flyout.Closed += OnFlyoutClosed;
    }

    private void UnsubscribeFromFlyout(Flyout flyout)
    {
        flyout.Opening -= OnFlyoutOpening;
        flyout.Closed -= OnFlyoutClosed;
    }

    private void OnFlyoutOpening(
        object? sender,
        FlyoutOpeningEventArgs e)
    {
        if (sender is not Flyout flyout)
        {
            return;
        }

        // Check nesting depth
        if (openFlyouts.Count >= MaxNestingDepth && !openFlyouts.Contains(flyout))
        {
            e.Cancel = true;
            return;
        }

        // Add to stack if not already present
        if (!openFlyouts.Contains(flyout))
        {
            openFlyouts.Push(flyout);
            UpdateState();
        }
    }

    private void OnFlyoutClosed(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is not Flyout flyout)
        {
            return;
        }

        // Remove from stack
        var tempStack = new Stack<Flyout>();
        while (openFlyouts.Count > 0)
        {
            var current = openFlyouts.Pop();
            if (current == flyout)
            {
                break;
            }

            tempStack.Push(current);
        }

        // Restore remaining flyouts (though typically closed flyout should be on top)
        while (tempStack.Count > 0)
        {
            openFlyouts.Push(tempStack.Pop());
        }

        UpdateState();
    }

    private void UpdateState()
    {
        OpenFlyoutCount = openFlyouts.Count;
        IsAnyFlyoutOpen = openFlyouts.Count > 0;
    }
}