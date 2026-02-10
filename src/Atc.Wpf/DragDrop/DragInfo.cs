namespace Atc.Wpf.DragDrop;

/// <summary>
/// Holds information about a drag operation in progress.
/// </summary>
public sealed class DragInfo
{
    /// <summary>
    /// Gets the dragged data item.
    /// </summary>
    public required object SourceItem { get; init; }

    /// <summary>
    /// Gets the source items collection.
    /// </summary>
    public System.Collections.IEnumerable? SourceCollection { get; init; }

    /// <summary>
    /// Gets the source UIElement (typically an ItemsControl).
    /// </summary>
    public required UIElement VisualSource { get; init; }

    /// <summary>
    /// Gets the index of the item within the source collection.
    /// </summary>
    public int SourceIndex { get; init; }

    /// <summary>
    /// Gets or sets the allowed drag-drop effects.
    /// </summary>
    public DragDropEffects Effects { get; set; } = DragDropEffects.Move;
}