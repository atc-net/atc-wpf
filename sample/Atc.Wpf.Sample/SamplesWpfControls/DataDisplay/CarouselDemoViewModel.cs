namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class CarouselDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Transition Type", "Animation", 1)]
    [ObservableProperty]
    private CarouselTransitionType transitionType = CarouselTransitionType.Slide;

    [PropertyDisplay("Transition Duration (ms)", "Animation", 2)]
    [PropertyRange(50, 2000, 50)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double transitionDuration = 300;

    [PropertyDisplay("Auto Play", "Behavior", 1)]
    [ObservableProperty]
    private bool autoPlay;

    [PropertyDisplay("Auto Play Interval (ms)", "Behavior", 2)]
    [PropertyRange(500, 10000, 500)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double autoPlayInterval = 5000;

    [PropertyDisplay("Pause On Hover", "Behavior", 3)]
    [ObservableProperty]
    private bool pauseOnHover = true;

    [PropertyDisplay("Is Infinite Loop", "Behavior", 4)]
    [ObservableProperty]
    private bool isInfiniteLoop = true;

    [PropertyDisplay("Show Navigation Arrows", "Appearance", 1)]
    [ObservableProperty]
    private bool showNavigationArrows = true;

    [PropertyDisplay("Show Indicators", "Appearance", 2)]
    [ObservableProperty]
    private bool showIndicators = true;

    [PropertyDisplay("Indicator Position", "Appearance", 3)]
    [ObservableProperty]
    private IndicatorPosition indicatorPosition = IndicatorPosition.Bottom;

    [PropertyDisplay("Indicator Size", "Appearance", 4)]
    [PropertyRange(4, 30, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double indicatorSize = 10;

    [PropertyDisplay("Indicator Spacing", "Appearance", 5)]
    [PropertyRange(0, 20, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double indicatorSpacing = 8;

    [PropertyDisplay("Is Drag Enabled", "Behavior", 5)]
    [ObservableProperty]
    private bool isDragEnabled = true;

    [PropertyDisplay("Corner Radius", "Appearance", 6)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius;
}