namespace Atc.Wpf.Components.Printing;

/// <summary>
/// Replaces tokens in header/footer format strings with actual values.
/// Supported tokens: {PageNumber}, {TotalPages}, {Date}, {DateTime}, {Title}.
/// </summary>
internal static class HeaderFooterFormatter
{
    public static string Format(
        string template,
        int pageNumber,
        int totalPages,
        string title)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        return template
            .Replace("{PageNumber}", pageNumber.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{TotalPages}", totalPages.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{Date}", DateTime.Now.ToString("d", CultureInfo.CurrentCulture), StringComparison.Ordinal)
            .Replace("{DateTime}", DateTime.Now.ToString("g", CultureInfo.CurrentCulture), StringComparison.Ordinal)
            .Replace("{Title}", title, StringComparison.Ordinal);
    }
}