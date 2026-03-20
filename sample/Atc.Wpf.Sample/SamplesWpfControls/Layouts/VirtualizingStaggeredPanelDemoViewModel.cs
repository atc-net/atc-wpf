namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class VirtualizingStaggeredPanelDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Desired Item Width", "Layout", 1)]
    [PropertyRange(50, 400, 10)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double desiredItemWidth = 150;

    [PropertyDisplay("Horizontal Spacing", "Layout", 2)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double horizontalSpacing = 10;

    [PropertyDisplay("Vertical Spacing", "Layout", 3)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double verticalSpacing = 10;

    [PropertyDisplay("Item Count", "Data", 1)]
    [PropertyRange(10, 10000, 10)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double itemCount = 1000;
}