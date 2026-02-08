namespace Atc.Wpf.Printing;

/// <summary>
/// Configuration for header and footer text on printed pages.
/// Supports tokens: {PageNumber}, {TotalPages}, {Date}, {DateTime}, {Title}.
/// </summary>
public sealed class HeaderFooterSettings
{
    /// <summary>
    /// Gets or sets the left-aligned header text.
    /// </summary>
    public string HeaderLeft { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the center-aligned header text.
    /// </summary>
    public string HeaderCenter { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the right-aligned header text.
    /// </summary>
    public string HeaderRight { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the left-aligned footer text.
    /// </summary>
    public string FooterLeft { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the center-aligned footer text.
    /// </summary>
    public string FooterCenter { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the right-aligned footer text.
    /// </summary>
    public string FooterRight { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the font size for header and footer text. Default is 10.
    /// </summary>
    public double FontSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the font family for header and footer text.
    /// </summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>
    /// Creates a default <see cref="HeaderFooterSettings"/> with a title in the header
    /// and page numbers in the footer.
    /// </summary>
    /// <param name="title">The document title to display in the header.</param>
    /// <returns>A new <see cref="HeaderFooterSettings"/> instance.</returns>
    public static HeaderFooterSettings CreateDefault(string title)
        => new()
        {
            HeaderLeft = title,
            HeaderRight = "{Date}",
            FooterCenter = "Page {PageNumber} of {TotalPages}",
        };
}