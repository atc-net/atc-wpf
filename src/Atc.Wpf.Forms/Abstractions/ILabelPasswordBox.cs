namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled password input control.
/// </summary>
public interface ILabelPasswordBox : ILabelControl
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
    /// Gets or sets the trimming of the watermark text.
    /// </summary>
    TextTrimming WatermarkTrimming { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed number of characters.
    /// </summary>
    uint MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the minimum required number of characters.
    /// </summary>
    uint MinLength { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown inside the input.
    /// </summary>
    bool ShowClearTextButton { get; set; }

    /// <summary>
    /// Gets or sets the password text.
    /// </summary>
    string Text { get; set; }
}