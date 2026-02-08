namespace Atc.Wpf.Printing;

/// <summary>
/// Service interface for simplified printing with preview support.
/// Provides methods for printing visuals and flow documents with optional
/// print preview, page setup, headers/footers, and multi-page support.
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Prints a visual element using the system print dialog.
    /// </summary>
    /// <param name="visual">The visual element to print.</param>
    /// <param name="description">Optional description shown in the print queue.</param>
    /// <returns>A <see cref="PrintResult"/> indicating the outcome.</returns>
    PrintResult Print(
        Visual visual,
        string? description = null);

    /// <summary>
    /// Prints a visual element with the specified settings.
    /// </summary>
    /// <param name="visual">The visual element to print.</param>
    /// <param name="settings">The print settings to apply.</param>
    /// <returns>A <see cref="PrintResult"/> indicating the outcome.</returns>
    PrintResult Print(
        Visual visual,
        PrintSettings settings);

    /// <summary>
    /// Shows a print preview of a visual element and optionally prints it.
    /// </summary>
    /// <param name="visual">The visual element to preview and print.</param>
    /// <param name="settings">Optional print settings. Uses defaults if null.</param>
    /// <returns>A <see cref="PrintResult"/> indicating the outcome.</returns>
    PrintResult PrintWithPreview(
        Visual visual,
        PrintSettings? settings = null);

    /// <summary>
    /// Prints a <see cref="FlowDocument"/> with the specified settings.
    /// </summary>
    /// <param name="document">The flow document to print.</param>
    /// <param name="settings">Optional print settings. Uses defaults if null.</param>
    /// <returns>A <see cref="PrintResult"/> indicating the outcome.</returns>
    PrintResult PrintDocument(
        FlowDocument document,
        PrintSettings? settings = null);

    /// <summary>
    /// Shows a print preview of a <see cref="FlowDocument"/> and optionally prints it.
    /// </summary>
    /// <param name="document">The flow document to preview and print.</param>
    /// <param name="settings">Optional print settings. Uses defaults if null.</param>
    /// <returns>A <see cref="PrintResult"/> indicating the outcome.</returns>
    PrintResult PrintDocumentWithPreview(
        FlowDocument document,
        PrintSettings? settings = null);
}