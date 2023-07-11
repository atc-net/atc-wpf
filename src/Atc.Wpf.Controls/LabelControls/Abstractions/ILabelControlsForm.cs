namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsForm
{
    Orientation ControlsOrientation { get; set; }

    int ControlsWidth { get; set; }

    ICollection<ILabelControlsFormColumn>? Columns { get; set; }

    void AddColumn(IList<ILabelControlBase> labelControls);

    void AddColumn(ILabelControlsFormColumn labelControlsFormColumn);

    void Clear();

    int GetMaxHeight();

    int GetMaxWidth();

    Panel GeneratePanel();

    bool IsValid();
}