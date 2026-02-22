namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class CarouselDemoViewModel : ViewModelBase
{
    private CarouselTransitionType transitionType = CarouselTransitionType.Slide;
    private double transitionDuration = 300;
    private bool autoPlay;
    private double autoPlayInterval = 5000;
    private bool pauseOnHover = true;
    private bool isInfiniteLoop = true;
    private bool showNavigationArrows = true;
    private bool showIndicators = true;
    private IndicatorPosition indicatorPosition = IndicatorPosition.Bottom;
    private double indicatorSize = 10;
    private double indicatorSpacing = 8;
    private bool isDragEnabled = true;
    private double cornerRadius;

    [PropertyDisplay("Transition Type", "Animation", 1)]
    public CarouselTransitionType TransitionType
    {
        get => transitionType;
        set => Set(ref transitionType, value);
    }

    [PropertyDisplay("Transition Duration (ms)", "Animation", 2)]
    [PropertyRange(50, 2000, 50)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double TransitionDuration
    {
        get => transitionDuration;
        set => Set(ref transitionDuration, value);
    }

    [PropertyDisplay("Auto Play", "Behavior", 1)]
    public bool AutoPlay
    {
        get => autoPlay;
        set => Set(ref autoPlay, value);
    }

    [PropertyDisplay("Auto Play Interval (ms)", "Behavior", 2)]
    [PropertyRange(500, 10000, 500)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double AutoPlayInterval
    {
        get => autoPlayInterval;
        set => Set(ref autoPlayInterval, value);
    }

    [PropertyDisplay("Pause On Hover", "Behavior", 3)]
    public bool PauseOnHover
    {
        get => pauseOnHover;
        set => Set(ref pauseOnHover, value);
    }

    [PropertyDisplay("Is Infinite Loop", "Behavior", 4)]
    public bool IsInfiniteLoop
    {
        get => isInfiniteLoop;
        set => Set(ref isInfiniteLoop, value);
    }

    [PropertyDisplay("Show Navigation Arrows", "Appearance", 1)]
    public bool ShowNavigationArrows
    {
        get => showNavigationArrows;
        set => Set(ref showNavigationArrows, value);
    }

    [PropertyDisplay("Show Indicators", "Appearance", 2)]
    public bool ShowIndicators
    {
        get => showIndicators;
        set => Set(ref showIndicators, value);
    }

    [PropertyDisplay("Indicator Position", "Appearance", 3)]
    public IndicatorPosition IndicatorPosition
    {
        get => indicatorPosition;
        set => Set(ref indicatorPosition, value);
    }

    [PropertyDisplay("Indicator Size", "Appearance", 4)]
    [PropertyRange(4, 30, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double IndicatorSize
    {
        get => indicatorSize;
        set => Set(ref indicatorSize, value);
    }

    [PropertyDisplay("Indicator Spacing", "Appearance", 5)]
    [PropertyRange(0, 20, 2)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double IndicatorSpacing
    {
        get => indicatorSpacing;
        set => Set(ref indicatorSpacing, value);
    }

    [PropertyDisplay("Is Drag Enabled", "Behavior", 5)]
    public bool IsDragEnabled
    {
        get => isDragEnabled;
        set => Set(ref isDragEnabled, value);
    }

    [PropertyDisplay("Corner Radius", "Appearance", 6)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double CornerRadius
    {
        get => cornerRadius;
        set => Set(ref cornerRadius, value);
    }
}