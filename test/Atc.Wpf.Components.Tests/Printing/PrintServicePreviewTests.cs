namespace Atc.Wpf.Components.Tests.Printing;

/// <summary>
/// The preview is laid out for the default printer before the user picks a printer; the document must be
/// printed laid out for the printer, paper and orientation chosen in the print dialog after the preview.
/// </summary>
public sealed class PrintServicePreviewTests
{
    private static readonly Size DefaultPrinterPage = new(600, 800);
    private static readonly Size ChosenPrinterPage = new(900, 500);

    [StaFact]
    public void PrintWithPreview_PrinterChangedAfterPreview_PrintsWithChosenPrinterPageSize()
    {
        // Arrange
        var printDialog = new FakePrintDialog(ChosenPrinterPage);
        var service = CreateService(printDialog, out var previewDocuments);

        // Act
        var result = service.PrintWithPreview(CreateArrangedVisual());

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        var previewDocument = Assert.Single(previewDocuments);
        Assert.Equal(DefaultPrinterPage, previewDocument.DocumentPaginator.PageSize);
        var printed = Assert.IsType<DocumentPaginator>(printDialog.PrintedPaginator, exactMatch: false);
        Assert.Equal(ChosenPrinterPage, printed.PageSize);
    }

    [StaFact]
    public void PrintDocumentWithPreview_PrinterChangedAfterPreview_PrintsWithChosenPrinterPageSize()
    {
        // Arrange
        var printDialog = new FakePrintDialog(ChosenPrinterPage);
        var service = CreateService(printDialog, out _);
        var document = new FlowDocument(new Paragraph(new Run("Hello")));

        // Act
        var result = service.PrintDocumentWithPreview(document);

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        var printed = Assert.IsType<DocumentPaginator>(printDialog.PrintedPaginator, exactMatch: false);
        Assert.Equal(ChosenPrinterPage, printed.PageSize);
    }

    [StaFact]
    public void PrintWithPreview_PrintDialogCancelled_DoesNotPrint()
    {
        // Arrange
        var printDialog = new FakePrintDialog(ChosenPrinterPage) { DialogResult = false };
        var service = CreateService(printDialog, out _);

        // Act
        var result = service.PrintWithPreview(CreateArrangedVisual());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(printDialog.PrintedPaginator);
    }

    private static PrintService CreateService(
        FakePrintDialog printDialog,
        out List<FixedDocument> previewDocuments)
    {
        var previews = new List<FixedDocument>();
        var dialogs = new Queue<IPrintDialogAdapter>([new FakePrintDialog(DefaultPrinterPage), printDialog]);
        previewDocuments = previews;

        return new PrintService
        {
            DialogFactory = _ => dialogs.Dequeue(),
            PreviewPresenter = document =>
            {
                previews.Add(document);
                return true;
            },
        };
    }

    private static Border CreateArrangedVisual()
    {
        var border = new Border
        {
            Width = 300,
            Height = 300,
            Background = Brushes.White,
            Child = new TextBlock { Text = "Hello" },
        };

        border.Measure(new Size(300, 300));
        border.Arrange(new Rect(0, 0, 300, 300));
        return border;
    }

    private sealed class FakePrintDialog : IPrintDialogAdapter
    {
        public FakePrintDialog(Size pageSize)
            => PageSize = pageSize;

        public Size PageSize { get; }

        public bool? DialogResult { get; init; } = true;

        public DocumentPaginator? PrintedPaginator { get; private set; }

        public bool? ShowDialog()
            => DialogResult;

        public void PrintDocument(
            DocumentPaginator paginator,
            string description)
            => PrintedPaginator = paginator;
    }
}