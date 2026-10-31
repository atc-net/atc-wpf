namespace Atc.Wpf.Components.Tests.TestSupport;

/// <summary>
/// Runs the current thread's dispatcher until a condition holds, for work that a background loop posts to it.
/// </summary>
internal static class DispatcherPump
{
    public static void Until(
        Func<bool> condition,
        TimeSpan? timeout = null)
    {
        var limit = timeout ?? TimeSpan.FromSeconds(10);
        var stopwatch = Stopwatch.StartNew();
        while (!condition() && stopwatch.Elapsed < limit)
        {
            var frame = new DispatcherFrame();
            _ = Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                () => frame.Continue = false);
            Dispatcher.PushFrame(frame);
            Thread.Sleep(10);
        }
    }
}