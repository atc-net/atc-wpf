namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class UniformSpacingPanelDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Spacing", "Interactive", 1)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 10;

    [PropertyDisplay("Orientation", "Interactive", 2)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    [PropertyDisplay("Horizontal Spacing", "Layout", 1)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double horizontalSpacing = 20;

    [PropertyDisplay("Vertical Spacing", "Layout", 2)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double verticalSpacing = 30;

    [PropertyDisplay("Uniform Spacing", "Layout", 3)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double uniformSpacing = 20;
}