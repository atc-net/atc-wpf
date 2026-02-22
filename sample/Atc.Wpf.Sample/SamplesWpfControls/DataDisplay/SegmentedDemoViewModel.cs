namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class SegmentedDemoViewModel : ViewModelBase
{
    private int selectedIndex;
    private double cornerRadius = 4;
    private bool equalSegmentWidth = true;

    [PropertyDisplay("Selected Index", "Behavior", 1)]
    [PropertyRange(-1, 5, 1)]
    public int SelectedIndex
    {
        get => selectedIndex;
        set => Set(ref selectedIndex, value);
    }

    [PropertyDisplay("Corner Radius", "Appearance", 1)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double CornerRadius
    {
        get => cornerRadius;
        set => Set(ref cornerRadius, value);
    }

    [PropertyDisplay("Equal Segment Width", "Layout", 1)]
    public bool EqualSegmentWidth
    {
        get => equalSegmentWidth;
        set => Set(ref equalSegmentWidth, value);
    }
}