namespace Atc.Wpf.Components.UndoRedo;

/// <summary>
/// Represents a single row in the undo/redo history view.
/// </summary>
public sealed class UndoRedoHistoryItem : INotifyPropertyChanged
{
    private bool isExpanded;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets an optional image for display in the history list.
    /// Populated from <see cref="IRichUndoCommand.Image"/> when the command provides one.
    /// </summary>
    public ImageSource? Image { get; init; }

    /// <summary>
    /// Gets the human-readable description for this history row.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item is in the redo portion of the history.
    /// </summary>
    public bool IsRedo { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item represents the current position.
    /// </summary>
    public bool IsHighlighted { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item represents the save point.
    /// </summary>
    public bool IsSavePoint { get; init; }

    /// <summary>
    /// Gets the UTC timestamp indicating when the command was created,
    /// or <see langword="null"/> for the root "initial state" row.
    /// </summary>
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the underlying undo command, or <see langword="null"/> for the root "initial state" row.
    /// </summary>
    public IUndoCommand? Command { get; init; }

    /// <summary>
    /// Gets the child history items when this item represents a grouped command,
    /// or <see langword="null"/> for non-group items.
    /// </summary>
    public IReadOnlyList<UndoRedoHistoryItem>? Children { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item represents a command group
    /// that can be expanded to show child commands.
    /// </summary>
    public bool IsGroup => Children is not null;

    /// <summary>
    /// Gets or sets a value indicating whether this group item is expanded
    /// to show its child commands. Only meaningful when <see cref="IsGroup"/> is <see langword="true"/>.
    /// </summary>
    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded == value)
            {
                return;
            }

            isExpanded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
        }
    }
}