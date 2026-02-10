namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Provides data for the <see cref="DualListSelector.ItemsReordered"/> event.
/// </summary>
public sealed class DualListSelectorItemsReorderedEventArgs : EventArgs
{
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