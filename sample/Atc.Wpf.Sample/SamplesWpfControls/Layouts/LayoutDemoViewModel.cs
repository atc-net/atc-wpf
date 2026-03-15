namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class LayoutDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Show Grid Lines", "Appearance", 1)]
    [ObservableProperty]
    private bool showGridLines;

    [PropertyDisplay("Spacing", "Layout", 1)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 10;

    [PropertyDisplay("Orientation", "Layout", 2)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;
}