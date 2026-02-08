namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Specifies the type of undo/redo action that was performed.
/// </summary>
public enum UndoRedoActionType
{
    /// <summary>
    /// No action.
    /// </summary>
    None,

    /// <summary>
    /// A command was executed (or added) for the first time.
    /// </summary>
    Execute,

    /// <summary>
    /// A command was undone.
    /// </summary>
    Undo,

    /// <summary>
    /// A command was redone.
    /// </summary>
    Redo,
}