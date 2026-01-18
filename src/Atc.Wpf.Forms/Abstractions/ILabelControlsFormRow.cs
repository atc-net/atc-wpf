namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelControlsFormRow
{
    ICollection<ILabelControlsFormColumn>? Columns { get; set; }
}