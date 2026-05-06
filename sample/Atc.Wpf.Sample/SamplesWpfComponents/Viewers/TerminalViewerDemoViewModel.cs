namespace Atc.Wpf.Sample.SamplesWpfComponents.Viewers;

public partial class TerminalViewerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Show Toolbar", "Toolbar", 1)]
    [ObservableProperty]
    private bool showToolbar = true;

    [PropertyDisplay("Show Clear", "Toolbar", 2)]
    [ObservableProperty]
    private bool showClearInToolbar = true;

    [PropertyDisplay("Show Copy", "Toolbar", 3)]
    [ObservableProperty]
    private bool showCopyInToolbar = true;

    [PropertyDisplay("Show AutoScroll", "Toolbar", 4)]
    [ObservableProperty]
    private bool showAutoScrollInToolbar = true;

    [PropertyDisplay("Show Pause", "Toolbar", 5)]
    [ObservableProperty]
    private bool showPauseInToolbar = true;

    [PropertyDisplay("Show Search", "Toolbar", 6)]
    [ObservableProperty]
    private bool showSearchInToolbar = true;

    [PropertyDisplay("Show Export", "Toolbar", 7)]
    [ObservableProperty]
    private bool showExportInToolbar;

    [PropertyDisplay("Show Wrap", "Toolbar", 8)]
    [ObservableProperty]
    private bool showWrapInToolbar = true;

    [PropertyDisplay("Auto Scroll", "Behavior", 1)]
    [ObservableProperty]
    private bool autoScroll = true;

    [PropertyDisplay("Is Paused", "Behavior", 2)]
    [ObservableProperty]
    private bool isPaused;

    [PropertyDisplay("Max Lines (0 = unbounded)", "Behavior", 3)]
    [ObservableProperty]
    private int maxLines = 10000;

    [PropertyDisplay("Word Wrap", "Behavior", 4)]
    [ObservableProperty]
    private bool wordWrap = true;

    [PropertyDisplay("Enable ANSI Parsing", "Behavior", 5)]
    [ObservableProperty]
    private bool enableAnsiParsing = true;

    [PropertyDisplay("Show Timestamps", "Display", 1)]
    [ObservableProperty]
    private bool showTimestamps;

    [PropertyDisplay("Show Line Numbers", "Display", 2)]
    [ObservableProperty]
    private bool showLineNumbers;

    [PropertyDisplay("Font Size", "Display", 3)]
    [ObservableProperty]
    private double terminalFontSize = 12d;

    private bool enableTimer;

    [PropertyDisplay("Enable Timer", "Behavior", 6)]
    public bool EnableTimer
    {
        get => enableTimer;
        set
        {
            if (Set(ref enableTimer, value))
            {
                EnableTimerChanged?.Invoke(this, value);
            }
        }
    }

    public event EventHandler<bool>? EnableTimerChanged;
}
