namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class RatingDemoViewModel : ViewModelBase
{
    private double value = 3;

    [PropertyDisplay("Maximum", "Range", 1)]
    [PropertyRange(1, 10, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int maximum = 5;

    [PropertyDisplay("Allow Half Stars", "Behavior", 1)]
    [ObservableProperty]
    private bool allowHalfStars;

    [PropertyDisplay("Is Read Only", "Behavior", 2)]
    [ObservableProperty]
    private bool isReadOnly;

    [PropertyDisplay("Item Size", "Appearance", 1)]
    [PropertyRange(12, 64, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double itemSize = 24;

    [PropertyDisplay("Item Spacing", "Appearance", 2)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double itemSpacing = 4;

    [PropertyDisplay("Show Preview On Hover", "Behavior", 3)]
    [ObservableProperty]
    private bool showPreviewOnHover = true;

    [PropertyDisplay("Value", "Value", 1)]
    [PropertyRange(0, 10, 0.5)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double Value
    {
        get => value;
        set => Set(ref this.value, value);
    }
}