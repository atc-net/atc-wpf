namespace Atc.Wpf.Components.UndoRedo;

/// <summary>
/// Represents a single row in the undo/redo history view.
/// </summary>
public sealed class UndoRedoHistoryItem
{
    /// <summary>
    /// Gets the human-readable description for this history row.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item is in the redo portion of the history.
    /// </summary>
    public bool IsRedo { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item represents the current position.
    /// </summary>
    public bool IsHighlighted { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item represents the save point.
    /// </summary>
    public bool IsSavePoint { get; init; }

    /// <summary>
    /// Gets the underlying undo command, or <see langword="null"/> for the root "initial state" row.
    /// </summary>
    public IUndoCommand? Command { get; init; }
}