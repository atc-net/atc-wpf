namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Extends <see cref="IUndoCommand"/> with rich metadata for designer-style applications
/// that need command identity, icons, arbitrary data, and user-action filtering.
/// </summary>
public interface IRichUndoCommand : IUndoCommand
{
    /// <summary>
    /// Gets a unique identifier for this command instance.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets an optional image reference for display in history panels.
    /// The value is presentation-agnostic (e.g., <c>ImageSource</c>, URI string, enum).
    /// </summary>
    object? Image { get; }

    /// <summary>
    /// Gets an optional command parameter for diagnostics or tracing.
    /// </summary>
    object? Parameter { get; }

    /// <summary>
    /// Gets an optional arbitrary payload attached to this command.
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets a value indicating whether this command represents a user-initiated action.
    /// When <see langword="false"/>, the command is considered programmatic/internal
    /// and may be skipped by user-action navigation methods.
    /// </summary>
    bool AllowUserAction { get; }
}