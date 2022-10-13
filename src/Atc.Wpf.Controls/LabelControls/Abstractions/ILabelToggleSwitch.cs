namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelToggleSwitch : ILabelControlBase
{
    FlowDirection ContentDirection { get; set; }

    string OffText { get; set; }

    string OnText { get; set; }

    bool IsOn { get; set; }
}