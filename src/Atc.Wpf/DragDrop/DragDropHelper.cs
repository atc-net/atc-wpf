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

        // Climb out of content elements (Run, Hyperlink) to the first visual.
        while (element is not null and not Visual)
        {
            element = GetParent(element);
        }

        if (element is null ||
            ItemsControl.ContainerFromElement(itemsControl, element) is not { } container)
        {
            return null;
        }

        var item = itemsControl.ItemContainerGenerator.ItemFromContainer(container);
        return item == DependencyProperty.UnsetValue
            ? null
            : item;
    }

    /// <summary>
    /// Calculates the insertion index within an <see cref="ItemsControl"/>
    /// based on the vertical position of the drag event.
    /// </summary>
    /// <param name="itemsControl">The target items control.</param>
    /// <param name="e">The drag event arguments providing position data.</param>
    /// <returns>The zero-based index where the item should be inserted.</returns>
    internal static int GetInsertIndex(
        ItemsControl itemsControl,
        DragEventArgs e)
    {
        for (var i = 0; i < itemsControl.Items.Count; i++)
        {
            if (itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is not FrameworkElement container)
            {
                continue;
            }

            var position = e.GetPosition(container);
            var bounds = VisualTreeHelper.GetDescendantBounds(container);
            if (position.Y < bounds.Height / 2)
            {
                return i;
            }
        }

        return itemsControl.Items.Count;
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
        if (itemsControl.ItemContainerGenerator.ContainerFromIndex(containerIndex) is FrameworkElement container)
        {
            var transform = container.TransformToAncestor(itemsControl);
            var point = transform.Transform(new Point(0, container.ActualHeight));
            return point.Y;
        }

        return 0;
    }
}