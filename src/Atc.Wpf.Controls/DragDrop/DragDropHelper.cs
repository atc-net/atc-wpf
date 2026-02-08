namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Shared helper methods for drag-and-drop index calculations.
/// Used by both <see cref="DragDropManager"/> and DualListSelector.
/// </summary>
internal static class DragDropHelper
{
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