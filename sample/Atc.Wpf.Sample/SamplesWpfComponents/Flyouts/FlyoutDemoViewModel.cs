namespace Atc.Wpf.Sample.SamplesWpfComponents.Flyouts;

public partial class FlyoutDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Show Overlay", "Behavior", 1)]
    [ObservableProperty]
    private bool showOverlay = true;

    [PropertyDisplay("Light Dismiss Enabled", "Behavior", 2)]
    [ObservableProperty]
    private bool isLightDismissEnabled = true;

    [PropertyDisplay("Show Close Button", "Behavior", 3)]
    [ObservableProperty]
    private bool showCloseButton = true;

    [PropertyDisplay("Flyout Width", "Layout", 1)]
    [PropertyRange(200, 600, 10)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double flyoutWidth = 400;
}