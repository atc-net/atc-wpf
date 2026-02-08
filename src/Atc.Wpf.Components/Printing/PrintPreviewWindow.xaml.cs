namespace Atc.Wpf.Components.Printing;

/// <summary>
/// A print preview window that hosts a <see cref="DocumentViewer"/>
/// showing a paginated document with Print and Close toolbar buttons.
/// </summary>
public partial class PrintPreviewWindow
{
    public PrintPreviewWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets a value indicating whether the user chose to print from the preview.
    /// </summary>
    public bool UserWantsToPrint { get; private set; }

    /// <summary>
    /// Sets the document to display in the preview.
    /// </summary>
    /// <param name="document">The fixed document to preview.</param>
    public void SetDocument(FixedDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        PreviewViewer.Document = document;
    }

    private void PrintButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        UserWantsToPrint = true;
        DialogResult = true;
    }

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        UserWantsToPrint = false;
        DialogResult = false;
    }
}