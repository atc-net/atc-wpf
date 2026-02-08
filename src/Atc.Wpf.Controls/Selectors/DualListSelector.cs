// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051
namespace Atc.Wpf.Controls.Selectors;

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
public sealed partial class DualListSelector : Control
{
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
    private TextBox? availableFilterTextBox;
    private TextBox? selectedFilterTextBox;
    private TextBlock? availableItemCount;
    private TextBlock? selectedItemCount;
    private TextBlock? availableEmptyText;
    private TextBlock? selectedEmptyText;
    private Point? dragStartPoint;
    private ListBox? dragSourceListBox;
    private DragDropAdorner? dragAdorner;
    private DropTargetInsertionAdorner? dropIndicatorAdorner;

    /// <summary>
    /// The source list of available items.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private ObservableCollection<DualListSelectorItem> availableItems = [];

    /// <summary>
    /// The target list of selected items.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
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
    [DependencyProperty(DefaultValue = false)]
    private bool showItemCount;

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

    public DualListSelector()
    {
        AvailableItems = [];
        SelectedItems = [];
        ListMaxHeight = double.PositiveInfinity;
    }

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

        if (availableListBox is not null)
        {
            availableListBox.SelectionChanged += OnListBoxSelectionChanged;
            availableListBox.MouseDoubleClick += OnAvailableListBoxDoubleClick;
        }

        if (selectedListBox is not null)
        {
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

    internal void MoveToSelected()
    {
        if (availableListBox is null)
        {
            return;
        }

        var itemsToMove = availableListBox.SelectedItems
            .Cast<DualListSelectorItem>()
            .Where(i => i.IsEnabled)
            .ToList();

        if (itemsToMove.Count == 0)
        {
            return;
        }

        foreach (var item in itemsToMove)
        {
            AvailableItems.Remove(item);
            SelectedItems.Add(item);
        }

        RecalculateSortOrder();
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToSelected);
        UpdateItemCounts();
        selectedListBox?.Focus();
    }

    internal void MoveToAvailable()
    {
        if (selectedListBox is null)
        {
            return;
        }

        var itemsToMove = selectedListBox.SelectedItems
            .Cast<DualListSelectorItem>()
            .Where(i => i.IsEnabled)
            .ToList();

        if (itemsToMove.Count == 0)
        {
            return;
        }

        foreach (var item in itemsToMove)
        {
            SelectedItems.Remove(item);
            AvailableItems.Add(item);
        }

        RecalculateSortOrder();
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToAvailable);
        UpdateItemCounts();
        availableListBox?.Focus();
    }

    internal void MoveAllToSelected()
    {
        var selectedCount = availableListBox?.SelectedItems.Count ?? 0;

        List<DualListSelectorItem> itemsToMove;
        if (selectedCount >= 2)
        {
            itemsToMove = availableListBox!.SelectedItems.Cast<DualListSelectorItem>().Where(i => i.IsEnabled).ToList();
        }
        else
        {
            var view = CollectionViewSource.GetDefaultView(AvailableItems);
            itemsToMove = view is not null && view.Filter is not null
                ? view.Cast<DualListSelectorItem>().Where(i => i.IsEnabled).ToList()
                : AvailableItems.Where(i => i.IsEnabled).ToList();
        }

        if (itemsToMove.Count == 0)
        {
            return;
        }

        foreach (var item in itemsToMove)
        {
            AvailableItems.Remove(item);
            SelectedItems.Add(item);
        }

        ClearFilters();
        RecalculateSortOrder();
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToSelected);
        UpdateItemCounts();
        selectedListBox?.Focus();
    }

    internal void MoveAllToAvailable()
    {
        var selectedCount = selectedListBox?.SelectedItems.Count ?? 0;

        List<DualListSelectorItem> itemsToMove;
        if (selectedCount >= 2)
        {
            itemsToMove = selectedListBox!.SelectedItems.Cast<DualListSelectorItem>().Where(i => i.IsEnabled).ToList();
        }
        else
        {
            var view = CollectionViewSource.GetDefaultView(SelectedItems);
            itemsToMove = view is not null && view.Filter is not null
                ? view.Cast<DualListSelectorItem>().Where(i => i.IsEnabled).ToList()
                : SelectedItems.Where(i => i.IsEnabled).ToList();
        }

        if (itemsToMove.Count == 0)
        {
            return;
        }

        foreach (var item in itemsToMove)
        {
            SelectedItems.Remove(item);
            AvailableItems.Add(item);
        }

        ClearFilters();
        RecalculateSortOrder();
        RaiseItemsTransferred(itemsToMove, DualListSelectorTransferDirection.ToAvailable);
        UpdateItemCounts();
        availableListBox?.Focus();
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
        => availableListBox is not null && availableListBox.SelectedItems.Count > 0;

    internal bool CanMoveToAvailable()
        => selectedListBox is not null && selectedListBox.SelectedItems.Count > 0;

    internal bool CanMoveAllToSelected()
        => AvailableItems.Count > 0 && (availableListBox?.SelectedItems.Count ?? 0) != 1;

    internal bool CanMoveAllToAvailable()
        => SelectedItems.Count > 0 && (selectedListBox?.SelectedItems.Count ?? 0) != 1;

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

    private static void ApplyFilter(
        ObservableCollection<DualListSelectorItem> items,
        string? filterText)
    {
        var view = CollectionViewSource.GetDefaultView(items);
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

        System.Windows.DragDrop.DoDragDrop(listBox, item, DragDropEffects.Move);
        RemoveDragAdorners();
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Drag over with adorner updates.")]
    private void OnListBoxDragOver(
        object sender,
        DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(typeof(DualListSelectorItem)))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.Effects = DragDropEffects.Move;
        e.Handled = true;

        if (sender is not ListBox targetListBox)
        {
            return;
        }

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

    private static void OnListBoxDragEnter(
        object sender,
        DragEventArgs e)
    {
        if (sender is ListBox listBox &&
            e.Data.GetDataPresent(typeof(DualListSelectorItem)))
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

        if (e.Data.GetData(typeof(DualListSelectorItem)) is not DualListSelectorItem item)
        {
            return;
        }

        if (ReferenceEquals(dragSourceListBox, selectedListBox))
        {
            SelectedItems.Remove(item);
            AvailableItems.Add(item);
            RecalculateSortOrder();
            RaiseItemsTransferred([item], DualListSelectorTransferDirection.ToAvailable);
            UpdateItemCounts();
            availableListBox?.Focus();
        }

        dragSourceListBox = null;
        e.Handled = true;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Drag-drop handling with reorder.")]
    private void OnSelectedListBoxDrop(
        object sender,
        DragEventArgs e)
    {
        RemoveDragAdorners();
        ResetListBoxBorder(sender as ListBox);

        if (e.Data.GetData(typeof(DualListSelectorItem)) is not DualListSelectorItem item ||
            selectedListBox is null)
        {
            return;
        }

        if (ReferenceEquals(dragSourceListBox, availableListBox))
        {
            AvailableItems.Remove(item);
            var dropIndex = DragDropHelper.GetInsertIndex(selectedListBox, e);
            if (dropIndex >= 0 && dropIndex < SelectedItems.Count)
            {
                SelectedItems.Insert(dropIndex, item);
            }
            else
            {
                SelectedItems.Add(item);
            }

            RecalculateSortOrder();
            RaiseItemsTransferred([item], DualListSelectorTransferDirection.ToSelected);
            UpdateItemCounts();
            selectedListBox.Focus();
        }
        else if (ReferenceEquals(dragSourceListBox, selectedListBox))
        {
            var oldIndex = SelectedItems.IndexOf(item);
            var dropIndex = System.Math.Clamp(
                DragDropHelper.GetInsertIndex(selectedListBox, e),
                0,
                SelectedItems.Count - 1);

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
        SelectionChanged?.Invoke(this, e);
    }

    private void OnAvailableListBoxDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (availableListBox?.SelectedItem is DualListSelectorItem { IsEnabled: true } item &&
            IsMouseOverListBoxItem(availableListBox, e))
        {
            AvailableItems.Remove(item);
            SelectedItems.Add(item);
            RecalculateSortOrder();
            RaiseItemsTransferred([item], DualListSelectorTransferDirection.ToSelected);
            UpdateItemCounts();
            selectedListBox?.Focus();
            e.Handled = true;
        }
    }

    private void OnSelectedListBoxDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (selectedListBox?.SelectedItem is DualListSelectorItem { IsEnabled: true } item &&
            IsMouseOverListBoxItem(selectedListBox, e))
        {
            SelectedItems.Remove(item);
            AvailableItems.Add(item);
            RecalculateSortOrder();
            RaiseItemsTransferred([item], DualListSelectorTransferDirection.ToAvailable);
            UpdateItemCounts();
            availableListBox?.Focus();
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

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Item count and empty text update.")]
    private void UpdateItemCounts()
    {
        if (availableItemCount is not null)
        {
            var view = CollectionViewSource.GetDefaultView(AvailableItems);
            if (view is not null && view.Filter is not null)
            {
                var filteredCount = view.Cast<object>().Count();
                availableItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Miscellaneous.ItemCountFilteredFormat2,
                    filteredCount,
                    AvailableItems.Count);
            }
            else
            {
                availableItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Miscellaneous.ItemCountFormat1,
                    AvailableItems.Count);
            }
        }

        if (selectedItemCount is not null)
        {
            var view = CollectionViewSource.GetDefaultView(SelectedItems);
            if (view?.Filter is not null)
            {
                var filteredCount = view.Cast<object>().Count();
                selectedItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Miscellaneous.ItemCountFilteredFormat2,
                    filteredCount,
                    SelectedItems.Count);
            }
            else
            {
                selectedItemCount.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Miscellaneous.ItemCountFormat1,
                    SelectedItems.Count);
            }
        }

        UpdateEmptyText(AvailableItems, availableEmptyText);
        UpdateEmptyText(SelectedItems, selectedEmptyText);
    }

    private static void UpdateEmptyText(
        ObservableCollection<DualListSelectorItem> items,
        TextBlock? emptyText)
    {
        if (emptyText is null)
        {
            return;
        }

        var view = CollectionViewSource.GetDefaultView(items);
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
}