// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051
using DualListSelectorStrings = Atc.Wpf.Components.Resources.DualListSelector;

namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// A dual-list transfer control that lets users move items between an Available list
/// and a Selected list, with support for reordering.
/// </summary>
[TemplatePart(Name = PART_AvailableListBox, Type = typeof(ListBox))]
[TemplatePart(Name = PART_SelectedListBox, Type = typeof(ListBox))]
[TemplatePart(Name = PART_AvailableFilterTextBox, Type = typeof(TextBox))]
[TemplatePart(Name = PART_SelectedFilterTextBox, Type = typeof(TextBox))]
[TemplatePart(Name = PART_AvailableItemCount, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_SelectedItemCount, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_AvailableEmptyText, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_SelectedEmptyText, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_AvailableSelectAll, Type = typeof(CheckBox))]
[TemplatePart(Name = PART_SelectedSelectAll, Type = typeof(CheckBox))]
[TemplatePart(Name = PART_AvailableHeaderCount, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_SelectedHeaderCount, Type = typeof(TextBlock))]
public sealed partial class DualListSelector : Control
{
    private const string PART_AvailableSelectAll = "PART_AvailableSelectAll";
    private const string PART_SelectedSelectAll = "PART_SelectedSelectAll";
    private const string PART_AvailableHeaderCount = "PART_AvailableHeaderCount";
    private const string PART_SelectedHeaderCount = "PART_SelectedHeaderCount";

    private const string PART_AvailableListBox = "PART_AvailableListBox";
    private const string PART_SelectedListBox = "PART_SelectedListBox";
    private const string PART_AvailableFilterTextBox = "PART_AvailableFilterTextBox";
    private const string PART_SelectedFilterTextBox = "PART_SelectedFilterTextBox";
    private const string PART_AvailableItemCount = "PART_AvailableItemCount";
    private const string PART_SelectedItemCount = "PART_SelectedItemCount";
    private const string PART_AvailableEmptyText = "PART_AvailableEmptyText";
    private const string PART_SelectedEmptyText = "PART_SelectedEmptyText";

    private ListBox? availableListBox;
    private ListBox? selectedListBox;
    private ListCollectionView? availableView;
    private ListCollectionView? selectedView;
    private List<DualListSelectorItem>? keptAvailableHighlight;
    private List<DualListSelectorItem>? keptSelectedHighlight;
    private TextBox? availableFilterTextBox;
    private TextBox? selectedFilterTextBox;
    private TextBlock? availableItemCount;
    private TextBlock? selectedItemCount;
    private TextBlock? availableEmptyText;
    private TextBlock? selectedEmptyText;
    private CheckBox? availableSelectAll;
    private CheckBox? selectedSelectAll;
    private TextBlock? availableHeaderCount;
    private TextBlock? selectedHeaderCount;
    private bool isGroupingPending;
    private bool isSyncingSelectAll;
    private int transferDepth;
    private Point? dragStartPoint;
    private ListBox? dragSourceListBox;
    private DragDropAdorner? dragAdorner;
    private DropTargetInsertionAdorner? dropIndicatorAdorner;

    /// <summary>
    /// The source list of available items.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback = nameof(OnItemsCollectionChanged))]
    private ObservableCollection<DualListSelectorItem> availableItems = [];

    /// <summary>
    /// The target list of selected items.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback = nameof(OnItemsCollectionChanged))]
    private ObservableCollection<DualListSelectorItem> selectedItems = [];

    /// <summary>
    /// Controls left/right placement of Available and Selected lists.
    /// </summary>
    [DependencyProperty(DefaultValue = default(DualListSelectorLayoutMode))]
    private DualListSelectorLayoutMode layoutMode;

    /// <summary>
    /// Header content above the available list.
    /// </summary>
    [DependencyProperty]
    private object? availableHeader;

    /// <summary>
    /// Header content above the selected list.
    /// </summary>
    [DependencyProperty]
    private object? selectedHeader;

    /// <summary>
    /// Minimum height of each ListBox.
    /// </summary>
    [DependencyProperty(DefaultValue = 200d)]
    private double listMinHeight;

    /// <summary>
    /// Minimum width of each ListBox.
    /// </summary>
    [DependencyProperty(DefaultValue = 200d)]
    private double listMinWidth;

    /// <summary>
    /// Whether to show the reorder buttons.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showReorderButtons;

    /// <summary>
    /// Whether to show the "Move All" transfer buttons.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showTransferAllButtons;

    /// <summary>
    /// Whether to auto-recalculate SortOrderNumber on every move.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool autoRecalculateSortOrder;

    /// <summary>
    /// Custom item DataTemplate for both ListBoxes.
    /// When null, the built-in default template is used.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Maximum height for each ListBox, enabling scrolling when exceeded.
    /// </summary>
    [DependencyProperty]
    private double listMaxHeight;

    /// <summary>
    /// Whether to show a filter TextBox above each ListBox.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool showFilter;

    /// <summary>
    /// Whether to allow drag-and-drop between lists and reordering within the Selected list.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool allowDragDrop;

    /// <summary>
    /// Whether to allow selecting multiple items in the list boxes.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool allowMultiSelect;

    /// <summary>
    /// Width of the transfer and reorder button columns.
    /// </summary>
    [DependencyProperty(DefaultValue = 48d)]
    private double buttonAreaWidth;

    /// <summary>
    /// Footer content below the available list.
    /// </summary>
    [DependencyProperty]
    private object? availableFooter;

    /// <summary>
    /// Footer content below the selected list.
    /// </summary>
    [DependencyProperty]
    private object? selectedFooter;

    /// <summary>
    /// Whether to show an item count below each list.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnStateChanged))]
    private bool showItemCount;

    /// <summary>
    /// How the rows show that they are picked: highlighted (the default) or with a check box per row
    /// and a tri-state "Select all" check box with a "checked / total" count in each list header.
    /// </summary>
    [DependencyProperty(DefaultValue = DualListItemSelectionStyle.Highlight, PropertyChangedCallback = nameof(OnStateChanged))]
    private DualListItemSelectionStyle itemSelectionStyle;

    /// <summary>
    /// The format of the "checked / total" count next to each "Select all" check box when
    /// <see cref="ItemSelectionStyle"/> is <see cref="DualListItemSelectionStyle.CheckBox"/>:
    /// <c>{0}</c> is the number of checked rows and <c>{1}</c> the number of shown rows.
    /// <see langword="null"/> uses the translated default <c>"{0}/{1}"</c>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged))]
    private string? headerCountFormat;

    /// <summary>
    /// Whether the selected list is just "what was picked": the "move to available" buttons are hidden,
    /// each selected row gets a × button and <c>Delete</c> removes the highlighted rows.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnStateChanged))]
    private bool isOneWay;

    /// <summary>
    /// The maximum number of items in the selected list; <see langword="null"/> for no limit.
    /// A transfer that would exceed the limit is blocked (buttons disabled, drop refused),
    /// and the selected list footer shows "{0} of max {1}".
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged))]
    private int? maxSelectedCount;

    /// <summary>
    /// Whether both lists virtualize their rows (with container recycling), so lists of thousands of items stay fast.
    /// Virtualization needs a bounded list height, e.g. <see cref="ListMaxHeight"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnIsVirtualizingChanged))]
    private bool isVirtualizing;

    /// <summary>
    /// The item property to group both lists by (e.g. <c>"Tag"</c> or a property of a derived item);
    /// <see langword="null"/> groups by <see cref="DualListSelectorItem.Group"/> when an item has one.
    /// The filter keeps only the groups with matches.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnGroupMemberPathChanged))]
    private string? groupMemberPath;

    /// <summary>
    /// Custom header template for the available list.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? availableHeaderTemplate;

    /// <summary>
    /// Custom header template for the selected list.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? selectedHeaderTemplate;

    /// <summary>
    /// Initializes a new instance of the <see cref="DualListSelector"/> class.
    /// </summary>
    public DualListSelector()
    {
        // Range-capable, so a transfer of many items reaches the lists as one Reset instead of one event per item.
        AvailableItems = new ObservableCollectionEx<DualListSelectorItem>();
        SelectedItems = new ObservableCollectionEx<DualListSelectorItem>();
        ListMaxHeight = double.PositiveInfinity;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets how many times the item counts, empty texts and header counts were brought up to date (a test hook for bulk transfers).
    /// </summary>
    internal int ItemCountUpdateCount { get; private set; }

    /// <summary>
    /// Occurs when items are transferred between lists.
    /// </summary>
    public event EventHandler<DualListSelectorItemsTransferredEventArgs>? ItemsTransferred;

    /// <summary>
    /// Occurs when items are reordered in the Selected list.
    /// </summary>
    public event EventHandler<DualListSelectorItemsReorderedEventArgs>? ItemsReordered;

    /// <summary>
    /// Occurs when the selection changes in either list box.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Command registrations.")]
    static DualListSelector()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DualListSelector),
            new FrameworkPropertyMetadata(typeof(DualListSelector)));

        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveToSelected,
                OnMoveToSelectedExecuted,
                OnMoveToSelectedCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveToAvailable,
                OnMoveToAvailableExecuted,
                OnMoveToAvailableCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveAllToSelected,
                OnMoveAllToSelectedExecuted,
                OnMoveAllToSelectedCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveAllToAvailable,
                OnMoveAllToAvailableExecuted,
                OnMoveAllToAvailableCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveToTop,
                OnMoveToTopExecuted,
                OnMoveToTopCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveUp,
                OnMoveUpExecuted,
                OnMoveUpCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveDown,
                OnMoveDownExecuted,
                OnMoveDownCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.MoveToBottom,
                OnMoveToBottomExecuted,
                OnMoveToBottomCanExecute));
        CommandManager.RegisterClassCommandBinding(
            typeof(DualListSelector),
            new CommandBinding(
                DualListSelectorCommands.RemoveFromSelected,
                OnRemoveFromSelectedExecuted,
                OnRemoveFromSelectedCanExecute));
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Template part wiring.")]
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        availableListBox = GetTemplateChild(PART_AvailableListBox) as ListBox;
        selectedListBox = GetTemplateChild(PART_SelectedListBox) as ListBox;
        availableFilterTextBox = GetTemplateChild(PART_AvailableFilterTextBox) as TextBox;
        selectedFilterTextBox = GetTemplateChild(PART_SelectedFilterTextBox) as TextBox;
        availableItemCount = GetTemplateChild(PART_AvailableItemCount) as TextBlock;
        selectedItemCount = GetTemplateChild(PART_SelectedItemCount) as TextBlock;
        availableEmptyText = GetTemplateChild(PART_AvailableEmptyText) as TextBlock;
        selectedEmptyText = GetTemplateChild(PART_SelectedEmptyText) as TextBlock;
        availableSelectAll = GetTemplateChild(PART_AvailableSelectAll) as CheckBox;
        selectedSelectAll = GetTemplateChild(PART_SelectedSelectAll) as CheckBox;
        availableHeaderCount = GetTemplateChild(PART_AvailableHeaderCount) as TextBlock;
        selectedHeaderCount = GetTemplateChild(PART_SelectedHeaderCount) as TextBlock;

        if (availableSelectAll is not null)
        {
            availableSelectAll.Checked += OnSelectAllToggled;
            availableSelectAll.Unchecked += OnSelectAllToggled;
            availableSelectAll.Indeterminate += OnSelectAllToggled;
        }

        if (selectedSelectAll is not null)
        {
            selectedSelectAll.Checked += OnSelectAllToggled;
            selectedSelectAll.Unchecked += OnSelectAllToggled;
            selectedSelectAll.Indeterminate += OnSelectAllToggled;
        }

        if (availableListBox is not null)
        {
            availableListBox.ItemsSource = availableView;
            availableListBox.SelectionChanged += OnListBoxSelectionChanged;
            availableListBox.MouseDoubleClick += OnAvailableListBoxDoubleClick;
        }

        if (selectedListBox is not null)
        {
            selectedListBox.ItemsSource = selectedView;
            selectedListBox.SelectionChanged += OnListBoxSelectionChanged;
            selectedListBox.MouseDoubleClick += OnSelectedListBoxDoubleClick;
        }

        if (availableFilterTextBox is not null)
        {
            availableFilterTextBox.TextChanged += OnAvailableFilterTextChanged;
        }

        if (selectedFilterTextBox is not null)
        {
            selectedFilterTextBox.TextChanged += OnSelectedFilterTextChanged;
        }

        SetupDragDrop();
        ApplyVirtualization();
        ApplyGrouping();
        UpdateItemCounts();
    }

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);

        if (e.Handled)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Enter when IsSourcedFrom(e, availableListBox) && CanMoveToSelected():
                MoveToSelected();
                e.Handled = true;
                break;
            case Key.Enter when IsSourcedFrom(e, selectedListBox) && CanMoveToAvailable():
                MoveToAvailable();
                e.Handled = true;
                break;
            case Key.Delete when IsOneWay && IsSourcedFrom(e, selectedListBox) && CanMoveToAvailable():
                TransferToAvailable(GetHighlighted(selectedListBox), clearFilters: false, focusAvailable: false);
                e.Handled = true;
                break;
            case Key.Up when Keyboard.Modifiers == ModifierKeys.Control &&
                             IsSourcedFrom(e, selectedListBox) && CanMoveUp():
                MoveUp();
                e.Handled = true;
                break;
            case Key.Down when Keyboard.Modifiers == ModifierKeys.Control &&
                               IsSourcedFrom(e, selectedListBox) && CanMoveDown():
                MoveDown();
                e.Handled = true;
                break;
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DualListSelectorAutomationPeer(this);

    internal void MoveToSelected()
        => TransferToSelected(GetHighlighted(availableListBox), insertIndex: -1, clearFilters: false);

    internal void MoveToAvailable()
        => TransferToAvailable(GetHighlighted(selectedListBox), clearFilters: false, focusAvailable: true);

    internal void MoveAllToSelected()
        => TransferToSelected(GetMoveAllCandidates(availableListBox, AvailableItems), insertIndex: -1, clearFilters: true);

    internal void MoveAllToAvailable()
        => TransferToAvailable(GetMoveAllCandidates(selectedListBox, SelectedItems), clearFilters: true, focusAvailable: true);

    /// <summary>
    /// Returns one item from the selected list to the available list (the × button in one-way mode). Locked items stay.
    /// </summary>
    internal void RemoveFromSelected(DualListSelectorItem item)
    {
        if (CanRemoveFromSelected(item))
        {
            TransferToAvailable([item], clearFilters: false, focusAvailable: false);
        }
    }

    internal bool CanRemoveFromSelected(object? parameter)
        => parameter is DualListSelectorItem item &&
           CanLeaveSelected(item) &&
           SelectedItems.Contains(item);

    /// <summary>
    /// Whether dropping <paramref name="items"/> on the selected list is allowed: a transfer from the available list
    /// must fit <see cref="MaxSelectedCount"/>; a reorder inside the selected list is always allowed.
    /// </summary>
    internal bool CanDropOnSelected(
        IReadOnlyCollection<DualListSelectorItem> items,
        bool fromAvailable)
        => !fromAvailable || FitsLimit(items.Count(i => i.IsEnabled));

    /// <summary>
    /// Handles a drop on the selected list: a transfer from the available list (inserted at the drop slot),
    /// or the reorder of one item inside the selected list.
    /// </summary>
    /// <param name="items">The dragged items.</param>
    /// <param name="fromAvailable">Whether the drag started in the available list.</param>
    /// <param name="viewInsertIndex">The drop slot ("insert before the shown row at this index") in the shown, filtered and grouped, rows.</param>
    internal void DropOnSelected(
        IReadOnlyList<DualListSelectorItem> items,
        bool fromAvailable,
        int viewInsertIndex)
    {
        if (items.Count == 0 || selectedListBox is null)
        {
            return;
        }

        var insertIndex = ToCollectionInsertIndex(selectedListBox, SelectedItems, viewInsertIndex);

        if (fromAvailable)
        {
            if (CanDropOnSelected(items, fromAvailable: true))
            {
                TransferToSelected(items, insertIndex, clearFilters: false);
            }

            return;
        }

        if (items.Count != 1)
        {
            return;
        }

        var item = items[0];
        var oldIndex = SelectedItems.IndexOf(item);
        var dropIndex = GetReorderTargetIndex(oldIndex, insertIndex, SelectedItems.Count);

        if (oldIndex != dropIndex && oldIndex >= 0)
        {
            SelectedItems.Move(oldIndex, dropIndex);
            selectedListBox.SelectedItem = item;
            RecalculateSortOrder();
            ItemsReordered?.Invoke(
                this,
                new DualListSelectorItemsReorderedEventArgs(item, oldIndex, dropIndex));
        }

        selectedListBox.Focus();
    }

    /// <summary>
    /// Handles a drop on the available list: items dragged from the selected list go back, except locked ones.
    /// </summary>
    internal void DropOnAvailable(
        IReadOnlyList<DualListSelectorItem> items,
        bool fromSelected)
    {
        if (fromSelected)
        {
            TransferToAvailable(items, clearFilters: false, focusAvailable: true);
        }
    }

    /// <summary>
    /// The "Select all" check box of a list: checks every shown, enabled row, or unchecks all when they all are checked.
    /// </summary>
    internal void ToggleSelectAll(ListBox? listBox)
    {
        if (listBox is null || listBox.SelectionMode == SelectionMode.Single)
        {
            return;
        }

        var shownEnabled = listBox.Items.OfType<DualListSelectorItem>().Count(i => i.IsEnabled);
        var checkedEnabled = listBox.SelectedItems.OfType<DualListSelectorItem>().Count(i => i.IsEnabled);

        if (shownEnabled > 0 && checkedEnabled >= shownEnabled)
        {
            listBox.UnselectAll();
        }
        else
        {
            listBox.SelectAll();
            foreach (var disabled in listBox.SelectedItems.OfType<DualListSelectorItem>().Where(i => !i.IsEnabled).ToList())
            {
                listBox.SelectedItems.Remove(disabled);
            }
        }

        UpdateHeaderCounts();
    }

    private bool TransferToSelected(
        IEnumerable<DualListSelectorItem> candidates,
        int insertIndex,
        bool clearFilters)
    {
        var itemsToMove = candidates
            .Where(i => i.IsEnabled)
            .ToList();

        if (itemsToMove.Count == 0 || !FitsLimit(itemsToMove.Count))
        {
            return false;
        }

        Transfer(itemsToMove, availableListBox, AvailableItems, SelectedItems, insertIndex, clearFilters);
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToSelected);
        selectedListBox?.Focus();
        return true;
    }

    private bool TransferToAvailable(
        IEnumerable<DualListSelectorItem> candidates,
        bool clearFilters,
        bool focusAvailable)
    {
        var itemsToMove = candidates
            .Where(CanLeaveSelected)
            .ToList();

        if (itemsToMove.Count == 0)
        {
            return false;
        }

        Transfer(itemsToMove, selectedListBox, SelectedItems, AvailableItems, insertIndex: -1, clearFilters);
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToAvailable);
        if (focusAvailable)
        {
            availableListBox?.Focus();
        }
        else
        {
            selectedListBox?.Focus();
        }

        return true;
    }

    /// <summary>
    /// Moves <paramref name="itemsToMove"/> (in that order) from <paramref name="source"/> to <paramref name="target"/>,
    /// at <paramref name="insertIndex"/> or at the end. The counts are brought up to date once, after the move,
    /// and each list gets one Reset when its collection is an <see cref="ObservableCollectionEx{T}"/>.
    /// </summary>
    private void Transfer(
        List<DualListSelectorItem> itemsToMove,
        ListBox? sourceListBox,
        ObservableCollection<DualListSelectorItem> source,
        ObservableCollection<DualListSelectorItem> target,
        int insertIndex,
        bool clearFilters)
    {
        var moving = new HashSet<DualListSelectorItem>(itemsToMove, ReferenceEqualityComparer.Instance);

        transferDepth++;
        try
        {
            var keptHighlight = DropHighlight(sourceListBox, moving);
            RemoveItems(source, moving);
            InsertItems(target, itemsToMove, insertIndex);
            RestoreHighlight(sourceListBox, keptHighlight);

            if (clearFilters)
            {
                ClearFilters();
            }

            RecalculateSortOrder();
        }
        finally
        {
            transferDepth--;
        }

        UpdateItemCounts();
    }

    /// <summary>
    /// Unhighlights the rows about to leave <paramref name="listBox"/> in one selection change (rather than one per
    /// removed row) and returns the highlighted rows that stay.
    /// </summary>
    private static List<DualListSelectorItem> DropHighlight(
        ListBox? listBox,
        HashSet<DualListSelectorItem> moving)
    {
        if (listBox is null || listBox.SelectedItems.Count == 0)
        {
            return [];
        }

        var kept = listBox.SelectedItems
            .Cast<DualListSelectorItem>()
            .Where(i => !moving.Contains(i))
            .ToList();
        if (kept.Count == listBox.SelectedItems.Count)
        {
            // No highlighted row moves (e.g. the × of a row that is not highlighted): the selection stays as it is.
            return [];
        }

        listBox.UnselectAll();
        return kept;
    }

    private static void RestoreHighlight(
        ListBox? listBox,
        List<DualListSelectorItem> kept)
    {
        if (listBox is null || kept.Count == 0)
        {
            return;
        }

        if (listBox.SelectionMode == SelectionMode.Single)
        {
            listBox.SelectedItem = kept[0];
            return;
        }

        foreach (var item in kept)
        {
            listBox.SelectedItems.Add(item);
        }
    }

    private static void RemoveItems(
        ObservableCollection<DualListSelectorItem> items,
        HashSet<DualListSelectorItem> moving)
    {
        var indexes = new List<int>(moving.Count);
        for (var i = 0; i < items.Count; i++)
        {
            if (moving.Contains(items[i]))
            {
                indexes.Add(i);
            }
        }

        if (indexes.Count == 0)
        {
            return;
        }

        if (items is ObservableCollectionEx<DualListSelectorItem> rangeItems)
        {
            ReplaceAll(rangeItems, rangeItems.Where(i => !moving.Contains(i)).ToList());
        }
        else if (indexes.Count == items.Count)
        {
            items.Clear();
        }
        else
        {
            // From the end, so the earlier indexes stay valid and each removal only shifts the tail.
            for (var i = indexes.Count - 1; i >= 0; i--)
            {
                items.RemoveAt(indexes[i]);
            }
        }
    }

    private static void InsertItems(
        ObservableCollection<DualListSelectorItem> items,
        List<DualListSelectorItem> itemsToInsert,
        int insertIndex)
    {
        var atIndex = insertIndex >= 0 && insertIndex <= items.Count;

        if (items is ObservableCollectionEx<DualListSelectorItem> rangeItems)
        {
            var content = new List<DualListSelectorItem>(rangeItems.Count + itemsToInsert.Count);
            content.AddRange(rangeItems);
            content.InsertRange(atIndex ? insertIndex : content.Count, itemsToInsert);
            ReplaceAll(rangeItems, content);
            return;
        }

        foreach (var item in itemsToInsert)
        {
            if (atIndex)
            {
                items.Insert(insertIndex++, item);
            }
            else
            {
                items.Add(item);
            }
        }
    }

    /// <summary>
    /// Replaces the content of <paramref name="items"/> with <paramref name="content"/> and raises one Reset
    /// (or none, when the owner of the collection already suppresses its notifications).
    /// </summary>
    private static void ReplaceAll(
        ObservableCollectionEx<DualListSelectorItem> items,
        List<DualListSelectorItem> content)
    {
        var wasSuppressed = items.SuppressOnChangedNotification;
        items.SuppressOnChangedNotification = true;
        try
        {
            items.Clear();
            foreach (var item in content)
            {
                items.Add(item);
            }
        }
        finally
        {
            items.SuppressOnChangedNotification = wasSuppressed;
        }
    }

    private static bool CanLeaveSelected(DualListSelectorItem item)
        => item is { IsEnabled: true, IsLocked: false };

    private bool FitsLimit(int addCount)
        => MaxSelectedCount is not { } max || SelectedItems.Count + addCount <= max;

    private static List<DualListSelectorItem> GetHighlighted(ListBox? listBox)
        => listBox is null
            ? []
            : listBox.SelectedItems.Cast<DualListSelectorItem>().ToList();

    /// <summary>
    /// "Move all" moves the highlighted items when two or more are highlighted, otherwise every shown (filtered) item.
    /// </summary>
    private List<DualListSelectorItem> GetMoveAllCandidates(
        ListBox? listBox,
        ObservableCollection<DualListSelectorItem> items)
    {
        if ((listBox?.SelectedItems.Count ?? 0) >= 2)
        {
            return GetHighlighted(listBox);
        }

        var view = GetView(items);
        return view is not null && view.Filter is not null
            ? view.Cast<DualListSelectorItem>().ToList()
            : items.ToList();
    }

    /// <summary>
    /// Maps a slot in the shown rows (filtered, grouped) to an insert index in the underlying collection.
    /// </summary>
    private static int ToCollectionInsertIndex(
        ListBox listBox,
        ObservableCollection<DualListSelectorItem> items,
        int viewInsertIndex)
    {
        var shown = listBox.Items;
        if (viewInsertIndex < 0)
        {
            return -1;
        }

        if (viewInsertIndex < shown.Count &&
            shown[viewInsertIndex] is DualListSelectorItem anchor)
        {
            var index = items.IndexOf(anchor);
            if (index >= 0)
            {
                return index;
            }
        }

        if (shown.Count > 0 &&
            shown[shown.Count - 1] is DualListSelectorItem last)
        {
            var index = items.IndexOf(last);
            if (index >= 0)
            {
                return index + 1;
            }
        }

        return items.Count;
    }

    internal void MoveToTop()
    {
        if (selectedListBox is null ||
            selectedListBox.SelectedItems.Count != 1)
        {
            return;
        }

        var item = (DualListSelectorItem)selectedListBox.SelectedItem!;
        var oldIndex = SelectedItems.IndexOf(item);
        if (oldIndex <= 0)
        {
            return;
        }

        SelectedItems.Move(oldIndex, 0);
        selectedListBox.SelectedItem = item;
        RecalculateSortOrder();
        ItemsReordered?.Invoke(
            this,
            new DualListSelectorItemsReorderedEventArgs(item, oldIndex, 0));
        selectedListBox.Focus();
    }

    internal void MoveUp()
    {
        if (selectedListBox is null ||
            selectedListBox.SelectedItems.Count != 1)
        {
            return;
        }

        var item = (DualListSelectorItem)selectedListBox.SelectedItem!;
        var oldIndex = SelectedItems.IndexOf(item);
        if (oldIndex <= 0)
        {
            return;
        }

        var newIndex = oldIndex - 1;
        SelectedItems.Move(oldIndex, newIndex);
        selectedListBox.SelectedItem = item;
        RecalculateSortOrder();
        ItemsReordered?.Invoke(
            this,
            new DualListSelectorItemsReorderedEventArgs(item, oldIndex, newIndex));
        selectedListBox.Focus();
    }

    internal void MoveDown()
    {
        if (selectedListBox is null ||
            selectedListBox.SelectedItems.Count != 1)
        {
            return;
        }

        var item = (DualListSelectorItem)selectedListBox.SelectedItem!;
        var oldIndex = SelectedItems.IndexOf(item);
        if (oldIndex < 0 || oldIndex >= SelectedItems.Count - 1)
        {
            return;
        }

        var newIndex = oldIndex + 1;
        SelectedItems.Move(oldIndex, newIndex);
        selectedListBox.SelectedItem = item;
        RecalculateSortOrder();
        ItemsReordered?.Invoke(
            this,
            new DualListSelectorItemsReorderedEventArgs(item, oldIndex, newIndex));
        selectedListBox.Focus();
    }

    internal void MoveToBottom()
    {
        if (selectedListBox is null ||
            selectedListBox.SelectedItems.Count != 1)
        {
            return;
        }

        var item = (DualListSelectorItem)selectedListBox.SelectedItem!;
        var oldIndex = SelectedItems.IndexOf(item);
        var lastIndex = SelectedItems.Count - 1;
        if (oldIndex < 0 || oldIndex >= lastIndex)
        {
            return;
        }

        SelectedItems.Move(oldIndex, lastIndex);
        selectedListBox.SelectedItem = item;
        RecalculateSortOrder();
        ItemsReordered?.Invoke(
            this,
            new DualListSelectorItemsReorderedEventArgs(item, oldIndex, lastIndex));
        selectedListBox.Focus();
    }

    internal bool CanMoveToSelected()
    {
        if (availableListBox is null || availableListBox.SelectedItems.Count == 0)
        {
            return false;
        }

        return MaxSelectedCount is null ||
               FitsLimit(availableListBox.SelectedItems.Cast<DualListSelectorItem>().Count(i => i.IsEnabled));
    }

    internal bool CanMoveToAvailable()
        => selectedListBox is not null &&
           selectedListBox.SelectedItems.Cast<DualListSelectorItem>().Any(CanLeaveSelected);

    internal bool CanMoveAllToSelected()
    {
        if (AvailableItems.Count == 0 || (availableListBox?.SelectedItems.Count ?? 0) == 1)
        {
            return false;
        }

        return MaxSelectedCount is null ||
               FitsLimit(GetMoveAllCandidates(availableListBox, AvailableItems).Count(i => i.IsEnabled));
    }

    internal bool CanMoveAllToAvailable()
        => (selectedListBox?.SelectedItems.Count ?? 0) != 1 &&
           SelectedItems.Any(CanLeaveSelected);

    [SuppressMessage("Minor Code Smell", "S2692:Index checks should not be for positive numbers", Justification = "Intentional: index > 0 means item is not already at top.")]
    internal bool CanMoveToTop()
        => selectedListBox is { SelectedItem: DualListSelectorItem item, SelectedItems.Count: 1 } &&
           SelectedItems.IndexOf(item) > 0;

    internal bool CanMoveUp()
        => CanMoveToTop();

    internal bool CanMoveDown()
    {
        if (selectedListBox?.SelectedItem is not DualListSelectorItem item ||
            selectedListBox.SelectedItems.Count != 1)
        {
            return false;
        }

        var index = SelectedItems.IndexOf(item);
        return index >= 0 && index < SelectedItems.Count - 1;
    }

    internal bool CanMoveToBottom()
        => CanMoveDown();

    private static void OnMoveToSelectedExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveToSelected();

    private static void OnMoveToSelectedCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveToSelected();

    private static void OnMoveToAvailableExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveToAvailable();

    private static void OnMoveToAvailableCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveToAvailable();

    private static void OnMoveAllToSelectedExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveAllToSelected();

    private static void OnMoveAllToSelectedCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveAllToSelected();

    private static void OnMoveAllToAvailableExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveAllToAvailable();

    private static void OnMoveAllToAvailableCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveAllToAvailable();

    private static void OnMoveToTopExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveToTop();

    private static void OnMoveToTopCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveToTop();

    private static void OnMoveUpExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveUp();

    private static void OnMoveUpCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveUp();

    private static void OnMoveDownExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveDown();

    private static void OnMoveDownCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveDown();

    private static void OnMoveToBottomExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
        => ((DualListSelector)sender).MoveToBottom();

    private static void OnMoveToBottomCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanMoveToBottom();

    private static void OnRemoveFromSelectedExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is DualListSelectorItem item)
        {
            ((DualListSelector)sender).RemoveFromSelected(item);
            e.Handled = true;
        }
    }

    private static void OnRemoveFromSelectedCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
        => e.CanExecute = ((DualListSelector)sender).CanRemoveFromSelected(e.Parameter);

    private static void OnStateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DualListSelector selector)
        {
            selector.UpdateItemCounts();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private static void OnIsVirtualizingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => (d as DualListSelector)?.ApplyVirtualization();

    private static void OnGroupMemberPathChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => (d as DualListSelector)?.ApplyGrouping();

    private void ApplyVirtualization()
    {
        foreach (var listBox in new[] { availableListBox, selectedListBox })
        {
            if (listBox is null)
            {
                continue;
            }

            VirtualizingPanel.SetIsVirtualizing(listBox, IsVirtualizing);
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(listBox, IsVirtualizing);
            VirtualizingPanel.SetVirtualizationMode(
                listBox,
                IsVirtualizing ? VirtualizationMode.Recycling : VirtualizationMode.Standard);
            if (IsVirtualizing)
            {
                ScrollViewer.SetCanContentScroll(listBox, true);
            }
            else
            {
                listBox.ClearValue(ScrollViewer.CanContentScrollProperty);
            }
        }
    }

    private void ApplyGrouping()
    {
        ApplyGrouping(AvailableItems);
        ApplyGrouping(SelectedItems);
    }

    private void ApplyGrouping(
        ObservableCollection<DualListSelectorItem>? items)
    {
        if (items is null)
        {
            return;
        }

        var view = GetView(items);
        if (view is null || !view.CanGroup)
        {
            return;
        }

        var path = !string.IsNullOrWhiteSpace(GroupMemberPath)
            ? GroupMemberPath
            : items.Any(i => i.Group is not null)
                ? nameof(DualListSelectorItem.Group)
                : null;

        var current = view.GroupDescriptions.OfType<OwnGroupDescription>().ToList();
        if (current.Count == 1 && string.Equals(current[0].PropertyName, path, StringComparison.Ordinal))
        {
            return;
        }

        if (current.Count == 0 && path is null)
        {
            return;
        }

        foreach (var description in current)
        {
            view.GroupDescriptions.Remove(description);
        }

        if (path is not null)
        {
            view.GroupDescriptions.Add(new OwnGroupDescription(path));
        }
    }

    private void ScheduleGroupingCheck()
    {
        if (isGroupingPending || !string.IsNullOrWhiteSpace(GroupMemberPath))
        {
            return;
        }

        // Changing the group descriptions while the collection is raising CollectionChanged would race the
        // view's own handler, so the check runs right after the change.
        isGroupingPending = true;
        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.DataBind,
            () =>
            {
                isGroupingPending = false;
                ApplyGrouping();
            });
    }

    private void OnSelectAllToggled(
        object sender,
        RoutedEventArgs e)
    {
        // A click (or the UI Automation Toggle) changes IsChecked; the control then sets the state from the list.
        if (!isSyncingSelectAll)
        {
            ToggleSelectAll(ReferenceEquals(sender, availableSelectAll) ? availableListBox : selectedListBox);
        }
    }

    private void UpdateHeaderCounts()
    {
        if (transferDepth > 0)
        {
            return;
        }

        UpdateHeaderCount(AvailableItems, availableListBox, availableSelectAll, availableHeaderCount);
        UpdateHeaderCount(SelectedItems, selectedListBox, selectedSelectAll, selectedHeaderCount);
    }

    private void UpdateHeaderCount(
        ObservableCollection<DualListSelectorItem>? items,
        ListBox? listBox,
        CheckBox? selectAll,
        TextBlock? headerCount)
    {
        if (items is null || listBox is null || ItemSelectionStyle != DualListItemSelectionStyle.CheckBox)
        {
            return;
        }

        // Counted from the collection and the filter rather than from the list box, which may not have
        // processed a collection change yet when this runs.
        var (shown, shownEnabled) = CountShown(items);

        var checkedCount = listBox.SelectedItems.Count;
        var checkedEnabled = listBox.SelectedItems.OfType<DualListSelectorItem>().Count(i => i.IsEnabled);

        if (selectAll is not null)
        {
            bool? state = checkedEnabled == 0
                ? false
                : checkedEnabled >= shownEnabled
                    ? true
                    : null;
            isSyncingSelectAll = true;
            try
            {
                selectAll.SetCurrentValue(ToggleButton.IsCheckedProperty, state);
                selectAll.SetCurrentValue(
                    IsEnabledProperty,
                    shownEnabled > 0 && listBox.SelectionMode != SelectionMode.Single);
            }
            finally
            {
                isSyncingSelectAll = false;
            }
        }

        var countText = string.Format(
            CultureInfo.CurrentCulture,
            HeaderCountFormat ?? DualListSelectorStrings.HeaderCountFormat2,
            checkedCount,
            shown);
        if (headerCount is not null)
        {
            headerCount.Text = countText;
        }

        // Text inside a template is hidden from UI Automation, so the count is also the help text of the check box.
        if (selectAll is not null)
        {
            System.Windows.Automation.AutomationProperties.SetHelpText(selectAll, countText);
        }
    }

    private (int Shown, int ShownEnabled) CountShown(
        ObservableCollection<DualListSelectorItem> items)
    {
        var filter = GetView(items)?.Filter;
        var shown = 0;
        var shownEnabled = 0;
        foreach (var item in items)
        {
            if (filter is not null && !filter(item))
            {
                continue;
            }

            shown++;
            if (item.IsEnabled)
            {
                shownEnabled++;
            }
        }

        return (shown, shownEnabled);
    }

    private void RecalculateSortOrder()
    {
        if (!AutoRecalculateSortOrder)
        {
            return;
        }

        for (var i = 0; i < SelectedItems.Count; i++)
        {
            SelectedItems[i].SortOrderNumber = i;
        }
    }

    private void RaiseItemsTransferred(
        List<DualListSelectorItem> items,
        DualListSelectorTransferDirection direction)
    {
        ItemsTransferred?.Invoke(
            this,
            new DualListSelectorItemsTransferredEventArgs(items, direction));
    }

    private void OnAvailableFilterTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ApplyFilter(AvailableItems, availableFilterTextBox?.Text);
        UpdateItemCounts();
    }

    private void OnSelectedFilterTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ApplyFilter(SelectedItems, selectedFilterTextBox?.Text);
        UpdateItemCounts();
    }

    private void ApplyFilter(
        ObservableCollection<DualListSelectorItem> items,
        string? filterText)
    {
        var view = GetView(items);
        if (view is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(filterText))
        {
            view.Filter = null;
        }
        else
        {
            view.Filter = obj =>
                obj is DualListSelectorItem item &&
                item.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }
    }

    private void ClearFilters()
    {
        if (availableFilterTextBox is not null)
        {
            availableFilterTextBox.Text = string.Empty;
        }

        if (selectedFilterTextBox is not null)
        {
            selectedFilterTextBox.Text = string.Empty;
        }
    }

    private void SetupDragDrop()
    {
        if (availableListBox is not null)
        {
            availableListBox.PreviewMouseLeftButtonDown += OnListBoxPreviewMouseLeftButtonDown;
            availableListBox.PreviewMouseMove += OnListBoxPreviewMouseMove;
            availableListBox.Drop += OnAvailableListBoxDrop;
            availableListBox.DragOver += OnListBoxDragOver;
            availableListBox.DragEnter += OnListBoxDragEnter;
            availableListBox.DragLeave += OnListBoxDragLeave;
        }

        if (selectedListBox is not null)
        {
            selectedListBox.PreviewMouseLeftButtonDown += OnListBoxPreviewMouseLeftButtonDown;
            selectedListBox.PreviewMouseMove += OnListBoxPreviewMouseMove;
            selectedListBox.Drop += OnSelectedListBoxDrop;
            selectedListBox.DragOver += OnListBoxDragOver;
            selectedListBox.DragEnter += OnListBoxDragEnter;
            selectedListBox.DragLeave += OnListBoxDragLeave;
        }
    }

    private void OnListBoxPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (AllowDragDrop)
        {
            dragStartPoint = e.GetPosition(null);
        }
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Drag initiation with adorner setup.")]
    private void OnListBoxPreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (!AllowDragDrop ||
            e.LeftButton != MouseButtonState.Pressed ||
            dragStartPoint is null ||
            sender is not ListBox listBox)
        {
            return;
        }

        var currentPoint = e.GetPosition(null);
        var diff = currentPoint - dragStartPoint.Value;

        if (System.Math.Abs(diff.X) <= SystemParameters.MinimumHorizontalDragDistance &&
            System.Math.Abs(diff.Y) <= SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        if (listBox.SelectedItem is not DualListSelectorItem item)
        {
            return;
        }

        var dragItems = listBox.SelectedItems
            .Cast<DualListSelectorItem>()
            .ToList();

        dragStartPoint = null;
        dragSourceListBox = listBox;

        if (listBox.ItemContainerGenerator.ContainerFromItem(item) is ListBoxItem container)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(listBox);
            if (adornerLayer is not null)
            {
                dragAdorner = new DragDropAdorner(listBox, container);
                adornerLayer.Add(dragAdorner);
            }
        }

        System.Windows.DragDrop.DoDragDrop(listBox, dragItems, DragDropEffects.Move);
        RemoveDragAdorners();
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Drag over with adorner updates.")]
    private void OnListBoxDragOver(
        object sender,
        DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(typeof(List<DualListSelectorItem>)))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.Handled = true;

        if (sender is not ListBox targetListBox)
        {
            e.Effects = DragDropEffects.Move;
            return;
        }

        if (IsDropRefused(targetListBox, e.Data.GetData(typeof(List<DualListSelectorItem>)) as List<DualListSelectorItem>))
        {
            e.Effects = DragDropEffects.None;
            return;
        }

        e.Effects = DragDropEffects.Move;

        if (dragAdorner is not null)
        {
            var position = e.GetPosition(dragAdorner.AdornedElement);
            dragAdorner.UpdatePosition(position);
        }

        if (ReferenceEquals(sender, selectedListBox) &&
            ReferenceEquals(dragSourceListBox, selectedListBox))
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(targetListBox);
            if (adornerLayer is not null)
            {
                var dropIndex = DragDropHelper.GetInsertIndex(targetListBox, e);
                if (dropIndicatorAdorner is null)
                {
                    dropIndicatorAdorner = new DropTargetInsertionAdorner(targetListBox);
                    adornerLayer.Add(dropIndicatorAdorner);
                }

                var yPosition = DragDropHelper.GetDropIndicatorY(targetListBox, dropIndex);
                dropIndicatorAdorner.UpdatePosition(yPosition);
            }
        }
        else if (dropIndicatorAdorner is not null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(dropIndicatorAdorner.AdornedElement);
            adornerLayer?.Remove(dropIndicatorAdorner);
            dropIndicatorAdorner = null;
        }
    }

    private bool IsDropRefused(
        ListBox targetListBox,
        List<DualListSelectorItem>? items)
    {
        if (items is null || items.Count == 0)
        {
            return false;
        }

        if (ReferenceEquals(targetListBox, selectedListBox) &&
            ReferenceEquals(dragSourceListBox, availableListBox))
        {
            return !CanDropOnSelected(items, fromAvailable: true);
        }

        return ReferenceEquals(targetListBox, availableListBox) &&
               ReferenceEquals(dragSourceListBox, selectedListBox) &&
               !items.Exists(CanLeaveSelected);
    }

    private static void OnListBoxDragEnter(
        object sender,
        DragEventArgs e)
    {
        if (sender is ListBox listBox &&
            e.Data.GetDataPresent(typeof(List<DualListSelectorItem>)))
        {
            listBox.BorderBrush = listBox.TryFindResource("AtcApps.Brushes.Accent") as Brush
                ?? SystemColors.HighlightBrush;
        }
    }

    private static void OnListBoxDragLeave(
        object sender,
        DragEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            listBox.BorderBrush = listBox.TryFindResource("AtcApps.Brushes.Gray6") as Brush
                ?? SystemColors.ActiveBorderBrush;
        }
    }

    private void OnAvailableListBoxDrop(
        object sender,
        DragEventArgs e)
    {
        RemoveDragAdorners();
        ResetListBoxBorder(sender as ListBox);

        if (e.Data.GetData(typeof(List<DualListSelectorItem>)) is not List<DualListSelectorItem> items ||
            items.Count == 0)
        {
            return;
        }

        DropOnAvailable(items, ReferenceEquals(dragSourceListBox, selectedListBox));

        dragSourceListBox = null;
        e.Handled = true;
    }

    private void OnSelectedListBoxDrop(
        object sender,
        DragEventArgs e)
    {
        RemoveDragAdorners();
        ResetListBoxBorder(sender as ListBox);

        if (e.Data.GetData(typeof(List<DualListSelectorItem>)) is not List<DualListSelectorItem> items ||
            items.Count == 0 ||
            selectedListBox is null)
        {
            return;
        }

        var fromAvailable = ReferenceEquals(dragSourceListBox, availableListBox);
        if (fromAvailable || ReferenceEquals(dragSourceListBox, selectedListBox))
        {
            DropOnSelected(items, fromAvailable, DragDropHelper.GetInsertIndex(selectedListBox, e));
        }

        dragSourceListBox = null;
        e.Handled = true;
    }

    private static void ResetListBoxBorder(ListBox? listBox)
    {
        if (listBox is null)
        {
            return;
        }

        listBox.BorderBrush = listBox.TryFindResource("AtcApps.Brushes.Gray6") as Brush
            ?? SystemColors.ActiveBorderBrush;
    }

    private static bool IsSourcedFrom(
        RoutedEventArgs e,
        ListBox? listBox)
    {
        if (listBox is null || e.OriginalSource is not DependencyObject source)
        {
            return false;
        }

        var parent = source;
        while (parent is not null)
        {
            if (ReferenceEquals(parent, listBox))
            {
                return true;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return false;
    }

    private void OnListBoxSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
        UpdateHeaderCounts();
        SelectionChanged?.Invoke(this, e);
    }

    private void OnAvailableListBoxDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (availableListBox?.SelectedItem is DualListSelectorItem { IsEnabled: true } item &&
            IsMouseOverListBoxItem(availableListBox, e) &&
            TransferToSelected([item], insertIndex: -1, clearFilters: false))
        {
            e.Handled = true;
        }
    }

    private void OnSelectedListBoxDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (selectedListBox?.SelectedItem is DualListSelectorItem { IsEnabled: true } item &&
            IsMouseOverListBoxItem(selectedListBox, e) &&
            TransferToAvailable([item], clearFilters: false, focusAvailable: true))
        {
            e.Handled = true;
        }
    }

    private static bool IsMouseOverListBoxItem(
        ListBox listBox,
        MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source)
        {
            return false;
        }

        var parent = source;
        while (parent is not null)
        {
            if (parent is ListBoxItem)
            {
                return true;
            }

            if (ReferenceEquals(parent, listBox))
            {
                return false;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return false;
    }

    private static void OnItemsCollectionChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DualListSelector selector)
        {
            return;
        }

        // The footer counts and the empty texts must follow changes made outside the control too (a view
        // model filling the list after load, or rebinding it). Weak, so a long-lived collection does not
        // keep the control alive.
        if (e.OldValue is INotifyCollectionChanged oldItems)
        {
            CollectionChangedEventManager.RemoveHandler(oldItems, selector.OnItemsChanged);
        }

        if (e.NewValue is INotifyCollectionChanged newItems)
        {
            CollectionChangedEventManager.AddHandler(newItems, selector.OnItemsChanged);
        }

        // The views live only while the control is loaded: a view subscribes strongly to its collection.
        if (selector.IsLoaded)
        {
            selector.ReplaceView(
                isAvailable: e.Property == AvailableItemsProperty,
                e.NewValue as ObservableCollection<DualListSelectorItem>);
        }

        selector.UpdateItemCounts();
    }

    /// <summary>
    /// Gives a list its own collection view, so the filter and the grouping never touch the collection's
    /// default view (shared with every other control bound to the same collection).
    /// </summary>
    private void ReplaceView(
        bool isAvailable,
        ObservableCollection<DualListSelectorItem>? items)
    {
        var oldView = isAvailable ? availableView : selectedView;
        var newView = items is null ? null : new ListCollectionView(items);
        if (isAvailable)
        {
            availableView = newView;
        }
        else
        {
            selectedView = newView;
        }

        if (items is not null)
        {
            ApplyGrouping(items);
            ApplyFilter(items, isAvailable ? availableFilterTextBox?.Text : selectedFilterTextBox?.Text);
        }

        var listBox = isAvailable ? availableListBox : selectedListBox;
        if (listBox is not null)
        {
            listBox.ItemsSource = newView;
        }

        oldView?.DetachFromSourceCollection();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (availableView is not null || selectedView is not null)
        {
            return;
        }

        RestoreView(isAvailable: true, AvailableItems, availableListBox, keptAvailableHighlight);
        RestoreView(isAvailable: false, SelectedItems, selectedListBox, keptSelectedHighlight);
        keptAvailableHighlight = null;
        keptSelectedHighlight = null;
        UpdateItemCounts();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        // A collection view subscribes strongly to its collection: let go of it, so a long-lived collection
        // does not keep one view per selector ever shown. The views (filter, grouping, highlight) come back on Loaded.
        keptAvailableHighlight = GetHighlighted(availableListBox);
        keptSelectedHighlight = GetHighlighted(selectedListBox);
        ReplaceView(isAvailable: true, items: null);
        ReplaceView(isAvailable: false, items: null);
    }

    private void RestoreView(
        bool isAvailable,
        ObservableCollection<DualListSelectorItem>? items,
        ListBox? listBox,
        List<DualListSelectorItem>? highlight)
    {
        ReplaceView(isAvailable, items);
        if (listBox is null || highlight is null || items is null)
        {
            return;
        }

        foreach (var item in highlight.Where(listBox.Items.Contains))
        {
            listBox.SelectedItems.Add(item);
        }
    }

    private ListCollectionView? GetView(
        ObservableCollection<DualListSelectorItem>? items)
    {
        if (items is null)
        {
            return null;
        }

        if (availableView is not null && ReferenceEquals(availableView.SourceCollection, items))
        {
            return availableView;
        }

        return selectedView is not null && ReferenceEquals(selectedView.SourceCollection, items)
            ? selectedView
            : null;
    }

    private void OnItemsChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        // A Reset (a range change) carries no new items, so the whole collection is checked then.
        var added = e.Action == NotifyCollectionChangedAction.Reset
            ? sender as IEnumerable
            : e.NewItems;

        if (added?.OfType<DualListSelectorItem>().Any(i => i.Group is not null) == true &&
            sender is ObservableCollection<DualListSelectorItem> items &&
            GetView(items)?.GroupDescriptions.OfType<OwnGroupDescription>().Any() == false)
        {
            ScheduleGroupingCheck();
        }

        UpdateItemCounts();
    }

    /// <summary>
    /// Converts an insertion index (insert before that item) into the target index for
    /// <see cref="ObservableCollection{T}.Move"/>, which removes the dragged item before inserting it.
    /// </summary>
    internal static int GetReorderTargetIndex(
        int oldIndex,
        int insertIndex,
        int count)
    {
        var target = insertIndex > oldIndex
            ? insertIndex - 1
            : insertIndex;

        return System.Math.Clamp(target, 0, System.Math.Max(0, count - 1));
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Item count and empty text update.")]
    private void UpdateItemCounts()
    {
        // A transfer brings the counts up to date once, when it is done.
        if (transferDepth > 0)
        {
            return;
        }

        ItemCountUpdateCount++;

        if (availableItemCount is not null)
        {
            var view = GetView(AvailableItems);
            availableItemCount.Visibility = ShowItemCount
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (view is not null && view.Filter is not null)
            {
                var filteredCount = view.Cast<object>().Count();
                availableItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    DualListSelectorStrings.ItemCountFilteredFormat2,
                    filteredCount,
                    AvailableItems.Count);
            }
            else
            {
                availableItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    DualListSelectorStrings.ItemCountFormat1,
                    AvailableItems.Count);
            }
        }

        if (selectedItemCount is not null)
        {
            var view = GetView(SelectedItems);
            selectedItemCount.Visibility = ShowItemCount || MaxSelectedCount is not null
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (MaxSelectedCount is { } max)
            {
                selectedItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Atc.Wpf.Resources.Miscellaneous.CountOfMax2,
                    SelectedItems.Count,
                    max);
            }
            else if (view?.Filter is not null)
            {
                var filteredCount = view.Cast<object>().Count();
                selectedItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    DualListSelectorStrings.ItemCountFilteredFormat2,
                    filteredCount,
                    SelectedItems.Count);
            }
            else
            {
                selectedItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    DualListSelectorStrings.ItemCountFormat1,
                    SelectedItems.Count);
            }
        }

        UpdateEmptyText(AvailableItems, availableEmptyText);
        UpdateEmptyText(SelectedItems, selectedEmptyText);
        UpdateHeaderCounts();
    }

    private void UpdateEmptyText(
        ObservableCollection<DualListSelectorItem> items,
        TextBlock? emptyText)
    {
        if (emptyText is null)
        {
            return;
        }

        var view = GetView(items);
        if (view?.Filter is not null && items.Count > 0)
        {
            var filteredCount = view.Cast<object>().Count();
            emptyText.Visibility = filteredCount == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else
        {
            emptyText.Visibility = Visibility.Collapsed;
        }
    }

    private void RemoveDragAdorners()
    {
        if (dragAdorner is not null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(dragAdorner.AdornedElement);
            adornerLayer?.Remove(dragAdorner);
            dragAdorner = null;
        }

        if (dropIndicatorAdorner is not null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(dropIndicatorAdorner.AdornedElement);
            adornerLayer?.Remove(dropIndicatorAdorner);
            dropIndicatorAdorner = null;
        }
    }

    /// <summary>
    /// A group description added by this control, so it can tell its own grouping from one a consumer added to the view.
    /// </summary>
    private sealed class OwnGroupDescription : PropertyGroupDescription
    {
        public OwnGroupDescription(string propertyName)
            : base(propertyName)
        {
        }
    }

    /// <summary>
    /// Exposes the selector as a group in the UI Automation tree (with its automation id and name), so the
    /// lists, buttons and check boxes inside it can be found under it.
    /// </summary>
    private sealed class DualListSelectorAutomationPeer : FrameworkElementAutomationPeer
    {
        public DualListSelectorAutomationPeer(DualListSelector owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
            => nameof(DualListSelector);

        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Group;
    }
}