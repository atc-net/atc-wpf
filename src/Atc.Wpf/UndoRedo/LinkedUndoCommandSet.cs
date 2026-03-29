namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Holds a set of linked (stack, command) pairs that were recorded
/// during a <see cref="IUndoRedoGroup.BeginLinkedGroup"/> scope.
/// When one linked command is undone or redone, the set coordinates
/// the same operation on all peer stacks.
/// </summary>
internal sealed class LinkedUndoCommandSet
{
    private readonly List<(IUndoRedoService Stack, IUndoCommand Command)> entries = [];
    private bool isProcessing;

    /// <summary>
    /// Gets the unique identifier for this linked set.
    /// </summary>
    public Guid LinkId { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the description of the linked group.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the entries in this linked set.
    /// </summary>
    public IReadOnlyList<(IUndoRedoService Stack, IUndoCommand Command)> Entries
        => entries;

    public LinkedUndoCommandSet(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        Description = description;
    }

    public void AddEntry(
        IUndoRedoService stack,
        IUndoCommand command)
    {
        entries.Add((stack, command));
    }

    /// <summary>
    /// Returns <see langword="true"/> if the given command is part of this linked set.
    /// </summary>
    public bool Contains(IUndoCommand command)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            if (ReferenceEquals(entries[i].Command, command))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Undoes all peer commands on other stacks when a linked command
    /// is undone on the specified stack.
    /// Returns <see langword="true"/> if peer operations were triggered.
    /// </summary>
    /// <param name="originStack">The stack that initiated the undo.</param>
    public bool UndoPeers(IUndoRedoService originStack)
    {
        if (isProcessing)
        {
            return false;
        }

        isProcessing = true;
        try
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var (stack, command) = entries[i];
                if (ReferenceEquals(stack, originStack))
                {
                    continue;
                }

                stack.UndoTo(command);
            }

            return true;
        }
        finally
        {
            isProcessing = false;
        }
    }

    /// <summary>
    /// Redoes all peer commands on other stacks when a linked command
    /// is redone on the specified stack.
    /// Returns <see langword="true"/> if peer operations were triggered.
    /// </summary>
    /// <param name="originStack">The stack that initiated the redo.</param>
    public bool RedoPeers(IUndoRedoService originStack)
    {
        if (isProcessing)
        {
            return false;
        }

        isProcessing = true;
        try
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var (stack, command) = entries[i];
                if (ReferenceEquals(stack, originStack))
                {
                    continue;
                }

                stack.RedoTo(command);
            }

            return true;
        }
        finally
        {
            isProcessing = false;
        }
    }
}