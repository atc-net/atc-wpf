namespace Atc.Wpf.Components.Printing;

/// <summary>
/// Wraps a <see cref="PrintDialog"/> with the orientation and copy count from <see cref="PrintSettings"/> applied.
/// </summary>
internal sealed class PrintDialogAdapter : IPrintDialogAdapter
{
    private readonly PrintDialog dialog;

    public PrintDialogAdapter(PrintSettings settings)
        : this(new PrintDialog(), settings)
    {
    }

    public PrintDialogAdapter(
        PrintDialog dialog,
        PrintSettings settings)
    {
        this.dialog = dialog;

        dialog.PrintTicket.PageOrientation = settings.Orientation == PrintOrientation.Landscape
            ? global::System.Printing.PageOrientation.Landscape
            : global::System.Printing.PageOrientation.Portrait;

        if (settings.Copies > 1)
        {
            dialog.PrintTicket.CopyCount = settings.Copies;
        }
    }

    /// <summary>
    /// Gets the printable area for the orientation already put on the print ticket.
    /// </summary>
    /// <remarks>
    /// PrintDialog reports PrintableAreaWidth/Height for the ticket's orientation, so the values are used as they are
    /// for both portrait and landscape; swapping them again for landscape paginated landscape pages as portrait.
    /// This follows the PrintDialog semantics and was not verified on a physical print queue.
    /// </remarks>
    public Size PageSize
        => new(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);

    public bool? ShowDialog()
        => dialog.ShowDialog();

    public void PrintDocument(
        DocumentPaginator paginator,
        string description)
        => dialog.PrintDocument(paginator, description);
}