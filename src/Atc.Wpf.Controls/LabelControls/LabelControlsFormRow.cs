namespace Atc.Wpf.Controls.LabelControls;

public sealed class LabelControlsFormRow : ILabelControlsFormRow
{
    public LabelControlsFormRow()
    {
        Columns ??= new List<ILabelControlsFormColumn>();
    }

    public LabelControlsFormRow(ILabelControlsFormColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);

        Columns ??= new List<ILabelControlsFormColumn>();
        Columns.Add(column);
    }

    public ICollection<ILabelControlsFormColumn>? Columns { get; set; }
}