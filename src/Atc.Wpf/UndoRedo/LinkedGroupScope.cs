namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Collects commands executed on any registered stack during the scope's lifetime.
/// On dispose, creates a <see cref="LinkedUndoCommandSet"/> that coordinates
/// undo/redo across all participating stacks.
/// </summary>
internal sealed class LinkedGroupScope : IDisposable
{
    private readonly string description;
    private readonly IReadOnlyList<IUndoRedoService> stacks;
    private readonly Action<LinkedUndoCommandSet?> commitAction;
    private readonly List<(IUndoRedoService Stack, IUndoCommand Command)> collected = [];
    private bool disposed;

    public LinkedGroupScope(
        string description,
        IReadOnlyList<IUndoRedoService> stacks,
        Action<LinkedUndoCommandSet?> commitAction)
    {
        this.description = description;
        this.stacks = stacks;
        this.commitAction = commitAction;

        for (var i = 0; i < stacks.Count; i++)
        {
            ActionPerformedEventManager.AddHandler(stacks[i], OnActionPerformed);
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        for (var i = 0; i < stacks.Count; i++)
        {
            ActionPerformedEventManager.RemoveHandler(stacks[i], OnActionPerformed);
        }

        if (collected.Count < 2)
        {
            // A linked group with fewer than 2 entries has no peers
            // to coordinate with, so there is nothing to link.
            commitAction(null);
            return;
        }

        var set = new LinkedUndoCommandSet(description);

        for (var i = 0; i < collected.Count; i++)
        {
            var (stack, command) = collected[i];
            set.AddEntry(stack, command);
        }

        commitAction(set);
    }

    private void OnActionPerformed(
        object? sender,
        UndoRedoEventArgs e)
    {
        if (disposed)
        {
            return;
        }

        if (e.ActionType != UndoRedoActionType.Execute)
        {
            return;
        }

        if (sender is not IUndoRedoService stack)
        {
            return;
        }

        collected.Add((stack, e.Command));
    }
}