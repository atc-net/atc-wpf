namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// Shared polling plumbing for the services that have no device watcher (drives, displays, network
/// adapters, printers, processes and windows).
/// </summary>
/// <remarks>
/// The poll runs on a thread-pool thread and is driven by a thread-pool timer, so neither depends on the
/// thread that constructed the service. Each service applies its results through <see cref="InvokeOnUiAsync(Action)"/>,
/// which targets the same UI dispatcher <see cref="DeviceWatcherHost"/> uses, so the bound collections only ever
/// change on the UI thread. Concurrent refreshes (timer or caller) share the poll that is already in flight.
/// </remarks>
internal sealed class PollingServiceHost : IDisposable
{
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(2);

    private readonly string serviceName;
    private readonly Func<Task> poll;
    private readonly Dispatcher uiDispatcher;
    private readonly Timer timer;
    private readonly Lock syncRoot = new();
    private TimeSpan interval = DefaultInterval;
    private Task? inFlightPoll;
    private bool started;
    private volatile bool disposed;

    public PollingServiceHost(
        string serviceName,
        Func<Task> poll,
        Dispatcher? uiDispatcher = null)
    {
        this.serviceName = serviceName;
        this.poll = poll;
        this.uiDispatcher = uiDispatcher ?? Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        timer = new Timer(OnTimerElapsed, state: null, Timeout.Infinite, Timeout.Infinite);
    }

    public bool IsDisposed => disposed;

    public TimeSpan Interval
    {
        get
        {
            lock (syncRoot)
            {
                return interval;
            }
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, TimeSpan.Zero);

            lock (syncRoot)
            {
                interval = value;
                if (started && !disposed)
                {
                    timer.Change(value, Timeout.InfiniteTimeSpan);
                }
            }
        }
    }

    public void Start()
    {
        lock (syncRoot)
        {
            if (started || disposed)
            {
                return;
            }

            started = true;
            timer.Change(interval, Timeout.InfiniteTimeSpan);
        }
    }

    public void Stop()
    {
        lock (syncRoot)
        {
            if (!started || disposed)
            {
                return;
            }

            started = false;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Starts a poll on the thread pool, or returns the one already in flight.
    /// </summary>
    public Task RefreshAsync()
    {
        lock (syncRoot)
        {
            if (disposed)
            {
                return Task.CompletedTask;
            }

            if (inFlightPoll is { IsCompleted: false })
            {
                return inFlightPoll;
            }

            inFlightPoll = Task.Run(poll);
            return inFlightPoll;
        }
    }

    /// <summary>
    /// Runs <paramref name="action"/> on the UI thread, inline when already there.
    /// </summary>
    public Task InvokeOnUiAsync(Action action)
    {
        if (uiDispatcher.CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return uiDispatcher.InvokeAsync(action).Task;
    }

    /// <summary>
    /// Runs <paramref name="func"/> on the UI thread, inline when already there, and returns its result.
    /// </summary>
    public Task<T> InvokeOnUiAsync<T>(Func<T> func)
    {
        if (uiDispatcher.CheckAccess())
        {
            return Task.FromResult(func());
        }

        return uiDispatcher.InvokeAsync(func).Task;
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            started = false;
        }

        timer.Dispose();
    }

    private void OnTimerElapsed(object? state)
    {
        if (disposed)
        {
            return;
        }

        _ = PollFromTimerAsync();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient enumeration errors.")]
    private async Task PollFromTimerAsync()
    {
        try
        {
            await RefreshAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{serviceName} poll failed: {ex.Message}");
        }
        finally
        {
            // One-shot timer, re-armed only once the poll is done, so a slow poll never overlaps the next tick.
            lock (syncRoot)
            {
                if (started && !disposed)
                {
                    timer.Change(interval, Timeout.InfiniteTimeSpan);
                }
            }
        }
    }
}