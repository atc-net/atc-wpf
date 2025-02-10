namespace Atc.Wpf.Controls.Documents.TextFormatters;

/// <summary>
/// Formats the FlowDocument text as colored C#.
/// </summary>
public sealed class CSharpFormatter : ITextFormatter
{
    /// <summary>
    /// The instance.
    /// </summary>
    public static readonly CSharpFormatter Instance = new();

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
        ArgumentNullException.ThrowIfNull(text);

        document.Blocks.Clear();
        document.SetCurrentValue(FlowDocument.PageWidthProperty, 2500D);
        var cSharp = new CSharp();
        var p = cSharp.FormatCode(text, themeMode);
        document.Blocks.Add(p);
    }
}