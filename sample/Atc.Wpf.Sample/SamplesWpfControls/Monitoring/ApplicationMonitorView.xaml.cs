namespace Atc.Wpf.Sample.SamplesWpfControls.Monitoring;

public partial class ApplicationMonitorView : IDisposable
{
    private readonly DispatcherTimer dispatcherTimer;

    public ApplicationMonitorView()
    {
        InitializeComponent();

        DataContext = this;
        ApplicationMonitorViewModel = new ApplicationMonitorViewModel();

        dispatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300),
        };

        dispatcherTimer.Tick += TimerTick;

        if (EnableTimer)
        {
            dispatcherTimer.Start();
        }
    }

    [DependencyProperty(PropertyChangedCallback = nameof(OnEnableTimerChanged))]
    private bool enableTimer;

    private static void OnEnableTimerChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var view = (ApplicationMonitorView)d;

        var isEnabled = (bool)e.NewValue;

        if (isEnabled)
        {
            view.dispatcherTimer.Start();
        }
        else
        {
            view.dispatcherTimer.Stop();
        }
    }

    public ApplicationMonitorViewModel ApplicationMonitorViewModel { get; set; }

    public IRelayCommand AddOneCommand => new RelayCommand(AddOneCommandHandler);

    public IRelayCommand AddManyCommand => new RelayCommand(AddManyCommandHandler);

    private static void AddOneCommandHandler()
        => LogSimulator.SendRandomLogMessage();

    private static void AddManyCommandHandler()
    {
        for (var i = 0; i < 10; i++)
        {
            LogSimulator.SendRandomLogMessage();
        }
    }

    private static void TimerTick(object? sender, EventArgs e)
        => LogSimulator.SendRandomLogMessage();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(
        bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        dispatcherTimer.Stop();
    }
}