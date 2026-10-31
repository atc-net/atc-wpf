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
    }

    private static void OnPreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
        => ClearPendingDrag();

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

        BeginDrag(pressedItemsControl, pressedItem);
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

    private static void ClearPendingDrag()
    {
        dragStartPoint = null;
        pressedItemsControl = null;
        pressedItem = null;
    }

    /// <summary>
    /// Asks the optional <see cref="IDragHandler"/> whether the drag may start and,
    /// if so, runs the OLE drag-drop operation for <paramref name="item"/>.
    /// </summary>
    internal static void BeginDrag(
        ItemsControl itemsControl,
        object item)
    {
        var sourceIndex = itemsControl.Items.IndexOf(item);

        var dragInfo = new DragInfo
        {
            SourceItem = item,
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

        var dropInfo = CreateDropInfo(targetElement, e);
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

        var dropInfo = CreateDropInfo(targetElement, e);
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

    private static DropInfo CreateDropInfo(
        UIElement targetElement,
        DragEventArgs e)
    {
        var isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
        StringCollection? fileDropList = null;
        object data;

        if (isFileDrop)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
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
            data = currentDragInfo?.SourceItem ?? e.Data;
        }

        var itemsControl = targetElement as ItemsControl;
        var insertIndex = itemsControl is not null
            ? DragDropHelper.GetInsertIndex(itemsControl, e)
            : 0;

        return new DropInfo
        {
            Data = data,
            TargetCollection = itemsControl?.ItemsSource,
            VisualTarget = targetElement,
            InsertIndex = insertIndex,
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

        var yPosition = DragDropHelper.GetDropIndicatorY(itemsControl, dropInfo.InsertIndex);
        insertionAdorner.UpdatePosition(yPosition);
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