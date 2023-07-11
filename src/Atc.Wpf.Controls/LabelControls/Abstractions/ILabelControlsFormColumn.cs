namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsFormColumn
{
    Orientation ControlOrientation { get; set; }

    int ControlWidth { get; set; }

    IReadOnlyList<ILabelControlBase> LabelControls { get; }

    int CalculateHeight();

    Panel GeneratePanel();

    bool IsValid();
}