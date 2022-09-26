namespace Atc.Wpf.Collections;

/// <inheritdoc />
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
    /// Adds the range.
    /// </summary>
    /// <param name="list">The list.</param>
    public void AddRange(
        IEnumerable<T> list)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        suppressNotification = true;
        foreach (var item in list)
        {
            Add(item);
        }

        suppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
    }

    /// <inheritdoc />
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