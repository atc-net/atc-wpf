namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class DividerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Orientation", "Layout", 1)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    [PropertyDisplay("Line Stroke Thickness", "Appearance", 1)]
    [PropertyRange(0.5, 10, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double lineStrokeThickness = 1.0;

    [PropertyDisplay("Label Text", "Content", 1)]
    [ObservableProperty]
    private string content = string.Empty;
}