namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class TimelineDemoViewModel : ViewModelBase
{
    private Orientation orientation = Orientation.Vertical;
    private TimelineMode mode = TimelineMode.Left;
    private TimelineConnectorStyle connectorStyle = TimelineConnectorStyle.Solid;
    private double dotSize = 12;
    private double lineThickness = 2;
    private double itemSpacing = 20;

    [PropertyDisplay("Orientation", "Layout", 1)]
    public Orientation Orientation
    {
        get => orientation;
        set => Set(ref orientation, value);
    }

    [PropertyDisplay("Mode", "Layout", 2)]
    public TimelineMode Mode
    {
        get => mode;
        set => Set(ref mode, value);
    }

    [PropertyDisplay("Connector Style", "Appearance", 1)]
    public TimelineConnectorStyle ConnectorStyle
    {
        get => connectorStyle;
        set => Set(ref connectorStyle, value);
    }

    [PropertyDisplay("Dot Size", "Appearance", 2)]
    [PropertyRange(4, 40, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double DotSize
    {
        get => dotSize;
        set => Set(ref dotSize, value);
    }

    [PropertyDisplay("Line Thickness", "Appearance", 3)]
    [PropertyRange(1, 8, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double LineThickness
    {
        get => lineThickness;
        set => Set(ref lineThickness, value);
    }

    [PropertyDisplay("Item Spacing", "Layout", 3)]
    [PropertyRange(0, 60, 4)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double ItemSpacing
    {
        get => itemSpacing;
        set => Set(ref itemSpacing, value);
    }
}