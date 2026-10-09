namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled font picker control.
/// </summary>
public interface ILabelFontPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected font family.
    /// </summary>
    FontFamily? SelectedFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the selected font size.
    /// </summary>
    double SelectedFontSize { get; set; }

    /// <summary>
    /// Gets or sets the selected font weight.
    /// </summary>
    FontWeight SelectedFontWeight { get; set; }

    /// <summary>
    /// Gets or sets the selected font style.
    /// </summary>
    FontStyle SelectedFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the selected font stretch.
    /// </summary>
    FontStretch SelectedFontStretch { get; set; }

    /// <summary>
    /// Gets or sets the selected foreground brush.
    /// </summary>
    SolidColorBrush? SelectedForegroundBrush { get; set; }

    /// <summary>
    /// Gets or sets the selected background brush.
    /// </summary>
    SolidColorBrush? SelectedBackgroundBrush { get; set; }

    /// <summary>
    /// Gets or sets the selected text decorations, such as underline or strikethrough.
    /// </summary>
    TextDecorationCollection? SelectedTextDecorations { get; set; }
}