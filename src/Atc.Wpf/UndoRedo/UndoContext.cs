namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Represents a named scope that can be associated with undo commands.
/// Commands tagged with a context can be filtered during undo/redo operations,
/// enabling scope-based undo (e.g., undo only drawing commands while leaving
/// selection commands intact).
/// </summary>
public sealed class UndoContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UndoContext"/> class.
    /// </summary>
    /// <param name="name">A descriptive name for this context.</param>
    public UndoContext(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
    }

    /// <summary>
    /// Gets the descriptive name of this context.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public override string ToString() => Name;
}