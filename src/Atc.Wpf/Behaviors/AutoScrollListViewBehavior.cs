namespace Atc.Wpf.Behaviors;

/// <summary>
/// A behavior that automatically scrolls a <see cref="ListView"/> to its first or last item when the items collection changes.
/// </summary>
/// <remarks>
/// The behavior scrolls once per frame, however many changes arrive: the first change posts one scroll to the
/// dispatcher at <see cref="DispatcherPriority.Background"/>, so it runs after the window has drawn the changes.
/// Adding thousands of items one by one therefore costs one layout pass, not one per item.
/// </remarks>
public sealed partial class AutoScrollListViewBehavior : Behavior<ListView>
{
    /// <summary>
    /// Gets or sets the direction to scroll to after items change (default is <see cref="ScrollDirectionType.Bottom"/>).
    /// </summary>
    [DependencyProperty(DefaultValue = ScrollDirectionType.Bottom)]
    private ScrollDirectionType scrollDirection;

    private bool isScrollPending;

    /// <summary>
    /// Gets or sets a value indicating whether auto-scrolling is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isEnabled;

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