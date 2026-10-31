namespace Atc.Wpf.DragDrop;

/// <summary>
/// Shared helper methods for drag-and-drop index calculations.
/// Used by both <see cref="DragDropManager"/> and DualListSelector.
/// </summary>
internal static class DragDropHelper
{
    /// <summary>
    /// Gets the parent of <paramref name="element"/>: the visual parent for visuals, otherwise
    /// the logical parent (a <see cref="Run"/> or <see cref="Hyperlink"/> inside a TextBlock is
    /// a <see cref="ContentElement"/>, which <see cref="VisualTreeHelper.GetParent"/> rejects).
    /// </summary>
    internal static DependencyObject? GetParent(DependencyObject element)
        => element is Visual or System.Windows.Media.Media3D.Visual3D
            ? VisualTreeHelper.GetParent(element)
            : LogicalTreeHelper.GetParent(element);

    /// <summary>
    /// Determines whether a press on <paramref name="source"/> must not start a drag, because it
    /// sits in a control that uses the mouse itself (text selection, scrolling, sliding, a drop-down,
    /// a resize thumb or a column header) somewhere between <paramref name="source"/> and
    /// <paramref name="boundary"/> (the drag source, exclusive).
    /// </summary>
    internal static bool IsDragExcludedSource(
        DependencyObject? source,
        DependencyObject boundary)
    {
        for (var element = source;
             element is not null && !ReferenceEquals(element, boundary);
             element = GetParent(element))
        {
            if (element is TextBoxBase or PasswordBox or ComboBox or RangeBase or Thumb
                or GridViewColumnHeader or DataGridColumnHeader)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds the data item of the <paramref name="itemsControl"/> container that holds
    /// <paramref name="element"/>, or <see langword="null"/> when the element is not inside an item
    /// (for example on the scroll bar or the empty area below the items).
    /// </summary>
    internal static object? GetItemFromElement(
        ItemsControl itemsControl,
        DependencyObject? element)
    {
        ArgumentNullException.ThrowIfNull(itemsControl);

        if (GetContainerFromElement(itemsControl, element) is not { } container)
        {
            return null;
        }

        var item = itemsControl.ItemContainerGenerator.ItemFromContainer(container);
        return item == DependencyProperty.UnsetValue
            ? null
            : item;
    }

    /// <summary>
    /// Finds the item container of <paramref name="itemsControl"/> that holds <paramref name="element"/>
    /// (also inside groups), or <see langword="null"/> when the element is not inside an item.
    /// </summary>
    internal static FrameworkElement? GetContainerFromElement(
        ItemsControl itemsControl,
        DependencyObject? element)
    {
        // Climb out of content elements (Run, Hyperlink) to the first visual.
        while (element is not null and not Visual)
        {
            element = GetParent(element);
        }

        return element is null
            ? null
            : ItemsControl.ContainerFromElement(itemsControl, element) as FrameworkElement;
    }

    /// <summary>
    /// Calculates the insertion index, in the items shown by <paramref name="itemsControl"/>
    /// (its view: filtered, sorted and grouped), for the position of the drag event.
    /// </summary>
    /// <param name="itemsControl">The target items control.</param>
    /// <param name="e">The drag event arguments providing position data.</param>
    /// <returns>The zero-based view index where the item should be inserted.</returns>
    internal static int GetInsertIndex(
        ItemsControl itemsControl,
        DragEventArgs e)
        => GetInsertIndex(itemsControl, e.GetPosition(itemsControl));

    /// <summary>
    /// Calculates the insertion index, in the items shown by <paramref name="itemsControl"/>
    /// (its view: filtered, sorted and grouped), for <paramref name="position"/>.
    /// </summary>
    /// <remarks>
    /// Only the container under the pointer is looked at (or, over a gap, the nearest realized one),
    /// so the cost does not grow with the number of items. The pointer is compared with the middle of
    /// that container along the flow of its panel: horizontally for a horizontal StackPanel or
    /// WrapPanel, vertically otherwise. The comparison is made in the container's own coordinates,
    /// so right-to-left layouts need no special case.
    /// </remarks>
    /// <param name="itemsControl">The target items control.</param>
    /// <param name="position">The pointer position relative to <paramref name="itemsControl"/>.</param>
    /// <returns>The zero-based view index where the item should be inserted.</returns>
    internal static int GetInsertIndex(
        ItemsControl itemsControl,
        Point position)
    {
        ArgumentNullException.ThrowIfNull(itemsControl);

        var container = GetContainerFromElement(itemsControl, itemsControl.InputHitTest(position) as DependencyObject)
                        ?? FindNearestContainer(itemsControl, position);
        if (container is null)
        {
            return itemsControl.Items.Count;
        }

        var index = IndexOfContainer(itemsControl, container);
        if (index < 0)
        {
            return itemsControl.Items.Count;
        }

        var pointInContainer = itemsControl.TranslatePoint(position, container);
        var isAfter = IsHorizontalFlow(container)
            ? pointInContainer.X >= container.ActualWidth / 2
            : pointInContainer.Y >= container.ActualHeight / 2;

        return isAfter
            ? index + 1
            : index;
    }

    /// <summary>
    /// Maps an insertion index in the items shown by <paramref name="itemsControl"/> (its view)
    /// to an insertion index in <paramref name="sourceCollection"/>, through the shown neighbour:
    /// before the shown item at <paramref name="viewInsertIndex"/>, or right after the shown item
    /// before it. With a filter, sort or grouping the two indexes differ.
    /// </summary>
    internal static int ToSourceInsertIndex(
        ItemsControl itemsControl,
        IList sourceCollection,
        int viewInsertIndex)
    {
        ArgumentNullException.ThrowIfNull(itemsControl);
        ArgumentNullException.ThrowIfNull(sourceCollection);

        var shown = itemsControl.Items;
        viewInsertIndex = System.Math.Max(viewInsertIndex, 0);

        if (viewInsertIndex < shown.Count)
        {
            var anchorIndex = sourceCollection.IndexOf(shown[viewInsertIndex]);
            if (anchorIndex >= 0)
            {
                return anchorIndex;
            }
        }

        if (viewInsertIndex > 0 && shown.Count > 0)
        {
            var previous = shown[System.Math.Min(viewInsertIndex, shown.Count) - 1];
            var previousIndex = sourceCollection.IndexOf(previous);
            if (previousIndex >= 0)
            {
                return previousIndex + 1;
            }
        }

        return sourceCollection.Count;
    }

    /// <summary>
    /// Calculates the Y position for a drop indicator line within an <see cref="ItemsControl"/>.
    /// </summary>
    /// <param name="itemsControl">The target items control.</param>
    /// <param name="dropIndex">The insertion index.</param>
    /// <returns>The Y coordinate relative to the items control where the indicator should be drawn.</returns>
    internal static double GetDropIndicatorY(
        ItemsControl itemsControl,
        int dropIndex)
    {
        if (dropIndex <= 0)
        {
            return 0;
        }

        var containerIndex = System.Math.Min(dropIndex, itemsControl.Items.Count) - 1;
        if (GetContainerAt(itemsControl, containerIndex) is { } container &&
            container.IsDescendantOf(itemsControl))
        {
            var transform = container.TransformToAncestor(itemsControl);
            var point = transform.Transform(new Point(0, container.ActualHeight));
            return point.Y;
        }

        return 0;
    }

    /// <summary>
    /// Calculates the insertion line for a drop at view index <paramref name="dropIndex"/>, relative to
    /// <paramref name="itemsControl"/>: a horizontal line across the control for vertical lists, and a
    /// vertical line along the neighbouring item for horizontal and wrapping lists.
    /// </summary>
    /// <returns>The line, or <see langword="null"/> when no neighbouring container is realized.</returns>
    internal static (Point Start, Point End)? GetDropIndicatorLine(
        ItemsControl itemsControl,
        int dropIndex)
    {
        ArgumentNullException.ThrowIfNull(itemsControl);

        var count = itemsControl.Items.Count;
        var isLeadingEdge = dropIndex >= 0 && dropIndex < count;
        var container = isLeadingEdge
            ? GetContainerAt(itemsControl, dropIndex)
            : null;

        if (container is null && dropIndex > 0)
        {
            isLeadingEdge = false;
            container = GetContainerAt(itemsControl, System.Math.Min(dropIndex, count) - 1);
        }

        if (container is null || !container.IsDescendantOf(itemsControl))
        {
            return null;
        }

        var transform = container.TransformToAncestor(itemsControl);
        if (IsHorizontalFlow(container))
        {
            var x = isLeadingEdge ? 0 : container.ActualWidth;
            return (transform.Transform(new Point(x, 0)), transform.Transform(new Point(x, container.ActualHeight)));
        }

        var y = transform.Transform(new Point(0, isLeadingEdge ? 0 : container.ActualHeight)).Y;
        return (new Point(0, y), new Point(itemsControl.RenderSize.Width, y));
    }

    /// <summary>
    /// Determines whether the panel hosting <paramref name="container"/> lays its items out horizontally.
    /// </summary>
    internal static bool IsHorizontalFlow(FrameworkElement container)
        => VisualTreeHelper.GetParent(container) switch
        {
            StackPanel stackPanel => stackPanel.Orientation == Orientation.Horizontal,
            VirtualizingStackPanel virtualizingStackPanel => virtualizingStackPanel.Orientation == Orientation.Horizontal,
            WrapPanel wrapPanel => wrapPanel.Orientation == Orientation.Horizontal,
            UniformGrid uniformGrid => uniformGrid.Columns != 1,
            TabPanel => true,
            _ => false,
        };

    private static FrameworkElement? GetContainerAt(
        ItemsControl itemsControl,
        int index)
        => index >= 0 && index < itemsControl.Items.Count
            ? itemsControl.ItemContainerGenerator.ContainerFromItem(itemsControl.Items[index]) as FrameworkElement
            : null;

    private static int IndexOfContainer(
        ItemsControl itemsControl,
        FrameworkElement container)
    {
        var index = itemsControl.ItemContainerGenerator.IndexFromContainer(container, returnLocalIndex: false);
        if (index >= 0)
        {
            return index;
        }

        var item = itemsControl.ItemContainerGenerator.ItemFromContainer(container);
        return item == DependencyProperty.UnsetValue
            ? -1
            : itemsControl.Items.IndexOf(item);
    }

    /// <summary>
    /// Finds the realized container closest to <paramref name="position"/>, for a pointer over a gap
    /// between items or the empty area after them. Only realized containers are looked at, so with
    /// virtualization the cost is bounded by the visible items.
    /// </summary>
    private static FrameworkElement? FindNearestContainer(
        ItemsControl itemsControl,
        Point position)
    {
        FrameworkElement? nearest = null;
        var nearestDistance = double.MaxValue;

        foreach (var container in EnumerateRealizedContainers(itemsControl, FindItemsHost(itemsControl)))
        {
            var bounds = container
                .TransformToAncestor(itemsControl)
                .TransformBounds(new Rect(container.RenderSize));
            var dx = System.Math.Max(System.Math.Max(bounds.Left - position.X, position.X - bounds.Right), 0);
            var dy = System.Math.Max(System.Math.Max(bounds.Top - position.Y, position.Y - bounds.Bottom), 0);
            var distance = (dx * dx) + (dy * dy);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = container;
            }
        }

        return nearest;
    }

    private static IEnumerable<FrameworkElement> EnumerateRealizedContainers(
        ItemsControl itemsControl,
        Panel? itemsHost)
    {
        if (itemsHost is null)
        {
            yield break;
        }

        foreach (var child in itemsHost.Children)
        {
            if (child is not FrameworkElement { IsVisible: true } element ||
                !element.IsDescendantOf(itemsControl))
            {
                continue;
            }

            if (element is GroupItem groupItem)
            {
                foreach (var nested in EnumerateRealizedContainers(itemsControl, FindItemsHost(groupItem)))
                {
                    yield return nested;
                }
            }
            else
            {
                yield return element;
            }
        }
    }

    private static Panel? FindItemsHost(DependencyObject root)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is Panel { IsItemsHost: true } panel)
            {
                return panel;
            }

            if (child is ItemsControl or GroupItem)
            {
                // The items hosts of nested items controls and groups belong to them.
                continue;
            }

            var found = FindItemsHost(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}