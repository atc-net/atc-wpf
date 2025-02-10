namespace Atc.Wpf.Controls.Documents.TextFormatters;

/// <summary>
/// Formats the FlowDocument text as plain text.
/// </summary>
public sealed class PlainTextFormatter : ITextFormatter
{
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The text.</returns>
    public string GetText(
        FlowDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new TextRange(document.ContentStart, document.ContentEnd).Text;
    }

    /// <summary>
    /// Sets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="text">The text.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    public void SetText(
        FlowDocument document,
        string text,
        ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(document);

        new TextRange(document.ContentStart, document.ContentEnd).Text = text;
    }
}