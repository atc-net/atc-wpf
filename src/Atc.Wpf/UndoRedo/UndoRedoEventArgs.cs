namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides data for the <see cref="IUndoRedoService.ActionPerformed"/> event.
/// </summary>
public sealed class UndoRedoEventArgs : EventArgs
{
    public UndoRedoEventArgs(
        UndoRedoActionType actionType,
        IUndoCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        ActionType = actionType;
        Command = command;
    }

    /// <summary>
    /// Gets the type of action that was performed.
    /// </summary>
    public UndoRedoActionType ActionType { get; }

    /// <summary>
    /// Gets the command that was affected.
    /// </summary>
    public IUndoCommand Command { get; }
}