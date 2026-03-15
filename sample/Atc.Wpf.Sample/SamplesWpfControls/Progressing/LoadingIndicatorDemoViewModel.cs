namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class LoadingIndicatorDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Is Active", "Behavior", 1)]
    [ObservableProperty]
    private bool isActive = true;

    [PropertyDisplay("Custom Color Brush", "Appearance", 1)]
    [ObservableProperty]
    private Color customColorBrush = Colors.Orange;

    [PropertyDisplay("Speed Ratio", "Appearance", 2)]
    [PropertyRange(0, 100, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private decimal speedRatio = 0.5m;
}