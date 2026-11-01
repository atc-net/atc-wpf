namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Represents an item in a <see cref="DualListSelector"/> control.
/// </summary>
public class DualListSelectorItem
{
    /// <summary>
    /// Gets or sets the internal key (not rendered).
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Gets or sets the primary display text.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary description text (hidden if null).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the sort order number in the Selected list.
    /// </summary>
    public int? SortOrderNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item can be transferred.
    /// Disabled items appear grayed out and cannot be selected or moved.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets an arbitrary object value that can be used to store custom information about this item.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is locked in the Selected list.
    /// A locked item cannot be moved back, dragged back or removed, shows a lock icon and can still be reordered.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets the group this item is shown under. Used when <see cref="DualListSelector.GroupMemberPath"/> is not set:
    /// as soon as an item in a list has a group, that list is grouped by it.
    /// </summary>
    public string? Group { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => Name;
}