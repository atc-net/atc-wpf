namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class SegmentedDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Selected Index", "Behavior", 1)]
    [PropertyRange(-1, 5, 1)]
    [ObservableProperty]
    private int selectedIndex;

    [PropertyDisplay("Corner Radius", "Appearance", 1)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius = 4;

    [PropertyDisplay("Equal Segment Width", "Layout", 1)]
    [ObservableProperty]
    private bool equalSegmentWidth = true;
}