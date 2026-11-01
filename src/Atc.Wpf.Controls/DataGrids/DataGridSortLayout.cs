namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// One persisted sort description in a <see cref="DataGridLayout"/>.
/// </summary>
public sealed class DataGridSortLayout
{
    /// <summary>
    /// Gets or sets the property the items are sorted by.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public ListSortDirection Direction { get; set; }
}