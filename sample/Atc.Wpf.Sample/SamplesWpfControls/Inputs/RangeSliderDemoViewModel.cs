namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class RangeSliderDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Range Start", "Value", 1)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double rangeStart = 20;

    [PropertyDisplay("Range End", "Value", 2)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double rangeEnd = 80;

    [PropertyDisplay("Minimum", "Range", 1)]
    [PropertyRange(-100, 0, 10)]
    [ObservableProperty]
    private double minimum;

    [PropertyDisplay("Maximum", "Range", 2)]
    [PropertyRange(0, 1000, 10)]
    [ObservableProperty]
    private double maximum = 100;

    [PropertyDisplay("Step", "Range", 3)]
    [PropertyRange(0.1, 50, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double step = 1;

    [PropertyDisplay("Thumb Size", "Appearance", 1)]
    [PropertyRange(10, 40, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double thumbSize = 20;

    [PropertyDisplay("Track Height", "Appearance", 2)]
    [PropertyRange(1, 12, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double trackHeight = 4;

    [PropertyDisplay("Show Value ToolTips", "Behavior", 1)]
    [ObservableProperty]
    private bool showValueToolTips = true;
}