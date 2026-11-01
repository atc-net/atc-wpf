namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// The persisted state of one <see cref="DataGrid"/> column in a <see cref="DataGridLayout"/>.
/// </summary>
public sealed class DataGridColumnLayout
{
    /// <summary>
    /// Gets or sets the column key (see <see cref="DataGridFilter.ResolveColumnKey"/>).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display position of the column.
    /// </summary>
    public int DisplayIndex { get; set; }

    /// <summary>
    /// Gets or sets the width value (pixels, or the star factor for a star width).
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the unit of <see cref="Width"/>.
    /// </summary>
    public DataGridLengthUnitType WidthUnit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is shown.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}