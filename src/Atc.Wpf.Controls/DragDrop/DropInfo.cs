namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Holds information about a drop operation.
/// </summary>
public sealed class DropInfo
{
    /// <summary>
    /// Gets the dragged data item (or IDataObject for file drops).
    /// </summary>
    public required object Data { get; init; }

    /// <summary>
    /// Gets the target items collection.
    /// </summary>
    public System.Collections.IEnumerable? TargetCollection { get; init; }

    /// <summary>
    /// Gets the target UIElement (typically an ItemsControl).
    /// </summary>
    public required UIElement VisualTarget { get; init; }

    /// <summary>
    /// Gets the computed insertion index in the target collection.
    /// </summary>
    public int InsertIndex { get; init; }

    /// <summary>
    /// Gets or sets the drag-drop effects. Set in <see cref="IDropHandler.DragOver"/>
    /// to allow or deny the drop.
    /// </summary>
    public DragDropEffects Effects { get; set; } = DragDropEffects.None;

    /// <summary>
    /// Gets the original drag information.
    /// </summary>
    public DragInfo? DragInfo { get; init; }

    /// <summary>
    /// Gets a value indicating whether files were dropped from the file system.
    /// </summary>
    public bool IsFileDrop { get; init; }

    /// <summary>
    /// Gets the file paths when <see cref="IsFileDrop"/> is <see langword="true"/>.
    /// </summary>
    public StringCollection? FileDropList { get; init; }
}