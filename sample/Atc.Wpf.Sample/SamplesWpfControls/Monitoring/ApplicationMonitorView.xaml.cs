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

    public static readonly DependencyProperty EnableTimerProperty = DependencyProperty.Register(
        nameof(EnableTimer),
        typeof(bool),
        typeof(ApplicationMonitorView),
        new PropertyMetadata(
            defaultValue: false,
            propertyChangedCallback: OnEnableTimerChanged));

    public bool EnableTimer
    {
        get => (bool)GetValue(EnableTimerProperty);
        set => SetValue(EnableTimerProperty, value);
    }

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
        dispatcherTimer.Dispatcher?.InvokeShutdown();
    }
}