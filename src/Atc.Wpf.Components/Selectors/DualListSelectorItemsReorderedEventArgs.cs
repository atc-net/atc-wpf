namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Provides data for the <see cref="DualListSelector.ItemsReordered"/> event.
/// </summary>
public sealed class DualListSelectorItemsReorderedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DualListSelectorItemsReorderedEventArgs"/> class.
    /// </summary>
    /// <param name="item">The item that was reordered.</param>
    /// <param name="oldIndex">The previous index of the item.</param>
    /// <param name="newIndex">The new index of the item.</param>
    public DualListSelectorItemsReorderedEventArgs(
        DualListSelectorItem item,
        int oldIndex,
        int newIndex)
    {
        Item = item;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    /// <summary>
    /// Gets the item that was reordered.
    /// </summary>
    public DualListSelectorItem Item { get; }

    /// <summary>
    /// Gets the previous index of the item.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the new index of the item.
    /// </summary>
    public int NewIndex { get; }
}