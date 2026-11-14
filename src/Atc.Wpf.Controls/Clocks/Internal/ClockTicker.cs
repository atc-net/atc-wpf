namespace Atc.Wpf.Controls.Clocks.Internal;

/// <summary>
/// Drives a clock control from the current local time: a <see cref="DispatcherTimer"/> that fires just after each
/// whole second while the owning control is loaded and asks to follow the current time.
/// </summary>
internal sealed class ClockTicker
{
    private readonly DispatcherTimer timer;
    private readonly Action<TimeSpan> onTick;
    private bool isLoaded;
    private bool isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClockTicker"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher of the owning control.</param>
    /// <param name="onTick">Receives the current time of day on each tick.</param>
    public ClockTicker(
        Dispatcher dispatcher,
        Action<TimeSpan> onTick)
    {
        this.onTick = onTick;
        timer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
        timer.Tick += OnTimerTick;
    }

    /// <summary>
    /// Gets or sets the source of the current local time (replaceable in tests).
    /// </summary>
    public Func<DateTime> Now { get; set; } = () => DateTime.Now;

    /// <summary>
    /// Gets a value indicating whether the timer is running.
    /// </summary>
    public bool IsRunning => timer.IsEnabled;

    /// <summary>
    /// Tells the ticker whether the owning control is loaded.
    /// </summary>
    public void SetLoaded(bool value)
    {
        isLoaded = value;
        Update();
    }

    /// <summary>
    /// Tells the ticker whether the owning control follows the current time.
    /// </summary>
    public void SetEnabled(bool value)
    {
        isEnabled = value;
        Update();
    }

    private static TimeSpan GetDelayToNextSecond(DateTime now)
        => TimeSpan.FromMilliseconds(1000 - now.Millisecond + 10);

    private void Update()
    {
        if (isLoaded && isEnabled)
        {
            if (timer.IsEnabled)
            {
                return;
            }

            var now = Now();
            onTick(now.TimeOfDay);
            timer.Interval = GetDelayToNextSecond(now);
            timer.Start();
        }
        else
        {
            timer.Stop();
        }
    }

    private void OnTimerTick(
        object? sender,
        EventArgs e)
    {
        var now = Now();
        onTick(now.TimeOfDay);
        timer.Interval = GetDelayToNextSecond(now);
    }
}