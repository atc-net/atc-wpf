namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Runs a <see cref="ToastNotification"/>'s expiration time, optionally paused while the mouse is over it, and
/// completes <see cref="Task"/> when the time has run out or the notification starts closing.
/// </summary>
internal sealed class ToastNotificationExpiry
{
    private static readonly TimeSpan MaxTimerInterval = TimeSpan.FromMilliseconds(int.MaxValue);

    private readonly ToastNotification notification;
    private readonly bool pauseOnHover;
    private readonly TaskCompletionSource completion = new();
    private readonly Stopwatch stopwatch = new();
    private readonly DispatcherTimer timer;
    private TimeSpan remaining;

    public ToastNotificationExpiry(
        ToastNotification notification,
        TimeSpan expirationTime,
        bool pauseOnHover)
    {
        this.notification = notification;
        this.pauseOnHover = pauseOnHover;
        remaining = expirationTime;
        timer = new DispatcherTimer(DispatcherPriority.Normal, notification.Dispatcher);

        timer.Tick += OnTick;
        notification.NotificationCloseInvoked += OnCloseInvoked;

        if (pauseOnHover)
        {
            notification.MouseEnter += OnMouseEnter;
            notification.MouseLeave += OnMouseLeave;
        }

        if (notification.IsClosing)
        {
            Finish();
        }
        else if (!pauseOnHover || !notification.IsMouseOver)
        {
            Run();
        }
    }

    /// <summary>
    /// Gets the task that completes when the notification has expired or starts closing.
    /// </summary>
    public Task Task => completion.Task;

    private void Run()
    {
        if (remaining <= TimeSpan.Zero)
        {
            Finish();
            return;
        }

        timer.Interval = remaining < MaxTimerInterval
            ? remaining
            : MaxTimerInterval;
        stopwatch.Restart();
        timer.Start();
    }

    private void Finish()
    {
        timer.Stop();
        timer.Tick -= OnTick;
        notification.NotificationCloseInvoked -= OnCloseInvoked;

        if (pauseOnHover)
        {
            notification.MouseEnter -= OnMouseEnter;
            notification.MouseLeave -= OnMouseLeave;
        }

        completion.TrySetResult();
    }

    private void OnTick(
        object? sender,
        EventArgs e)
    {
        timer.Stop();
        remaining -= stopwatch.Elapsed;

        // An expiration time longer than one timer interval allows runs in several steps.
        if (remaining > TimeSpan.Zero && timer.Interval == MaxTimerInterval)
        {
            Run();
            return;
        }

        Finish();
    }

    private void OnMouseEnter(
        object sender,
        MouseEventArgs e)
    {
        if (!timer.IsEnabled)
        {
            return;
        }

        timer.Stop();
        remaining -= stopwatch.Elapsed;
    }

    private void OnMouseLeave(
        object sender,
        MouseEventArgs e)
    {
        if (!timer.IsEnabled && !completion.Task.IsCompleted)
        {
            Run();
        }
    }

    private void OnCloseInvoked(
        object sender,
        RoutedEventArgs e)
        => Finish();
}