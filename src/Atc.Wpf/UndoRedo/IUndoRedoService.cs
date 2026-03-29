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
    /// Gets a value indicating whether undo recording is currently suspended.
    /// When suspended, <see cref="Execute"/> still runs commands but does not
    /// push them onto the undo stack, and <see cref="Add"/> is a no-op.
    /// </summary>
    bool IsRecordingSuspended { get; }

    /// <summary>
    /// Gets the type of action currently being performed.
    /// Returns <see cref="UndoRedoActionType.None"/> when idle.
    /// </summary>
    UndoRedoActionType ExecutingAction { get; }

    /// <summary>
    /// Gets the command currently being executed, undone, or redone,
    /// or <see langword="null"/> when idle.
    /// </summary>
    IUndoCommand? ExecutingCommand { get; }

    /// <summary>
    /// Gets or sets the maximum number of commands retained in the undo history.
    /// Default is 100. Oldest commands are discarded when the limit is exceeded.
    /// </summary>
    int MaxHistorySize { get; set; }

    /// <summary>
    /// Gets or sets the maximum total estimated memory (in bytes) for commands
    /// in the undo history. When exceeded, oldest commands are purged.
    /// A value of 0 (the default) disables memory-based trimming.
    /// </summary>
    long MaxHistoryMemory { get; set; }

    /// <summary>
    /// Gets the current total estimated memory (in bytes) of all commands
    /// in the undo stack that implement <see cref="IMemoryAwareUndoCommand"/>.
    /// Commands that do not implement the interface contribute 0 bytes.
    /// </summary>
    long CurrentHistoryMemory { get; }

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
    /// Undoes the specified number of commands.
    /// </summary>
    /// <param name="levels">The number of commands to undo. Must be greater than zero.</param>
    /// <returns><see langword="true"/> if at least one command was undone; otherwise <see langword="false"/>.</returns>
    bool Undo(int levels);

    /// <summary>
    /// Undoes the most recent command that belongs to the specified context.
    /// Scans from the top of the undo stack for the first command matching the context,
    /// then undoes all commands above it (inclusive), equivalent to calling
    /// <see cref="UndoTo"/> on the matching command.
    /// </summary>
    /// <param name="context">The context to filter by.</param>
    /// <returns><see langword="true"/> if at least one command was undone; otherwise <see langword="false"/>.</returns>
    bool Undo(UndoContext context);

    /// <summary>
    /// Redoes the most recently undone command.
    /// </summary>
    /// <returns><see langword="true"/> if a command was redone; otherwise <see langword="false"/>.</returns>
    bool Redo();

    /// <summary>
    /// Redoes the specified number of commands.
    /// </summary>
    /// <param name="levels">The number of commands to redo. Must be greater than zero.</param>
    /// <returns><see langword="true"/> if at least one command was redone; otherwise <see langword="false"/>.</returns>
    bool Redo(int levels);

    /// <summary>
    /// Redoes the most recent command that belongs to the specified context.
    /// Scans from the top of the redo stack for the first command matching the context,
    /// then redoes all commands above it (inclusive), equivalent to calling
    /// <see cref="RedoTo"/> on the matching command.
    /// </summary>
    /// <param name="context">The context to filter by.</param>
    /// <returns><see langword="true"/> if at least one command was redone; otherwise <see langword="false"/>.</returns>
    bool Redo(UndoContext context);

    /// <summary>
    /// Finds the undo commands that belong to the specified context.
    /// Only commands implementing <see cref="IRichUndoCommand"/> with the
    /// given context in their <see cref="IRichUndoCommand.Contexts"/> list are returned.
    /// </summary>
    /// <param name="context">The context to filter by.</param>
    /// <returns>A filtered list of undo commands matching the context.</returns>
    IReadOnlyList<IUndoCommand> FindUndoCommands(UndoContext context);

    /// <summary>
    /// Finds the redo commands that belong to the specified context.
    /// Only commands implementing <see cref="IRichUndoCommand"/> with the
    /// given context in their <see cref="IRichUndoCommand.Contexts"/> list are returned.
    /// </summary>
    /// <param name="context">The context to filter by.</param>
    /// <returns>A filtered list of redo commands matching the context.</returns>
    IReadOnlyList<IUndoCommand> FindRedoCommands(UndoContext context);

    /// <summary>
    /// Undoes all commands in the undo stack.
    /// </summary>
    void UndoAll();

    /// <summary>
    /// Redoes all commands in the redo stack.
    /// </summary>
    void RedoAll();

    /// <summary>
    /// Undoes commands until a command with <see cref="IRichUndoCommand.AllowUserAction"/>
    /// set to <see langword="true"/> is reached (inclusive). Plain <see cref="IUndoCommand"/>
    /// instances are treated as user actions.
    /// </summary>
    /// <returns><see langword="true"/> if at least one command was undone; otherwise <see langword="false"/>.</returns>
    bool UndoToLastUserAction();

    /// <summary>
    /// Redoes commands until a command with <see cref="IRichUndoCommand.AllowUserAction"/>
    /// set to <see langword="true"/> is reached (inclusive). Plain <see cref="IUndoCommand"/>
    /// instances are treated as user actions.
    /// </summary>
    /// <returns><see langword="true"/> if at least one command was redone; otherwise <see langword="false"/>.</returns>
    bool RedoToLastUserAction();

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
    /// Temporarily suspends undo recording. While the returned scope is active,
    /// <see cref="Execute"/> still runs commands but does not push them onto the
    /// undo stack or fire events, and <see cref="Add"/> is a no-op.
    /// Supports nesting: recording resumes only when all scopes are disposed.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> scope; dispose to resume recording.</returns>
    IDisposable SuspendRecording();

    /// <summary>
    /// Registers an approver that can veto undo/redo operations.
    /// </summary>
    /// <param name="approver">The approver to register.</param>
    void RegisterApprover(IUndoOperationApprover approver);

    /// <summary>
    /// Removes a previously registered approver.
    /// </summary>
    /// <param name="approver">The approver to remove.</param>
    void UnregisterApprover(IUndoOperationApprover approver);

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

    /// <summary>
    /// Gets or sets a value indicating whether non-linear history (branching) is enabled.
    /// When <see langword="true"/>, executing a new command after an undo preserves
    /// the current redo stack as a branch instead of discarding it.
    /// Default is <see langword="false"/> (standard linear behavior).
    /// </summary>
    bool AllowNonLinearHistory { get; set; }

    /// <summary>
    /// Gets the saved redo branches. Each branch is a read-only list of commands
    /// representing a previously saved redo path. Only populated when
    /// <see cref="AllowNonLinearHistory"/> is <see langword="true"/>.
    /// </summary>
    IReadOnlyList<IReadOnlyList<IUndoCommand>> RedoBranches { get; }

    /// <summary>
    /// Switches the current redo stack with the branch at the specified index.
    /// The current redo stack (if non-empty) is saved as a new branch, and
    /// the selected branch becomes the active redo stack.
    /// </summary>
    /// <param name="branchIndex">The zero-based index of the branch to restore.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="branchIndex"/> is outside the valid range.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="AllowNonLinearHistory"/> is <see langword="false"/>.
    /// </exception>
    void SwitchRedoBranch(int branchIndex);

    /// <summary>
    /// Gets or sets an optional audit logger that records every
    /// execute, undo, redo, and clear operation with timestamps.
    /// </summary>
    IUndoRedoAuditLogger? AuditLogger { get; set; }

    /// <summary>
    /// Gets the list of snapshots that have been created.
    /// </summary>
    IReadOnlyList<IUndoSnapshot> Snapshots { get; }

    /// <summary>
    /// Creates a named snapshot capturing the current undo stack position.
    /// </summary>
    /// <param name="name">A descriptive name for the snapshot.</param>
    /// <returns>The created snapshot.</returns>
    IUndoSnapshot CreateSnapshot(string name);

    /// <summary>
    /// Restores the undo/redo state to the position captured by the specified snapshot.
    /// Navigates (undo/redo) to reach the snapshot position.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the snapshot's command has been trimmed from history.
    /// </exception>
    void RestoreSnapshot(IUndoSnapshot snapshot);

    /// <summary>
    /// Removes a previously created snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot to remove.</param>
    void RemoveSnapshot(IUndoSnapshot snapshot);

    /// <summary>
    /// Saves the current undo and redo history to a stream in a binary format.
    /// Only commands implementing <see cref="ISerializableUndoCommand"/> are persisted;
    /// non-serializable commands are skipped.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    void SaveHistory(Stream stream);

    /// <summary>
    /// Loads undo and redo history from a stream, replacing the current state.
    /// Commands are reconstructed via the provided <paramref name="deserializer"/>.
    /// Entries that cannot be deserialized (returning <see langword="null"/>) are skipped.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="deserializer">The deserializer used to reconstruct commands.</param>
    void LoadHistory(
        Stream stream,
        IUndoCommandDeserializer deserializer);
}