namespace Atc.Wpf.Sample.SamplesWpfControls.ColorEditing;

public partial class AdvancedColorPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Saturation/Brightness", "Panels", 1)]
    [ObservableProperty]
    private bool showSaturationBrightnessPicker = true;

    [PropertyDisplay("Available Colors", "Panels", 2)]
    [ObservableProperty]
    private bool showAvailableColors = true;

    [PropertyDisplay("Standard Colors", "Panels", 3)]
    [ObservableProperty]
    private bool showStandardColors = true;

    [PropertyDisplay("Hue Slider", "Panels", 4)]
    [ObservableProperty]
    private bool showHueSlider = true;

    [PropertyDisplay("Transparency Slider", "Panels", 5)]
    [ObservableProperty]
    private bool showTransparencySlider = true;

    [PropertyDisplay("Before/After", "Details", 1)]
    [ObservableProperty]
    private bool showBeforeAfterColorResult = true;

    [PropertyDisplay("HSV", "Details", 2)]
    [ObservableProperty]
    private bool showHsv = true;

    [PropertyDisplay("RGBA", "Details", 3)]
    [ObservableProperty]
    private bool showRgba = true;

    [PropertyDisplay("ARGB", "Details", 4)]
    [ObservableProperty]
    private bool showArgb = true;
}