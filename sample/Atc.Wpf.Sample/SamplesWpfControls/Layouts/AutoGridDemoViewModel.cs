namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class AutoGridDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Spacing", "Layout", 1)]
    [PropertyRange(0, 50, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spacing = 10;

    [PropertyDisplay("Column Count", "Layout", 2)]
    [PropertyRange(1, 4, 1)]
    [ObservableProperty]
    private int columnCount = 1;
}