// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

/// <summary>
/// Provides Debounce() and Throttle() methods.
/// Use these methods to ensure that events aren't handled too frequently.
///
/// Throttle() ensures that events are throttled by the interval specified.
/// Only the last event in the interval sequence of events fires.
///
/// Debounce() fires an event only after the specified interval has passed
/// in which no other pending event has fired. Only the last event in the
/// sequence is fired.
/// </summary>
/// <remarks>
/// Inspiration source: https://weblog.west-wind.com/posts/2017/jul/02/debouncing-and-throttling-dispatcher-events
/// </remarks>
public sealed class DebounceDispatcher
{
    private DispatcherTimer? timer;
    private Action<object?>? pendingAction;
    private object? pendingParam;
    private DispatcherPriority cachedPriority;
    private Dispatcher? cachedDispatcher;

    private DateTime TimerStarted { get; set; } = DateTime.UtcNow.AddYears(-1);

    private void OnTimerTick(
        object? sender,
        EventArgs e)
    {
        timer?.Stop();

        var action = pendingAction;
        var param = pendingParam;
        pendingAction = null;
        pendingParam = null;

        action?.Invoke(param);
    }

    private bool TryReuseTimer(
        DispatcherPriority priority,
        Dispatcher dispatcher)
        => timer is not null
            && cachedPriority == priority
            && ReferenceEquals(cachedDispatcher, dispatcher);

    /// <summary>
    /// Debounce an event by resetting the event timeout every time the event is
    /// fired. The behavior is that the Action passed is fired only after events
    /// stop firing for the given timeout period.
    ///
    /// Use Debounce when you want events to fire only after events stop firing
    /// after the given interval timeout period.
    ///
    /// Wrap the logic you would normally use in your event code into
    /// the  Action you pass to this method to debounce the event.
    /// </summary>
    /// <param name="interval">Timeout in Milliseconds.</param>
    /// <param name="action">Action&lt;object?&gt; to fire when debounce event fires.</param>
    /// <param name="param">optional parameter.</param>
    /// <param name="priority">optional priority for the dispatcher.</param>
    /// <param name="dispatcher">optional dispatcher. If not passed or null CurrentDispatcher is used.</param>
    public void Debounce(
        int interval,
        Action<object?> action,
        object? param = null,
        DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
        Dispatcher? dispatcher = null)
    {
        dispatcher ??= Dispatcher.CurrentDispatcher;

        // Stop any pending tick; reuse the timer when priority + dispatcher match,
        // otherwise rebuild (DispatcherTimer locks priority/dispatcher at construction).
        timer?.Stop();

        if (TryReuseTimer(priority, dispatcher))
        {
            timer!.Interval = TimeSpan.FromMilliseconds(interval);
        }
        else
        {
            timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(interval),
                priority,
                OnTimerTick,
                dispatcher);
            cachedPriority = priority;
            cachedDispatcher = dispatcher;
        }

        // Latest call wins — debounce semantics are "only the last event fires".
        pendingAction = action;
        pendingParam = param;
        timer.Start();
    }

    /// <summary>
    /// This method throttles events by allowing only 1 event to fire for the given
    /// timeout period. Only the last event fired is handled - all others are ignored.
    /// Throttle will fire events every timeout ms even if additional events are pending.
    ///
    /// Use Throttle where you need to ensure that events fire at given intervals.
    /// </summary>
    /// <param name="interval">Timeout in Milliseconds.</param>
    /// <param name="action">Action&lt;object?&gt; to fire when debounce event fires.</param>
    /// <param name="param">optional parameter.</param>
    /// <param name="priority">optional priority for the dispatcher.</param>
    /// <param name="dispatcher">optional dispatcher. If not passed or null CurrentDispatcher is used.</param>
    public void Throttle(
        int interval,
        Action<object?> action,
        object? param = null,
        DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
        Dispatcher? dispatcher = null)
    {
        dispatcher ??= Dispatcher.CurrentDispatcher;

        var currentTime = DateTime.UtcNow;

        // If timeout is not up yet - adjust timeout to fire with the latest action+param
        if (currentTime.Subtract(TimerStarted).TotalMilliseconds < interval)
        {
            interval -= (int)currentTime.Subtract(TimerStarted).TotalMilliseconds;
        }

        timer?.Stop();

        if (TryReuseTimer(priority, dispatcher))
        {
            timer!.Interval = TimeSpan.FromMilliseconds(interval);
        }
        else
        {
            timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(interval),
                priority,
                OnTimerTick,
                dispatcher);
            cachedPriority = priority;
            cachedDispatcher = dispatcher;
        }

        pendingAction = action;
        pendingParam = param;
        timer.Start();
        TimerStarted = currentTime;
    }
}