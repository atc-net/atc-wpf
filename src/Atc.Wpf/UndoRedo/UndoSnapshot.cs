namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Internal snapshot implementation that captures the current
/// top-of-undo-stack command reference at creation time.
/// </summary>
internal sealed class UndoSnapshot : IUndoSnapshot
{
    public UndoSnapshot(
        string name,
        IUndoCommand? command)
    {
        Name = name;
        Command = command;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Name { get; }

    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the command that was at the top of the undo stack
    /// when this snapshot was created, or <see langword="null"/>
    /// if the undo stack was empty (initial state).
    /// </summary>
    internal IUndoCommand? Command { get; }
}