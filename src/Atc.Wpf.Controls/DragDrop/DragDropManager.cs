namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Internal manager that handles all drag-drop event wiring and coordination.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Event handler methods for drag-drop coordination.")]
internal static class DragDropManager
{
    private static readonly DefaultDropHandler FallbackDropHandler = new();

    private static Point? dragStartPoint;
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
        dragStartPoint = e.GetPosition(null);
    }

    private static void OnPreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        dragStartPoint = null;
    }

    private static void OnPreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed ||
            dragStartPoint is null ||
            sender is not UIElement sourceElement)
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

        var itemsControl = sourceElement as ItemsControl
            ?? FindVisualParent<ItemsControl>(sourceElement);

        if (itemsControl is null)
        {
            return;
        }

        var item = GetDraggedItem(itemsControl, e);
        if (item is null)
        {
            return;
        }

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
            return;
        }

        currentDragInfo = dragInfo;
        dragStartPoint = null;

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

    private static object? GetDraggedItem(
        ItemsControl itemsControl,
        MouseEventArgs e)
    {
        var position = e.GetPosition(itemsControl);
        var element = itemsControl.InputHitTest(position) as DependencyObject;

        while (element is not null)
        {
            if (element is FrameworkElement fe)
            {
                var item = itemsControl.ItemContainerGenerator.ItemFromContainer(fe);
                if (item != DependencyProperty.UnsetValue)
                {
                    return item;
                }
            }

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
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