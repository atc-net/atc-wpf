namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// Decides when a <see cref="DeviceWatcher"/> may be started. Start is only legal from Created, Stopped or
/// Aborted, and Stop is asynchronous, so a quick stop-then-start has to wait for the Stopped event, which is
/// raised on a WinRT thread. Reading the status and recording the deferred start happen under one lock, so
/// the Stopped event cannot slip in between and lose the restart.
/// </summary>
internal sealed class WatcherStartGate
{
    private readonly Lock gate = new();
    private bool startWanted;
    private bool restartPending;

    /// <summary>
    /// Gets a value indicating whether the watcher should currently be running.
    /// </summary>
    public bool IsStartWanted
    {
        get
        {
            lock (gate)
            {
                return startWanted;
            }
        }
    }

    /// <summary>
    /// Records that the watcher should run.
    /// </summary>
    /// <param name="readStatus">Reads the watcher's current status; called under the lock.</param>
    /// <returns><see langword="true"/> when the caller must start the watcher now.</returns>
    public bool RequestStart(Func<DeviceWatcherStatus> readStatus)
    {
        ArgumentNullException.ThrowIfNull(readStatus);

        lock (gate)
        {
            startWanted = true;

            switch (readStatus())
            {
                case DeviceWatcherStatus.Started:
                case DeviceWatcherStatus.EnumerationCompleted:
                    restartPending = false;
                    return false;
                case DeviceWatcherStatus.Stopping:
                    // Start again once the Stopped event arrives.
                    restartPending = true;
                    return false;
                default:
                    restartPending = false;
                    return true;
            }
        }
    }

    /// <summary>
    /// Records that the watcher should no longer run, cancelling a deferred start.
    /// </summary>
    public void RequestStop()
    {
        lock (gate)
        {
            startWanted = false;
            restartPending = false;
        }
    }

    /// <summary>
    /// Called from the watcher's Stopped event.
    /// </summary>
    /// <returns><see langword="true"/> when a start was deferred while the watcher was stopping and is still wanted.</returns>
    public bool OnStopped()
    {
        lock (gate)
        {
            if (!restartPending)
            {
                return false;
            }

            restartPending = false;
            return startWanted;
        }
    }
}