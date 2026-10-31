namespace Atc.Wpf.DragDrop;

/// <summary>
/// Holds information about a drag operation in progress.
/// </summary>
public sealed class DragInfo
{
    private readonly IReadOnlyList<object>? sourceItems;

    /// <summary>
    /// Gets the dragged data item: the item the drag was started on.
    /// </summary>
    public required object SourceItem { get; init; }

    /// <summary>
    /// Gets all dragged data items, in the order of <see cref="SourceCollection"/>.
    /// </summary>
    /// <remarks>
    /// When the drag starts on a selected item of a multi-select ListBox or DataGrid with more
    /// than one selected item, this holds the whole selection; otherwise it holds only
    /// <see cref="SourceItem"/>.
    /// </remarks>
    public IReadOnlyList<object> SourceItems
    {
        get => sourceItems is { Count: > 0 } ? sourceItems : [SourceItem];
        init => sourceItems = value;
    }

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