// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A joined row (or column) of segments for switching between options, similar to iOS UISegmentedControl or Material
/// Design segmented buttons. By default exactly one segment is selected; <see cref="SelectionMode"/> also allows none
/// (<see cref="SegmentedSelectionMode.SingleOrNone"/>) or any number of segments
/// (<see cref="SegmentedSelectionMode.Multiple"/>), e.g. weekdays or bold / italic / underline.
/// </summary>
/// <remarks>
/// <see cref="SelectedKeys"/> holds the keys (<see cref="SegmentedItem.Key"/>, or the content text) of the selected
/// segments in every mode. Changes made in the control update it in place and raise <see cref="SelectedKeysChanged"/>
/// once; changes made to it from outside (or a new instance) only re-sync the segments and raise no event.
/// </remarks>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(Panel))]
public sealed partial class Segmented : Control
{
    private readonly ObservableCollection<SegmentedItem> selectedItemsSource = [];
    private readonly List<Border> separators = [];
    private readonly SelectedKeysCleanup modeTrim;
    private Panel? itemsHost;
    private bool isUpdatingSelection;
    private bool isWritingSelectedKeys;
    private bool isSyncingFromKeys;
    private ObservableCollection<string>? subscribedSelectedKeys;
    private HashSet<string> itemKeySet = new(StringComparer.Ordinal);

    /// <summary>
    /// The index of the currently selected segment (-1 means none). In <see cref="SegmentedSelectionMode.Multiple"/>
    /// mode it is the most recently selected segment that is still selected.
    /// </summary>
    [DependencyProperty(
        DefaultValue = -1,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectedIndexChanged))]
    private int selectedIndex;

    /// <summary>
    /// The currently selected <see cref="SegmentedItem"/>, or null if none. In
    /// <see cref="SegmentedSelectionMode.Multiple"/> mode it is the most recently selected segment that is still selected.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedItemChanged))]
    private SegmentedItem? selectedItem;

    /// <summary>
    /// How many segments can be selected: exactly one (default), at most one, or any number.
    /// </summary>
    [DependencyProperty(
        DefaultValue = SegmentedSelectionMode.Single,
        PropertyChangedCallback = nameof(OnSelectionModeChanged))]
    private SegmentedSelectionMode selectionMode;

    /// <summary>
    /// The keys of the selected segments (<see cref="SegmentedItem.Key"/>, or the content text). Two-way by default;
    /// updated in place. Keys without a matching segment are kept.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// The direction the segments are laid out in; <see cref="System.Windows.Controls.Orientation.Vertical"/> stacks
    /// them and rounds the top of the first and the bottom of the last segment.
    /// </summary>
    [DependencyProperty(
        DefaultValue = Orientation.Horizontal,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Orientation orientation;

    /// <summary>
    /// Whether the segments form one joined shape (default); <see langword="false"/> shows separate pill buttons with
    /// <see cref="ItemSpacing"/> between them.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private bool isJoined;

    /// <summary>
    /// The space between two separate pill buttons; only used when <see cref="IsJoined"/> is <see langword="false"/>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 4d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double itemSpacing;

    /// <summary>
    /// Whether the segments show only their icon; the text becomes the tooltip and the UI Automation name.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemPresentationChanged))]
    private bool iconOnly;

    /// <summary>
    /// The size of the segments; the heights match the <see cref="ChipSize"/> values.
    /// </summary>
    [DependencyProperty(
        DefaultValue = SegmentedSize.Medium,
        PropertyChangedCallback = nameof(OnItemPresentationChanged))]
    private SegmentedSize size;

    /// <summary>
    /// The corner radius of the outer pill shape.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private CornerRadius cornerRadius;

    /// <summary>
    /// The background brush applied to the selected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedBackground;

    /// <summary>
    /// The foreground brush applied to the selected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedForeground;

    /// <summary>
    /// The background brush shown when hovering over an unselected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? hoverBackground;

    /// <summary>
    /// The brush used for vertical separators between segments.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSeparatorBrushChanged))]
    private Brush? separatorBrush;

    /// <summary>
    /// Whether all segments should share equal width.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private bool equalSegmentWidth;

    /// <summary>
    /// Occurs when the selected segment changes. In <see cref="SegmentedSelectionMode.Multiple"/> mode it is raised
    /// for each toggle, with the toggled segment as <see cref="SegmentedSelectionChangedEventArgs.NewItem"/>
    /// (check its <see cref="SegmentedItem.IsSelected"/> to see whether it was turned on or off).
    /// </summary>
    public event EventHandler<SegmentedSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when the control changes <see cref="SelectedKeys"/>: once per click, toggle, key press or
    /// <see cref="SelectionMode"/> change that changes the selection.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Gets the collection of segments.
    /// </summary>
    public ObservableCollection<SegmentedItem> Items { get; } = [];

    /// <summary>
    /// Gets the selected segments, in item order. Read-only; it changes with the selection.
    /// </summary>
    public ReadOnlyObservableCollection<SegmentedItem> SelectedItems { get; }

    static Segmented()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Segmented),
            new FrameworkPropertyMetadata(typeof(Segmented)));
    }

    /// <summary>Initializes a new instance of the <see cref="Segmented"/> class.</summary>
    public Segmented()
    {
        SelectedItems = new ReadOnlyObservableCollection<SegmentedItem>(selectedItemsSource);
        modeTrim = new SelectedKeysCleanup(this, SelectedKeysProperty, _ => TrimToSingleSelection());
        CornerRadius = new CornerRadius(4);
        Items.CollectionChanged += OnItemsCollectionChanged;
        Loaded += OnLoaded;
        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        itemsHost = GetTemplateChild("PART_ItemsHost") as Panel;
        RebuildVisualTree();
    }

    /// <summary>
    /// Re-syncs the selection after the key of a segment changed.
    /// </summary>
    internal void OnItemKeyChanged()
        => SyncAfterItemsChanged();

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new SegmentedAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (e.Handled || Items.Count == 0)
        {
            return;
        }

        var direction = GetDirection(FlowDirectionKeyHelper.ToLayoutKey(e.Key, FlowDirection));
        if (direction is null)
        {
            return;
        }

        if (SelectionMode == SegmentedSelectionMode.Multiple)
        {
            MoveFocus(GetItemIndex(e.OriginalSource as DependencyObject), direction.Value);
            e.Handled = true;
            return;
        }

        switch (direction.Value)
        {
            case int.MinValue:
                SelectFirstEnabled();
                break;
            case int.MaxValue:
                SelectLastEnabled();
                break;
            default:
                MoveSelection(direction.Value);
                break;
        }

        e.Handled = true;
    }

    private static void OnSelectedIndexChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.UpdateSelection((int)e.OldValue, (int)e.NewValue);
        }
    }

    private static void OnSelectedItemChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control && !control.isUpdatingSelection)
        {
            var newItem = e.NewValue as SegmentedItem;
            var newIndex = newItem is null ? -1 : control.Items.IndexOf(newItem);
            control.SetCurrentValue(SelectedIndexProperty, newIndex);
        }
    }

    private static void OnSelectionModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Segmented control)
        {
            return;
        }

        if ((SegmentedSelectionMode)e.NewValue == SegmentedSelectionMode.Multiple)
        {
            control.SyncItemsFromKeys();
            return;
        }

        // Leaving the multi-select mode keeps only the most recently selected segment. With a bound SelectedKeys the
        // trim waits for the bindings to settle: on a DataContext swap the new mode can arrive before the new keys,
        // and trimming at once would write into the previous view model's collection.
        control.ApplySingleSelectionFlags(control.SelectedIndex);
        control.modeTrim.Request(control.itemKeySet);
    }

    private void TrimToSingleSelection()
    {
        if (SelectionMode == SegmentedSelectionMode.Multiple)
        {
            return;
        }

        var item = GetItemAt(SelectedIndex);
        ApplySingleSelectionFlags(SelectedIndex);
        CommitKeys(item?.EffectiveKey is { } key ? [key] : []);
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Segmented control)
        {
            return;
        }

        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged -= control.OnSelectedKeysCollectionChanged;
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged += control.OnSelectedKeysCollectionChanged;
        }

        if (!control.isWritingSelectedKeys)
        {
            control.SyncFromExternalKeys();
        }
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.UpdateItemMetadata();
            control.RebuildVisualTree();
        }
    }

    private static void OnSeparatorBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.UpdateSeparators();
        }
    }

    private static void OnItemPresentationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.UpdateItemMetadata();
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        RebuildVisualTree();
    }

    private void OnItemsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (SegmentedItem item in e.OldItems)
            {
                item.Click -= OnItemClick;
                item.Owner = null;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (SegmentedItem item in e.NewItems)
            {
                item.Click += OnItemClick;
                item.Owner = this;
            }
        }

        UpdateItemMetadata();
        SyncAfterItemsChanged();
        RebuildVisualTree();
    }

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            SyncFromExternalKeys();
        }
    }

    private void UpdateItemMetadata()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            item.Index = i;
            item.IsFirst = i == 0;
            item.IsLast = i == Items.Count - 1;
            item.SetCurrentValue(SegmentedItem.IsJoinedProperty, IsJoined);
            item.SetCurrentValue(SegmentedItem.SizeProperty, Size);
            item.ApplyIconOnly(IconOnly);
            ApplyCornerRadius(item, i);
        }
    }

    private void UpdateSelection(
        int oldIndex,
        int newIndex)
    {
        if (isUpdatingSelection)
        {
            return;
        }

        isUpdatingSelection = true;

        var oldKeys = GetCurrentKeys();
        var keysChanged = false;

        try
        {
            var oldItem = GetItemAt(oldIndex);
            var newItem = GetItemAt(newIndex);

            if (SelectionMode == SegmentedSelectionMode.Multiple)
            {
                // Set from outside (e.g. a binding): select that segment too, keeping the others.
                if (newItem is { IsSelected: false, EffectiveKey: { } key } && !isSyncingFromKeys)
                {
                    keysChanged = WriteKeys(KeySelectionHelper.GetSelectionWith(oldKeys, [key], []));
                    ApplyMultipleSelectionFlags();
                }
            }
            else
            {
                ApplySingleSelectionFlags(newIndex);

                if (!isSyncingFromKeys)
                {
                    keysChanged = WriteKeys(newItem?.EffectiveKey is { } key ? [key] : []);
                }
            }

            SetCurrentValue(SelectedItemProperty, newItem);

            SelectionChanged?.Invoke(
                this,
                new SegmentedSelectionChangedEventArgs(oldIndex, newIndex, oldItem, newItem));
        }
        finally
        {
            isUpdatingSelection = false;
        }

        if (keysChanged)
        {
            RaiseSelectedKeysChanged(oldKeys);
        }
    }

    private void ApplySingleSelectionFlags(int selected)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].IsSelected = i == selected;
        }

        RefreshSelectedItems();
    }

    private void ApplyMultipleSelectionFlags()
    {
        var keys = new HashSet<string>(GetCurrentKeys(), StringComparer.Ordinal);
        foreach (var item in Items)
        {
            item.IsSelected = item.EffectiveKey is { } key && keys.Contains(key);
        }

        RefreshSelectedItems();
    }

    private void RefreshSelectedItems()
    {
        var selected = Items.Where(x => x.IsSelected).ToList();
        if (!selectedItemsSource.SequenceEqual(selected))
        {
            selectedItemsSource.Clear();
            foreach (var item in selected)
            {
                selectedItemsSource.Add(item);
            }
        }

        UpdateSeparators();
    }

    /// <summary>
    /// Multiple mode: applies <see cref="SelectedKeys"/> to the segments and points <see cref="SelectedIndex"/> at
    /// the most recently selected segment, without raising events.
    /// </summary>
    private void SyncItemsFromKeys()
    {
        ApplyMultipleSelectionFlags();

        var keys = GetCurrentKeys();
        var mostRecent = -1;
        for (var k = keys.Count - 1; k >= 0 && mostRecent < 0; k--)
        {
            mostRecent = IndexOfKey(keys[k]);
        }

        SetSelectedIndexSilently(mostRecent);
    }

    private void SyncFromExternalKeys()
    {
        if (SelectionMode == SegmentedSelectionMode.Multiple)
        {
            SyncItemsFromKeys();
            return;
        }

        var index = GetCurrentKeys()
            .Select(IndexOfKey)
            .FirstOrDefault(x => x >= 0, -1);

        if (index == SelectedIndex)
        {
            ApplySingleSelectionFlags(index);
            return;
        }

        isSyncingFromKeys = true;
        try
        {
            SetCurrentValue(SelectedIndexProperty, index);
        }
        finally
        {
            isSyncingFromKeys = false;
        }
    }

    /// <summary>
    /// Re-syncs the selection after the segments or their keys changed. The selection follows the keys, not the
    /// positions: keys of segments that are gone are dropped (with one <see cref="SelectedKeysChanged"/>), keys that
    /// never had a segment are kept so segments added later pick them up.
    /// </summary>
    private void SyncAfterItemsChanged()
    {
        var oldItemKeys = itemKeySet;
        itemKeySet = new HashSet<string>(
            Items.Select(x => x.EffectiveKey).OfType<string>(),
            StringComparer.Ordinal);

        var oldKeys = GetCurrentKeys();
        var keys = oldKeys
            .Where(x => !oldItemKeys.Contains(x) || itemKeySet.Contains(x))
            .ToList();
        var hasStaleKeys = keys.Count != oldKeys.Count;

        if (SelectionMode == SegmentedSelectionMode.Multiple)
        {
            var multipleChanged = WriteKeys(keys);
            SyncItemsFromKeys();
            if (multipleChanged)
            {
                RaiseSelectedKeysChanged(oldKeys);
            }

            return;
        }

        if (GetSingleIndexAfterItemsChanged(keys, hasStaleKeys) is not { } index)
        {
            // Nothing to select yet: keep SelectedIndex for segments added later.
            ApplySingleSelectionFlags(-1);
            SetSelectedItemSilently(null);
            return;
        }

        var item = GetItemAt(index);
        if (item?.EffectiveKey is { } key)
        {
            keys = [key];
        }

        SetSelectedIndexSilently(index);
        ApplySingleSelectionFlags(index);
        if (WriteKeys(keys))
        {
            RaiseSelectedKeysChanged(oldKeys);
        }
    }

    /// <summary>
    /// Single modes: returns the index to select after the segments changed (-1 for none), or
    /// <see langword="null"/> when there is nothing to select yet.
    /// </summary>
    private int? GetSingleIndexAfterItemsChanged(
        IReadOnlyList<string> keys,
        bool hasStaleKeys)
    {
        var index = keys
            .Select(IndexOfKey)
            .FirstOrDefault(x => x >= 0, -1);
        if (index >= 0)
        {
            return index;
        }

        if (hasStaleKeys)
        {
            // The key of the selected segment is gone: keep the segment when it is still there (its key changed).
            return SelectedItem is { } selected
                ? Items.IndexOf(selected)
                : -1;
        }

        // SelectedIndex may have been set before the segments were added.
        return GetItemAt(SelectedIndex) is not null
            ? SelectedIndex
            : null;
    }

    private void SetSelectedIndexSilently(int index)
    {
        isUpdatingSelection = true;
        try
        {
            SetCurrentValue(SelectedIndexProperty, index);
            SetCurrentValue(SelectedItemProperty, GetItemAt(index));
        }
        finally
        {
            isUpdatingSelection = false;
        }
    }

    private void SetSelectedItemSilently(SegmentedItem? item)
    {
        isUpdatingSelection = true;
        try
        {
            SetCurrentValue(SelectedItemProperty, item);
        }
        finally
        {
            isUpdatingSelection = false;
        }
    }

    private void CommitKeys(IReadOnlyList<string> newKeys)
    {
        var oldKeys = GetCurrentKeys();
        if (WriteKeys(newKeys))
        {
            RaiseSelectedKeysChanged(oldKeys);
        }
    }

    /// <summary>
    /// Writes <paramref name="newKeys"/> into <see cref="SelectedKeys"/> in place; returns whether it changed.
    /// </summary>
    private bool WriteKeys(IReadOnlyList<string> newKeys)
    {
        if (KeySelectionHelper.SequenceEquals(GetCurrentKeys(), newKeys))
        {
            return false;
        }

        isWritingSelectedKeys = true;
        try
        {
            var target = SelectedKeys;
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

        return true;
    }

    private void RaiseSelectedKeysChanged(IReadOnlyList<string> oldKeys)
        => SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldKeys,
                GetCurrentKeys()));

    private IReadOnlyList<string> GetCurrentKeys()
        => SelectedKeys is null
            ? []
            : SelectedKeys.ToList();

    private int IndexOfKey(string key)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (string.Equals(Items[i].EffectiveKey, key, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private SegmentedItem? GetItemAt(int index)
        => index >= 0 && index < Items.Count
            ? Items[index]
            : null;

    private void ToggleItem(SegmentedItem item)
    {
        if (item.EffectiveKey is not { } key)
        {
            return;
        }

        var oldKeys = GetCurrentKeys();
        var oldIndex = SelectedIndex;
        var oldItem = SelectedItem;

        var newKeys = item.IsSelected
            ? KeySelectionHelper.GetSelectionWith(oldKeys, [], [key])
            : KeySelectionHelper.GetSelectionWith(oldKeys, [key], []);

        if (!WriteKeys(newKeys))
        {
            return;
        }

        SyncItemsFromKeys();

        SelectionChanged?.Invoke(
            this,
            new SegmentedSelectionChangedEventArgs(oldIndex, Items.IndexOf(item), oldItem, item));

        RaiseSelectedKeysChanged(oldKeys);
    }

    private void RebuildVisualTree()
    {
        if (itemsHost is null)
        {
            return;
        }

        itemsHost.Children.Clear();
        separators.Clear();

        if (itemsHost is Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }

        if (Items.Count == 0)
        {
            return;
        }

        var isVertical = Orientation == Orientation.Vertical;

        if (EqualSegmentWidth && itemsHost is Grid itemsGrid)
        {
            FillGrid(itemsGrid, isVertical);
        }
        else if (itemsHost is StackPanel stackPanel)
        {
            FillStackPanel(stackPanel, isVertical);
        }

        UpdateSeparators();
    }

    private void FillGrid(
        Grid grid,
        bool isVertical)
    {
        var slot = 0;
        for (var i = 0; i < Items.Count; i++)
        {
            // Slot for the item: equal width side by side, auto height when stacked.
            AddSlot(grid, isVertical, isVertical ? GridLength.Auto : new GridLength(1, GridUnitType.Star));
            PlaceInSlot(grid, Items[i], slot++, isVertical);

            // Slot for the separator or spacing (except after last item)
            if (i == Items.Count - 1)
            {
                continue;
            }

            if (IsJoined)
            {
                AddSlot(grid, isVertical, GridLength.Auto);
                PlaceInSlot(grid, CreateSeparator(isVertical), slot++, isVertical);
            }
            else
            {
                AddSlot(grid, isVertical, new GridLength(System.Math.Max(0, ItemSpacing)));
                slot++;
            }
        }
    }

    private void FillStackPanel(
        StackPanel stackPanel,
        bool isVertical)
    {
        stackPanel.Orientation = Orientation;
        for (var i = 0; i < Items.Count; i++)
        {
            stackPanel.Children.Add(Items[i]);

            if (i < Items.Count - 1)
            {
                stackPanel.Children.Add(IsJoined
                    ? CreateSeparator(isVertical)
                    : CreateSpacer(isVertical));
            }
        }
    }

    private static void AddSlot(
        Grid grid,
        bool isVertical,
        GridLength length)
    {
        if (isVertical)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = length });
        }
        else
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
        }
    }

    private static void PlaceInSlot(
        Grid grid,
        UIElement element,
        int slot,
        bool isVertical)
    {
        Grid.SetRow(element, isVertical ? slot : 0);
        Grid.SetColumn(element, isVertical ? 0 : slot);
        grid.Children.Add(element);
    }

    private void ApplyCornerRadius(
        SegmentedItem item,
        int itemIndex)
    {
        var cr = CornerRadius;

        if (Items.Count == 1 || !IsJoined)
        {
            item.Tag = cr;
        }
        else if (itemIndex == 0)
        {
            item.Tag = Orientation == Orientation.Vertical
                ? new CornerRadius(cr.TopLeft, cr.TopRight, 0, 0)
                : new CornerRadius(cr.TopLeft, 0, 0, cr.BottomLeft);
        }
        else if (itemIndex == Items.Count - 1)
        {
            item.Tag = Orientation == Orientation.Vertical
                ? new CornerRadius(0, 0, cr.BottomRight, cr.BottomLeft)
                : new CornerRadius(0, cr.TopRight, cr.BottomRight, 0);
        }
        else
        {
            item.Tag = new CornerRadius(0);
        }
    }

    private Border CreateSeparator(bool isVertical)
    {
        var separator = isVertical
            ? new Border
            {
                Height = 1,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(4, 0, 4, 0),
            }
            : new Border
            {
                Width = 1,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 4, 0, 4),
            };

        separators.Add(separator);
        return separator;
    }

    private Border CreateSpacer(bool isVertical)
        => isVertical
            ? new Border { Height = System.Math.Max(0, ItemSpacing) }
            : new Border { Width = System.Math.Max(0, ItemSpacing) };

    /// <summary>
    /// Hides each separator next to a selected segment.
    /// </summary>
    private void UpdateSeparators()
    {
        for (var i = 0; i < separators.Count && i + 1 < Items.Count; i++)
        {
            var isAdjacentToSelected = Items[i].IsSelected || Items[i + 1].IsSelected;
            separators[i].Background = isAdjacentToSelected
                ? Brushes.Transparent
                : (SeparatorBrush ?? Brushes.Gray);
        }
    }

    private void OnItemClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not SegmentedItem item)
        {
            return;
        }

        var index = Items.IndexOf(item);
        if (index < 0)
        {
            return;
        }

        switch (SelectionMode)
        {
            case SegmentedSelectionMode.Multiple:
                ToggleItem(item);
                break;
            case SegmentedSelectionMode.SingleOrNone when index == SelectedIndex:
                SetCurrentValue(SelectedIndexProperty, -1);
                break;
            default:
                SetCurrentValue(SelectedIndexProperty, index);
                break;
        }
    }

    /// <summary>
    /// Maps a (layout) key to a move: -1 / +1 for previous / next, <see cref="int.MinValue"/> for the first and
    /// <see cref="int.MaxValue"/> for the last segment; <see langword="null"/> for other keys.
    /// </summary>
    private int? GetDirection(Key key)
        => key switch
        {
            Key.Left => -1,
            Key.Right => 1,
            Key.Up when Orientation == Orientation.Vertical => -1,
            Key.Down when Orientation == Orientation.Vertical => 1,
            Key.Home => int.MinValue,
            Key.End => int.MaxValue,
            _ => null,
        };

    private int GetItemIndex(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is SegmentedItem item)
            {
                return Items.IndexOf(item);
            }

            if (ReferenceEquals(element, this))
            {
                break;
            }

            element = element is Visual
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        return SelectedIndex;
    }

    private void MoveFocus(
        int from,
        int direction)
    {
        var target = direction switch
        {
            int.MinValue => FindEnabled(-1, 1),
            int.MaxValue => FindEnabled(Items.Count, -1),
            _ => FindEnabled(from, direction),
        };

        target?.Focus();
    }

    private SegmentedItem? FindEnabled(
        int from,
        int direction)
    {
        var next = from + direction;
        while (next >= 0 && next < Items.Count)
        {
            if (Items[next].IsEnabled)
            {
                return Items[next];
            }

            next += direction;
        }

        return null;
    }

    private void MoveSelection(int direction)
    {
        if (FindEnabled(SelectedIndex, direction) is { } item)
        {
            SetCurrentValue(SelectedIndexProperty, Items.IndexOf(item));
        }
    }

    private void SelectFirstEnabled()
    {
        if (FindEnabled(-1, 1) is { } item)
        {
            SetCurrentValue(SelectedIndexProperty, Items.IndexOf(item));
        }
    }

    private void SelectLastEnabled()
    {
        if (FindEnabled(Items.Count, -1) is { } item)
        {
            SetCurrentValue(SelectedIndexProperty, Items.IndexOf(item));
        }
    }
}