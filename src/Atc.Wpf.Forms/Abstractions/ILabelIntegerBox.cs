namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelIntegerBox : ILabelIntegerNumberControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    string PrefixText { get; set; }

    string SuffixText { get; set; }

    int Value { get; set; }
}