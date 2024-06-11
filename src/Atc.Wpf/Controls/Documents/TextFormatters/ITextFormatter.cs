namespace Atc.Wpf.Controls.Documents.TextFormatters;

/// <summary>
/// Interface TextFormatter.
/// </summary>
public interface ITextFormatter
{
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The text.</returns>
    string GetText(FlowDocument document);

    /// <summary>
    /// Sets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="text">The text.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    void SetText(FlowDocument document, string text, ThemeMode themeMode);
}