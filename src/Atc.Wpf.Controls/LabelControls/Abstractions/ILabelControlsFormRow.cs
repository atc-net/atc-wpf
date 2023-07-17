namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsFormRow
{
    ICollection<ILabelControlsFormColumn>? Columns { get; set; }
}