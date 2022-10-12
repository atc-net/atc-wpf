namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlBase
{
    Orientation Orientation { get; set; }

    string InformationText { get; set; }

    Color InformationColor { get; set; }

    bool HideValidationTextArea { get; set; }
}