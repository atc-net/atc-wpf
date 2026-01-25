namespace Atc.Wpf.Forms.PropertyEditing;

/// <summary>
/// Configuration options for the PropertyGrid control.
/// </summary>
public sealed class PropertyGridOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to show category groupings.
    /// </summary>
    public bool ShowCategories { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show property descriptions.
    /// </summary>
    public bool ShowDescriptions { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the entire grid is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the sort mode for properties.
    /// </summary>
    public PropertySortMode SortMode { get; set; } = PropertySortMode.Categorized;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically expand nested objects.
    /// </summary>
    public bool AutoExpandNestedObjects { get; set; }

    /// <summary>
    /// Gets or sets the maximum depth for nested object expansion.
    /// A value of 0 means no limit.
    /// </summary>
    public int MaxNestedDepth { get; set; } = 3;

    /// <summary>
    /// Gets or sets a value indicating whether to include properties without setters.
    /// </summary>
    public bool IncludeReadOnlyProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets a filter predicate for properties.
    /// If set, only properties that satisfy this predicate will be shown.
    /// </summary>
    public Func<PropertyInfo, bool>? PropertyFilter { get; set; }

    /// <summary>
    /// Gets or sets the default category name for properties without a CategoryAttribute.
    /// </summary>
    public string DefaultCategoryName { get; set; } = "Misc";
}