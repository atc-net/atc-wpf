namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Displays the network scanner results as a sortable list of discovered network hosts,
/// with a busy overlay showing scan progress.
/// </summary>
public partial class NetworkScannerView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkScannerView"/> class.
    /// </summary>
    public NetworkScannerView()
        => InitializeComponent();

    // One handler for every column header: a binding per header would be resolved before the
    // headers join the view's tree, and log a data binding error for each of them.
    private void OnColumnHeaderClick(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is NetworkScannerViewModel viewModel &&
            viewModel.SortCommand.CanExecute(e))
        {
            viewModel.SortCommand.Execute(e);
        }
    }
}