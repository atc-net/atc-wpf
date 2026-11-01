namespace Atc.Wpf.Components.Printing;

/// <summary>
/// The part of <see cref="PrintDialog"/> that <see cref="PrintService"/> uses, so the service can be tested without a printer.
/// </summary>
internal interface IPrintDialogAdapter
{
    /// <summary>
    /// Gets the printable area for the selected printer, paper and orientation.
    /// </summary>
    Size PageSize { get; }

    /// <summary>
    /// Shows the print dialog.
    /// </summary>
    /// <returns><see langword="true"/> when the user chose to print.</returns>
    bool? ShowDialog();

    /// <summary>
    /// Prints the paginated document on the selected printer.
    /// </summary>
    /// <param name="paginator">The paginator to print.</param>
    /// <param name="description">The print job description.</param>
    void PrintDocument(
        DocumentPaginator paginator,
        string description);
}