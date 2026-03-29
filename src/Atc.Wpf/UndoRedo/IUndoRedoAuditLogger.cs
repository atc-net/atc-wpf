namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Defines a structured audit logger for undo/redo operations.
/// </summary>
public interface IUndoRedoAuditLogger
{
    /// <summary>
    /// Gets the recorded audit entries.
    /// </summary>
    IReadOnlyList<UndoRedoAuditEntry> Entries { get; }

    /// <summary>
    /// Logs an audit entry.
    /// </summary>
    /// <param name="entry">The entry to log.</param>
    void Log(UndoRedoAuditEntry entry);

    /// <summary>
    /// Clears all recorded entries.
    /// </summary>
    void Clear();
}