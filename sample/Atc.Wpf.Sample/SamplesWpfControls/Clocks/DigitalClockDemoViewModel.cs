namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class DigitalClockDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Follow Current Time", "Time", 1)]
    [ObservableProperty]
    private bool followCurrentTime = true;

    [PropertyDisplay("Time", "Time", 2)]
    [ObservableProperty]
    private TimeSpan time = new(12, 34, 56);

    [PropertyDisplay("Show Seconds", "Behavior", 1)]
    [ObservableProperty]
    private bool showSeconds = true;

    [PropertyDisplay("Show Colon", "Behavior", 2)]
    [ObservableProperty]
    private bool showColon = true;

    [PropertyDisplay("Animate Colon", "Behavior", 3)]
    [ObservableProperty]
    private bool animateColon = true;

    [PropertyDisplay("Play Animation", "Behavior", 4)]
    [ObservableProperty]
    private bool playAnimation = true;

    [PropertyDisplay("Number Blur Radius", "Appearance", 1)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int numberBlurRadius = 5;

    [PropertyDisplay("Number", "Custom Colors", 1)]
    [ObservableProperty]
    private Color numberColor = Colors.Chocolate;

    [PropertyDisplay("Colon", "Custom Colors", 2)]
    [ObservableProperty]
    private Color colonColor = Colors.Brown;
}