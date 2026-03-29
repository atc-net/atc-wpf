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
    private readonly List<IUndoOperationApprover> approvers = [];
    private readonly List<UndoSnapshot> snapshots = [];
    private readonly List<LinkedList<IUndoCommand>> redoBranches = [];

    private bool allowNonLinearHistory;
    private IReadOnlyList<IReadOnlyList<IUndoCommand>>? cachedRedoBranches;

    private IReadOnlyList<IUndoCommand>? cachedUndoCommands;
    private IReadOnlyList<IUndoCommand>? cachedRedoCommands;

    private UndoCommandGroupScope? activeScope;
    private UndoRedoActionType executingAction;
    private IUndoCommand? executingCommand;
    private IUndoCommand? savedCommand;
    private int suspendCount;
    private long maxHistoryMemory;
    private long currentHistoryMemory;
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
                return cachedUndoCommands ??= undoStack.ToArray();
            }
        }
    }

    public IReadOnlyList<IUndoCommand> RedoCommands
    {
        get
        {
            lock (stackLock)
            {
                return cachedRedoCommands ??= redoStack.ToArray();
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

    public bool IsRecordingSuspended => Volatile.Read(ref suspendCount) > 0;

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

    public IUndoRedoAuditLogger? AuditLogger { get; set; }

    public bool AllowNonLinearHistory
    {
        get
        {
            lock (stackLock)
            {
                return allowNonLinearHistory;
            }
        }

        set
        {
            lock (stackLock)
            {
                allowNonLinearHistory = value;
            }
        }
    }

    public IReadOnlyList<IReadOnlyList<IUndoCommand>> RedoBranches
    {
        get
        {
            lock (stackLock)
            {
                return cachedRedoBranches ??= redoBranches
                    .Select(branch => (IReadOnlyList<IUndoCommand>)branch.ToArray())
                    .ToArray();
            }
        }
    }

    public void SwitchRedoBranch(int branchIndex)
    {
        lock (stackLock)
        {
            if (!allowNonLinearHistory)
            {
                throw new InvalidOperationException(
                    "Non-linear history is not enabled. Set AllowNonLinearHistory to true.");
            }

            ArgumentOutOfRangeException.ThrowIfNegative(branchIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(branchIndex, redoBranches.Count);

            var selectedBranch = redoBranches[branchIndex];
            redoBranches.RemoveAt(branchIndex);

            if (redoStack.Count > 0)
            {
                var currentBranch = new LinkedList<IUndoCommand>(redoStack);
                redoBranches.Add(currentBranch);
            }

            redoStack.Clear();
            foreach (var command in selectedBranch)
            {
                redoStack.AddLast(command);
            }

            cachedRedoCommands = null;
            cachedRedoBranches = null;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public long MaxHistoryMemory
    {
        get => Interlocked.Read(ref maxHistoryMemory);
        set => Interlocked.Exchange(ref maxHistoryMemory, value);
    }

    public long CurrentHistoryMemory
    {
        get
        {
            lock (stackLock)
            {
                return currentHistoryMemory;
            }
        }
    }

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
        bool suspended;
        lock (stackLock)
        {
            if (executingAction != UndoRedoActionType.None)
            {
                return;
            }

            suspended = suspendCount > 0;
            useScope = !suspended && activeScope is not null;
            executingAction = UndoRedoActionType.Execute;
            executingCommand = command;
        }

        if (suspended)
        {
            ExecuteSuspended(command);
            return;
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

            if (suspendCount > 0)
            {
                return;
            }

            if (command.IsObsolete)
            {
                return;
            }

            if (activeScope is not null)
            {
                activeScope.AddCommand(command);
                return;
            }

            if (!TryMergeIntoTop(command))
            {
                PushUndo(command);
            }

            SaveRedoBranchIfNonLinear();
            redoStack.Clear();
            cachedRedoCommands = null;
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
        LogAudit(UndoRedoActionType.Execute, command.Description);
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

            if (!IsApprovedForUndo(command))
            {
                return false;
            }

            undoStack.RemoveFirst();
            currentHistoryMemory -= GetCommandMemory(command);
            cachedUndoCommands = null;
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
                cachedRedoCommands = null;
            }
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Undo, command));
        StateChanged?.Invoke(this, EventArgs.Empty);
        LogAudit(UndoRedoActionType.Undo, command.Description);
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

    public bool Undo(UndoContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IUndoCommand? target;
        lock (stackLock)
        {
            target = FindFirstMatchingContext(undoStack, context);
        }

        if (target is null)
        {
            return false;
        }

        UndoTo(target);
        return true;
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

            if (!IsApprovedForRedo(command))
            {
                return false;
            }

            redoStack.RemoveFirst();
            cachedRedoCommands = null;
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
        LogAudit(UndoRedoActionType.Redo, command.Description);
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

    public bool Redo(UndoContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IUndoCommand? target;
        lock (stackLock)
        {
            target = FindFirstMatchingContext(redoStack, context);
        }

        if (target is null)
        {
            return false;
        }

        RedoTo(target);
        return true;
    }

    public IReadOnlyList<IUndoCommand> FindUndoCommands(UndoContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (stackLock)
        {
            return FilterByContext(undoStack, context);
        }
    }

    public IReadOnlyList<IUndoCommand> FindRedoCommands(UndoContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (stackLock)
        {
            return FilterByContext(redoStack, context);
        }
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

    public IDisposable SuspendRecording()
    {
        Interlocked.Increment(ref suspendCount);
        return new SuspendRecordingScope(this);
    }

    public void RegisterApprover(IUndoOperationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(approver);

        lock (stackLock)
        {
            approvers.Add(approver);
        }
    }

    public void UnregisterApprover(IUndoOperationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(approver);

        lock (stackLock)
        {
            approvers.Remove(approver);
        }
    }

    public void Clear()
    {
        lock (stackLock)
        {
            undoStack.Clear();
            redoStack.Clear();
            redoBranches.Clear();
            snapshots.Clear();
            cachedUndoCommands = null;
            cachedRedoCommands = null;
            cachedRedoBranches = null;
            currentHistoryMemory = 0;
            savedCommand = null;
            savedAtInitialState = true;
            savedCommandTrimmed = false;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
        LogAudit(UndoRedoActionType.None, "Clear");
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

    public IReadOnlyList<IUndoSnapshot> Snapshots
    {
        get
        {
            lock (stackLock)
            {
                return snapshots.ToArray();
            }
        }
    }

    public IUndoSnapshot CreateSnapshot(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        lock (stackLock)
        {
            var command = undoStack.First?.Value;
            var snapshot = new UndoSnapshot(name, command);
            snapshots.Add(snapshot);
            return snapshot;
        }
    }

    public void RestoreSnapshot(IUndoSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot is not UndoSnapshot typed)
        {
            throw new InvalidOperationException(
                "Snapshot was not created by this service.");
        }

        if (typed.Command is null)
        {
            UndoAll();
            return;
        }

        // Check if command is in the undo stack.
        bool foundInUndo;
        bool foundInRedo;
        lock (stackLock)
        {
            foundInUndo = ContainsCommand(undoStack, typed.Command);
            foundInRedo = ContainsCommand(redoStack, typed.Command);
        }

        if (foundInUndo)
        {
            // The snapshot command is in the undo stack; undo to it, then redo once
            // so we land *after* the command (the position when the snapshot was taken).
            UndoTo(typed.Command);
            Redo();
            return;
        }

        if (foundInRedo)
        {
            RedoTo(typed.Command);
            return;
        }

        throw new InvalidOperationException(
            "Snapshot command no longer in history.");
    }

    public void RemoveSnapshot(IUndoSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot is not UndoSnapshot typed)
        {
            return;
        }

        lock (stackLock)
        {
            snapshots.Remove(typed);
        }
    }

    public void SaveHistory(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        lock (stackLock)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

            // Version
            writer.Write(1);

            // Undo stack (oldest to newest = last to first in LinkedList)
            WriteCommandStack(writer, undoStack, reverseOrder: true);

            // Redo stack (oldest to newest = last to first in LinkedList)
            WriteCommandStack(writer, redoStack, reverseOrder: true);
        }
    }

    public void LoadHistory(
        Stream stream,
        IUndoCommandDeserializer deserializer)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(deserializer);

        lock (stackLock)
        {
            // Clear existing state
            undoStack.Clear();
            redoStack.Clear();
            redoBranches.Clear();
            snapshots.Clear();
            cachedUndoCommands = null;
            cachedRedoCommands = null;
            cachedRedoBranches = null;
            currentHistoryMemory = 0;
            savedCommand = null;
            savedAtInitialState = true;
            savedCommandTrimmed = false;

            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

            // Version
            var version = reader.ReadInt32();
            if (version != 1)
            {
                throw new InvalidOperationException(
                    $"Unsupported history format version: {version}.");
            }

            // Undo stack (written oldest to newest, so append in order = AddLast,
            // then they appear newest-first when read via First)
            ReadCommandStack(reader, deserializer, undoStack);

            // Redo stack
            ReadCommandStack(reader, deserializer, redoStack);

            // Recompute memory tracking for undo stack
            for (var node = undoStack.First; node is not null; node = node.Next)
            {
                currentHistoryMemory += GetCommandMemory(node.Value);
            }
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void WriteCommandStack(
        BinaryWriter writer,
        LinkedList<IUndoCommand> stack,
        bool reverseOrder)
    {
        // Count serializable commands
        var count = 0;
        for (var node = stack.First; node is not null; node = node.Next)
        {
            if (node.Value is ISerializableUndoCommand)
            {
                count++;
            }
        }

        writer.Write(count);

        if (reverseOrder)
        {
            for (var node = stack.Last; node is not null; node = node.Previous)
            {
                WriteCommandEntry(writer, node.Value);
            }
        }
        else
        {
            for (var node = stack.First; node is not null; node = node.Next)
            {
                WriteCommandEntry(writer, node.Value);
            }
        }
    }

    private static void WriteCommandEntry(
        BinaryWriter writer,
        IUndoCommand command)
    {
        if (command is not ISerializableUndoCommand serializable)
        {
            return;
        }

        var typeId = serializable.TypeId;
        var data = serializable.Serialize();

        writer.Write(typeId);
        writer.Write(data.Length);
        writer.Write(data);
    }

    private static void ReadCommandStack(
        BinaryReader reader,
        IUndoCommandDeserializer deserializer,
        LinkedList<IUndoCommand> stack)
    {
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var typeId = reader.ReadString();
            var dataLength = reader.ReadInt32();
            var data = reader.ReadBytes(dataLength);

            var command = deserializer.Deserialize(typeId, data);
            if (command is not null)
            {
                // Commands are written oldest-to-newest, so AddFirst
                // builds newest-first order (matching LinkedList convention).
                stack.AddFirst(command);
            }
        }
    }

    private static bool HasContext(
        IUndoCommand command,
        UndoContext context)
        => command is IRichUndoCommand rich && rich.Contexts.Contains(context);

    private static IUndoCommand? FindFirstMatchingContext(
        LinkedList<IUndoCommand> stack,
        UndoContext context)
    {
        for (var node = stack.First; node is not null; node = node.Next)
        {
            if (HasContext(node.Value, context))
            {
                return node.Value;
            }
        }

        return null;
    }

    private static IReadOnlyList<IUndoCommand> FilterByContext(
        LinkedList<IUndoCommand> stack,
        UndoContext context)
    {
        var result = new List<IUndoCommand>();
        for (var node = stack.First; node is not null; node = node.Next)
        {
            if (HasContext(node.Value, context))
            {
                result.Add(node.Value);
            }
        }

        return result;
    }

    private static bool ContainsCommand(
        LinkedList<IUndoCommand> stack,
        IUndoCommand command)
    {
        for (var node = stack.First; node is not null; node = node.Next)
        {
            if (ReferenceEquals(node.Value, command))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsUserAction(IUndoCommand command)
        => command is not IRichUndoCommand rich || rich.AllowUserAction;

    /// <summary>
    /// Returns <see langword="true"/> if all registered approvers allow the undo.
    /// Caller must hold <see cref="stackLock"/>.
    /// </summary>
    private bool IsApprovedForUndo(IUndoCommand command)
    {
        for (var i = 0; i < approvers.Count; i++)
        {
            if (!approvers[i].ApproveUndo(command))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <see langword="true"/> if all registered approvers allow the redo.
    /// Caller must hold <see cref="stackLock"/>.
    /// </summary>
    private bool IsApprovedForRedo(IUndoCommand command)
    {
        for (var i = 0; i < approvers.Count; i++)
        {
            if (!approvers[i].ApproveRedo(command))
            {
                return false;
            }
        }

        return true;
    }

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

                if (!command.IsObsolete)
                {
                    activeScope!.AddCommand(command);
                }
            }
        }
    }

    private void ExecuteSuspended(IUndoCommand command)
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

                if (!command.IsObsolete)
                {
                    if (!TryMergeIntoTop(command))
                    {
                        PushUndo(command);
                    }

                    SaveRedoBranchIfNonLinear();
                    redoStack.Clear();
                    cachedRedoCommands = null;
                }
            }
        }

        if (!command.IsObsolete)
        {
            ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, command));
            StateChanged?.Invoke(this, EventArgs.Empty);
            LogAudit(UndoRedoActionType.Execute, command.Description);
        }
    }

    /// <summary>
    /// If non-linear history is enabled and the redo stack is non-empty,
    /// saves the current redo stack as a branch before it is cleared.
    /// Caller must hold <see cref="stackLock"/>.
    /// </summary>
    private void SaveRedoBranchIfNonLinear()
    {
        if (!allowNonLinearHistory || redoStack.Count == 0)
        {
            return;
        }

        var branch = new LinkedList<IUndoCommand>(redoStack);
        redoBranches.Add(branch);
        cachedRedoBranches = null;
    }

    /// <summary>
    /// Attempts to merge the incoming command into the top of the undo stack.
    /// Both the incoming and the top command must implement <see cref="IMergeableUndoCommand"/>
    /// with the same <see cref="IMergeableUndoCommand.MergeId"/>.
    /// Caller must hold <see cref="stackLock"/>.
    /// </summary>
    private bool TryMergeIntoTop(IUndoCommand command)
    {
        if (command is not IMergeableUndoCommand incoming ||
            undoStack.First?.Value is not IMergeableUndoCommand existing ||
            existing.MergeId != incoming.MergeId)
        {
            return false;
        }

        if (!existing.TryMergeWith(command))
        {
            return false;
        }

        // The existing command absorbed the new one; invalidate the cached snapshot
        // so consumers see the updated description.
        cachedUndoCommands = null;
        return true;
    }

    private void PushUndo(IUndoCommand command)
    {
        undoStack.AddFirst(command);
        currentHistoryMemory += GetCommandMemory(command);
        cachedUndoCommands = null;
        TrimUndoStack();
    }

    private void TrimUndoStack()
    {
        // Count-based trim.
        while (undoStack.Count > MaxHistorySize)
        {
            RemoveOldestUndoCommand();
        }

        // Memory-based trim.
        var memoryBudget = Interlocked.Read(ref maxHistoryMemory);
        if (memoryBudget > 0)
        {
            while (currentHistoryMemory > memoryBudget && undoStack.Count > 0)
            {
                RemoveOldestUndoCommand();
            }
        }
    }

    /// <summary>
    /// Removes the oldest (last) command from the undo stack, updating
    /// memory tracking, saved-state bookkeeping, and snapshot references.
    /// Caller must hold <see cref="stackLock"/>.
    /// </summary>
    private void RemoveOldestUndoCommand()
    {
        var removed = undoStack.Last!.Value;
        undoStack.RemoveLast();
        currentHistoryMemory -= GetCommandMemory(removed);

        if (!savedAtInitialState && ReferenceEquals(removed, savedCommand))
        {
            savedCommandTrimmed = true;
        }

        // Remove any snapshots that reference the trimmed command.
        for (var i = snapshots.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(snapshots[i].Command, removed))
            {
                snapshots.RemoveAt(i);
            }
        }
    }

    private static long GetCommandMemory(IUndoCommand command)
        => command is IMemoryAwareUndoCommand memoryAware
            ? memoryAware.EstimatedMemoryBytes
            : 0;

    private void CommitGroup(UndoCommandGroup group)
    {
        lock (stackLock)
        {
            PushUndo(group);
            SaveRedoBranchIfNonLinear();
            redoStack.Clear();
            cachedRedoCommands = null;
        }

        ActionPerformed?.Invoke(this, new UndoRedoEventArgs(UndoRedoActionType.Execute, group));
        StateChanged?.Invoke(this, EventArgs.Empty);
        LogAudit(UndoRedoActionType.Execute, group.Description);
    }

    private void LogAudit(
        UndoRedoActionType actionType,
        string description)
    {
        AuditLogger?.Log(
            new UndoRedoAuditEntry(
                actionType,
                description,
                DateTimeOffset.UtcNow));
    }

    private sealed class SuspendRecordingScope : IDisposable
    {
        private readonly UndoRedoService service;
        private bool disposed;

        public SuspendRecordingScope(UndoRedoService service)
        {
            this.service = service;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Interlocked.Decrement(ref service.suspendCount);
            }
        }
    }
}