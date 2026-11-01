namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// The persisted layout of a <see cref="DataGrid"/>: column order, widths, visibility, sorting and the active
/// column filters. Written by <see cref="DataGridFilter.SaveLayout"/> and read by <see cref="DataGridFilter.LoadLayout"/>.
/// </summary>
public sealed class DataGridLayout
{
    /// <summary>
    /// The layout version written by <see cref="DataGridFilter.GetLayout"/>.
    /// </summary>
    public const int CurrentVersion = 1;

    /// <summary>
    /// Gets or sets the layout format version.
    /// </summary>
    public int Version { get; set; } = CurrentVersion;

    /// <summary>
    /// Gets or sets the name of the culture the grid formatted its values in when the layout was captured (its
    /// <see cref="FrameworkElement.Language"/>). <see cref="DataGridFilter.ApplyLayout"/> translates the texts of
    /// values filters to the grid's culture when it differs; <see langword="null"/> (older layouts) keeps them as
    /// they are.
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets the column states, keyed by column key.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Used for JSON serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Used for JSON serialization")]
    public List<DataGridColumnLayout> Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets the sort order (first entry sorts first).
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Used for JSON serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Used for JSON serialization")]
    public List<DataGridSortLayout> Sorts { get; set; } = [];

    /// <summary>
    /// Gets or sets the active column filters.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Used for JSON serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Used for JSON serialization")]
    public List<DataGridColumnFilter> Filters { get; set; } = [];
}