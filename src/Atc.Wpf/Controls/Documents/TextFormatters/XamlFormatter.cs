namespace Atc.Wpf.Controls.Documents.TextFormatters;

/// <summary>
/// Formats the RichTextBox text as Xaml.
/// </summary>
public class XamlFormatter : ITextFormatter
{
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The text.</returns>
    public string GetText(FlowDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var tr = new TextRange(document.ContentStart, document.ContentEnd);
        using var memoryStream = new MemoryStream();
        tr.Save(memoryStream, DataFormats.Xaml);
        return Encoding.Default.GetString(memoryStream.ToArray());
    }

    /// <summary>
    /// Sets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="text">The text.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    /// <exception cref="InvalidDataException">Data provided is not in the correct Xaml format.</exception>
    public void SetText(FlowDocument document, string text, ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            //// If the text is null/empty clear the contents of the RTB. If you were to pass a null/empty string
            //// to the TextRange.Load method an exception would occur.
            if (string.IsNullOrEmpty(text))
            {
                document.Blocks.Clear();
            }
            else
            {
                var tr = new TextRange(document.ContentStart, document.ContentEnd);
                using var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(text));
                tr.Load(memoryStream, DataFormats.Xaml);
            }
        }
        catch
        {
            throw new InvalidDataException("Data provided is not in the correct Xaml format.");
        }
    }
}