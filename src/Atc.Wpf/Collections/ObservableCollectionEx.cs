namespace Atc.Wpf.Collections;

/// <summary>
/// Extends ObservableCollection&lt;T&gt; to provide enhanced features such as the ability to suppress collection changed notifications
/// and efficiently add multiple items with a single notification. This is useful in scenarios where bulk updates are needed,
/// improving performance by minimizing UI updates and providing a more efficient way to refresh bound views.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public class ObservableCollectionEx<T> : ObservableCollection<T>
{
    private bool suppressNotification;

    /// <summary>
    /// Override the event so this class can access it
    /// </summary>
    public override event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    ///   Gets or sets a value indicating whether set this property to suppress change notifications.
    /// </summary>
    public bool SuppressOnChangedNotification
    {
        get => suppressNotification;

        set
        {
            if (suppressNotification == value)
            {
                return;
            }

            suppressNotification = value;
            if (!suppressNotification)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }

    /// <summary>
    /// Adds a range of items to the collection. This method temporarily suppresses collection changed notifications,
    /// allowing for the bulk addition of items without triggering individual UI updates for each addition.
    /// A single collection changed notification is raised after all items are added, improving performance and responsiveness
    /// when updating bound views with multiple items at once.
    /// </summary>
    /// <param name="list">The collection of items to be added to the ObservableCollectionEx. The items in this collection
    /// will be added to the ObservableCollectionEx, and a single notification will be raised after all items are added.</param>
    public void AddRange(
        IEnumerable<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        suppressNotification = true;
        foreach (var item in list)
        {
            Add(item);
        }

        suppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
    }

    /// <summary>
    /// Forces a refresh of any views bound to this collection. This method raises a collection changed notification
    /// with the Reset action, indicating to bound views that they should re-evaluate the entire collection. This is
    /// particularly useful for manually triggering UI updates in response to changes in the collection that are not
    /// automatically captured by the standard collection changed notifications.
    /// </summary>
    /// <remarks>
    /// When the <see cref="Refresh"/> method is called, it triggers a <see cref="NotifyCollectionChangedAction.Reset"/>
    /// notification on the collection. This is the equivalent of telling any bound views that they should discard their
    /// current state and re-evaluate the entire collection. This can be useful in scenarios where the collection has been
    /// modified in a way that is not automatically detectable by the bound views (e.g., when properties of items within
    /// the collection have changed without modifying the collection itself).
    /// It's important to use this method judiciously as it can be resource-intensive in scenarios where the bound view is
    /// large or complex, since it effectively requires the view to be re-rendered from scratch.
    /// </remarks>
    public void Refresh()
    {
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Forces a refresh of any views bound to this collection. This method raises a collection changed notification
    /// with the Reset action, indicating to bound views that they should re-evaluate the entire collection. This is
    /// particularly useful for manually triggering UI updates in response to changes in the collection that are not
    /// automatically captured by the standard collection changed notifications.
    /// </summary>
    /// <param name="e">The NotifyCollectionChanged EventArgs.</param>
    protected override void OnCollectionChanged(
        NotifyCollectionChangedEventArgs e)
    {
        // SuppressNotification is true during AddRange method
        if (suppressNotification)
        {
            return;
        }

        // ReSharper disable once CommentTypo
        // Be nice - use BlockReentrancy like MSDN said
        using (BlockReentrancy())
        {
            var eventHandler = CollectionChanged;
            if (eventHandler is null)
            {
                return;
            }

            var delegates = eventHandler.GetInvocationList();

            // Walk through invocation list
            foreach (var o in delegates)
            {
                var handler = (NotifyCollectionChangedEventHandler)o;

                // If the subscriber is a DispatcherObject and different thread
                switch (handler.Target)
                {
                    case DispatcherObject dispatcherObject when !dispatcherObject.CheckAccess():
                        dispatcherObject.Dispatcher?.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        break;
                    case CollectionView target:
                        target.Refresh();
                        break;
                    default:
                        handler(this, e);
                        break;
                }
            }
        }
    }
}