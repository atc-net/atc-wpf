namespace Atc.Wpf.UndoRedo;

/// <summary>
/// A pre-built <see cref="IUndoCommand"/> for property value changes.
/// </summary>
/// <typeparam name="T">The type of the property value.</typeparam>
public sealed class PropertyChangeCommand<T> : IUndoCommand
{
    private readonly Action<T> setter;
    private readonly T oldValue;
    private readonly T newValue;

    public PropertyChangeCommand(
        string description,
        Action<T> setter,
        T oldValue,
        T newValue)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(setter);

        Description = description;
        this.setter = setter;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public void Execute()
        => setter(newValue);

    /// <inheritdoc />
    public void UnExecute()
        => setter(oldValue);
}