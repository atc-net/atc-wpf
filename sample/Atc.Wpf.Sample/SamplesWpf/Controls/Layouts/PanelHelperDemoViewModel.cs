namespace Atc.Wpf.Sample.SamplesWpf.Controls.Layouts;

public partial class PanelHelperDemoViewModel : ViewModelBase
{
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

    [PropertyDisplay("Spacing", "Layout", 3)]
    [PropertyRange(0, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 20;
}