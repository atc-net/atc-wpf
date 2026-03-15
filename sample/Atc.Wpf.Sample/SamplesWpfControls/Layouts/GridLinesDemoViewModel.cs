namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class GridLinesDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Spacing", "Layout", 1)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 10;
    private bool isVisible = true;

    [PropertyDisplay("Line Brush Color", "Appearance", 1)]
    [ObservableProperty]
    private Color lineBrushColor = Colors.Orange;

    [PropertyDisplay("Horizontal Step", "Layout", 2)]
    [PropertyRange(3, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double horizontalStep = 20;

    [PropertyDisplay("Vertical Step", "Layout", 3)]
    [PropertyRange(3, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double verticalStep = 20;

    [PropertyDisplay("Is Visible", "Behavior", 1)]
    public new bool IsVisible
    {
        get => isVisible;
        set => Set(ref isVisible, value);
    }
}