namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class SplitButtonView
{
    public SplitButtonView()
    {
        InitializeComponent();
    }

    private void OnSaveClick(
        object sender,
        RoutedEventArgs e)
    {
        StatusText.Text = "Save clicked.";
    }

    private void OnSaveAsClick(
        object sender,
        RoutedEventArgs e)
    {
        StatusText.Text = "Save As clicked.";
        SaveSplitButton.IsDropdownOpen = false;
    }

    private void OnExportClick(
        object sender,
        RoutedEventArgs e)
    {
        StatusText.Text = "Export clicked.";
        SaveSplitButton.IsDropdownOpen = false;
    }

    private void OnSaveAllClick(
        object sender,
        RoutedEventArgs e)
    {
        StatusText.Text = "Save All clicked.";
        SaveSplitButton.IsDropdownOpen = false;
    }
}