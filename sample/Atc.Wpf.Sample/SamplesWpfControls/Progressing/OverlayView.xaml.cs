namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class OverlayView
{
    public OverlayView()
    {
        InitializeComponent();
    }

    private void OnToggleBasicClick(
        object sender,
        RoutedEventArgs e)
    {
        BasicOverlay.IsActive = !BasicOverlay.IsActive;
        BtnToggleBasic.Content = BasicOverlay.IsActive ? "Hide Overlay" : "Show Overlay";
    }

    private void OnToggleCustomClick(
        object sender,
        RoutedEventArgs e)
    {
        CustomOverlay.IsActive = !CustomOverlay.IsActive;
        BtnToggleCustom.Content = CustomOverlay.IsActive ? "Hide Overlay" : "Show Overlay";
    }

    private void OnDismissCustomClick(
        object sender,
        RoutedEventArgs e)
    {
        CustomOverlay.IsActive = false;
        BtnToggleCustom.Content = "Show Overlay";
    }
}