namespace Atc.Wpf.Sample.SamplesWpfControls.ColorEditing;

public partial class SaturationBrightnessPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Hue", "Value", 1)]
    [PropertyRange(0, 360, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double hue;
}