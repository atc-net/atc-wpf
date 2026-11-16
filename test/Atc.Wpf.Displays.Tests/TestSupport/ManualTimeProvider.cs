namespace Atc.Wpf.Displays.Tests.TestSupport;

/// <summary>
/// A <see cref="TimeProvider"/> whose clock only moves when the test calls <see cref="Advance"/>.
/// Timers created by it fire synchronously on the thread that advances the clock, in due-time order,
/// and a periodic timer fires once for every period that elapses.
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    private readonly List<ManualTimer> timers = [];
    private DateTimeOffset now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Gets the number of timers that have been created and not yet disposed.
    /// </summary>
    public int ActiveTimerCount
    {
        get
        {
            lock (timers)
            {
                return timers.Count;
            }
        }
    }

    public override DateTimeOffset GetUtcNow()
        => now;

    public override ITimer CreateTimer(
        TimerCallback callback,
        object? state,
        TimeSpan dueTime,
        TimeSpan period)
    {
        var timer = new ManualTimer(this, callback, state);
        lock (timers)
        {
            timers.Add(timer);
        }

        timer.Change(dueTime, period);
        return timer;
    }

    /// <summary>
    /// Moves the clock forward and fires every timer that becomes due on the way.
    /// </summary>
    /// <param name="delta">The time to move forward.</param>
    public void Advance(TimeSpan delta)
    {
        var target = now + delta;
        while (true)
        {
            ManualTimer? next = null;
            lock (timers)
            {
                foreach (var timer in timers)
                {
                    if (timer.Due is { } due &&
                        due <= target &&
                        (next is null || due < next.Due))
                    {
                        next = timer;
                    }
                }
            }

            if (next is null)
            {
                break;
            }

            now = next.Due!.Value;
            next.Fire();
        }

        now = target;
    }

    private void Remove(ManualTimer timer)
    {
        lock (timers)
        {
            timers.Remove(timer);
        }
    }

    private sealed class ManualTimer : ITimer
    {
        private readonly ManualTimeProvider owner;
        private readonly TimerCallback callback;
        private readonly object? state;
        private TimeSpan period;

        public ManualTimer(
            ManualTimeProvider owner,
            TimerCallback callback,
            object? state)
        {
            this.owner = owner;
            this.callback = callback;
            this.state = state;
        }

        public DateTimeOffset? Due { get; private set; }

        public bool Change(
            TimeSpan dueTime,
            TimeSpan period)
        {
            this.period = period;
            Due = dueTime == Timeout.InfiniteTimeSpan
                ? null
                : owner.now + dueTime;
            return true;
        }

        public void Fire()
        {
            Due = period == Timeout.InfiniteTimeSpan || period <= TimeSpan.Zero
                ? null
                : Due + period;
            callback(state);
        }

        public void Dispose()
        {
            Due = null;
            owner.Remove(this);
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}