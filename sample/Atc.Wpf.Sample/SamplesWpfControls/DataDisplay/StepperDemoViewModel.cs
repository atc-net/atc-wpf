namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class StepperDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Orientation", "Layout", 1)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    [PropertyDisplay("Active Step Index", "Behavior", 1)]
    [PropertyRange(0, 5, 1)]
    [ObservableProperty]
    private int activeStepIndex;

    [PropertyDisplay("Is Clickable", "Behavior", 2)]
    [ObservableProperty]
    private bool isClickable = true;

    [PropertyDisplay("Indicator Size", "Appearance", 1)]
    [PropertyRange(16, 64, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double indicatorSize = 32;

    [PropertyDisplay("Step Spacing", "Layout", 2)]
    [PropertyRange(0, 40, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double stepSpacing;

    [PropertyDisplay("Line Thickness", "Appearance", 2)]
    [PropertyRange(1, 8, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double lineThickness = 2;
}