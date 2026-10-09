namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled text input control.
/// </summary>
public interface ILabelTextBox : ILabelControl
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
    /// Gets or sets a value indicating whether the default set of not-allowed characters is used.
    /// </summary>
    bool UseDefaultNotAllowedCharacters { get; set; }

    /// <summary>
    /// Gets or sets the characters that are not allowed in the text.
    /// </summary>
    string CharactersNotAllowed { get; set; }

    /// <summary>
    /// Gets or sets the regular expression the text must match to be valid.
    /// </summary>
    string? RegexPattern { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown inside the input.
    /// </summary>
    bool ShowClearTextButton { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    string Text { get; set; }
}