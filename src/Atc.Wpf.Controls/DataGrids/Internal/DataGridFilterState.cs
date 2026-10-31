namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The filter state of one <see cref="DataGrid"/>: the column filters, the search text, the composed view filter and
/// the open pop-up. It is stored on the grid (a private attached property), so it lives exactly as long as the grid.
/// </summary>
/// <remarks>
/// The view filter itself belongs to the view's <see cref="DataGridViewFilterOwner"/>, which combines the filters of
/// every grid on the view with the consumer's own filter and holds this state weakly.
/// </remarks>
internal sealed class DataGridFilterState
{
    private readonly DataGrid grid;
    private readonly Dictionary<string, DataGridColumnFilter> filters = new(StringComparer.Ordinal);
    private readonly List<PendingViewWork> pendingWork = [];
    private ICollectionView? view;
    private DataGridViewFilterOwner? owner;
    private string appliedSearchText = string.Empty;
    private DispatcherTimer? searchTimer;
    private List<DataGridColumnFilterMatcher> matchers = [];
    private List<DataGridColumnValueAccessor> searchAccessors = [];
    private Popup? popup;
    private DataGridColumn? popupColumn;
    private DataGridFilterPopupContent? popupContent;
    private bool isReapplyQueued;
    private DispatcherTimer? retryTimer;

    public DataGridFilterState(DataGrid grid)
    {
        this.grid = grid;
        view = GetView(grid.ItemsSource);

        grid.Columns.CollectionChanged += OnColumnsChanged;
        grid.Unloaded += OnGridUnloaded;
        grid.PreviewKeyDown += OnGridPreviewKeyDown;
        grid.ContextMenuOpening += OnGridContextMenuOpening;
        grid.RowEditEnding += OnGridRowEditEnding;
        UpdateActiveFlags();
    }

    /// <summary>
    /// Gets the content of the open filter pop-up, or <see langword="null"/>.
    /// </summary>
    public DataGridFilterPopupContent? PopupContent
        => popup is { IsOpen: true } ? popupContent : null;

    /// <summary>
    /// Gets the column of the open filter pop-up, or <see langword="null"/>.
    /// </summary>
    public DataGridColumn? PopupColumn
        => popup is { IsOpen: true } ? popupColumn : null;

    /// <summary>
    /// Gets the open pop-up, or <see langword="null"/>.
    /// </summary>
    public Popup? Popup => popup is { IsOpen: true } ? popup : null;

    /// <summary>
    /// Gets a value indicating whether a search is pending (debounced).
    /// </summary>
    public bool IsSearchPending => searchTimer is { IsEnabled: true };

    /// <summary>
    /// Gets a value indicating whether filter work waits for an edit or add transaction of the view to end.
    /// </summary>
    public bool IsApplyPending => pendingWork.Count > 0;

    private bool HasAnything
        => filters.Count > 0 || appliedSearchText.Length > 0;

    /// <summary>
    /// Returns the active column filters, in the order they were set.
    /// </summary>
    public IReadOnlyList<DataGridColumnFilter> GetFilters()
        => filters.Values.ToList();

    /// <summary>
    /// Sets or removes (<paramref name="filter"/> is <see langword="null"/>) the filter of a column and applies.
    /// </summary>
    public void SetFilter(
        string columnKey,
        DataGridColumnFilter? filter)
    {
        if (filter is not null && DataGridColumnFilterMatcher.IsActive(filter))
        {
            filters[columnKey] = filter.ColumnKey == columnKey
                ? filter
                : CopyWithKey(filter, columnKey);
        }
        else if (!filters.Remove(columnKey))
        {
            return;
        }

        Apply();
    }

    /// <summary>
    /// Removes every column filter and applies once.
    /// </summary>
    public void ClearFilters()
    {
        if (filters.Count == 0)
        {
            return;
        }

        filters.Clear();
        Apply();
    }

    /// <summary>
    /// Drops the column filters when the column filter buttons are switched off: without a search the view gets
    /// the consumer's own filter back, else the search alone is applied.
    /// </summary>
    public void DisableColumnFilters()
    {
        ClosePopup();
        if (filters.Count == 0)
        {
            return;
        }

        filters.Clear();
        if (appliedSearchText.Length > 0)
        {
            Apply(commitEdit: false);
            return;
        }

        UpdateActiveFlags();
        RestoreConsumerFilter();
        RaiseFilterChanged();
    }

    /// <summary>
    /// Replaces all column filters (layout load) without applying.
    /// </summary>
    public void ReplaceFilters(IEnumerable<DataGridColumnFilter> newFilters)
    {
        filters.Clear();
        foreach (var filter in newFilters.Where(DataGridColumnFilterMatcher.IsActive))
        {
            filters[filter.ColumnKey] = filter;
        }
    }

    /// <summary>
    /// Translates the texts of a values filter formatted in <paramref name="fromCulture"/> to the grid's culture,
    /// through the rows the grid has (a text no row formats to stays as it is). Other filters are returned as they are.
    /// </summary>
    public DataGridColumnFilter TranslateValues(
        DataGridColumn column,
        DataGridColumnFilter filter,
        CultureInfo fromCulture)
    {
        if (filter.Kind != DataGridColumnFilterKind.Values || filter.Values.Count == 0)
        {
            return filter;
        }

        var to = DataGridColumnValueAccessor.Create(column, DataGridFilter.GetGridCulture(grid));
        var from = DataGridColumnValueAccessor.Create(column, fromCulture);
        if (to is null || from is null)
        {
            return filter;
        }

        var wanted = new HashSet<string>(filter.Values, StringComparer.Ordinal);
        var translated = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in GetSourceItems())
        {
            if (item is null)
            {
                continue;
            }

            var raw = to.GetValue(item);
            var saved = from.Format(raw);
            if (wanted.Contains(saved))
            {
                translated.TryAdd(saved, to.Format(raw));
                if (translated.Count == wanted.Count)
                {
                    break;
                }
            }
        }

        return translated.Count == 0
            ? filter
            : DataGridColumnFilter.ForValues(
                filter.ColumnKey,
                filter.Values
                    .Select(x => translated.GetValueOrDefault(x, x))
                    .Distinct(StringComparer.Ordinal));
    }

    /// <summary>
    /// Takes a new search text: applies it after the grid's search delay (or at once for a zero delay).
    /// </summary>
    public void OnSearchTextChanged()
    {
        var delay = DataGridFilter.GetSearchDelay(grid);
        if (delay <= TimeSpan.Zero)
        {
            ApplySearchNow();
            return;
        }

        searchTimer ??= CreateSearchTimer();
        searchTimer.Stop();
        searchTimer.Interval = delay;
        searchTimer.Start();
    }

    /// <summary>
    /// Applies a pending (debounced) search now.
    /// </summary>
    public void ApplySearchNow()
    {
        searchTimer?.Stop();
        var text = (DataGridFilter.GetSearchText(grid) ?? string.Empty).Trim();
        if (string.Equals(text, appliedSearchText, StringComparison.Ordinal))
        {
            return;
        }

        appliedSearchText = text;

        // A search typed elsewhere (or a debounce tick) does not commit a half-typed row: it waits for the edit.
        Apply(commitEdit: false);
    }

    /// <summary>
    /// Re-applies the search after a column was shown or hidden (the search covers the visible columns; the column
    /// filters do not depend on visibility, so without a search nothing is refreshed).
    /// </summary>
    public void ReapplySearch()
    {
        if (appliedSearchText.Length > 0)
        {
            Apply();
        }
    }

    /// <summary>
    /// Installs the filter on a view whose refresh is deferred (layout load): assigning the filter marks the view
    /// for the one refresh that ends the deferral. Call <see cref="RaiseFilterChanged"/> after the deferral.
    /// </summary>
    public void ApplyToDeferredView()
    {
        RebuildMatchers();
        UpdateActiveFlags();

        var currentView = EnsureView();
        if (!IsFilterable(currentView))
        {
            return;
        }

        if (HasAnything)
        {
            owner = DataGridViewFilterOwner.GetOrCreate(currentView!);
            owner.ApplyDeferred(this);
        }
        else
        {
            DetachFromOwner();
        }
    }

    /// <summary>
    /// Builds the filter of the view and refreshes it once, then raises <c>FilterChanged</c>. A row the grid is
    /// editing is committed first; while the view is still in an edit or add transaction the refresh waits for it.
    /// </summary>
    public void Apply()
        => Apply(commitEdit: true);

    /// <summary>
    /// Runs work that filters, sorts or refreshes the grid's view (layout load) when the view allows it: a row the
    /// grid is editing is committed first, and while the view is still in an edit or add transaction the work waits
    /// for it (a later call with the same <paramref name="key"/> replaces it).
    /// </summary>
    public void RunViewWork(
        object key,
        Action work)
        => RunWhenIdle(key, EnsureView(), commitEdit: true, work);

    private void Apply(bool commitEdit)
    {
        RebuildMatchers();
        UpdateActiveFlags();
        RunWhenIdle(this, EnsureView(), commitEdit, ApplyToView);
    }

    private void ApplyToView()
    {
        var currentView = EnsureView();
        if (IsFilterable(currentView))
        {
            if (HasAnything)
            {
                owner = DataGridViewFilterOwner.GetOrCreate(currentView!);
                owner.Apply(this);
            }
            else
            {
                DetachFromOwner();
            }
        }

        RaiseFilterChanged();
    }

    /// <summary>
    /// Returns the distinct formatted values of a column over the rows that pass every other filter (the other
    /// columns, the search and the consumer's filter), sorted by their raw value, with blanks last.
    /// </summary>
    public IReadOnlyList<DataGridFilterValue> GetDistinctValues(
        DataGridColumn column,
        out DataGridFilterValueKind kind)
        => GetDistinctValues(column, allTexts: null, out kind);

    /// <summary>
    /// Returns the distinct formatted values of a column over the rows that pass every other filter, and collects
    /// into <paramref name="allTexts"/> the formatted values of every row, filtered or not.
    /// </summary>
    private List<DataGridFilterValue> GetDistinctValues(
        DataGridColumn column,
        HashSet<string>? allTexts,
        out DataGridFilterValueKind kind)
    {
        kind = DataGridFilterValueKind.Text;
        var accessor = DataGridColumnValueAccessor.Create(column, DataGridFilter.GetGridCulture(grid));
        if (accessor is null)
        {
            return [];
        }

        RebuildMatchers();
        var others = matchers.Where(x => !ReferenceEquals(x.Column, column)).ToArray();
        var currentView = EnsureView();
        var viewOwner = DataGridViewFilterOwner.Find(currentView);
        var consumer = viewOwner is null
            ? currentView?.Filter
            : item => viewOwner.PassesOthers(item, this);
        var firstRawByText = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var item in GetSourceItems())
        {
            if (item is null)
            {
                continue;
            }

            if (allTexts is null)
            {
                if (!PassesOthers(item, consumer, others))
                {
                    continue;
                }

                var raw = accessor.GetValue(item);
                firstRawByText.TryAdd(accessor.Format(raw), raw);
            }
            else
            {
                var raw = accessor.GetValue(item);
                var text = accessor.Format(raw);
                allTexts.Add(text);
                if (PassesOthers(item, consumer, others))
                {
                    firstRawByText.TryAdd(text, raw);
                }
            }
        }

        kind = GetValueKind(firstRawByText.Values);
        return SortValues(firstRawByText, DataGridFilter.GetGridCulture(grid));
    }

    /// <summary>
    /// Returns whether the row item passes this grid's column filters and search (the view's owner ANDs the
    /// consumer's filter and the other grids' filters).
    /// </summary>
    public bool Passes(object item)
        => PassesOthers(item, consumer: null, matchers);

    /// <summary>
    /// Returns this grid's column filters and search as a row filter expression of a
    /// <see cref="System.Data.DataTable"/> source, or <see langword="null"/> when they let every row pass.
    /// </summary>
    public string? BuildRowFilter(System.Data.DataTable table)
    {
        var parts = new List<string?>(matchers.Count + 1);
        foreach (var matcher in matchers)
        {
            if (DataGridRowFilterBuilder.GetColumn(table, matcher.Accessor) is { } dataColumn)
            {
                parts.Add(DataGridRowFilterBuilder.BuildColumnExpression(dataColumn, matcher.MatchesValue));
            }
        }

        if (appliedSearchText.Length > 0)
        {
            parts.Add(BuildSearchRowFilter(table));
        }

        return DataGridRowFilterBuilder.And(parts);
    }

    /// <summary>
    /// Returns whether the grid's source can apply a filter of the column: any column of a predicate-filterable
    /// view, a column of the table of a <see cref="System.Data.DataView"/> source, and nothing of other sources.
    /// </summary>
    public bool IsColumnSupported(DataGridColumn column)
    {
        var currentView = EnsureView();
        if (currentView is null || currentView.CanFilter)
        {
            return true;
        }

        if (DataGridRowFilterBuilder.GetDataView(currentView) is not { Table: { } table })
        {
            return false;
        }

        var accessor = DataGridColumnValueAccessor.Create(column, DataGridFilter.GetGridCulture(grid));
        return accessor is not null && DataGridRowFilterBuilder.GetColumn(table, accessor) is not null;
    }

    /// <summary>
    /// Opens the filter pop-up of a column under <paramref name="placementTarget"/> (the header button, or the grid).
    /// </summary>
    public void OpenPopup(
        DataGridColumn column,
        UIElement? placementTarget)
    {
        ClosePopup();

        var key = DataGridFilter.ResolveColumnKey(column);
        var allTexts = new HashSet<string>(StringComparer.Ordinal);
        var values = GetDistinctValues(column, allTexts, out var kind);
        var current = filters.GetValueOrDefault(key);
        var content = new DataGridFilterPopupContent();
        content.Load(key, values, kind, current, DataGridFilter.GetGridCulture(grid));
        content.SetHiddenValues(GetHiddenValueStates(values, allTexts, current));
        AutomationProperties.SetName(
            content,
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.FilterColumn1, DataGridFilter.GetColumnTitle(column)));

        content.OkRequested += (_, _) =>
        {
            var filter = content.BuildFilter();
            ClosePopup();
            SetFilter(key, filter);
        };
        content.CancelRequested += (_, _) => ClosePopup();
        content.ClearRequested += (_, _) =>
        {
            ClosePopup();
            SetFilter(key, filter: null);
        };

        var newPopup = new Popup
        {
            Child = content,
            PlacementTarget = placementTarget ?? grid,
            Placement = placementTarget is null ? PlacementMode.Relative : PlacementMode.Bottom,
            StaysOpen = false,
            AllowsTransparency = false,
            Focusable = false,
        };
        newPopup.Closed += OnPopupClosed;

        popup = newPopup;
        popupColumn = column;
        popupContent = content;
        newPopup.IsOpen = true;

        _ = content.Dispatcher.BeginInvoke(DispatcherPriority.Input, content.FocusFirst);
        if (placementTarget is not null && UIElementAutomationPeer.FromElement(placementTarget) is { } peer)
        {
            peer.ResetChildrenCache();
        }
    }

    /// <summary>
    /// Closes the filter pop-up if it is open.
    /// </summary>
    public void ClosePopup()
    {
        if (popup is { IsOpen: true } open)
        {
            open.IsOpen = false;
        }
    }

    /// <summary>
    /// Stops the search timer and closes the pop-up (the grid was unloaded or the behaviour switched off).
    /// </summary>
    public void Stop()
    {
        searchTimer?.Stop();
        ClosePopup();
    }

    /// <summary>
    /// Takes the new <c>ItemsSource</c>: gives the old view its own filter back and applies the filters to the new one.
    /// </summary>
    public void OnItemsSourceChanged()
    {
        ClosePopup();
        var newView = GetView(grid.ItemsSource);
        if (ReferenceEquals(newView, view))
        {
            return;
        }

        RestoreConsumerFilter();
        view = newView;
        if (HasAnything)
        {
            Apply();
        }
        else
        {
            UpdateActiveFlags();
        }
    }

    /// <summary>
    /// Takes this grid's filters off the view (the behaviour is switched off, or the source changed): the other
    /// grids on the view keep theirs, and without any the view gets the consumer's own filter back.
    /// </summary>
    public void RestoreConsumerFilter()
        => DetachFromOwner();

    private static bool IsFilterable(ICollectionView? targetView)
        => targetView is not null &&
           (targetView.CanFilter || DataGridRowFilterBuilder.GetDataView(targetView) is not null);

    private string? BuildSearchRowFilter(System.Data.DataTable table)
    {
        var alternatives = new List<string>();
        foreach (var accessor in searchAccessors)
        {
            if (DataGridRowFilterBuilder.GetColumn(table, accessor) is not { } dataColumn)
            {
                continue;
            }

            var expression = DataGridRowFilterBuilder.BuildColumnExpression(
                dataColumn,
                value => accessor.Format(value).Contains(appliedSearchText, StringComparison.OrdinalIgnoreCase));
            if (expression is null)
            {
                // Every row has a match in this column.
                return null;
            }

            if (!string.Equals(expression, DataGridRowFilterBuilder.None, StringComparison.Ordinal))
            {
                alternatives.Add(expression);
            }
        }

        return alternatives.Count == 0
            ? DataGridRowFilterBuilder.None
            : string.Join(" OR ", alternatives.Select(x => $"({x})"));
    }

    private static ICollectionView? GetView(IEnumerable? itemsSource)
        => itemsSource is null
            ? null
            : CollectionViewSource.GetDefaultView(itemsSource);

    private static DataGridColumnFilter CopyWithKey(
        DataGridColumnFilter filter,
        string columnKey)
        => new()
        {
            ColumnKey = columnKey,
            Kind = filter.Kind,
            Values = filter.Values,
            TextOperator = filter.TextOperator,
            Text = filter.Text,
            Minimum = filter.Minimum,
            Maximum = filter.Maximum,
            From = filter.From,
            To = filter.To,
        };

    /// <summary>
    /// Returns how the values the pop-up does not list (their rows are hidden by other filters) pass the column's
    /// filter: they keep the state they have, so OK on the pop-up changes the listed values only.
    /// </summary>
    private static (IReadOnlyList<string> Included, bool HasExcluded) GetHiddenValueStates(
        IReadOnlyList<DataGridFilterValue> shownValues,
        HashSet<string> allTexts,
        DataGridColumnFilter? current)
    {
        var shown = new HashSet<string>(shownValues.Select(x => x.Text), StringComparer.Ordinal);
        if (current?.Kind != DataGridColumnFilterKind.Values)
        {
            // No values filter yet: every hidden value passes.
            return (allTexts.Where(x => !shown.Contains(x)).ToList(), false);
        }

        // Allowed values without rows (e.g. from a saved layout) are kept as well.
        var allowed = new HashSet<string>(current.Values, StringComparer.Ordinal);
        return (
            current.Values.Where(x => !shown.Contains(x)).Distinct(StringComparer.Ordinal).ToList(),
            allTexts.Any(x => !shown.Contains(x) && !allowed.Contains(x)));
    }

    private static DataGridFilterValueKind GetValueKind(
        IEnumerable<object?> rawValues)
    {
        var kind = (DataGridFilterValueKind?)null;
        foreach (var raw in rawValues)
        {
            if (raw is null || (raw is string s && s.Length == 0))
            {
                continue;
            }

            var rawKind = DataGridColumnFilterMatcher.ToNumber(raw) is not null
                ? DataGridFilterValueKind.Number
                : DataGridColumnFilterMatcher.ToDate(raw) is not null
                    ? DataGridFilterValueKind.Date
                    : DataGridFilterValueKind.Text;
            if (kind is null)
            {
                kind = rawKind;
            }
            else if (kind != rawKind)
            {
                return DataGridFilterValueKind.Text;
            }
        }

        return kind ?? DataGridFilterValueKind.Text;
    }

    private static List<DataGridFilterValue> SortValues(
        Dictionary<string, object?> firstRawByText,
        CultureInfo culture)
    {
        var blankText = Atc.Wpf.Resources.Miscellaneous.Blanks;
        var hasBlank = firstRawByText.Remove(string.Empty);
        var entries = firstRawByText.ToList();

        var rawType = entries.Count > 0 ? entries[0].Value?.GetType() : null;
        var comparable = rawType is not null &&
                         rawType != typeof(string) &&
                         typeof(IComparable).IsAssignableFrom(rawType) &&
                         entries.TrueForAll(x => x.Value?.GetType() == rawType);
        var textComparer = StringComparer.Create(culture, ignoreCase: true);

        entries.Sort((a, b) =>
        {
            var result = comparable
                ? ((IComparable)a.Value!).CompareTo(b.Value)
                : textComparer.Compare(a.Key, b.Key);
            return result != 0
                ? result
                : string.CompareOrdinal(a.Key, b.Key);
        });

        var values = entries
            .Select(x => new DataGridFilterValue(x.Key, x.Key))
            .ToList();
        if (hasBlank)
        {
            values.Add(new DataGridFilterValue(string.Empty, blankText));
        }

        return values;
    }

    private bool PassesOthers(
        object item,
        Predicate<object>? consumer,
        IReadOnlyList<DataGridColumnFilterMatcher> columnMatchers)
    {
        if (consumer is not null && !consumer(item))
        {
            return false;
        }

        for (var i = 0; i < columnMatchers.Count; i++)
        {
            if (!columnMatchers[i].Matches(item))
            {
                return false;
            }
        }

        return appliedSearchText.Length == 0 || MatchesSearch(item);
    }

    private bool MatchesSearch(object item)
    {
        foreach (var accessor in searchAccessors)
        {
            if (accessor.GetText(item).Contains(appliedSearchText, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void DetachFromOwner()
    {
        var current = owner;
        owner = null;
        if (current is not null)
        {
            RunWhenIdle(current, current.View, commitEdit: false, () => current.Detach(this));
        }
    }

    private static bool IsInTransaction(ICollectionView? targetView)
        => targetView is IEditableCollectionView editable &&
           (editable.IsEditingItem || editable.IsAddingNew);

    private void RunWhenIdle(
        object key,
        ICollectionView? targetView,
        bool commitEdit,
        Action work)
    {
        pendingWork.RemoveAll(x => ReferenceEquals(x.Key, key));
        if (commitEdit && IsInTransaction(targetView))
        {
            // The grid's own row edit: commit it (a failed validation keeps the row editing, and the work waits).
            grid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
        }

        if (IsInTransaction(targetView))
        {
            pendingWork.Add(new PendingViewWork(key, targetView, work));
            StartRetryTimer();
            return;
        }

        FlushPendingWork();
        work();
    }

    private void FlushPendingWork()
    {
        var runnable = pendingWork.FindAll(x => !IsInTransaction(x.View));
        pendingWork.RemoveAll(runnable.Contains);
        if (pendingWork.Count == 0)
        {
            retryTimer?.Stop();
        }

        foreach (var item in runnable)
        {
            item.Work();
        }
    }

    private void StartRetryTimer()
    {
        if (retryTimer is null)
        {
            // A transaction started in code ends without any event, so poll; the timer holds the state weakly.
            var weakState = new WeakReference<DataGridFilterState>(this);
            retryTimer = new DispatcherTimer(DispatcherPriority.Background, grid.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(200),
            };
            retryTimer.Tick += (sender, _) =>
            {
                if (weakState.TryGetTarget(out var state))
                {
                    state.FlushPendingWork();
                }
                else
                {
                    ((DispatcherTimer)sender!).Stop();
                }
            };
        }

        retryTimer.Start();
    }

    private void OnGridRowEditEnding(
        object? sender,
        DataGridRowEditEndingEventArgs e)
    {
        if (pendingWork.Count > 0)
        {
            // The transaction is still open while the event is raised: retry once it has ended.
            _ = grid.Dispatcher.BeginInvoke(DispatcherPriority.Background, FlushPendingWork);
        }
    }

    private IEnumerable GetSourceItems()
    {
        var currentView = EnsureView();
        if (DataGridRowFilterBuilder.GetDataView(currentView) is { Table: { } table } dataView)
        {
            // The data view itself holds only the rows that pass the filters: read a copy with the consumer's own.
            var consumerRowFilter = DataGridViewFilterOwner.Find(currentView) is { IsRowFilterInstalled: true } viewOwner
                ? viewOwner.ConsumerRowFilter
                : dataView.RowFilter;
            using var copy = new System.Data.DataView(table, consumerRowFilter, dataView.Sort, dataView.RowStateFilter);
            return copy.Cast<object>().ToList();
        }

        return currentView?.SourceCollection ?? grid.Items;
    }

    private ICollectionView? EnsureView()
    {
        var current = GetView(grid.ItemsSource);
        if (!ReferenceEquals(current, view))
        {
            RestoreConsumerFilter();
            view = current;
            UpdateActiveFlags();
        }

        return view;
    }

    private void RebuildMatchers()
    {
        var culture = DataGridFilter.GetGridCulture(grid);
        var newMatchers = new List<DataGridColumnFilterMatcher>(filters.Count);
        var newSearch = new List<DataGridColumnValueAccessor>();
        foreach (var column in grid.Columns)
        {
            var accessor = (DataGridColumnValueAccessor?)null;
            if (filters.TryGetValue(DataGridFilter.ResolveColumnKey(column), out var filter) &&
                IsColumnSupported(column))
            {
                accessor = DataGridColumnValueAccessor.Create(column, culture);
                if (accessor is not null)
                {
                    newMatchers.Add(new DataGridColumnFilterMatcher(column, filter, accessor));
                }
            }

            if (appliedSearchText.Length > 0 &&
                column.Visibility == Visibility.Visible &&
                column is not DataGridCheckBoxColumn)
            {
                accessor ??= DataGridColumnValueAccessor.Create(column, culture);
                if (accessor is not null)
                {
                    newSearch.Add(accessor);
                }
            }
        }

        matchers = newMatchers;
        searchAccessors = newSearch;
    }

    private void UpdateActiveFlags()
    {
        foreach (var column in grid.Columns)
        {
            // A source that cannot apply the column's filter shows no active funnel (the button is disabled).
            var isSupported = IsColumnSupported(column);
            DataGridFilter.SetIsColumnFilterSupported(column, isSupported);
            var isActive = isSupported && filters.ContainsKey(DataGridFilter.ResolveColumnKey(column));
            if (DataGridFilter.GetIsColumnFilterActive(column) != isActive)
            {
                column.SetValue(DataGridFilter.IsColumnFilterActivePropertyKey, isActive);
            }
        }
    }

    /// <summary>
    /// Raises <c>FilterChanged</c> on the grid with the active filters, the search text and the row count.
    /// </summary>
    public void RaiseFilterChanged()
    {
        var count = view is CollectionView collectionView
            ? collectionView.Count
            : -1;
        if (count > 0 && view is IEditableCollectionView { NewItemPlaceholderPosition: not NewItemPlaceholderPosition.None })
        {
            // The new-item row of a grid with CanUserAddRows is not a row that passed the filters.
            count--;
        }

        grid.RaiseEvent(
            new DataGridFilterChangedEventArgs(
                DataGridFilter.FilterChangedEvent,
                GetFilters(),
                appliedSearchText,
                count));
    }

    private DispatcherTimer CreateSearchTimer()
    {
        var timer = new DispatcherTimer(DispatcherPriority.Background, grid.Dispatcher);
        timer.Tick += (_, _) => ApplySearchNow();
        return timer;
    }

    private void OnColumnsChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (DataGridFilter.GetIsEnabled(grid) && e.NewItems is not null)
        {
            foreach (DataGridColumn column in e.NewItems)
            {
                DataGridFilter.WrapHeader(column);
            }
        }

        if (e.NewItems is not null)
        {
            UpdateActiveFlags();
        }

        // An added column (e.g. auto-generated after a new ItemsSource) may be one a filter refers to.
        if (e.Action != NotifyCollectionChangedAction.Move && HasAnything && !isReapplyQueued)
        {
            isReapplyQueued = true;
            _ = grid.Dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                () =>
                {
                    isReapplyQueued = false;
                    Apply(commitEdit: false);
                });
        }
    }

    private void OnGridUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (searchTimer is { IsEnabled: true })
        {
            ApplySearchNow();
        }

        ClosePopup();
    }

    /// <summary>
    /// Handles Alt+Down: on a filter button it opens that column's filter, elsewhere in the grid the filter of the
    /// current column. Returns whether the key was handled.
    /// </summary>
    public bool HandleKey(
        Key key,
        Key systemKey,
        object? originalSource)
    {
        if (key != Key.System || systemKey != Key.Down || !DataGridFilter.GetIsEnabled(grid))
        {
            return false;
        }

        if (originalSource is DataGridFilterButton { Column: { } buttonColumn } button)
        {
            if (!DataGridFilter.CanFilterColumn(grid, buttonColumn))
            {
                return false;
            }

            OpenPopup(buttonColumn, button);
            return true;
        }

        // Only from a cell (or the grid itself): an editing combo box or date picker keeps its own Alt+Down.
        if (originalSource is (DataGridCell or DataGrid) &&
            grid.CurrentColumn is { } column &&
            DataGridFilter.CanFilterColumn(grid, column))
        {
            DataGridFilter.OpenColumnFilter(grid, column);
            return true;
        }

        return false;
    }

    private void OnGridPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (HandleKey(e.Key, e.SystemKey, e.OriginalSource))
        {
            e.Handled = true;
        }
    }

    private void OnGridContextMenuOpening(
        object sender,
        ContextMenuEventArgs e)
    {
        if (!DataGridFilter.GetShowColumnChooser(grid) ||
            e.OriginalSource is not DependencyObject source ||
            FindHeader(source) is not { } header)
        {
            return;
        }

        e.Handled = true;
        var menu = DataGridFilter.CreateColumnChooserMenu(grid);
        menu.PlacementTarget = header;
        menu.Placement = PlacementMode.MousePoint;
        menu.IsOpen = true;
    }

    private void OnPopupClosed(
        object? sender,
        EventArgs e)
    {
        if (sender is not Popup closed)
        {
            return;
        }

        closed.Closed -= OnPopupClosed;
        var target = closed.PlacementTarget;
        closed.Child = null;
        if (ReferenceEquals(closed, popup))
        {
            popup = null;
            popupColumn = null;
            popupContent = null;
        }

        if (target is DataGridFilterButton button)
        {
            if (UIElementAutomationPeer.FromElement(button) is { } peer)
            {
                peer.ResetChildrenCache();
            }

            if (button.IsVisible)
            {
                button.Focus();
            }
        }
    }

    private static DataGridColumnHeader? FindHeader(DependencyObject? element)
    {
        while (element is not null and not DataGrid)
        {
            if (element is DataGridColumnHeader header)
            {
                return header.Column is null ? null : header;
            }

            element = element is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        return null;
    }

    private sealed record PendingViewWork(
        object Key,
        ICollectionView? View,
        Action Work);
}