namespace Atc.Wpf.UndoRedo;

/// <summary>
/// A simple <see cref="IUndoCommand"/> that wraps execute and un-execute delegates.
/// </summary>
public sealed class UndoCommand : IUndoCommand
{
    private readonly Action execute;
    private readonly Action unExecute;

    public UndoCommand(
        string description,
        Action execute,
        Action unExecute)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(unExecute);

        Description = description;
        this.execute = execute;
        this.unExecute = unExecute;
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public void Execute()
        => execute();

    /// <inheritdoc />
    public void UnExecute()
        => unExecute();
}