namespace Atc.Wpf.Printing;

/// <summary>
/// Configuration settings for a print operation.
/// </summary>
public sealed class PrintSettings
{
    /// <summary>
    /// Gets or sets the document title shown in the print queue.
    /// </summary>
    public string Title { get; set; } = "Print Document";

    /// <summary>
    /// Gets or sets the page orientation.
    /// </summary>
    public PrintOrientation Orientation { get; set; } = PrintOrientation.Portrait;

    /// <summary>
    /// Gets or sets the page margins in device-independent units (1/96 inch).
    /// Default is 48 units (0.5 inch) on all sides.
    /// </summary>
    public Thickness Margins { get; set; } = new(48);

    /// <summary>
    /// Gets or sets the content scaling mode.
    /// </summary>
    public PrintScaleMode ScaleMode { get; set; } = PrintScaleMode.ShrinkToFit;

    /// <summary>
    /// Gets or sets optional header and footer configuration.
    /// When null, no headers or footers are printed.
    /// </summary>
    public HeaderFooterSettings? HeaderFooter { get; set; }

    /// <summary>
    /// Gets or sets the number of copies to print.
    /// </summary>
    public int Copies { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether to show the system print dialog
    /// before printing. Default is true.
    /// </summary>
    public bool ShowPrintDialog { get; set; } = true;
}