namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class StaggeredPanelDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Spacing", "Layout", 1)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 10;

    [PropertyDisplay("Desired Item Width", "Layout", 2)]
    [PropertyRange(10, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double desiredItemWidth = 50;

    [PropertyDisplay("Horizontal Spacing", "Layout", 3)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double horizontalSpacing;

    [PropertyDisplay("Vertical Spacing", "Layout", 4)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double verticalSpacing;
}