namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Represents a named checkpoint in the undo/redo history
/// that can be restored independently of the linear undo stack.
/// </summary>
public interface IUndoSnapshot
{
    /// <summary>
    /// Gets the user-provided name for this snapshot.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the UTC timestamp when this snapshot was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }
}