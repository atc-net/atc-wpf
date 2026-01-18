namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelDecimalXyBox : ILabelDecimalNumberControl
{
    string PrefixTextX { get; set; }

    string PrefixTextY { get; set; }

    string SuffixText { get; set; }

    decimal ValueX { get; set; }

    decimal ValueY { get; set; }
}