// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// An always-visible list with a check box (or highlight, or switch) per item, a tri-state "Select all" row,
/// an optional filter box, optional groups and an optional selection limit.
/// </summary>
/// <remarks>
/// Ticks made in the control (a row, Select all, Clear, a group header, a range) update <see cref="SelectedKeys"/>
/// in place and raise <see cref="SelectedKeysChanged"/> once per action. Changes made to <see cref="SelectedKeys"/>
/// from outside (or a new instance) only re-sync the rows and raise no event.
/// </remarks>
public partial class CheckListBox
{
    private const int AutoFilterItemCount = 10;

    private readonly Dictionary<string, CheckListItem> rowsByKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, CheckListItem> groupRowsByName = new(StringComparer.Ordinal);
    private readonly List<CheckListItem> itemRows = [];
    private readonly ObservableCollectionEx<CheckListItem> visibleRows = [];
    private readonly ItemsPanelTemplate defaultItemsPanel;
    private readonly SelectedKeysCleanup selectionCleanup;

    /// <summary>
    /// The items, keyed by their selection key, with their display value. Changing it keeps the ticks of the keys
    /// still present and drops the others from <see cref="SelectedKeys"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemsChanged))]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the ticked items. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Whether keys that appear in <see cref="Items"/> (including the first items) are ticked automatically.
    /// </summary>
    [DependencyProperty]
    private bool selectNewItems;

    /// <summary>
    /// Whether the top row with the tri-state "Select all" check box and the count is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool showSelectAllRow;

    /// <summary>
    /// Whether a "Clear" button is shown in the "Select all" row.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool showClearButton;

    /// <summary>
    /// Whether the filter box is shown; <see langword="null"/> (the default) shows it when there are more than 10 items.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the filter box; only items whose display value contains it are shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnFilterTextChanged))]
    private string filterText;

    /// <summary>
    /// The group of each key (key to group name); <see langword="null"/> for no groups. Keys without a group are listed
    /// first; the groups follow in the order of first appearance, each under a header.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemGroupsChanged))]
    private Dictionary<string, string>? itemGroups;

    /// <summary>
    /// Whether a group header has a tri-state check box that ticks or unticks the shown items of its group.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnTemplateChanged))]
    private bool isGroupCheckable;

    /// <summary>
    /// The maximum number of ticked keys; <see langword="null"/> for no limit. At the limit the unticked items are disabled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys whose items are shown but cannot be toggled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDisabledKeysChanged))]
    private ObservableCollection<string> disabledKeys;

    /// <summary>
    /// How a row shows whether it is ticked: a check box, a highlight or a switch.
    /// </summary>
    [DependencyProperty(DefaultValue = CheckListItemStyle.CheckBox, PropertyChangedCallback = nameof(OnTemplateChanged))]
    private CheckListItemStyle itemStyle;

    /// <summary>
    /// The number of columns the items are laid out in; virtualization only applies at 1.
    /// </summary>
    [DependencyProperty(DefaultValue = 1, PropertyChangedCallback = nameof(OnColumnsChanged))]
    private int columns;

    /// <summary>
    /// The template for the text part of a row; its data context is the <see cref="CheckListItem"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    private HashSet<string> itemKeys = new(StringComparer.Ordinal);
    private HashSet<string> selectedSet = new(StringComparer.Ordinal);
    private HashSet<string> disabledSet = new(StringComparer.Ordinal);
    private ObservableCollection<string>? subscribedSelectedKeys;
    private ObservableCollection<string>? subscribedDisabledKeys;
    private DispatcherTimer? filterTimer;
    private string? anchorKey;
    private bool? selectAllState = false;
    private bool isWritingSelectedKeys;
    private bool isSyncingSelectAll;

    /// <summary>
    /// Occurs when the control changes <see cref="SelectedKeys"/>: once per tick, Select all, Clear, group tick,
    /// range tick, or <see cref="Items"/> change that drops or adds keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckListBox"/> class.
    /// </summary>
    public CheckListBox()
    {
        selectionCleanup = new SelectedKeysCleanup(this, SelectedKeysProperty, CleanUpSelection);
        InitializeComponent();

        defaultItemsPanel = PartList.ItemsPanel;
        PartList.ItemsSource = visibleRows;
        PartList.ItemTemplateSelector = new CheckListRowTemplateSelector(this);
        PreviewKeyDown += OnPreviewKeyDown;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ApplyTexts();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));
        UpdateChrome();
        RefreshStates();
    }

    /// <summary>
    /// Gets the rows currently shown (group headers and items, after the filter).
    /// </summary>
    internal IReadOnlyList<CheckListItem> VisibleRows => visibleRows;

    /// <summary>
    /// Gets how many times the check states were refreshed (for change-storm tests).
    /// </summary>
    internal int StateRefreshCount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control is writing its own change into <c>SelectedKeys</c>; a wrapper that
    /// shares the collection skips the notifications of that write and updates once afterwards.
    /// </summary>
    internal bool IsWritingSelectedKeys => isWritingSelectedKeys;

    /// <summary>
    /// Ticks every shown, enabled item in one change. Does nothing when the selection limit has no room for them all.
    /// </summary>
    public void SelectAll()
    {
        var keys = GetShownToggleableItemRows()
            .Select(x => x.Key)
            .ToList();

        if (!Fits(keys))
        {
            RefreshStates();
            return;
        }

        Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), keys, []));
    }

    /// <summary>
    /// Unticks every enabled item (shown or not) in one change. Disabled keys keep their state.
    /// </summary>
    public void ClearSelection()
    {
        var keys = itemRows
            .Where(x => !disabledSet.Contains(x.Key))
            .Select(x => x.Key);

        Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], keys));
    }

    /// <summary>
    /// Returns the row of an item, or <see langword="null"/> when the key is not in <see cref="Items"/>.
    /// Setting its <see cref="CheckListItem.IsChecked"/> ticks or unticks the item as a click would.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <returns>The row, or <see langword="null"/>.</returns>
    public CheckListItem? GetItem(string key)
        => rowsByKey.GetValueOrDefault(key);

    /// <summary>
    /// Applies a pending (debounced) filter now.
    /// </summary>
    internal void FlushPendingFilter()
    {
        if (filterTimer is not { IsEnabled: true })
        {
            return;
        }

        filterTimer.Stop();
        ApplyFilter(keepScrollPosition: false);
    }

    /// <summary>
    /// Ticks or unticks (the new state of <paramref name="row"/>) every shown, enabled item from the last clicked row
    /// to <paramref name="row"/>, in one change. This is what <c>Shift+click</c> does.
    /// </summary>
    internal void ToggleRange(CheckListItem row)
    {
        ArgumentNullException.ThrowIfNull(row);

        var targetIndex = IndexOfVisible(row);
        var anchorIndex = anchorKey is null ? -1 : IndexOfVisible(rowsByKey.GetValueOrDefault(anchorKey));
        if (row.IsGroupHeader || targetIndex < 0 || anchorIndex < 0)
        {
            RequestToggle(row, row.IsChecked != true);
            return;
        }

        var tick = row.IsChecked != true;
        var from = System.Math.Min(anchorIndex, targetIndex);
        var to = System.Math.Max(anchorIndex, targetIndex);
        var keys = new List<string>();
        for (var i = from; i <= to; i++)
        {
            var candidate = visibleRows[i];
            if (!candidate.IsGroupHeader && !disabledSet.Contains(candidate.Key))
            {
                keys.Add(candidate.Key);
            }
        }

        if (anchorIndex > targetIndex)
        {
            keys.Reverse();
        }

        anchorKey = row.Key;
        if (tick)
        {
            var toAdd = keys.Where(x => !selectedSet.Contains(x));
            if (MaxSelectionCount is { } max)
            {
                toAdd = toAdd.Take(System.Math.Max(0, max - GetSelectedItemCount()));
            }

            Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), toAdd.ToList(), []));
        }
        else
        {
            Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], keys));
        }
    }

    /// <summary>
    /// Returns the row template for a row, according to <see cref="ItemStyle"/> and <see cref="IsGroupCheckable"/>.
    /// </summary>
    internal DataTemplate? SelectRowTemplate(CheckListItem row)
    {
        var key = row.IsGroupHeader
            ? IsGroupCheckable ? "CheckListGroupHeaderTemplate" : "CheckListPlainGroupHeaderTemplate"
            : ItemStyle switch
            {
                CheckListItemStyle.Highlight => "CheckListHighlightRowTemplate",
                CheckListItemStyle.Switch => "CheckListSwitchRowTemplate",
                _ => "CheckListCheckBoxRowTemplate",
            };

        return Resources[key] as DataTemplate;
    }

    private static void OnItemsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        var oldItemKeys = control.itemKeys;
        control.RebuildRows(keepScrollPosition: true);
        control.RefreshStates();
        control.selectionCleanup.Request(oldItemKeys);
    }

    /// <summary>
    /// Drops the keys of items that are gone (and ticks new ones with <see cref="SelectNewItems"/>) after
    /// <see cref="Items"/> changed, in one <see cref="SelectedKeysChanged"/>.
    /// </summary>
    private void CleanUpSelection(IReadOnlySet<string> oldItemKeys)
        => Commit(KeySelectionHelper.GetSelectionAfterItemsChanged(
            GetCurrentSelection(),
            oldItemKeys,
            Items,
            SelectNewItems,
            MaxSelectionCount));

    private static void OnItemGroupsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CheckListBox)d).RebuildRows(keepScrollPosition: true);

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.RemoveHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.AddHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        if (!control.isWritingSelectedKeys)
        {
            control.RefreshStates();
        }
    }

    private static void OnDisabledKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        if (control.subscribedDisabledKeys is not null)
        {
            CollectionChangedEventManager.RemoveHandler(control.subscribedDisabledKeys, control.OnDisabledKeysCollectionChanged);
        }

        control.subscribedDisabledKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedDisabledKeys is not null)
        {
            CollectionChangedEventManager.AddHandler(control.subscribedDisabledKeys, control.OnDisabledKeysCollectionChanged);
        }

        control.RefreshStates();
    }

    private static void OnStateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CheckListBox)d).RefreshStates();

    private static void OnChromeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CheckListBox)d).UpdateChrome();

    private static void OnTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        control.PartList.ItemTemplateSelector = new CheckListRowTemplateSelector(control);
        control.PartList.Items.Refresh();
        control.RefreshStates();
    }

    private static void OnColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        var columnCount = control.Columns;
        if (columnCount > 1)
        {
            var factory = new FrameworkElementFactory(typeof(UniformGrid));
            factory.SetValue(UniformGrid.ColumnsProperty, columnCount);
            factory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Top);
            control.PartList.SetCurrentValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate(factory));
            VirtualizingPanel.SetIsVirtualizing(control.PartList, false);
            ScrollViewer.SetCanContentScroll(control.PartList, false);
        }
        else
        {
            control.PartList.SetCurrentValue(ItemsControl.ItemsPanelProperty, control.defaultItemsPanel);
            VirtualizingPanel.SetIsVirtualizing(control.PartList, true);
            ScrollViewer.SetCanContentScroll(control.PartList, true);
        }
    }

    private static void OnFilterTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckListBox)d;
        if (control.itemRows.Count <= KeySelectionHelper.FilterDebounceThreshold)
        {
            control.filterTimer?.Stop();
            control.ApplyFilter(keepScrollPosition: false);
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

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        ApplyTexts();
        RefreshStates();
    }

    private void ApplyTexts()
    {
        PartSelectAll.Content = Atc.Wpf.Resources.Miscellaneous.SelectAll;
        PartClear.Content = Atc.Wpf.Resources.Miscellaneous.Clear;
        PartNoMatches.Text = Atc.Wpf.Resources.Miscellaneous.NoMatches;
        TextBoxHelper.SetWatermark(PartFilter, Atc.Wpf.Resources.Word.Filter);
        AutomationProperties.SetName(PartFilter, Atc.Wpf.Resources.Word.Filter);
    }

    private void OnFilterTimerTick(
        object? sender,
        EventArgs e)
        => FlushPendingFilter();

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            RefreshStates();
        }
    }

    private void OnDisabledKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => RefreshStates();

    private void RebuildRows(bool keepScrollPosition)
    {
        var source = Items ?? new Dictionary<string, string>(StringComparer.Ordinal);
        var groups = ItemGroups;
        var oldRows = new Dictionary<string, CheckListItem>(rowsByKey, StringComparer.Ordinal);

        rowsByKey.Clear();
        itemRows.Clear();
        foreach (var (key, value) in source)
        {
            string? groupName = null;
            if (groups is not null &&
                groups.TryGetValue(key, out var name) &&
                !string.IsNullOrEmpty(name))
            {
                groupName = name;
            }

            if (oldRows.TryGetValue(key, out var row) &&
                string.Equals(row.GroupName, groupName, StringComparison.Ordinal))
            {
                row.DisplayValue = value ?? string.Empty;
            }
            else
            {
                row = new CheckListItem(key, value ?? string.Empty, groupName, isGroupHeader: false, OnRowToggleRequested);
            }

            rowsByKey[key] = row;
            itemRows.Add(row);
        }

        itemKeys = new HashSet<string>(source.Keys, StringComparer.Ordinal);
        UpdateChrome();
        ApplyFilter(keepScrollPosition);
    }

    private void ApplyFilter(bool keepScrollPosition)
    {
        var rows = BuildVisibleRows(FilterText);

        if (!rows.SequenceEqual(visibleRows))
        {
            ReplaceVisibleRows(rows, keepScrollPosition);
        }

        RefreshStates();
    }

    private List<CheckListItem> BuildVisibleRows(string? filter)
    {
        var rows = new List<CheckListItem>(itemRows.Count);
        var groupOrder = new List<string>();
        var groupMatches = new Dictionary<string, List<CheckListItem>>(StringComparer.Ordinal);

        foreach (var row in itemRows)
        {
            if (!KeySelectionHelper.MatchesFilter(row.DisplayValue, filter))
            {
                continue;
            }

            if (row.GroupName is null)
            {
                rows.Add(row);
                continue;
            }

            if (!groupMatches.TryGetValue(row.GroupName, out var list))
            {
                list = [];
                groupMatches[row.GroupName] = list;
                groupOrder.Add(row.GroupName);
            }

            list.Add(row);
        }

        foreach (var groupName in groupOrder)
        {
            if (!groupRowsByName.TryGetValue(groupName, out var header))
            {
                header = new CheckListItem(groupName, groupName, groupName, isGroupHeader: true, OnRowToggleRequested);
                groupRowsByName[groupName] = header;
            }

            rows.Add(header);
            rows.AddRange(groupMatches[groupName]);
        }

        return rows;
    }

    private void ReplaceVisibleRows(
        List<CheckListItem> rows,
        bool keepScrollPosition)
    {
        var scrollViewer = keepScrollPosition ? FindScrollViewer(PartList) : null;
        var offset = scrollViewer?.VerticalOffset ?? 0;

        visibleRows.SuppressOnChangedNotification = true;
        try
        {
            visibleRows.Clear();
            foreach (var row in rows)
            {
                visibleRows.Add(row);
            }
        }
        finally
        {
            visibleRows.SuppressOnChangedNotification = false;
        }

        if (scrollViewer is not null && offset > 0)
        {
            _ = Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                () => scrollViewer.ScrollToVerticalOffset(offset));
        }
    }

    private void RefreshStates()
    {
        if (PartList is null)
        {
            return;
        }

        StateRefreshCount++;

        selectedSet = new HashSet<string>((IEnumerable<string>?)SelectedKeys ?? Array.Empty<string>(), StringComparer.Ordinal);
        disabledSet = new HashSet<string>((IEnumerable<string>?)DisabledKeys ?? Array.Empty<string>(), StringComparer.Ordinal);

        var selectedCount = GetSelectedItemCount();
        var atLimit = MaxSelectionCount is { } max && selectedCount >= max;

        foreach (var row in itemRows)
        {
            var isChecked = selectedSet.Contains(row.Key);
            row.SetIsChecked(isChecked);
            row.IsEnabled = !disabledSet.Contains(row.Key) && (isChecked || !atLimit);
        }

        RefreshGroupHeaders();
        RefreshSelectAll();

        PartCount.Text = MaxSelectionCount is { } limit
            ? string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfMax2, selectedCount, limit)
            : string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOf2, selectedCount, itemRows.Count);

        PartNoMatches.Visibility = itemRows.Count > 0 && !visibleRows.Any(x => !x.IsGroupHeader)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void RefreshGroupHeaders()
    {
        foreach (var header in visibleRows.Where(x => x.IsGroupHeader))
        {
            var groupRows = GetShownToggleableItemRows(header.Key);
            var state = KeySelectionHelper.GetAggregateState(groupRows.Count(x => selectedSet.Contains(x.Key)), groupRows.Count);
            header.SetIsChecked(state);
            header.IsEnabled = IsGroupCheckable &&
                               groupRows.Count > 0 &&
                               (state == true || Fits(groupRows.Select(x => x.Key)));
        }
    }

    private void RefreshSelectAll()
    {
        var shownRows = GetShownToggleableItemRows();
        selectAllState = KeySelectionHelper.GetAggregateState(shownRows.Count(x => selectedSet.Contains(x.Key)), shownRows.Count);
        isSyncingSelectAll = true;
        try
        {
            PartSelectAll.SetCurrentValue(ToggleButton.IsCheckedProperty, selectAllState);
            PartSelectAll.SetCurrentValue(
                IsEnabledProperty,
                shownRows.Count > 0 && (selectAllState == true || Fits(shownRows.Select(x => x.Key))));
        }
        finally
        {
            isSyncingSelectAll = false;
        }
    }

    private void UpdateChrome()
    {
        if (PartList is null)
        {
            return;
        }

        PartSelectAllRow.Visibility = ShowSelectAllRow ? Visibility.Visible : Visibility.Collapsed;
        PartClear.Visibility = ShowClearButton ? Visibility.Visible : Visibility.Collapsed;
        var showFilter = IsFilterVisible ?? itemRows.Count > AutoFilterItemCount;
        PartFilter.Visibility = showFilter ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnRowToggleRequested(
        CheckListItem row,
        bool? requested)
    {
        if (row.IsGroupHeader)
        {
            ToggleGroup(row);
            return;
        }

        RequestToggle(row, requested == true);
    }

    private void RequestToggle(
        CheckListItem row,
        bool tick)
    {
        if (row.IsGroupHeader)
        {
            ToggleGroup(row);
            return;
        }

        if (!row.IsEnabled || (tick && !Fits([row.Key])))
        {
            RefuseToggle(row);
            return;
        }

        anchorKey = row.Key;
        var newSelection = tick
            ? KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [row.Key], [])
            : KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], [row.Key]);

        Commit(newSelection);
    }

    private void ToggleGroup(CheckListItem header)
    {
        if (!IsGroupCheckable || !header.IsEnabled)
        {
            RefuseToggle(header);
            return;
        }

        var keys = GetShownToggleableItemRows(header.Key)
            .Select(x => x.Key)
            .ToList();

        if (header.IsChecked == true)
        {
            Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], keys));
            return;
        }

        if (!Fits(keys))
        {
            RefuseToggle(header);
            return;
        }

        Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), keys, []));
    }

    private void RefuseToggle(CheckListItem row)
    {
        // A binding that pushed the refused value reads the real one back after its update finishes.
        _ = Dispatcher.BeginInvoke(DispatcherPriority.DataBind, row.RefreshIsChecked);
    }

    private void Commit(IReadOnlyList<string> newSelection)
    {
        var oldSelection = GetCurrentSelection();
        if (KeySelectionHelper.SequenceEquals(oldSelection, newSelection))
        {
            RefreshStates();
            return;
        }

        isWritingSelectedKeys = true;
        try
        {
            var target = SelectedKeys;
            if (target is null)
            {
                target = new ObservableCollectionEx<string>();
                SetCurrentValue(SelectedKeysProperty, target);
            }

            KeySelectionHelper.ApplyInPlace(target, newSelection);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }

        RefreshStates();

        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldSelection,
                SelectedKeys.ToList()));
    }

    private IReadOnlyList<string> GetCurrentSelection()
        => SelectedKeys is null
            ? []
            : SelectedKeys.ToList();

    private int GetSelectedItemCount()
        => selectedSet.Count(itemKeys.Contains);

    private bool Fits(IEnumerable<string> keysToTick)
    {
        if (MaxSelectionCount is not { } max)
        {
            return true;
        }

        var additional = keysToTick.Count(x => !selectedSet.Contains(x));
        return GetSelectedItemCount() + additional <= max;
    }

    private List<CheckListItem> GetShownToggleableItemRows(
        string? groupName = null)
        => visibleRows
            .Where(x => !x.IsGroupHeader &&
                        !disabledSet.Contains(x.Key) &&
                        (groupName is null || string.Equals(x.GroupName, groupName, StringComparison.Ordinal)))
            .ToList();

    private int IndexOfVisible(CheckListItem? row)
        => row is null ? -1 : visibleRows.IndexOf(row);

    private void OnSelectAllIsCheckedChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (isSyncingSelectAll)
        {
            return;
        }

        if (selectAllState == true)
        {
            var keys = GetShownToggleableItemRows().Select(x => x.Key);
            Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], keys));
        }
        else
        {
            SelectAll();
        }

        // Push the real state back: the check box may have moved to a state the selection does not have.
        RefreshStates();
    }

    private void OnClearClick(
        object sender,
        RoutedEventArgs e)
        => ClearSelection();

    private void OnItemKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Space ||
            sender is not ListBoxItem { DataContext: CheckListItem row })
        {
            return;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            ToggleRange(row);
        }
        else
        {
            RequestToggle(row, row.IsChecked != true);
        }

        e.Handled = true;
    }

    private void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.A &&
            Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
            e.OriginalSource is not TextBox)
        {
            SelectAll();
            e.Handled = true;
        }
    }

    private void OnFilterPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Down || visibleRows.Count == 0)
        {
            return;
        }

        PartList.ScrollIntoView(visibleRows[0]);
        PartList.UpdateLayout();
        if (PartList.ItemContainerGenerator.ContainerFromIndex(0) is ListBoxItem container)
        {
            container.Focus();
            e.Handled = true;
        }
    }

    private void OnListPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(PartList, source) is not ListBoxItem { DataContext: CheckListItem row } container)
        {
            return;
        }

        container.Focus();

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && !row.IsGroupHeader)
        {
            ToggleRange(row);
            e.Handled = true;
            return;
        }

        if (ItemStyle == CheckListItemStyle.Switch &&
            !row.IsGroupHeader &&
            FindAncestor<Inputs.ToggleSwitch>(source) is null)
        {
            if (row.IsEnabled)
            {
                RequestToggle(row, row.IsChecked != true);
            }

            e.Handled = true;
        }
    }

    private static T? FindAncestor<T>(DependencyObject? element)
        where T : DependencyObject
    {
        while (element is not null)
        {
            if (element is T match)
            {
                return match;
            }

            element = element is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        return null;
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            var descendant = FindScrollViewer(child);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return null;
    }
}