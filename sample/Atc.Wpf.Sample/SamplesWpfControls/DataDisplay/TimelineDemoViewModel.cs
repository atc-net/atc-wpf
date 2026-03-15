namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class TimelineDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Orientation", "Layout", 1)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Vertical;

    [PropertyDisplay("Mode", "Layout", 2)]
    [ObservableProperty]
    private TimelineMode mode = TimelineMode.Left;

    [PropertyDisplay("Connector Style", "Appearance", 1)]
    [ObservableProperty]
    private TimelineConnectorStyle connectorStyle = TimelineConnectorStyle.Solid;

    [PropertyDisplay("Dot Size", "Appearance", 2)]
    [PropertyRange(4, 40, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double dotSize = 12;

    [PropertyDisplay("Line Thickness", "Appearance", 3)]
    [PropertyRange(1, 8, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double lineThickness = 2;

    [PropertyDisplay("Item Spacing", "Layout", 3)]
    [PropertyRange(0, 60, 4)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double itemSpacing = 20;
}