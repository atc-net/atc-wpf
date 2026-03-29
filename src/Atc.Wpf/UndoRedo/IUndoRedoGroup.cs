namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Manages multiple <see cref="IUndoRedoService"/> stacks and delegates
/// undo/redo operations to the currently active stack.
/// Designed for applications with multiple documents or editors that
/// share a unified undo/redo toolbar.
/// </summary>
public interface IUndoRedoGroup
{
    /// <summary>
    /// Gets the registered stacks.
    /// </summary>
    IReadOnlyList<IUndoRedoService> Stacks { get; }

    /// <summary>
    /// Gets or sets the currently active stack.
    /// Undo/Redo operations are delegated to this stack.
    /// Set to <see langword="null"/> to deactivate.
    /// </summary>
    IUndoRedoService? ActiveStack { get; set; }

    /// <summary>
    /// Adds an <see cref="IUndoRedoService"/> to the group with the given name.
    /// </summary>
    /// <param name="stack">The undo/redo service to add.</param>
    /// <param name="name">A display name for the stack.</param>
    void AddStack(
        IUndoRedoService stack,
        string name);

    /// <summary>
    /// Removes an <see cref="IUndoRedoService"/> from the group.
    /// If the removed stack was the active stack, <see cref="ActiveStack"/>
    /// is set to <see langword="null"/>.
    /// </summary>
    /// <param name="stack">The undo/redo service to remove.</param>
    void RemoveStack(IUndoRedoService stack);

    /// <summary>
    /// Gets a value indicating whether the active stack has commands to undo.
    /// Returns <see langword="false"/> when there is no active stack.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Gets a value indicating whether the active stack has commands to redo.
    /// Returns <see langword="false"/> when there is no active stack.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Undoes the most recent command on the active stack.
    /// </summary>
    /// <returns><see langword="true"/> if a command was undone; otherwise <see langword="false"/>.</returns>
    bool Undo();

    /// <summary>
    /// Redoes the most recently undone command on the active stack.
    /// </summary>
    /// <returns><see langword="true"/> if a command was redone; otherwise <see langword="false"/>.</returns>
    bool Redo();

    /// <summary>
    /// Begins a linked group scope. All commands executed on ANY registered
    /// stack during the scope are linked together. When any one of the linked
    /// commands is later undone, all linked commands across all participating
    /// stacks are undone as a single atomic unit. The same applies for redo.
    /// </summary>
    /// <param name="description">A description for the linked operation.</param>
    /// <returns>An <see cref="IDisposable"/> scope; dispose to commit the linked group.</returns>
    IDisposable BeginLinkedGroup(string description);

    /// <summary>
    /// Raised when <see cref="ActiveStack"/> changes.
    /// </summary>
    event EventHandler? ActiveStackChanged;
}