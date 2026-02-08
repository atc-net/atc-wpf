namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Groups multiple <see cref="IUndoCommand"/> instances into a single atomic undo unit.
/// Execute runs commands in order; UnExecute runs them in reverse order.
/// </summary>
public sealed class UndoCommandGroup : IUndoCommand
{
    private readonly IReadOnlyList<IUndoCommand> commands;

    public UndoCommandGroup(
        string description,
        IReadOnlyList<IUndoCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(commands);

        Description = description;
        this.commands = commands;
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public void Execute()
    {
        for (var i = 0; i < commands.Count; i++)
        {
            commands[i].Execute();
        }
    }

    /// <inheritdoc />
    public void UnExecute()
    {
        for (var i = commands.Count - 1; i >= 0; i--)
        {
            commands[i].UnExecute();
        }
    }
}