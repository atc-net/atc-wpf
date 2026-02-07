namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Segmented.SelectionChanged"/> event.
/// </summary>
public sealed class SegmentedSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentedSelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldIndex">The previously selected index (-1 if none).</param>
    /// <param name="newIndex">The newly selected index (-1 if none).</param>
    /// <param name="oldItem">The previously selected item, or null.</param>
    /// <param name="newItem">The newly selected item, or null.</param>
    public SegmentedSelectionChangedEventArgs(
        int oldIndex,
        int newIndex,
        SegmentedItem? oldItem,
        SegmentedItem? newItem)
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Gets the previously selected index (-1 if none).
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the newly selected index (-1 if none).
    /// </summary>
    public int NewIndex { get; }

    /// <summary>
    /// Gets the previously selected item, or null.
    /// </summary>
    public SegmentedItem? OldItem { get; }

    /// <summary>
    /// Gets the newly selected item, or null.
    /// </summary>
    public SegmentedItem? NewItem { get; }
}