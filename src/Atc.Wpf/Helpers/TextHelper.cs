namespace Atc.Wpf.Helpers;

/// <summary>
/// Provides helper methods for text manipulation.
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Normalizes line breaks in text by converting HTML-style breaks to newlines.
    /// Supports: &lt;br/&gt;, &lt;br&gt;, &lt;br /&gt;, Environment.NewLine (platform), \r\n, \r
    /// </summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>Text with normalized line breaks (all converted to \n).</returns>
    public static string NormalizeLineBreaks(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text
            .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace(Environment.NewLine, "\n", StringComparison.Ordinal)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
    }
}