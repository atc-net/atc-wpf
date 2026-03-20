namespace Atc.Wpf.Sample.SamplesWpfComponents.Monitoring;

public partial class ApplicationMonitorDemoViewModel : ViewModelBase, IDisposable
{
    private readonly DispatcherTimer dispatcherTimer;

    public ApplicationMonitorDemoViewModel()
    {
        ApplicationMonitorViewModel = new ApplicationMonitorViewModel();

        dispatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300),
        };

        dispatcherTimer.Tick += TimerTick;
    }

    public ApplicationMonitorViewModel ApplicationMonitorViewModel { get; set; }

    [PropertyDisplay("Show Toolbar", "Toolbar", 1)]
    [ObservableProperty]
    private bool showToolbar = true;

    [PropertyDisplay("Show Clear In Toolbar", "Toolbar", 2)]
    [ObservableProperty]
    private bool showClearInToolbar = true;

    [PropertyDisplay("Show AutoScroll In Toolbar", "Toolbar", 3)]
    [ObservableProperty]
    private bool showAutoScrollInToolbar = true;

    [PropertyDisplay("Show Search In Toolbar", "Toolbar", 4)]
    [ObservableProperty]
    private bool showSearchInToolbar = true;

    private bool enableTimer;

    [PropertyDisplay("Enable Timer", "Behavior", 1)]
    public bool EnableTimer
    {
        get => enableTimer;
        set
        {
            if (Set(ref enableTimer, value))
            {
                if (value)
                {
                    dispatcherTimer.Start();
                }
                else
                {
                    dispatcherTimer.Stop();
                }
            }
        }
    }

    [RelayCommand]
    private static void AddOne()
        => LogSimulator.SendRandomLogMessage();

    [RelayCommand]
    private static void AddMany()
    {
        for (var i = 0; i < 10; i++)
        {
            LogSimulator.SendRandomLogMessage();
        }
    }

    private static void TimerTick(
        object? sender,
        EventArgs e)
        => LogSimulator.SendRandomLogMessage();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        dispatcherTimer.Stop();
    }
}