namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelControl : ILabelControlBase
{
    bool ShowAsteriskOnMandatory { get; set; }

    bool IsMandatory { get; set; }

    SolidColorBrush MandatoryColor { get; set; }

    SolidColorBrush ValidationColor { get; set; }

    string ValidationText { get; set; }

    bool IsValid();
}