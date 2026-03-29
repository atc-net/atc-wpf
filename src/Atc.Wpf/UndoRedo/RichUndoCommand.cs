namespace Atc.Wpf.UndoRedo;

/// <summary>
/// A <see cref="IRichUndoCommand"/> that wraps execute and un-execute delegates
/// with additional metadata for history visualization and filtering.
/// </summary>
public sealed class RichUndoCommand : IRichUndoCommand
{
    private readonly Action execute;
    private readonly Action unExecute;

    public RichUndoCommand(
        string description,
        Action execute,
        Action unExecute,
        Guid? id = null,
        object? image = null,
        object? parameter = null,
        object? data = null,
        bool allowUserAction = true)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(unExecute);

        Description = description;
        this.execute = execute;
        this.unExecute = unExecute;
        Id = id ?? Guid.NewGuid();
        Image = image;
        Parameter = parameter;
        Data = data;
        AllowUserAction = allowUserAction;
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public object? Image { get; }

    /// <inheritdoc />
    public object? Parameter { get; }

    /// <inheritdoc />
    public object? Data { get; }

    /// <inheritdoc />
    public bool AllowUserAction { get; }

    /// <inheritdoc />
    public void Execute()
        => execute();

    /// <inheritdoc />
    public void UnExecute()
        => unExecute();
}