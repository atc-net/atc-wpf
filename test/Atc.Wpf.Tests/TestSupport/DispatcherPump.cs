namespace Atc.Wpf.Tests.TestSupport;

/// <summary>
/// Runs the current thread's dispatcher loop inside an STA test, so animation clocks tick
/// and continuations posted to the dispatcher run.
/// </summary>
internal static class DispatcherPump
{
    /// <summary>
    /// Pumps until <paramref name="condition"/> holds or <paramref name="timeout"/> has passed.
    /// </summary>
    /// <param name="condition">The condition to wait for.</param>
    /// <param name="timeout">The longest time to pump.</param>
    /// <returns><see langword="true"/> when the condition held before the timeout.</returns>
    public static bool Until(
        Func<bool> condition,
        TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (!condition())
        {
            if (DateTime.UtcNow >= deadline)
            {
                return false;
            }

            For(TimeSpan.FromMilliseconds(10));
        }

        return true;
    }

    /// <summary>
    /// Pumps for a fixed time.
    /// </summary>
    /// <param name="duration">How long to pump.</param>
    public static void For(TimeSpan duration)
    {
        var frame = new DispatcherFrame();
        var stopTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = duration,
        };

        stopTimer.Tick += (_, _) =>
        {
            stopTimer.Stop();
            frame.Continue = false;
        };

        stopTimer.Start();
        Dispatcher.PushFrame(frame);
    }
}