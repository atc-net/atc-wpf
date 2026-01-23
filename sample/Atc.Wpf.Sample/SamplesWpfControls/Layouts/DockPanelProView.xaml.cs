namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class DockPanelProView
{
    private string? savedLayout;

    public DockPanelProView()
    {
        InitializeComponent();
    }

    private void SaveLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        savedLayout = MainDockPanel.SaveLayout();
        MessageBox.Show(
            "Layout saved successfully!",
            "Layout Saved",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void LoadLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(savedLayout))
        {
            MessageBox.Show(
                "No layout has been saved yet. Please save a layout first.",
                "No Saved Layout",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        MainDockPanel.LoadLayout(savedLayout);
        MessageBox.Show(
            "Layout loaded successfully!",
            "Layout Loaded",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ResetLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainDockPanel.ResetLayout();
        MessageBox.Show(
            "Layout reset to defaults.",
            "Layout Reset",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}