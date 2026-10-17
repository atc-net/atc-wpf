namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Shared plumbing that lets the live preview controls (camera, microphone meter, speaker tester) stop their
/// device without blocking. <c>Dispose</c> usually runs on the UI thread, and the stop awaits work that resumes
/// on that thread, so waiting for it there would deadlock.
/// </summary>
internal static class LiveDeviceTeardown
{
    /// <summary>
    /// Starts <paramref name="teardownOnUiThread"/> on the dispatcher's thread and returns without waiting for
    /// it. When the dispatcher has already shut down, <paramref name="teardownWithoutUiThread"/> runs instead
    /// on the calling thread. The returned task never faults.
    /// </summary>
    public static Task Start(
        Dispatcher dispatcher,
        Func<Task> teardownOnUiThread,
        Func<Task> teardownWithoutUiThread)
    {
        if (dispatcher.CheckAccess())
        {
            return ObserveAsync(teardownOnUiThread);
        }

        if (dispatcher.HasShutdownStarted)
        {
            return ObserveAsync(teardownWithoutUiThread);
        }

        return ObserveAsync(() => dispatcher.InvokeAsync(teardownOnUiThread).Task.Unwrap());
    }

    /// <summary>
    /// Runs <paramref name="action"/> on the dispatcher's thread: inline when already there, otherwise posted
    /// without waiting. Skipped when the dispatcher is shutting down, as there is no UI left to update.
    /// </summary>
    public static void OnUiThread(
        Dispatcher dispatcher,
        Action action)
    {
        if (dispatcher.CheckAccess())
        {
            action();
            return;
        }

        if (!dispatcher.HasShutdownStarted)
        {
            _ = dispatcher.BeginInvoke(action);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A fire-and-forget teardown must not surface as an unobserved task exception.")]
    private static async Task ObserveAsync(Func<Task> teardown)
    {
        try
        {
            await teardown().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Best-effort cleanup; the device may already be gone or the dispatcher shut down mid-way.
        }
    }
}