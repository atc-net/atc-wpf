namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Collects commands during a grouped operation and commits them
/// as a single <see cref="UndoCommandGroup"/> when disposed.
/// </summary>
internal sealed class UndoCommandGroupScope : IDisposable
{
    private readonly string description;
    private readonly Action<UndoCommandGroup> commitAction;
    private readonly Action disposeAction;
    private readonly List<IUndoCommand> commands = [];
    private bool disposed;

    public UndoCommandGroupScope(
        string description,
        Action<UndoCommandGroup> commitAction,
        Action disposeAction)
    {
        this.description = description;
        this.commitAction = commitAction;
        this.disposeAction = disposeAction;
    }

    public void AddCommand(IUndoCommand command)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        commands.Add(command);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (commands.Count > 0)
        {
            var group = new UndoCommandGroup(description, commands.ToList().AsReadOnly());
            commitAction(group);
        }

        disposeAction();
    }
}