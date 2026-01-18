namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelIntegerXyBox : ILabelIntegerNumberControl
{
    string PrefixTextX { get; set; }

    string PrefixTextY { get; set; }

    string SuffixText { get; set; }

    int ValueX { get; set; }

    int ValueY { get; set; }
}