namespace Atc.Wpf.Forms.PropertyEditing;

/// <summary>
/// Specifies how properties are sorted in the PropertyGrid control.
/// </summary>
public enum PropertySortMode
{
    /// <summary>
    /// Properties are grouped by category and sorted alphabetically within each category.
    /// </summary>
    Categorized,

    /// <summary>
    /// Properties are sorted alphabetically without category grouping.
    /// </summary>
    Alphabetical,

    /// <summary>
    /// Properties are displayed in the order they are declared in the type.
    /// </summary>
    NoSort,
}