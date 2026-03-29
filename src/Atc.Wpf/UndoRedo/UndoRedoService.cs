namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides undo/redo functionality with dual stacks,
/// command grouping, configurable history limit, and thread safety.
/// </summary>
[SuppressMessage("Usage", "CA1001:Types that own disposable fields should be disposable", Justification = "activeScope is returned to the caller via BeginGroup and disposed by them.")]
public sealed class UndoRedoService : IUndoRedoService
{
    private const int DefaultMaxHistorySize = 100;

    private readonly Lock stackLock = new();
    private readonly LinkedList<IUndoCommand> undoStack = [];
    private readonly LinkedList<IUndoCommand> redoStack = [];

    private UndoCommandGroupScope? activeScope;
    private UndoRedoActionType executingAction;
    private IUndoCommand? executingCommand;
    private IUndoCommand? savedCommand;
    private bool savedAtInitialState = true;
    private bool savedCommandTrimmed;

    public event EventHandler<UndoRedoEventArgs>? ActionPerformed;

    public event EventHandler? StateChanged;

    public IReadOnlyList<IUndoCommand> UndoCommands
    {
        get
        {
            lock (stackLock)
            {
                return undoStack.ToList().AsReadOnly();
            }
        }
    }

    public IReadOnlyList<IUndoCommand> RedoCommands
    {
        get
        {
            lock (stackLock)
            {
                return redoStack.ToList().AsReadOnly();
            }
        }
    }

    public bool CanUndo
    {
        get
        {
            lock (stackLock)
            {
                return undoStack.Count > 0;
            }
        }
    }

    public bool CanRedo
    {
        get
        {
            lock (stackLock)
            {
                return redoStack.Count > 0;
            }
        }
    }

    public bool IsExecuting
    {
        get
        {
            lock (stackLock)
            {
                return executingAction != UndoRedoActionType.None;
            }
        }
    }

    public UndoRedoActionType ExecutingAction
    {
        get
        {
            lock (stackLock)
            {
                return executingAction;
            }
        }
    }

    public IUndoCommand? ExecutingCommand
    {
        get
        {
            lock (stackLock)
            {
                return executingCommand;
            }
        }
    }

    public int MaxHistorySize { get; set; } = DefaultMaxHistorySize;

    public bool HasUnsavedChanges
    {
        get
        {
            lock (stackLock)
            {
                if (savedCommandTrimmed)
                {
                    return true;
                }

                if (savedAtInitialState)
                {
                    return undoStack.Count > 0;
                }

                return undoStack.First is null ||
                       !ReferenceEquals(undoStack.First.Value, savedCommand);
            }
        }
    }

    public void Execute(IUndoCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        bool useScope;
        lock (stackLock)
        {
            if (executingAction != UndoRedoActionType.None)
            {
                return;
            }

            useScope = activeScope is not null;
            executingAction = UndoRedoActionType.Execute;
            executingCommand = command;
        }

        if (useScope)
        {
            ExecuteInScope(command);
            return;
        }

        ExecuteDirectly(command);
    }

    public void Add(IUndoCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        lock (stackLock)
        {
            if (executingAction != UndoRedoActionType.None)
            {
                return;
            }

            if (activeScope is not null)
            {
                activeScope.AddCommand(command);
                return;
            }

            PushUndo(command);
            redoStack.Clear();
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Undo()
    {
        IUndoCommand command;

        lock (stackLock)
        {
            if (undoStack.Count == 0 || executingAction != UndoRedoActionType.None)
            {
                return false;
            }

            command = undoStack.First!.Value;
            undoStack.RemoveFirst();
            executingAction = UndoRedoActionType.Undo;
            executingCommand = command;
        }

        try
        {
            command.UnExecute();
        }
        finally
        {
            lock (stackLock)
            {
                executingAction = UndoRedoActionType.None;
                executingCommand = null;
                redoStack.AddFirst(command);
            }
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Undo, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Undo(int levels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levels);

        var any = false;
        for (var i = 0; i < levels; i++)
        {
            if (!Undo())
            {
                break;
            }

            any = true;
        }

        return any;
    }

    public bool Redo()
    {
        IUndoCommand command;

        lock (stackLock)
        {
            if (redoStack.Count == 0 || executingAction != UndoRedoActionType.None)
            {
                return false;
            }

            command = redoStack.First!.Value;
            redoStack.RemoveFirst();
            executingAction = UndoRedoActionType.Redo;
            executingCommand = command;
        }

        try
        {
            command.Execute();
        }
        finally
        {
            lock (stackLock)
            {
                executingAction = UndoRedoActionType.None;
                executingCommand = null;
                PushUndo(command);
            }
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Redo, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Redo(int levels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levels);

        var any = false;
        for (var i = 0; i < levels; i++)
        {
            if (!Redo())
            {
                break;
            }

            any = true;
        }

        return any;
    }

    public void UndoAll()
    {
        while (Undo())
        {
            // Keep undoing until the stack is empty.
        }
    }

    public void RedoAll()
    {
        while (Redo())
        {
            // Keep redoing until the stack is empty.
        }
    }

    public bool UndoToLastUserAction()
    {
        // First undo any non-user-action commands on top of the stack,
        // then undo exactly one user-action command, then undo any
        // trailing non-user-action commands below it.
        // This treats [trailing-internals, user, leading-internals] as
        // one atomic "user step" so the user lands on the previous
        // user-action boundary.
        var any = false;

        // Phase 1: undo non-user-action commands on top
        while (CanUndo)
        {
            lock (stackLock)
            {
                if (undoStack.Count == 0 || IsUserAction(undoStack.First!.Value))
                {
                    break;
                }
            }

            if (!Undo())
            {
                break;
            }

            any = true;
        }

        // Phase 2: undo one user-action command
        if (CanUndo && Undo())
        {
            any = true;
        }

        // Phase 3: undo trailing non-user-action commands
        while (CanUndo)
        {
            lock (stackLock)
            {
                if (undoStack.Count == 0 || IsUserAction(undoStack.First!.Value))
                {
                    break;
                }
            }

            if (!Undo())
            {
                break;
            }
        }

        return any;
    }

    public bool RedoToLastUserAction()
    {
        // Mirror of UndoToLastUserAction: redo any non-user-action commands
        // on top of the redo stack, then redo exactly one user-action command.
        // Trailing non-user-action commands are left for the next call,
        // where they will be grouped with the following user action.
        // This ensures Undo/Redo round-trips land on the same user-action
        // boundaries.
        var any = false;

        // Phase 1: redo non-user-action commands on top
        while (CanRedo)
        {
            lock (stackLock)
            {
                if (redoStack.Count == 0 || IsUserAction(redoStack.First!.Value))
                {
                    break;
                }
            }

            if (!Redo())
            {
                break;
            }

            any = true;
        }

        // Phase 2: redo one user-action command
        if (CanRedo && Redo())
        {
            any = true;
        }

        return any;
    }

    public void UndoTo(IUndoCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        while (CanUndo)
        {
            IUndoCommand current;
            lock (stackLock)
            {
                if (undoStack.Count == 0)
                {
                    return;
                }

                current = undoStack.First!.Value;
            }

            Undo();

            if (ReferenceEquals(current, command))
            {
                return;
            }
        }
    }

    public void RedoTo(IUndoCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        while (CanRedo)
        {
            IUndoCommand current;
            lock (stackLock)
            {
                if (redoStack.Count == 0)
                {
                    return;
                }

                current = redoStack.First!.Value;
            }

            Redo();

            if (ReferenceEquals(current, command))
            {
                return;
            }
        }
    }

    public IDisposable BeginGroup(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        lock (stackLock)
        {
            if (activeScope is not null)
            {
                throw new InvalidOperationException(
                    "A command group is already active. Nested groups are not supported.");
            }

            activeScope = new UndoCommandGroupScope(
                description,
                CommitGroup,
                () =>
                {
                    lock (stackLock)
                    {
                        activeScope = null;
                    }
                });

            return activeScope;
        }
    }

    public void Clear()
    {
        lock (stackLock)
        {
            undoStack.Clear();
            redoStack.Clear();
            savedCommand = null;
            savedAtInitialState = true;
            savedCommandTrimmed = false;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void MarkSaved()
    {
        lock (stackLock)
        {
            savedCommandTrimmed = false;

            if (undoStack.First is null)
            {
                savedCommand = null;
                savedAtInitialState = true;
            }
            else
            {
                savedCommand = undoStack.First.Value;
                savedAtInitialState = false;
            }
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private static bool IsUserAction(IUndoCommand command)
        => command is not IRichUndoCommand rich || rich.AllowUserAction;

    private void ExecuteInScope(IUndoCommand command)
    {
        try
        {
            command.Execute();
        }
        finally
        {
            lock (stackLock)
            {
                executingAction = UndoRedoActionType.None;
                executingCommand = null;
                activeScope!.AddCommand(command);
            }
        }
    }

    private void ExecuteDirectly(IUndoCommand command)
    {
        try
        {
            command.Execute();
        }
        finally
        {
            lock (stackLock)
            {
                executingAction = UndoRedoActionType.None;
                executingCommand = null;
                PushUndo(command);
                redoStack.Clear();
            }
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PushUndo(IUndoCommand command)
    {
        undoStack.AddFirst(command);
        TrimUndoStack();
    }

    private void TrimUndoStack()
    {
        while (undoStack.Count > MaxHistorySize)
        {
            var removed = undoStack.Last!.Value;
            undoStack.RemoveLast();

            if (!savedAtInitialState && ReferenceEquals(removed, savedCommand))
            {
                savedCommandTrimmed = true;
            }
        }
    }

    private void CommitGroup(UndoCommandGroup group)
    {
        lock (stackLock)
        {
            PushUndo(group);
            redoStack.Clear();
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, group));
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}