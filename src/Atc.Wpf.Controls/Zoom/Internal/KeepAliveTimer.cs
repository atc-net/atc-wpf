namespace Atc.Wpf.Controls.Zoom.Internal;

internal sealed class KeepAliveTimer
{
    private readonly DispatcherTimer dispatcherTimer;
    private DateTime startTime;
    private TimeSpan? runTime;

    public KeepAliveTimer(
        TimeSpan time,
        Action action)
    {
        Time = time;
        Action = action;
        dispatcherTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
        {
            Interval = time,
        };
        dispatcherTimer.Tick += TimerExpired;
    }

    public TimeSpan Time { get; set; }

    public Action Action { get; set; }

    public bool Running { get; private set; }

    public void Nudge()
    {
        lock (dispatcherTimer)
        {
            if (!Running)
            {
                startTime = DateTime.UtcNow;
                runTime = null;
                dispatcherTimer.Start();
                Running = true;
            }
            else
            {
                // Reset the timer
                dispatcherTimer.Stop();
                dispatcherTimer.Start();
            }
        }
    }

    public TimeSpan GetTimeSpan()
        => runTime ?? DateTime.UtcNow.Subtract(startTime);

    private void TimerExpired(
        object? sender,
        EventArgs e)
    {
        lock (dispatcherTimer)
        {
            Running = false;
            dispatcherTimer.Stop();
            runTime = DateTime.UtcNow.Subtract(startTime);
            Action();
        }
    }
}