namespace Atc.Wpf.DragDrop;

/// <summary>
/// Internal manager that handles all drag-drop event wiring and coordination.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Event handler methods for drag-drop coordination.")]
internal static class DragDropManager
{
    private static readonly DefaultDropHandler FallbackDropHandler = new();

    private static Point? dragStartPoint;
    private static ItemsControl? pressedItemsControl;
    private static object? pressedItem;
    private static IReadOnlyList<object>? pressedSelection;
    private static bool isSingleSelectionDeferred;
    private static DragInfo? currentDragInfo;
    private static DragDropAdorner? dragAdorner;
    private static DropTargetInsertionAdorner? insertionAdorner;
    private static DropTargetHighlightAdorner? highlightAdorner;

    internal static void RegisterDragSource(UIElement element)
    {
        element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        element.PreviewMouseMove += OnPreviewMouseMove;
        element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
    }

    internal static void UnregisterDragSource(UIElement element)
    {
        element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        element.PreviewMouseMove -= OnPreviewMouseMove;
        element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
    }

    internal static void RegisterDropTarget(UIElement element)
    {
        element.AllowDrop = true;
        element.DragOver += OnDragOver;
        element.DragEnter += OnDragEnter;
        element.DragLeave += OnDragLeave;
        element.Drop += OnDrop;
    }

    internal static void UnregisterDropTarget(UIElement element)
    {
        element.AllowDrop = false;
        element.DragOver -= OnDragOver;
        element.DragEnter -= OnDragEnter;
        element.DragLeave -= OnDragLeave;
        element.Drop -= OnDrop;
    }

    private static void OnPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        ClearPendingDrag();

        if (sender is not UIElement sourceElement)
        {
            return;
        }

        var itemsControl = sourceElement as ItemsControl
            ?? FindVisualParent<ItemsControl>(sourceElement);

        if (itemsControl is null)
        {
            return;
        }

        // Resolve the item at the press point, not later at the point where the drag threshold is passed.
        var item = ResolvePressedItem(itemsControl, e.OriginalSource as DependencyObject);
        if (item is null)
        {
            return;
        }

        dragStartPoint = e.GetPosition(null);
        pressedItemsControl = itemsControl;
        pressedItem = item;

        // Snapshot the selection now: a multi-select list collapses it to the pressed item on mouse down.
        pressedSelection = GetDraggedItems(itemsControl, item);
        if (pressedSelection.Count > 1 &&
            Keyboard.Modifiers == ModifierKeys.None &&
            !IsInsideButton(e.OriginalSource as DependencyObject, itemsControl))
        {
            // Keep the selection so the whole selection can be dragged; a click without a drag
            // still selects just the pressed item, on mouse up.
            isSingleSelectionDeferred = true;
            e.Handled = true;
        }
    }

    private static void OnPreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (isSingleSelectionDeferred &&
            pressedItemsControl is Selector selector &&
            pressedItem is not null)
        {
            selector.SelectedItem = pressedItem;
        }

        ClearPendingDrag();
    }

    private static void OnPreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed ||
            dragStartPoint is null ||
            pressedItemsControl is null ||
            pressedItem is null)
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

        BeginDrag(pressedItemsControl, pressedItem, pressedSelection);
    }

    /// <summary>
    /// Gets the items a drag started on <paramref name="item"/> carries: the whole selection, in
    /// source-collection order, when <paramref name="item"/> is one of several selected items of a
    /// multi-select <see cref="ListBox"/> or <see cref="MultiSelector"/> (such as a DataGrid);
    /// otherwise just <paramref name="item"/>.
    /// </summary>
    internal static IReadOnlyList<object> GetDraggedItems(
        ItemsControl itemsControl,
        object item)
    {
        var selectedItems = itemsControl switch
        {
            ListBox { SelectionMode: not SelectionMode.Single } listBox => listBox.SelectedItems,
            MultiSelector multiSelector => multiSelector.SelectedItems,
            _ => null,
        };

        if (selectedItems is null ||
            selectedItems.Count < 2 ||
            !selectedItems.Contains(item))
        {
            return [item];
        }

        var order = itemsControl.ItemsSource as IList ?? itemsControl.Items;
        return selectedItems
            .Cast<object>()
            .Select(x => (Item: x, Index: order.IndexOf(x)))
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => x.Item)
            .ToList();
    }

    /// <summary>
    /// Resolves the item a press on <paramref name="originalSource"/> would drag, or
    /// <see langword="null"/> when the press is not on an item or starts inside a control
    /// that uses the mouse itself (a TextBox, ScrollBar, Slider, ComboBox, Thumb, ...).
    /// </summary>
    internal static object? ResolvePressedItem(
        ItemsControl itemsControl,
        DependencyObject? originalSource)
    {
        if (originalSource is null ||
            DragDropHelper.IsDragExcludedSource(originalSource, itemsControl))
        {
            return null;
        }

        return DragDropHelper.GetItemFromElement(itemsControl, originalSource);
    }

    /// <summary>
    /// Determines whether <paramref name="source"/> sits in a button (Button, CheckBox, ToggleButton, ...)
    /// inside <paramref name="boundary"/>. Such a press must reach the button, so the selection-keeping
    /// suppression of the press is skipped; the drag itself is still possible.
    /// </summary>
    private static bool IsInsideButton(
        DependencyObject? source,
        DependencyObject boundary)
    {
        for (var element = source;
             element is not null && !ReferenceEquals(element, boundary);
             element = DragDropHelper.GetParent(element))
        {
            if (element is ButtonBase)
            {
                return true;
            }
        }

        return false;
    }

    private static void ClearPendingDrag()
    {
        dragStartPoint = null;
        pressedItemsControl = null;
        pressedItem = null;
        pressedSelection = null;
        isSingleSelectionDeferred = false;
    }

    /// <summary>
    /// Asks the optional <see cref="IDragHandler"/> whether the drag may start and,
    /// if so, runs the OLE drag-drop operation for <paramref name="item"/>.
    /// </summary>
    internal static void BeginDrag(
        ItemsControl itemsControl,
        object item)
        => BeginDrag(itemsControl, item, items: null);

    /// <summary>
    /// Asks the optional <see cref="IDragHandler"/> whether the drag may start and,
    /// if so, runs the OLE drag-drop operation for <paramref name="item"/> carrying
    /// <paramref name="items"/> (all dragged items; <see langword="null"/> for just <paramref name="item"/>).
    /// </summary>
    internal static void BeginDrag(
        ItemsControl itemsControl,
        object item,
        IReadOnlyList<object>? items)
    {
        var sourceIndex = itemsControl.ItemsSource is IList sourceList
            ? sourceList.IndexOf(item)
            : itemsControl.Items.IndexOf(item);

        var dragInfo = new DragInfo
        {
            SourceItem = item,
            SourceItems = items ?? [item],
            SourceCollection = itemsControl.ItemsSource,
            VisualSource = itemsControl,
            SourceIndex = sourceIndex,
            Effects = DragDropAttach.GetAllowedEffects(itemsControl),
        };

        var dragHandler = DragDropAttach.GetDragHandler(itemsControl);
        dragHandler?.StartDrag(dragInfo);

        if (dragInfo.Effects == DragDropEffects.None)
        {
            // The handler cancelled this drag; forget the press so the next
            // mouse move does not ask the handler again until a new press.
            ClearPendingDrag();
            return;
        }

        currentDragInfo = dragInfo;
        ClearPendingDrag();

        if (DragDropAttach.GetShowDragAdorner(itemsControl))
        {
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
            if (container is not null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                if (adornerLayer is not null)
                {
                    dragAdorner = new DragDropAdorner(itemsControl, container);
                    adornerLayer.Add(dragAdorner);
                }
            }
        }

        try
        {
            System.Windows.DragDrop.DoDragDrop(itemsControl, item, dragInfo.Effects);
        }
        finally
        {
            RemoveAllAdorners();
            currentDragInfo = null;
        }
    }

    private static void OnDragOver(
        object sender,
        DragEventArgs e)
    {
        if (sender is not UIElement targetElement)
        {
            return;
        }

        var dropInfo = CreateDropInfo(targetElement, e.Data, e.GetPosition(targetElement));
        var handler = GetDropHandler(targetElement);
        handler.DragOver(dropInfo);

        e.Effects = dropInfo.Effects;
        e.Handled = true;

        if (dragAdorner is not null)
        {
            var adornerSource = dragAdorner.AdornedElement;
            var position = e.GetPosition(adornerSource);
            dragAdorner.UpdatePosition(position);
        }

        UpdateInsertionAdorner(targetElement, dropInfo);
        AutoScroll(targetElement, e.GetPosition);
    }

    /// <summary>
    /// Scrolls the target one line while the pointer is in its edge zone. OLE keeps raising DragOver
    /// while the pointer rests, so holding the pointer at an edge keeps scrolling.
    /// </summary>
    internal static void AutoScroll(
        UIElement targetElement,
        Func<IInputElement, Point> getPosition)
    {
        var scrollViewer = targetElement as ScrollViewer
            ?? FindVisualChild<ScrollViewer>(targetElement);
        if (scrollViewer is null)
        {
            return;
        }

        var direction = DragDropHelper.GetAutoScrollDirection(
            new Size(scrollViewer.ActualWidth, scrollViewer.ActualHeight),
            getPosition(scrollViewer));

        if (direction.Y < 0 && scrollViewer.VerticalOffset > 0)
        {
            scrollViewer.LineUp();
        }
        else if (direction.Y > 0 && scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight)
        {
            scrollViewer.LineDown();
        }

        if (direction.X < 0 && scrollViewer.HorizontalOffset > 0)
        {
            scrollViewer.LineLeft();
        }
        else if (direction.X > 0 && scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
        {
            scrollViewer.LineRight();
        }
    }

    private static void OnDragEnter(
        object sender,
        DragEventArgs e)
    {
        if (sender is not UIElement targetElement)
        {
            return;
        }

        if (!DragDropAttach.GetShowDropIndicator(targetElement))
        {
            return;
        }

        var adornerLayer = AdornerLayer.GetAdornerLayer(targetElement);
        if (adornerLayer is null)
        {
            return;
        }

        RemoveHighlightAdorner();
        highlightAdorner = new DropTargetHighlightAdorner(targetElement);
        adornerLayer.Add(highlightAdorner);
    }

    private static void OnDragLeave(
        object sender,
        DragEventArgs e)
    {
        RemoveInsertionAdorner();
        RemoveHighlightAdorner();
    }

    private static void OnDrop(
        object sender,
        DragEventArgs e)
    {
        if (sender is not UIElement targetElement)
        {
            return;
        }

        var dropInfo = CreateDropInfo(targetElement, e.Data, e.GetPosition(targetElement));
        var handler = GetDropHandler(targetElement);

        handler.DragOver(dropInfo);
        if (dropInfo.Effects != DragDropEffects.None)
        {
            handler.Drop(dropInfo);
        }

        e.Handled = true;

        RemoveInsertionAdorner();
        RemoveHighlightAdorner();
    }

    /// <summary>
    /// Builds the <see cref="DropInfo"/> for a drag over <paramref name="targetElement"/> at
    /// <paramref name="position"/> (relative to the target). <see cref="DropInfo.InsertIndex"/> is an
    /// index in the target collection: the slot under the pointer in the shown items (the view, which
    /// may be filtered, sorted or grouped) is mapped to the source collection through its neighbour.
    /// </summary>
    internal static DropInfo CreateDropInfo(
        UIElement targetElement,
        IDataObject dataObject,
        Point position)
    {
        var isFileDrop = dataObject.GetDataPresent(DataFormats.FileDrop);
        StringCollection? fileDropList = null;
        object data;

        if (isFileDrop)
        {
            var files = dataObject.GetData(DataFormats.FileDrop) as string[];
            fileDropList = [];
            if (files is not null)
            {
                foreach (var file in files)
                {
                    fileDropList.Add(file);
                }
            }

            data = fileDropList;
        }
        else
        {
            data = currentDragInfo?.SourceItem ?? dataObject;
        }

        var itemsControl = targetElement as ItemsControl;
        var viewInsertIndex = itemsControl is not null
            ? DragDropHelper.GetInsertIndex(itemsControl, position)
            : 0;
        var insertIndex = itemsControl?.ItemsSource is IList targetList
            ? DragDropHelper.ToSourceInsertIndex(itemsControl, targetList, viewInsertIndex)
            : viewInsertIndex;

        return new DropInfo
        {
            Data = data,
            TargetCollection = itemsControl?.ItemsSource,
            VisualTarget = targetElement,
            InsertIndex = insertIndex,
            ViewInsertIndex = viewInsertIndex,
            DragInfo = currentDragInfo,
            IsFileDrop = isFileDrop,
            FileDropList = fileDropList,
        };
    }

    private static void UpdateInsertionAdorner(
        UIElement targetElement,
        DropInfo dropInfo)
    {
        if (!DragDropAttach.GetShowDropIndicator(targetElement) ||
            dropInfo.Effects == DragDropEffects.None)
        {
            RemoveInsertionAdorner();
            return;
        }

        if (targetElement is not ItemsControl itemsControl)
        {
            return;
        }

        var adornerLayer = AdornerLayer.GetAdornerLayer(targetElement);
        if (adornerLayer is null)
        {
            return;
        }

        if (insertionAdorner is null)
        {
            insertionAdorner = new DropTargetInsertionAdorner(targetElement);
            adornerLayer.Add(insertionAdorner);
        }

        // The line is drawn at the slot in the shown items, not at the index in the source collection.
        if (DragDropHelper.GetDropIndicatorLine(itemsControl, dropInfo.ViewInsertIndex) is { } line)
        {
            insertionAdorner.UpdateLine(line.Start, line.End);
        }
        else
        {
            insertionAdorner.UpdatePosition(0);
        }
    }

    private static IDropHandler GetDropHandler(UIElement element)
        => DragDropAttach.GetDropHandler(element) ?? FallbackDropHandler;

    private static void RemoveAllAdorners()
    {
        if (dragAdorner is not null)
        {
            var layer = AdornerLayer.GetAdornerLayer(dragAdorner.AdornedElement);
            layer?.Remove(dragAdorner);
            dragAdorner = null;
        }

        RemoveInsertionAdorner();
        RemoveHighlightAdorner();
    }

    private static void RemoveInsertionAdorner()
    {
        if (insertionAdorner is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(insertionAdorner.AdornedElement);
        layer?.Remove(insertionAdorner);
        insertionAdorner = null;
    }

    private static void RemoveHighlightAdorner()
    {
        if (highlightAdorner is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(highlightAdorner.AdornedElement);
        layer?.Remove(highlightAdorner);
        highlightAdorner = null;
    }

    private static T? FindVisualChild<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                return match;
            }

            var found = FindVisualChild<T>(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static T? FindVisualParent<T>(DependencyObject child)
        where T : DependencyObject
    {
        var parentObject = VisualTreeHelper.GetParent(child);
        while (parentObject is not null)
        {
            if (parentObject is T parent)
            {
                return parent;
            }

            parentObject = VisualTreeHelper.GetParent(parentObject);
        }

        return null;
    }
}