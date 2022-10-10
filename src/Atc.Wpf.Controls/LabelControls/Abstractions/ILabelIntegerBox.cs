namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelIntegerBox : ILabelTextControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    string PrefixText { get; set; }

    string SuffixText { get; set; }

    int Maximum { get; set; }

    int Minimum { get; set; }

    int Value { get; set; }
}