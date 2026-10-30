namespace Atc.Wpf.Behaviors;

/// <summary>
/// A behavior that automatically scrolls a <see cref="ListView"/> to its first or last item when the items collection changes.
/// </summary>
/// <remarks>
/// The behavior scrolls once per frame, however many changes arrive: the first change posts one scroll to the
/// dispatcher at <see cref="DispatcherPriority.Background"/>, so it runs after the window has drawn the changes.
/// Adding thousands of items one by one therefore costs one layout pass, not one per item.
/// </remarks>
public sealed class AutoScrollListViewBehavior : Behavior<ListView>
{
    /// <summary>
    /// Identifies the <see cref="ScrollDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollDirectionProperty =
        DependencyProperty.Register(
            nameof(ScrollDirection),
            typeof(ScrollDirectionType),
            typeof(AutoScrollListViewBehavior),
            new PropertyMetadata(ScrollDirectionType.Bottom));

    /// <summary>
    /// Gets or sets the direction to scroll to after items change (default is <see cref="ScrollDirectionType.Bottom"/>).
    /// </summary>
    public ScrollDirectionType ScrollDirection
    {
        get => (ScrollDirectionType)GetValue(ScrollDirectionProperty);
        set => SetValue(ScrollDirectionProperty, value);
    }

    private bool isScrollPending;

    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(AutoScrollListViewBehavior),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value indicating whether auto-scrolling is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        var items = AssociatedObject.Items;
        ((INotifyCollectionChanged)items).CollectionChanged += OnCollectionChanged;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        var items = AssociatedObject.Items;
        ((INotifyCollectionChanged)items).CollectionChanged -= OnCollectionChanged;
        base.OnDetaching();
    }

    private void OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!IsEnabled || isScrollPending)
        {
            return;
        }

        isScrollPending = true;
        _ = AssociatedObject.Dispatcher.BeginInvoke(DispatcherPriority.Background, ScrollToTarget);
    }

    private void ScrollToTarget()
    {
        isScrollPending = false;

        var listView = AssociatedObject;
        if (listView is null ||
            !IsEnabled ||
            listView.Items.Count <= 1)
        {
            return;
        }

        var targetItem = ScrollDirection == ScrollDirectionType.Bottom
            ? listView.Items[^1]!
            : listView.Items[0]!;

        listView.ScrollIntoView(targetItem);
    }
}