namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelNumberControl : ILabelControl
{
    string LabelText { get; set; }

    bool HideUpDownButtons { get; set; }
}