namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled read-only text control.
/// </summary>
public interface ILabelTextInfo : ILabelControlBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the text can be copied to the clipboard from a context menu.
    /// </summary>
    bool EnableCopyToClipboard { get; set; }

    /// <summary>
    /// Gets or sets the displayed text.
    /// </summary>
    string Text { get; set; }
}