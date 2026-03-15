namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class OverlayDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Auto Close", "Behavior", 1)]
    [ObservableProperty]
    private bool autoClose = true;

    [PropertyDisplay("Overlay Opacity", "Appearance", 1)]
    [PropertyRange(0, 1, 0.05)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double overlayOpacity = 0.6;
}