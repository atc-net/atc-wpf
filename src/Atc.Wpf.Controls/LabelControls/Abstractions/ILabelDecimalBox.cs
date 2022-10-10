namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelDecimalBox : ILabelTextControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    string PrefixText { get; set; }

    string SuffixText { get; set; }

    decimal Maximum { get; set; }

    decimal Minimum { get; set; }

    decimal Value { get; set; }
}