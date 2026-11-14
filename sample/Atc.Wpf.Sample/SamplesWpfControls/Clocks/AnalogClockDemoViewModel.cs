namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class AnalogClockDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Follow Current Time", "Time", 1)]
    [ObservableProperty]
    private bool followCurrentTime = true;

    [PropertyDisplay("Time", "Time", 2)]
    [ObservableProperty]
    private TimeSpan time = new(10, 10, 30);

    [PropertyDisplay("Show Numbers", "Visibility", 1)]
    [ObservableProperty]
    private bool showNumbers = true;

    [PropertyDisplay("Show Seconds", "Visibility", 2)]
    [ObservableProperty]
    private bool showSeconds = true;

    [PropertyDisplay("Show AM/PM", "Visibility", 3)]
    [ObservableProperty]
    private bool showAmPm = true;

    [PropertyDisplay("Clock Background", "Custom Colors", 1)]
    [ObservableProperty]
    private Color clockBackgroundColor = Colors.Beige;

    [PropertyDisplay("Clock Border", "Custom Colors", 2)]
    [ObservableProperty]
    private Color clockBorderColor = Colors.SaddleBrown;

    [PropertyDisplay("Clock Center Dot", "Custom Colors", 3)]
    [ObservableProperty]
    private Color clockCenterDotColor = Colors.SaddleBrown;

    [PropertyDisplay("Hour Indicator", "Custom Colors", 4)]
    [ObservableProperty]
    private Color hourIndicatorColor = Colors.SaddleBrown;

    [PropertyDisplay("Minute Indicator", "Custom Colors", 5)]
    [ObservableProperty]
    private Color minuteIndicatorColor = Colors.Peru;

    [PropertyDisplay("Number", "Custom Colors", 6)]
    [ObservableProperty]
    private Color numberColor = Colors.SaddleBrown;

    [PropertyDisplay("Hour Hand", "Custom Colors", 7)]
    [ObservableProperty]
    private Color hourHandColor = Colors.Black;

    [PropertyDisplay("Minute Hand", "Custom Colors", 8)]
    [ObservableProperty]
    private Color minuteHandColor = Colors.Black;

    [PropertyDisplay("Second Hand", "Custom Colors", 9)]
    [ObservableProperty]
    private Color secondHandColor = Colors.Red;

    [PropertyDisplay("AM/PM", "Custom Colors", 10)]
    [ObservableProperty]
    private Color amPmColor = Colors.Peru;
}