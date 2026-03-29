namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Represents a single audit log entry for an undo/redo operation.
/// </summary>
public sealed class UndoRedoAuditEntry
{
    public UndoRedoAuditEntry(
        UndoRedoActionType actionType,
        string description,
        DateTimeOffset timestamp)
    {
        ActionType = actionType;
        Description = description;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Gets the type of action that was performed.
    /// </summary>
    public UndoRedoActionType ActionType { get; }

    /// <summary>
    /// Gets a human-readable description of the operation.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the UTC timestamp when the operation occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; }
}