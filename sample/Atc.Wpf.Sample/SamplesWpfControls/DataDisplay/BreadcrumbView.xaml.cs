namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class BreadcrumbView
{
    public BreadcrumbView()
    {
        InitializeComponent();
    }

    private void OnBreadcrumbItemClicked(
        object? sender,
        Atc.Wpf.Controls.DataDisplay.BreadcrumbItemClickedEventArgs e)
    {
        NavigationStatus.Text = $"Navigated to: {e.Item.Content} (index {e.Index})";
    }
}