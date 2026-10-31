namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The one owner of the filter of a collection view that <see cref="DataGridFilter"/> grids filter: grids that share
/// an <c>ItemsSource</c> share its default view, so their filters are combined here (AND) with the consumer's own
/// view filter, and only this class assigns or refreshes the view filter.
/// </summary>
/// <remarks>
/// The owner lives as long as its predicate is installed on the view (the view's filter delegate points at it) and
/// holds the grids' filter states weakly, so a long-lived view does not keep a dropped grid alive; without any live
/// grid only the consumer's own filter applies.
/// </remarks>
internal sealed class DataGridViewFilterOwner
{
    private static readonly ConditionalWeakTable<ICollectionView, DataGridViewFilterOwner> Owners = new();

    private readonly ICollectionView view;
    private readonly List<WeakReference<DataGridFilterState>> participants = [];
    private readonly Predicate<object> predicate;
    private Predicate<object>? consumerFilter;
    private bool isInstalled;

    private DataGridViewFilterOwner(ICollectionView view)
    {
        this.view = view;
        predicate = Evaluate;
    }

    /// <summary>
    /// Returns the owner of a view, or <see langword="null"/> when no grid filters it.
    /// </summary>
    public static DataGridViewFilterOwner? Find(ICollectionView? view)
        => view is not null && Owners.TryGetValue(view, out var owner)
            ? owner
            : null;

    /// <summary>
    /// Returns the owner of a view, creating it when no grid filters the view yet.
    /// </summary>
    public static DataGridViewFilterOwner GetOrCreate(ICollectionView view)
        => Owners.GetValue(view, static x => new DataGridViewFilterOwner(x));

    /// <summary>
    /// Gets the consumer's own filter of the view (the one the owner ANDs), or the view's current filter when the
    /// owner's predicate is not installed.
    /// </summary>
    public Predicate<object>? ConsumerFilter
        => ReferenceEquals(view.Filter, predicate)
            ? consumerFilter
            : view.Filter;

    /// <summary>
    /// Adds a grid's filter state, combines the filters and refreshes the view once.
    /// </summary>
    public void Apply(DataGridFilterState state)
    {
        Attach(state);
        Install(refresh: true);
    }

    /// <summary>
    /// Adds a grid's filter state and installs the predicate on a view whose refresh is deferred (assigning the
    /// filter marks the view for the one refresh that ends the deferral).
    /// </summary>
    public void ApplyDeferred(DataGridFilterState state)
    {
        Attach(state);
        Install(refresh: false);
    }

    /// <summary>
    /// Removes a grid's filter state: the remaining grids' filters are applied, or, when none is left, the view gets
    /// the consumer's own filter back.
    /// </summary>
    public void Detach(DataGridFilterState state)
    {
        var removed = participants.RemoveAll(x => !x.TryGetTarget(out var target) || ReferenceEquals(target, state)) > 0;
        if (participants.Count > 0)
        {
            if (removed && isInstalled)
            {
                Install(refresh: true);
            }

            return;
        }

        if (isInstalled && ReferenceEquals(view.Filter, predicate))
        {
            view.Filter = consumerFilter;
        }

        consumerFilter = null;
        isInstalled = false;
        Owners.Remove(view);
    }

    /// <summary>
    /// Returns whether an item passes the consumer's filter and the filters of every grid except
    /// <paramref name="except"/>.
    /// </summary>
    public bool PassesOthers(
        object item,
        DataGridFilterState? except)
    {
        if (ConsumerFilter is { } consumer && !consumer(item))
        {
            return false;
        }

        foreach (var reference in participants)
        {
            if (reference.TryGetTarget(out var state) &&
                !ReferenceEquals(state, except) &&
                !state.Passes(item))
            {
                return false;
            }
        }

        return true;
    }

    private bool Evaluate(object item)
        => PassesOthers(item, except: null);

    private void Attach(DataGridFilterState state)
    {
        participants.RemoveAll(x => !x.TryGetTarget(out _));
        if (!participants.Exists(x => x.TryGetTarget(out var target) && ReferenceEquals(target, state)))
        {
            participants.Add(new WeakReference<DataGridFilterState>(state));
        }
    }

    private void Install(bool refresh)
    {
        if (isInstalled && ReferenceEquals(view.Filter, predicate))
        {
            if (refresh)
            {
                view.Refresh();
            }
            else
            {
                view.Filter = predicate;
            }

            return;
        }

        // First install, or the consumer replaced the filter since: keep theirs and AND it. The owner's own
        // predicate is never adopted (there is one owner per view), so filters cannot call each other in a loop.
        consumerFilter = view.Filter;
        isInstalled = true;
        view.Filter = predicate;
    }
}