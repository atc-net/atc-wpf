namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Keeps a live device session (camera preview, microphone meter, ...) in line with what the control wants.
/// Start and stop are asynchronous, so the desired device can change - or the control can unload - while a
/// start is still in flight. Instead of dropping such requests, they are coalesced and the desired state is
/// re-evaluated once the in-flight start has completed, so a session is never left running for a control
/// that no longer wants it.
/// </summary>
/// <remarks>
/// Not thread-safe; call it from the control's dispatcher thread.
/// </remarks>
internal sealed class DeviceSessionCoordinator
{
    private readonly Func<string, Task> startAsync;
    private readonly Func<Task> stopAsync;
    private readonly Func<string?> getDesiredDeviceId;
    private Task? activeSync;
    private bool resyncRequested;

    /// <param name="startAsync">Starts a session for the given device id.</param>
    /// <param name="stopAsync">Stops the current session, if any.</param>
    /// <param name="getDesiredDeviceId">The device that should be running now, or <see langword="null"/> when nothing should run.</param>
    public DeviceSessionCoordinator(
        Func<string, Task> startAsync,
        Func<Task> stopAsync,
        Func<string?> getDesiredDeviceId)
    {
        this.startAsync = startAsync;
        this.stopAsync = stopAsync;
        this.getDesiredDeviceId = getDesiredDeviceId;
    }

    /// <summary>
    /// Brings the running session in line with <c>getDesiredDeviceId</c>. When a sync is already running,
    /// the request is folded into it and the returned task completes when that sync has finished.
    /// </summary>
    public Task SyncAsync()
    {
        if (activeSync is not null)
        {
            resyncRequested = true;
            return activeSync;
        }

        var sync = RunAsync();
        if (!sync.IsCompleted)
        {
            activeSync = sync;
        }

        return sync;
    }

    private async Task RunAsync()
    {
        try
        {
            do
            {
                resyncRequested = false;

                await stopAsync().ConfigureAwait(true);

                var deviceId = getDesiredDeviceId();
                if (deviceId is null)
                {
                    continue;
                }

                await startAsync(deviceId).ConfigureAwait(true);
            }
            while (resyncRequested);
        }
        finally
        {
            activeSync = null;
        }
    }
}