namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Extends <see cref="IUndoCommand"/> with an estimated memory footprint.
/// Commands implementing this interface participate in memory-based history trimming
/// when <see cref="IUndoRedoService.MaxHistoryMemory"/> is configured.
/// </summary>
public interface IMemoryAwareUndoCommand : IUndoCommand
{
    /// <summary>
    /// Gets the estimated number of bytes this command occupies in memory.
    /// Used by the undo/redo service to enforce a memory budget.
    /// </summary>
    long EstimatedMemoryBytes { get; }
}