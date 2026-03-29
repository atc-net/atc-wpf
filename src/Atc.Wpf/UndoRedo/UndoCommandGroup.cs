namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Groups multiple <see cref="IUndoCommand"/> instances into a single atomic undo unit.
/// Execute runs commands in order; UnExecute runs them in reverse order.
/// </summary>
public sealed class UndoCommandGroup : IUndoCommand
{
    public UndoCommandGroup(
        string description,
        IReadOnlyList<IUndoCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(commands);

        Description = description;
        Commands = commands;
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <summary>
    /// Gets the child commands that make up this group.
    /// </summary>
    public IReadOnlyList<IUndoCommand> Commands { get; }

    /// <inheritdoc />
    public void Execute()
    {
        for (var i = 0; i < Commands.Count; i++)
        {
            Commands[i].Execute();
        }
    }

    /// <inheritdoc />
    public void UnExecute()
    {
        for (var i = Commands.Count - 1; i >= 0; i--)
        {
            Commands[i].UnExecute();
        }
    }
}