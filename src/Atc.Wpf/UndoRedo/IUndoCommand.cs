namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Represents a reversible command that can be executed and un-executed.
/// </summary>
public interface IUndoCommand
{
    /// <summary>
    /// Gets a human-readable description of this command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes (or re-executes) the command.
    /// </summary>
    void Execute();

    /// <summary>
    /// Reverses the effect of <see cref="Execute"/>.
    /// </summary>
    void UnExecute();
}