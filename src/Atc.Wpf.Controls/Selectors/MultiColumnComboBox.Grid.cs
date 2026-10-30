// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <content>
/// The pop-up grid: items and keys, columns, filtering, row state, toggling and the deferred (OK / Cancel) draft.
/// </content>
public partial class MultiColumnComboBox
{
    /// <summary>
    /// With <see cref="IsFilterVisible"/> left at <see langword="null"/>, the filter box is shown above this many items.
    /// </summary>
    private const int AutoFilterThreshold = 10;

    private readonly List<object> sourceItems = [];
    private readonly List<object> rows = [];
    private readonly Dictionary<string, object> itemsByKey = new(StringComparer.Ordinal);
    private readonly Dictionary<object, string> keysByItem = new(ReferenceEqualityComparer.Instance);
    private readonly List<DataGridColumn> autoColumns = [];
    private readonly SelectedKeysCleanup selectionCleanup;
    private ListCollectionView view = null!;
    private MultiColumnCheckColumn checkColumn = null!;
    private HashSet<string> previousItemKeys = new(StringComparer.Ordinal);
    private HashSet<string> checkedSet = new(StringComparer.Ordinal);
    private int checkedItemCount;
    private List<string>? draftKeys;
    private Dictionary<object, string>? searchTexts;
    private string? searchSignature;
    private List<string> searchPaths = [];
    private string activeFilter = string.Empty;
    private INotifyCollectionChanged? subscribedSource;
    private ObservableCollection<DataGridColumn>? subscribedColumns;
    private Type? autoColumnsType;
    private DispatcherTimer? filterTimer;
    private Style? appliedRowBaseStyle;
    private Style? appliedCellBaseStyle;
    private bool areGridStylesApplied;
    private bool isWritingSelectedKeys;
    private bool isCommitRequested;

    /// <summary>
    /// Gets the tri-state "select all shown" check box in the header of the check column.
    /// </summary>
    internal MultiColumnCheckBox HeaderCheckBox { get; private set; } = null!;

    /// <summary>
    /// Gets the state of the header check box: <see langword="true"/> when every shown, enabled row is ticked,
    /// <see langword="false"/> when none is, otherwise <see langword="null"/>.
    /// </summary>
    internal bool? HeaderCheckState => HeaderCheckBox.IsChecked;

    /// <summary>
    /// Gets a value indicating whether the filter box is shown.
    /// </summary>
    internal bool IsFilterShown => PartFilter.Visibility == Visibility.Visible;

    /// <summary>
    /// Gets a value indicating whether the "No matches" text is shown.
    /// </summary>
    internal bool IsNoMatchesShown
        => PartNoMatches.Visibility == Visibility.Visible;

    /// <summary>
    /// Gets a value indicating whether a filter change waits for the pause in typing (large item sets).
    /// </summary>
    internal bool IsFilterPending => filterTimer is { IsEnabled: true };

    /// <summary>
    /// Gets a value indicating whether the pop-up is open with a draft of the ticks (<see cref="MultiSelectCommitMode.Deferred"/>).
    /// </summary>
    internal bool IsDrafting => draftKeys is not null;

    /// <summary>
    /// Ticks every shown, enabled row in one change; does nothing when they do not fit <see cref="MaxSelectionCount"/>.
    /// </summary>
    public void SelectAll()
    {
        var keys = GetShownToggleableKeys();
        if (!Fits(keys))
        {
            RefreshPopupState();
            return;
        }

        Commit(KeySelectionHelper.GetSelectionWith(GetWorkingKeys(), keys, []));
    }

    /// <summary>
    /// Unticks every enabled item (shown or not) in one change. Disabled rows keep their state.
    /// </summary>
    public void ClearSelection()
    {
        var keys = sourceItems
            .Where(IsItemEnabled)
            .Select(GetKey)
            .OfType<string>()
            .ToList();

        Commit(KeySelectionHelper.GetSelectionWith(GetWorkingKeys(), [], keys));
    }

    /// <summary>
    /// Ticks or unticks an item as a click on its row does, raising <see cref="SelectedKeysChanged"/> once (in
    /// <see cref="MultiSelectCommitMode.Deferred"/> mode with the pop-up open, only the draft changes). Does nothing for
    /// a disabled row or an unticked row at the <see cref="MaxSelectionCount"/> limit.
    /// </summary>
    /// <param name="item">An item of <see cref="ItemsSource"/>.</param>
    /// <returns><see langword="true"/> when the item was toggled.</returns>
    public bool ToggleItem(object? item)
    {
        if (!CanToggle(item))
        {
            return false;
        }

        var key = GetKey(item!)!;
        var working = GetWorkingKeys();
        Commit(checkedSet.Contains(key)
            ? KeySelectionHelper.GetSelectionWith(working, [], [key])
            : KeySelectionHelper.GetSelectionWith(working, [key], []));
        return true;
    }

    /// <summary>
    /// Returns whether the item with this key is ticked (in the draft while one is open).
    /// </summary>
    internal bool IsKeyChecked(string key)
        => checkedSet.Contains(key);

    /// <summary>
    /// Returns whether an item can be ticked or unticked now (it has a key, is enabled and is not blocked by the limit).
    /// </summary>
    internal bool CanToggle(object? item)
    {
        if (item is null || GetKey(item) is not { } key || !IsItemEnabled(item))
        {
            return false;
        }

        return checkedSet.Contains(key) ||
               MaxSelectionCount is not { } max ||
               checkedItemCount < max;
    }

    /// <summary>
    /// Acts on the header check box: unticks the shown rows when all are ticked, otherwise ticks them.
    /// </summary>
    internal void ToggleAllShown()
    {
        if (HeaderCheckBox.IsChecked == true)
        {
            Commit(KeySelectionHelper.GetSelectionWith(GetWorkingKeys(), [], GetShownToggleableKeys()));
            return;
        }

        SelectAll();
    }

    /// <summary>
    /// Toggles the row under a mouse press. Presses on a column header, a gripper or a scroll bar do nothing.
    /// </summary>
    /// <returns><see langword="true"/> when the press was on a row.</returns>
    internal bool HandleRowPress(DependencyObject? source)
    {
        for (var current = source; current is not null && !ReferenceEquals(current, PartGrid); current = GetParent(current))
        {
            switch (current)
            {
                case DataGridColumnHeader or DataGridColumnHeadersPresenter or ScrollBar or Thumb:
                    return false;
                case DataGridRow row:
                    ToggleItem(row.Item);
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Applies a pending (debounced) filter now.
    /// </summary>
    internal void FlushPendingFilter()
    {
        if (IsFilterPending)
        {
            ApplyFilter();
        }
    }

    /// <summary>
    /// Unticks a key without opening the pop-up (a chip's remove button), raising <see cref="SelectedKeysChanged"/>
    /// once. Does nothing for a disabled row or a key that is not ticked.
    /// </summary>
    internal void RemoveKey(string key)
    {
        if (SelectedKeys is null ||
            !SelectedKeys.Contains(key, StringComparer.Ordinal) ||
            (itemsByKey.TryGetValue(key, out var item) && !IsItemEnabled(item)))
        {
            return;
        }

        draftKeys?.Remove(key);
        WriteSelectedKeys(KeySelectionHelper.GetSelectionWith(SelectedKeys.ToList(), [], [key]));
    }

    private static DependencyObject? GetParent(DependencyObject element)
        => element is Visual or System.Windows.Media.Media3D.Visual3D
            ? VisualTreeHelper.GetParent(element)
            : LogicalTreeHelper.GetParent(element);

    private static void OnItemsSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        if (control.subscribedSource is not null)
        {
            control.subscribedSource.CollectionChanged -= control.OnSourceCollectionChanged;
        }

        control.subscribedSource = e.NewValue as INotifyCollectionChanged;
        if (control.subscribedSource is not null)
        {
            control.subscribedSource.CollectionChanged += control.OnSourceCollectionChanged;
        }

        control.RebuildItems();
    }

    private static void OnKeyMemberPathChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        // A new key path never drops keys: only items that go away do.
        var control = (MultiColumnComboBox)d;
        control.previousItemKeys = new HashSet<string>(StringComparer.Ordinal);
        control.RebuildItems();
    }

    private static void OnDisplayMemberPathChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        control.chipsSignature = null;
        control.UpdateSummary();
        control.RefreshPopupState();
    }

    private static void OnColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        if (control.subscribedColumns is not null)
        {
            control.subscribedColumns.CollectionChanged -= control.OnColumnsCollectionChanged;
        }

        control.subscribedColumns = e.NewValue as ObservableCollection<DataGridColumn>;
        if (control.subscribedColumns is not null)
        {
            control.subscribedColumns.CollectionChanged += control.OnColumnsCollectionChanged;
        }

        control.RebuildGridColumns();
    }

    private static void OnAutoGenerateColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((MultiColumnComboBox)d).RebuildGridColumns();

    private static void OnRowStateInputChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        control.chipsSignature = null;
        control.UpdateSummary();
        control.RefreshPopupState();
    }

    private static void OnIsFilterVisibleChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((MultiColumnComboBox)d).UpdateFilterVisibility();

    private static void OnFilterTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        if (control.sourceItems.Count <= KeySelectionHelper.FilterDebounceThreshold)
        {
            control.ApplyFilter();
            return;
        }

        if (control.filterTimer is null)
        {
            control.filterTimer = new DispatcherTimer(DispatcherPriority.Input, control.Dispatcher)
            {
                Interval = KeySelectionHelper.FilterDebounceDelay,
            };
            control.filterTimer.Tick += control.OnFilterTimerTick;
        }

        control.filterTimer.Stop();
        control.filterTimer.Start();
    }

    private static void OnFilterMemberPathsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((MultiColumnComboBox)d).ApplyFilter();

    private void InitializeGrid()
    {
        HeaderCheckBox = new MultiColumnCheckBox(OnCheckBoxToggled)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsChecked = false,
        };
        AutomationProperties.SetAutomationId(HeaderCheckBox, "SelectAll");
        checkColumn = new MultiColumnCheckColumn(HeaderCheckBox, OnCheckBoxToggled);

        view = new ListCollectionView(rows);
        PartGrid.ItemsSource = view;
        PartGrid.Columns.Add(checkColumn);
        PartGrid.LoadingRow += OnGridLoadingRow;
        ApplyGridStyles();
    }

    private void OnCheckBoxToggled(MultiColumnCheckBox checkBox)
    {
        if (checkBox.IsRowCheckBox)
        {
            ToggleItem(checkBox.DataContext);
        }
        else
        {
            ToggleAllShown();
        }
    }

    private void OnGridPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
        => HandleRowPress(e.OriginalSource as DependencyObject);

    private void OnGridLoadingRow(
        object? sender,
        DataGridRowEventArgs e)
        => UpdateRow(e.Row);

    private void OnSourceCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => RebuildItems();

    private void OnColumnsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => RebuildGridColumns();

    private void OnFilterTimerTick(
        object? sender,
        EventArgs e)
        => ApplyFilter();

    /// <summary>
    /// Builds the row and cell styles on top of the theme's <c>DataGrid</c> styles (when the theme is loaded): ticked
    /// rows get the selected background, the keyboard row only a border, and rows that cannot be toggled grey text.
    /// </summary>
    private void ApplyGridStyles()
    {
        var rowBase = TryFindResource("AtcApps.Styles.DataGridRow") as Style;
        var cellBase = TryFindResource("AtcApps.Styles.DataGridCell") as Style;
        if (areGridStylesApplied &&
            ReferenceEquals(rowBase, appliedRowBaseStyle) &&
            ReferenceEquals(cellBase, appliedCellBaseStyle))
        {
            return;
        }

        areGridStylesApplied = true;
        appliedRowBaseStyle = rowBase;
        appliedCellBaseStyle = cellBase;

        var rowStyle = new Style(typeof(DataGridRow), rowBase);
        var ownBackground = new Binding
        {
            Path = new PropertyPath(BackgroundProperty),
            RelativeSource = RelativeSource.Self,
        };
        rowStyle.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(1)));
        rowStyle.Setters.Add(new Setter(BorderBrushProperty, Brushes.Transparent));
        rowStyle.Setters.Add(new Setter(ItemHelper.DisableAlternatingRowColoringProperty, BooleanBoxes.TrueBox));
        rowStyle.Setters.Add(new Setter(ItemHelper.SelectedBackgroundBrushProperty, ownBackground));
        rowStyle.Setters.Add(new Setter(ItemHelper.ActiveSelectionBackgroundBrushProperty, ownBackground));
        rowStyle.Setters.Add(new Setter(ItemHelper.HoverSelectedBackgroundBrushProperty, ownBackground));
        rowStyle.Setters.Add(new Setter(ItemHelper.SelectedForegroundBrushProperty, new DynamicResourceExtension("AtcApps.Brushes.Text")));
        rowStyle.Setters.Add(new Setter(ItemHelper.ActiveSelectionForegroundBrushProperty, new DynamicResourceExtension("AtcApps.Brushes.Text")));
        rowStyle.Setters.Add(new Setter(ItemHelper.HoverSelectedForegroundBrushProperty, new DynamicResourceExtension("AtcApps.Brushes.Text")));
        var tickedTrigger = new Trigger { Property = MultiColumnRowState.IsCheckedProperty, Value = BooleanBoxes.TrueBox };
        tickedTrigger.Setters.Add(new Setter(BackgroundProperty, new DynamicResourceExtension("AtcApps.Brushes.Accent3")));
        rowStyle.Triggers.Add(tickedTrigger);
        PartGrid.RowStyle = rowStyle;

        var cellStyle = new Style(typeof(DataGridCell), cellBase);
        var selectedTrigger = new Trigger { Property = DataGridCell.IsSelectedProperty, Value = BooleanBoxes.TrueBox };
        selectedTrigger.Setters.Add(new Setter(ForegroundProperty, new DynamicResourceExtension("AtcApps.Brushes.Text")));
        cellStyle.Triggers.Add(selectedTrigger);
        var lockedTrigger = new DataTrigger
        {
            Binding = new Binding
            {
                Path = new PropertyPath(MultiColumnRowState.CanToggleProperty),
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1),
            },
            Value = BooleanBoxes.FalseBox,
        };
        lockedTrigger.Setters.Add(new Setter(ForegroundProperty, new DynamicResourceExtension("AtcApps.Brushes.Gray")));
        cellStyle.Triggers.Add(lockedTrigger);
        PartGrid.CellStyle = cellStyle;
    }

    private string? GetKey(object item)
        => keysByItem.TryGetValue(item, out var key)
            ? key
            : ComputeKey(item);

    private string? ComputeKey(object item)
        => string.IsNullOrWhiteSpace(KeyMemberPath)
            ? item.ToString()
            : MemberPathEvaluator.GetString(item, KeyMemberPath);

    private string GetDisplayText(object item)
    {
        if (string.IsNullOrWhiteSpace(DisplayMemberPath))
        {
            return item.ToString() ?? string.Empty;
        }

        return Convert.ToString(MemberPathEvaluator.GetValue(item, DisplayMemberPath), CultureInfo.CurrentCulture) ?? string.Empty;
    }

    private bool IsItemEnabled(object item)
        => string.IsNullOrWhiteSpace(IsEnabledMemberPath) ||
           MemberPathEvaluator.GetValue(item, IsEnabledMemberPath) is not false;

    /// <summary>
    /// Returns the ticked items of <see cref="SelectedKeys"/> (not the draft) in item order, one per key.
    /// </summary>
    private List<(string Key, string Text, bool IsEnabled)> GetTickedItems()
    {
        var result = new List<(string Key, string Text, bool IsEnabled)>();
        if (SelectedKeys is not { Count: > 0 } keys)
        {
            return result;
        }

        var selectedSet = new HashSet<string>(keys, StringComparer.Ordinal);
        foreach (var (key, item) in itemsByKey)
        {
            if (selectedSet.Contains(key))
            {
                result.Add((key, GetDisplayText(item), IsItemEnabled(item)));
            }
        }

        return result;
    }

    private IReadOnlyList<string> GetWorkingKeys()
        => draftKeys ?? (IReadOnlyList<string>?)SelectedKeys?.ToList() ?? [];

    private List<string> GetShownToggleableKeys()
        => view
            .Cast<object>()
            .Where(IsItemEnabled)
            .Select(GetKey)
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .ToList();

    private bool Fits(IEnumerable<string> keysToTick)
    {
        if (MaxSelectionCount is not { } max)
        {
            return true;
        }

        var additional = keysToTick.Count(x => !checkedSet.Contains(x) && itemsByKey.ContainsKey(x));
        return checkedItemCount + additional <= max;
    }

    private void Commit(IReadOnlyList<string> newKeys)
    {
        if (draftKeys is not null)
        {
            draftKeys = newKeys.ToList();
            RefreshPopupState();
            return;
        }

        WriteSelectedKeys(newKeys);
    }

    private void WriteSelectedKeys(IReadOnlyList<string> newKeys)
    {
        var target = SelectedKeys;
        var oldKeys = target is null ? [] : target.ToList();
        if (KeySelectionHelper.SequenceEquals(oldKeys, newKeys))
        {
            RefreshPopupState();
            return;
        }

        isWritingSelectedKeys = true;
        try
        {
            if (target is null)
            {
                target = [];
                SetCurrentValue(SelectedKeysProperty, target);
            }

            KeySelectionHelper.ApplyInPlace(target, newKeys);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }

        OnSelectionChanged();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    private void OnSelectionChanged()
    {
        if (summaryTextBlock is null)
        {
            return;
        }

        SelectedItems = ((IEnumerable<string>?)SelectedKeys ?? [])
            .Select(x => itemsByKey.GetValueOrDefault(x))
            .OfType<object>()
            .ToList();
        UpdateSummary();
        RefreshPopupState();
    }

    private void RebuildItems()
    {
        sourceItems.Clear();
        keysByItem.Clear();
        itemsByKey.Clear();
        if (ItemsSource is { } source)
        {
            foreach (var item in source)
            {
                if (item is null)
                {
                    continue;
                }

                sourceItems.Add(item);
                if (ComputeKey(item) is { } key)
                {
                    keysByItem[item] = key;
                    itemsByKey.TryAdd(key, item);
                }
            }
        }

        searchTexts = null;
        rows.Clear();
        rows.AddRange(sourceItems);
        view.Refresh();

        RebuildGridColumns();
        UpdateFilterVisibility();
        var oldItemKeys = previousItemKeys;
        previousItemKeys = new HashSet<string>(itemsByKey.Keys, StringComparer.Ordinal);
        selectionCleanup.Request(oldItemKeys);
        UpdateNoMatches();
        chipsSignature = null;
        OnSelectionChanged();
    }

    /// <summary>
    /// Drops the keys of items that are gone from the selection and the draft (one event).
    /// </summary>
    private void DropStaleKeys(IReadOnlySet<string> oldItemKeys)
    {
        var stale = ((IEnumerable<string>?)SelectedKeys ?? [])
            .Concat(draftKeys ?? [])
            .Where(x => oldItemKeys.Contains(x) && !itemsByKey.ContainsKey(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (stale.Count == 0)
        {
            return;
        }

        if (draftKeys is not null)
        {
            draftKeys = KeySelectionHelper.GetSelectionWith(draftKeys, [], stale).ToList();
        }

        if (SelectedKeys is { } keys)
        {
            WriteSelectedKeys(KeySelectionHelper.GetSelectionWith(keys.ToList(), [], stale));
        }
    }

    private void RebuildGridColumns()
    {
        if (checkColumn is null)
        {
            return;
        }

        var desired = new List<DataGridColumn> { checkColumn };
        if (Columns is { Count: > 0 } explicitColumns)
        {
            desired.AddRange(explicitColumns);
        }
        else if (AutoGenerateColumns)
        {
            desired.AddRange(GetAutoColumns());
        }

        if (PartGrid.Columns.SequenceEqual(desired))
        {
            return;
        }

        PartGrid.Columns.Clear();
        foreach (var column in desired)
        {
            PartGrid.Columns.Add(column);
        }
    }

    private List<DataGridColumn> GetAutoColumns()
    {
        var type = sourceItems.FirstOrDefault()?.GetType();
        if (type is null)
        {
            return [];
        }

        if (type == autoColumnsType)
        {
            return autoColumns;
        }

        autoColumnsType = type;
        autoColumns.Clear();
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanRead && x.GetIndexParameters().Length == 0)
            .OrderBy(x => x.MetadataToken);
        foreach (var property in properties)
        {
            autoColumns.Add(new DataGridTextColumn
            {
                Header = property.Name,
                Binding = new Binding(property.Name) { Mode = BindingMode.OneWay },
                SortMemberPath = property.Name,
            });
        }

        return autoColumns;
    }

    private void UpdateFilterVisibility()
    {
        var isVisible = IsFilterVisible ?? sourceItems.Count > AutoFilterThreshold;
        PartFilter.Visibility = isVisible
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void ApplyFilter()
    {
        filterTimer?.Stop();
        activeFilter = FilterText?.Trim() ?? string.Empty;
        if (activeFilter.Length == 0)
        {
            view.Filter = null;
        }
        else
        {
            PrepareSearchTexts();
            view.Filter = MatchesFilter;
        }

        UpdateNoMatches();
        RefreshPopupState();
    }

    private bool MatchesFilter(object item)
    {
        searchTexts ??= new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
        if (!searchTexts.TryGetValue(item, out var text))
        {
            text = string.Join(
                "\u001f",
                searchPaths.Select(path => Convert.ToString(MemberPathEvaluator.GetValue(item, path), CultureInfo.CurrentCulture)));
            searchTexts[item] = text;
        }

        return KeySelectionHelper.MatchesFilter(text, activeFilter);
    }

    /// <summary>
    /// Works out which paths the filter searches (<see cref="FilterMemberPaths"/>, or the visible columns' binding or
    /// sort paths) and drops the cached search texts when they changed.
    /// </summary>
    private void PrepareSearchTexts()
    {
        List<string> paths;
        if (FilterMemberPaths is { } explicitPaths)
        {
            paths = explicitPaths
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
        else
        {
            paths = PartGrid.Columns
                .Where(x => x is not MultiColumnCheckColumn && x.Visibility == Visibility.Visible)
                .Select(GetColumnPath)
                .OfType<string>()
                .ToList();
        }

        var signature = string.Join("\u001f", paths);
        if (!string.Equals(signature, searchSignature, StringComparison.Ordinal))
        {
            searchSignature = signature;
            searchTexts = null;
        }

        searchPaths = paths;
    }

    private static string? GetColumnPath(DataGridColumn column)
    {
        var path = column is DataGridBoundColumn { Binding: Binding { Path.Path: { Length: > 0 } bindingPath } }
            ? bindingPath
            : column.SortMemberPath;

        return string.IsNullOrWhiteSpace(path) || string.Equals(path, ".", StringComparison.Ordinal)
            ? null
            : path;
    }

    private void UpdateNoMatches()
        => PartNoMatches.Visibility = activeFilter.Length > 0 && view.IsEmpty
            ? Visibility.Visible
            : Visibility.Collapsed;

    /// <summary>
    /// Refreshes everything the pop-up shows for the ticks: the realized rows, the header check box and the footer.
    /// </summary>
    private void RefreshPopupState()
    {
        if (view is null)
        {
            return;
        }

        checkedSet = new HashSet<string>(GetWorkingKeys(), StringComparer.Ordinal);
        checkedItemCount = checkedSet.Count(itemsByKey.ContainsKey);

        foreach (var row in GetRealizedRows())
        {
            UpdateRow(row);
        }

        var shownTotal = 0;
        var shownTicked = 0;
        foreach (var item in view)
        {
            if (!IsItemEnabled(item) || GetKey(item) is not { } key)
            {
                continue;
            }

            shownTotal++;
            if (checkedSet.Contains(key))
            {
                shownTicked++;
            }
        }

        HeaderCheckBox.IsChecked = KeySelectionHelper.GetAggregateState(shownTicked, shownTotal);
        HeaderCheckBox.IsEnabled = shownTotal > 0;
        PartFooterText.Text = string.Format(
            CultureInfo.CurrentCulture,
            Atc.Wpf.Resources.Miscellaneous.CountOfSelected2,
            checkedItemCount,
            itemsByKey.Count);
    }

    private void UpdateRow(DataGridRow row)
    {
        var item = row.Item;
        if (item is null || ReferenceEquals(item, CollectionView.NewItemPlaceholder))
        {
            return;
        }

        var key = GetKey(item);
        MultiColumnRowState.SetIsChecked(row, key is not null && checkedSet.Contains(key));
        MultiColumnRowState.SetCanToggle(row, CanToggle(item));
        MultiColumnRowState.SetDisplayText(row, GetDisplayText(item));
    }

    private IEnumerable<DataGridRow> GetRealizedRows()
    {
        var pending = new Queue<DependencyObject>();
        pending.Enqueue(PartGrid);
        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            if (current is DataGridRowsPresenter presenter)
            {
                foreach (var child in presenter.Children)
                {
                    if (child is DataGridRow row)
                    {
                        yield return row;
                    }
                }

                yield break;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                pending.Enqueue(VisualTreeHelper.GetChild(current, i));
            }
        }
    }

    private void ApplyKeepSelectedOnTop()
    {
        if (!KeepSelectedOnTop)
        {
            return;
        }

        var ticked = new HashSet<string>(GetWorkingKeys(), StringComparer.Ordinal);
        bool IsTicked(object item)
            => GetKey(item) is { } key && ticked.Contains(key);

        using (view.DeferRefresh())
        {
            if (sourceItems.Any(IsTicked))
            {
                PartGrid.Items.SortDescriptions.Clear();
                foreach (var column in PartGrid.Columns)
                {
                    column.SortDirection = null;
                }
            }

            rows.Clear();
            rows.AddRange(sourceItems.Where(IsTicked));
            rows.AddRange(sourceItems.Where(x => !IsTicked(x)));
        }
    }

    private bool ToggleFocusedRow()
    {
        var item = PartGrid.SelectedItem ?? PartGrid.CurrentItem;
        if (item is null)
        {
            return false;
        }

        ToggleItem(item);
        return true;
    }

    private bool FocusRowAt(int index)
    {
        if (index < 0 || index >= PartGrid.Items.Count)
        {
            return false;
        }

        var item = PartGrid.Items[index];
        PartGrid.SelectedItem = item;
        PartGrid.ScrollIntoView(item);
        PartGrid.UpdateLayout();
        if (PartGrid.ItemContainerGenerator.ContainerFromItem(item) is not DataGridRow row)
        {
            return true;
        }

        var column = PartGrid.Columns.Count > 1 ? PartGrid.Columns[1] : PartGrid.Columns[0];
        PartGrid.CurrentCell = new DataGridCellInfo(item, column);
        if (column.GetCellContent(row)?.Parent is DataGridCell cell)
        {
            cell.Focus();
        }
        else
        {
            row.Focus();
        }

        return true;
    }

    private void BeginDraft()
    {
        var isDeferred = CommitMode == MultiSelectCommitMode.Deferred;
        PartCommitFooter.Visibility = isDeferred ? Visibility.Visible : Visibility.Collapsed;
        if (!isDeferred || draftKeys is not null)
        {
            return;
        }

        draftKeys = ((IEnumerable<string>?)SelectedKeys ?? []).ToList();
    }

    private void EndDraft()
    {
        var commit = isCommitRequested;
        isCommitRequested = false;
        if (draftKeys is null)
        {
            return;
        }

        var draft = draftKeys;
        draftKeys = null;
        if (commit)
        {
            WriteSelectedKeys(draft);
        }
        else
        {
            RefreshPopupState();
        }
    }

    private void CommitDraftAndClose()
    {
        isCommitRequested = IsDrafting;
        SetCurrentValue(IsDropDownOpenProperty, false);
        isCommitRequested = false;
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
        => CommitDraftAndClose();

    private void OnCancelClick(
        object sender,
        RoutedEventArgs e)
        => SetCurrentValue(IsDropDownOpenProperty, false);
}