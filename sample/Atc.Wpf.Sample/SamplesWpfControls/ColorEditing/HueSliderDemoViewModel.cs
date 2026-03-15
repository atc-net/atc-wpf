namespace Atc.Wpf.Sample.SamplesWpfControls.ColorEditing;

public partial class HueSliderDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Hue", "Value", 1)]
    [PropertyRange(0, 360, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double hue;

    [PropertyDisplay("Width", "Appearance", 1)]
    [PropertyRange(20, 100, 5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double width = 50;

    [PropertyDisplay("Height", "Appearance", 2)]
    [PropertyRange(50, 400, 10)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double height = 200;
}