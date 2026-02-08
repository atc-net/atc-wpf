namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides an MVVM-friendly undo/redo service with dual stacks,
/// command grouping, configurable history limit, and change notifications.
/// </summary>
public interface IUndoRedoService
{
    /// <summary>
    /// Gets the current undo stack (most recent first).
    /// </summary>
    IReadOnlyList<IUndoCommand> UndoCommands { get; }

    /// <summary>
    /// Gets the current redo stack (most recent first).
    /// </summary>
    IReadOnlyList<IUndoCommand> RedoCommands { get; }

    /// <summary>
    /// Gets a value indicating whether there are commands to undo.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Gets a value indicating whether there are commands to redo.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Gets a value indicating whether a command is currently being executed,
    /// undone, or redone. Use this to guard against recursive additions.
    /// </summary>
    bool IsExecuting { get; }

    /// <summary>
    /// Gets or sets the maximum number of commands retained in the undo history.
    /// Default is 100. Oldest commands are discarded when the limit is exceeded.
    /// </summary>
    int MaxHistorySize { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current state differs from the
    /// last saved state. Use with <see cref="MarkSaved"/> to track document
    /// modifications (e.g., enabling/disabling a Save button or showing a
    /// dirty indicator in the title bar).
    /// </summary>
    bool HasUnsavedChanges { get; }

    /// <summary>
    /// Raised after a command is executed, undone, or redone.
    /// </summary>
    event EventHandler<UndoRedoEventArgs>? ActionPerformed;

    /// <summary>
    /// Raised when the undo/redo state changes (stacks modified or cleared).
    /// </summary>
    event EventHandler? StateChanged;

    /// <summary>
    /// Executes the command and pushes it onto the undo stack.
    /// Clears the redo stack.
    /// </summary>
    /// <param name="command">The command to execute and record.</param>
    void Execute(IUndoCommand command);

    /// <summary>
    /// Records a command on the undo stack without executing it.
    /// Use this for actions that have already been performed.
    /// Clears the redo stack.
    /// </summary>
    /// <param name="command">The command to record.</param>
    void Add(IUndoCommand command);

    /// <summary>
    /// Undoes the most recent command.
    /// </summary>
    /// <returns><see langword="true"/> if a command was undone; otherwise <see langword="false"/>.</returns>
    bool Undo();

    /// <summary>
    /// Redoes the most recently undone command.
    /// </summary>
    /// <returns><see langword="true"/> if a command was redone; otherwise <see langword="false"/>.</returns>
    bool Redo();

    /// <summary>
    /// Undoes all commands in the undo stack.
    /// </summary>
    void UndoAll();

    /// <summary>
    /// Redoes all commands in the redo stack.
    /// </summary>
    void RedoAll();

    /// <summary>
    /// Undoes commands until the specified command has been undone (inclusive).
    /// </summary>
    /// <param name="command">The target command to undo to.</param>
    void UndoTo(IUndoCommand command);

    /// <summary>
    /// Redoes commands until the specified command has been redone (inclusive).
    /// </summary>
    /// <param name="command">The target command to redo to.</param>
    void RedoTo(IUndoCommand command);

    /// <summary>
    /// Begins a command group. All commands executed or added while
    /// the returned scope is active are collected into a single
    /// <see cref="UndoCommandGroup"/> that is committed when the scope is disposed.
    /// </summary>
    /// <param name="description">A description for the grouped operation.</param>
    /// <returns>An <see cref="IDisposable"/> scope; dispose to commit the group.</returns>
    IDisposable BeginGroup(string description);

    /// <summary>
    /// Clears both the undo and redo stacks.
    /// </summary>
    void Clear();

    /// <summary>
    /// Marks the current state as saved. <see cref="HasUnsavedChanges"/> will
    /// return <see langword="false"/> until the undo/redo position changes
    /// from this point.
    /// </summary>
    void MarkSaved();
}