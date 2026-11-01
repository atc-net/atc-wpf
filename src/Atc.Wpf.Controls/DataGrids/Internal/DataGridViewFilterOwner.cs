namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The one owner of the filter of a collection view that <see cref="DataGridFilter"/> grids filter: grids that share
/// an <c>ItemsSource</c> share its default view, so their filters are combined here (AND) with the consumer's own
/// view filter, and only this class assigns or refreshes the view filter.
/// </summary>
/// <remarks>
/// <para>
/// A <see cref="BindingListCollectionView"/> of a <see cref="System.Data.DataView"/> cannot filter with a predicate:
/// there the owner writes the combined filters as its <see cref="BindingListCollectionView.CustomFilter"/> (the
/// view's <c>RowFilter</c>), ANDed with the consumer's own row filter.
/// </para>
/// <para>
/// The owner lives as long as its predicate is installed on the view (the view's filter delegate points at it) and
/// holds the grids' filter states weakly, so a long-lived view does not keep a dropped grid alive; without any live
/// grid only the consumer's own filter applies.
/// </para>
/// </remarks>
internal sealed class DataGridViewFilterOwner
{
    private static readonly ConditionalWeakTable<ICollectionView, DataGridViewFilterOwner> Owners = new();

    private readonly ICollectionView view;
    private readonly List<WeakReference<DataGridFilterState>> participants = [];
    private readonly Predicate<object> predicate;
    private readonly System.Data.DataView? dataView;
    private Predicate<object>? consumerFilter;
    private string? consumerRowFilter;
    private string? installedRowFilter;
    private bool isInstalled;

    private DataGridViewFilterOwner(ICollectionView view)
    {
        this.view = view;
        predicate = Evaluate;
        dataView = DataGridRowFilterBuilder.GetDataView(view);
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
    /// Gets the view the owner filters.
    /// </summary>
    public ICollectionView View => view;

    /// <summary>
    /// Gets the consumer's own filter of the view (the one the owner ANDs), or the view's current filter when the
    /// owner's predicate is not installed.
    /// </summary>
    public Predicate<object>? ConsumerFilter
        => dataView is not null
            ? null
            : ReferenceEquals(view.Filter, predicate)
                ? consumerFilter
                : view.Filter;

    /// <summary>
    /// Gets the consumer's own row filter of a <see cref="System.Data.DataView"/> source while the owner's filter is
    /// installed, else <see langword="null"/>.
    /// </summary>
    public string? ConsumerRowFilter
        => dataView is not null && isInstalled ? consumerRowFilter : null;

    /// <summary>
    /// Gets a value indicating whether the owner's row filter is installed on a <see cref="System.Data.DataView"/>
    /// source.
    /// </summary>
    public bool IsRowFilterInstalled
        => dataView is not null && isInstalled;

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

        if (dataView is not null)
        {
            var listView = (BindingListCollectionView)view;
            if (isInstalled && string.Equals(listView.CustomFilter, installedRowFilter, StringComparison.Ordinal))
            {
                listView.CustomFilter = consumerRowFilter;
            }
        }
        else if (isInstalled && ReferenceEquals(view.Filter, predicate))
        {
            view.Filter = consumerFilter;
        }

        consumerFilter = null;
        consumerRowFilter = null;
        installedRowFilter = null;
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
        if (dataView is not null)
        {
            InstallRowFilter();
            return;
        }

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

    private void InstallRowFilter()
    {
        // Assigning the custom filter refreshes the view, or marks a deferred view for its one refresh.
        var listView = (BindingListCollectionView)view;
        var current = listView.CustomFilter;
        if (!isInstalled)
        {
            consumerRowFilter = string.IsNullOrEmpty(current)
                ? NullIfEmpty(dataView!.RowFilter)
                : current;
        }
        else if (!string.Equals(current, installedRowFilter, StringComparison.Ordinal))
        {
            // The consumer replaced the filter since: keep theirs and AND it.
            consumerRowFilter = NullIfEmpty(current);
        }

        var table = dataView!.Table!;
        var expressions = new List<string?> { consumerRowFilter };
        participants.RemoveAll(x => !x.TryGetTarget(out _));
        foreach (var reference in participants)
        {
            if (reference.TryGetTarget(out var state))
            {
                expressions.Add(state.BuildRowFilter(table));
            }
        }

        installedRowFilter = DataGridRowFilterBuilder.And(expressions);
        isInstalled = true;
        listView.CustomFilter = installedRowFilter;
    }

    private static string? NullIfEmpty(string? text)
        => string.IsNullOrEmpty(text) ? null : text;
}