namespace Atc.Wpf.Forms;

/// <summary>
/// A row of columns in a <see cref="LabelControlsForm"/>.
/// </summary>
public sealed class LabelControlsFormRow : ILabelControlsFormRow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelControlsFormRow"/> class with no columns.
    /// </summary>
    public LabelControlsFormRow()
    {
        Columns ??= new List<ILabelControlsFormColumn>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelControlsFormRow"/> class with one column.
    /// </summary>
    /// <param name="column">The first column of the row.</param>
    public LabelControlsFormRow(ILabelControlsFormColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);

        Columns ??= new List<ILabelControlsFormColumn>();
        Columns.Add(column);
    }

    /// <inheritdoc />
    public ICollection<ILabelControlsFormColumn>? Columns { get; set; }
}