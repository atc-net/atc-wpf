namespace Atc.Wpf.Hardware.Tests.TestSupport;

/// <summary>
/// Runs the current thread's dispatcher loop for a fixed time so that
/// <see cref="DispatcherTimer"/> ticks are delivered inside an STA test.
/// </summary>
internal static class DispatcherPump
{
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