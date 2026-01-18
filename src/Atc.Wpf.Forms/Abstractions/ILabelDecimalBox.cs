namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelDecimalBox : ILabelDecimalNumberControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    string PrefixText { get; set; }

    string SuffixText { get; set; }

    decimal Value { get; set; }
}