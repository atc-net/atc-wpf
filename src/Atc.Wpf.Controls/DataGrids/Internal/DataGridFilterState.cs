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

    public DataGridFilterState(DataGrid grid)
    {
        this.grid = grid;
        view = GetView(grid.ItemsSource);

        grid.Columns.CollectionChanged += OnColumnsChanged;
        grid.Unloaded += OnGridUnloaded;
        grid.PreviewKeyDown += OnGridPreviewKeyDown;
        grid.ContextMenuOpening += OnGridContextMenuOpening;
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
            Apply();
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
        Apply();
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
        if (currentView is not { CanFilter: true })
        {
            return;
        }

        if (HasAnything)
        {
            owner = DataGridViewFilterOwner.GetOrCreate(currentView);
            owner.ApplyDeferred(this);
        }
        else
        {
            DetachFromOwner();
        }
    }

    /// <summary>
    /// Builds the filter of the view and refreshes it once, then raises <c>FilterChanged</c>.
    /// </summary>
    public void Apply()
    {
        RebuildMatchers();
        UpdateActiveFlags();

        var currentView = EnsureView();
        if (currentView is { CanFilter: true })
        {
            if (HasAnything)
            {
                owner = DataGridViewFilterOwner.GetOrCreate(currentView);
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
            if (item is null || !PassesOthers(item, consumer, others))
            {
                continue;
            }

            var raw = accessor.GetValue(item);
            var text = accessor.Format(raw);
            firstRawByText.TryAdd(text, raw);
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
    /// Opens the filter pop-up of a column under <paramref name="placementTarget"/> (the header button, or the grid).
    /// </summary>
    public void OpenPopup(
        DataGridColumn column,
        UIElement? placementTarget)
    {
        ClosePopup();

        var key = DataGridFilter.ResolveColumnKey(column);
        var values = GetDistinctValues(column, out var kind);
        var content = new DataGridFilterPopupContent();
        content.Load(key, values, kind, filters.GetValueOrDefault(key), DataGridFilter.GetGridCulture(grid));
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
    }

    /// <summary>
    /// Takes this grid's filters off the view (the behaviour is switched off, or the source changed): the other
    /// grids on the view keep theirs, and without any the view gets the consumer's own filter back.
    /// </summary>
    public void RestoreConsumerFilter()
        => DetachFromOwner();

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
        current?.Detach(this);
    }

    private IEnumerable GetSourceItems()
        => EnsureView()?.SourceCollection ?? grid.Items;

    private ICollectionView? EnsureView()
    {
        var current = GetView(grid.ItemsSource);
        if (!ReferenceEquals(current, view))
        {
            RestoreConsumerFilter();
            view = current;
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
            if (filters.TryGetValue(DataGridFilter.ResolveColumnKey(column), out var filter))
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
            var isActive = filters.ContainsKey(DataGridFilter.ResolveColumnKey(column));
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

        // An added column (e.g. auto-generated after a new ItemsSource) may be one a filter refers to.
        if (e.Action != NotifyCollectionChangedAction.Move && HasAnything && !isReapplyQueued)
        {
            isReapplyQueued = true;
            _ = grid.Dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                () =>
                {
                    isReapplyQueued = false;
                    Apply();
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
}