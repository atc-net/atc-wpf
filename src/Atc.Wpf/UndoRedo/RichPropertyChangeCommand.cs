namespace Atc.Wpf.UndoRedo;

/// <summary>
/// A pre-built <see cref="IRichUndoCommand"/> for property value changes
/// with additional metadata for history visualization and filtering.
/// </summary>
/// <typeparam name="T">The type of the property value.</typeparam>
public sealed class RichPropertyChangeCommand<T> : IRichUndoCommand
{
    private readonly Action<T> setter;
    private readonly T oldValue;
    private readonly T newValue;

    public RichPropertyChangeCommand(
        string description,
        Action<T> setter,
        T oldValue,
        T newValue,
        Guid? id = null,
        object? image = null,
        object? parameter = null,
        object? data = null,
        bool allowUserAction = true,
        IReadOnlyList<UndoContext>? contexts = null)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(setter);

        Description = description;
        this.setter = setter;
        this.oldValue = oldValue;
        this.newValue = newValue;
        Id = id ?? Guid.NewGuid();
        Image = image;
        Parameter = parameter;
        Data = data;
        AllowUserAction = allowUserAction;
        Contexts = contexts ?? [];
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
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public bool AllowUserAction { get; }

    /// <inheritdoc />
    public IReadOnlyList<UndoContext> Contexts { get; }

    /// <inheritdoc />
    public void Execute()
        => setter(newValue);

    /// <inheritdoc />
    public void UnExecute()
        => setter(oldValue);
}