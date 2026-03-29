namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Manages multiple <see cref="IUndoRedoService"/> stacks and delegates
/// undo/redo operations to the currently active stack.
/// Implements <see cref="INotifyPropertyChanged"/> so that
/// <see cref="CanUndo"/> and <see cref="CanRedo"/> are observable in MVVM bindings.
/// </summary>
public sealed class UndoRedoGroup : IUndoRedoGroup, INotifyPropertyChanged
{
    private readonly List<(IUndoRedoService Stack, string Name)> stacks = [];
    private readonly List<LinkedUndoCommandSet> linkedSets = [];
    private IReadOnlyList<IUndoRedoService>? cachedStacks;
    private IUndoRedoService? activeStack;
    private bool isProcessingLinkedAction;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? ActiveStackChanged;

    public IReadOnlyList<IUndoRedoService> Stacks
    {
        get => cachedStacks ??= stacks.Select(s => s.Stack).ToArray();
    }

    public IUndoRedoService? ActiveStack
    {
        get => activeStack;
        set
        {
            if (ReferenceEquals(activeStack, value))
            {
                return;
            }

            if (value is not null && !stacks.Exists(s => ReferenceEquals(s.Stack, value)))
            {
                throw new ArgumentException(
                    "The specified stack is not registered in this group.",
                    nameof(value));
            }

            if (activeStack is not null)
            {
                activeStack.StateChanged -= OnActiveStackStateChanged;
            }

            activeStack = value;

            if (activeStack is not null)
            {
                activeStack.StateChanged += OnActiveStackStateChanged;
            }

            ActiveStackChanged?.Invoke(this, EventArgs.Empty);
            RaiseCanUndoRedoChanged();
        }
    }

    public bool CanUndo => activeStack?.CanUndo ?? false;

    public bool CanRedo => activeStack?.CanRedo ?? false;

    public void AddStack(
        IUndoRedoService stack,
        string name)
    {
        ArgumentNullException.ThrowIfNull(stack);
        ArgumentNullException.ThrowIfNull(name);

        stacks.Add((stack, name));
        cachedStacks = null;

        stack.ActionPerformed += OnAnyStackActionPerformed;
    }

    public void RemoveStack(IUndoRedoService stack)
    {
        ArgumentNullException.ThrowIfNull(stack);

        var index = stacks.FindIndex(s => ReferenceEquals(s.Stack, stack));
        if (index < 0)
        {
            return;
        }

        stack.ActionPerformed -= OnAnyStackActionPerformed;
        stacks.RemoveAt(index);
        cachedStacks = null;

        if (ReferenceEquals(activeStack, stack))
        {
            ActiveStack = null;
        }
    }

    public bool Undo()
        => activeStack?.Undo() ?? false;

    public bool Redo()
        => activeStack?.Redo() ?? false;

    public IDisposable BeginLinkedGroup(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        var currentStacks = stacks.Select(s => s.Stack).ToArray();
        return new LinkedGroupScope(
            description,
            currentStacks,
            set =>
            {
                if (set is not null)
                {
                    linkedSets.Add(set);
                }
            });
    }

    private void OnActiveStackStateChanged(
        object? sender,
        EventArgs e)
        => RaiseCanUndoRedoChanged();

    private void OnAnyStackActionPerformed(
        object? sender,
        UndoRedoEventArgs e)
    {
        if (isProcessingLinkedAction)
        {
            return;
        }

        if (e.ActionType is not (UndoRedoActionType.Undo or UndoRedoActionType.Redo))
        {
            return;
        }

        if (sender is not IUndoRedoService originStack)
        {
            return;
        }

        var set = FindLinkedSet(e.Command);
        if (set is null)
        {
            return;
        }

        isProcessingLinkedAction = true;
        try
        {
            if (e.ActionType == UndoRedoActionType.Undo)
            {
                set.UndoPeers(originStack);
            }
            else
            {
                set.RedoPeers(originStack);
            }
        }
        finally
        {
            isProcessingLinkedAction = false;
        }
    }

    private LinkedUndoCommandSet? FindLinkedSet(IUndoCommand command)
    {
        for (var i = 0; i < linkedSets.Count; i++)
        {
            if (linkedSets[i].Contains(command))
            {
                return linkedSets[i];
            }
        }

        return null;
    }

    private void RaiseCanUndoRedoChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
    }
}