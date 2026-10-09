namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled integer input control.
/// </summary>
public interface ILabelIntegerBox : ILabelIntegerNumberControl
{
    /// <summary>
    /// Gets or sets the watermark text shown when the input is empty.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Gets or sets the alignment of the watermark text.
    /// </summary>
    TextAlignment WatermarkAlignment { get; set; }

    /// <summary>
    /// Gets or sets the text shown before the value.
    /// </summary>
    string PrefixText { get; set; }

    /// <summary>
    /// Gets or sets the text shown after the value.
    /// </summary>
    string SuffixText { get; set; }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    int Value { get; set; }
}