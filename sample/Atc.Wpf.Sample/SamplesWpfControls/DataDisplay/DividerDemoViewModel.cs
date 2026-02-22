namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class DividerDemoViewModel : ViewModelBase
{
    private Orientation orientation = Orientation.Horizontal;
    private double lineStrokeThickness = 1.0;
    private string content = string.Empty;

    [PropertyDisplay("Orientation", "Layout", 1)]
    public Orientation Orientation
    {
        get => orientation;
        set => Set(ref orientation, value);
    }

    [PropertyDisplay("Line Stroke Thickness", "Appearance", 1)]
    [PropertyRange(0.5, 10, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double LineStrokeThickness
    {
        get => lineStrokeThickness;
        set => Set(ref lineStrokeThickness, value);
    }

    [PropertyDisplay("Label Text", "Content", 1)]
    public string Content
    {
        get => content;
        set => Set(ref content, value);
    }
}