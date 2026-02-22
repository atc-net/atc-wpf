namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class StepperDemoViewModel : ViewModelBase
{
    private Orientation orientation = Orientation.Horizontal;
    private int activeStepIndex;
    private bool isClickable = true;
    private double indicatorSize = 32;
    private double stepSpacing;
    private double lineThickness = 2;

    [PropertyDisplay("Orientation", "Layout", 1)]
    public Orientation Orientation
    {
        get => orientation;
        set => Set(ref orientation, value);
    }

    [PropertyDisplay("Active Step Index", "Behavior", 1)]
    [PropertyRange(0, 5, 1)]
    public int ActiveStepIndex
    {
        get => activeStepIndex;
        set => Set(ref activeStepIndex, value);
    }

    [PropertyDisplay("Is Clickable", "Behavior", 2)]
    public bool IsClickable
    {
        get => isClickable;
        set => Set(ref isClickable, value);
    }

    [PropertyDisplay("Indicator Size", "Appearance", 1)]
    [PropertyRange(16, 64, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double IndicatorSize
    {
        get => indicatorSize;
        set => Set(ref indicatorSize, value);
    }

    [PropertyDisplay("Step Spacing", "Layout", 2)]
    [PropertyRange(0, 40, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double StepSpacing
    {
        get => stepSpacing;
        set => Set(ref stepSpacing, value);
    }

    [PropertyDisplay("Line Thickness", "Appearance", 2)]
    [PropertyRange(1, 8, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double LineThickness
    {
        get => lineThickness;
        set => Set(ref lineThickness, value);
    }
}