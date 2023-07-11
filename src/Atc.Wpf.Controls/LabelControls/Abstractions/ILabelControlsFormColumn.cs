namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsFormColumn
{
    Orientation ControlOrientation { get; set; }

    int ControlWidth { get; set; }

    IList<ILabelControlBase> LabelControls { get; }

    int CalculateHeight();

    Panel GeneratePanel();

    bool IsValid();

    Dictionary<string, object> GetKeyValues();
}