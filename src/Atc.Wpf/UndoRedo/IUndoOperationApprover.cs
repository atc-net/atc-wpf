namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Allows an application to approve or veto undo/redo operations
/// before they are executed. This is useful when an operation is
/// destructive or expensive and requires user confirmation.
/// </summary>
public interface IUndoOperationApprover
{
    /// <summary>
    /// Determines whether the specified undo command should proceed.
    /// </summary>
    /// <param name="command">The command about to be undone.</param>
    /// <returns><see langword="true"/> to allow the undo; <see langword="false"/> to veto it.</returns>
    bool ApproveUndo(IUndoCommand command);

    /// <summary>
    /// Determines whether the specified redo command should proceed.
    /// </summary>
    /// <param name="command">The command about to be redone.</param>
    /// <returns><see langword="true"/> to allow the redo; <see langword="false"/> to veto it.</returns>
    bool ApproveRedo(IUndoCommand command);
}