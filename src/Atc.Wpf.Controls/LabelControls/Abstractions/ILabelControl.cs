namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControl : ILabelControlBase
{
    bool ShowAsteriskOnMandatory { get; set; }

    bool IsMandatory { get; set; }

    SolidColorBrush MandatoryColor { get; set; }

    string LabelText { get; set; }

    SolidColorBrush ValidationColor { get; set; }
}