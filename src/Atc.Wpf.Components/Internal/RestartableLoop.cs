namespace Atc.Wpf.Components.Internal;

/// <summary>
/// A background loop that can be stopped and started again, each run with its own cancellation token.
/// </summary>
/// <remarks>
/// A control starts it when loaded and stops it when unloaded, so a control that leaves the visual tree
/// without being disposed leaves no loop running.
/// </remarks>
internal sealed class RestartableLoop
{
    private readonly Func<CancellationToken, Task> body;
    private CancellationTokenSource? cts;
    private Task? task;

    public RestartableLoop(Func<CancellationToken, Task> body)
    {
        ArgumentNullException.ThrowIfNull(body);
        this.body = body;
    }

    public bool IsRunning
        => task is { IsCompleted: false };

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        var source = new CancellationTokenSource();
        cts = source;
        task = Task.Run(() => body(source.Token), source.Token);
    }

    public void Stop()
    {
        var source = cts;
        var running = task;
        cts = null;
        task = null;
        if (source is null)
        {
            return;
        }

        source.Cancel();

        // Dispose the source only after the loop has actually exited, so a loop that is in the middle of
        // a dispatcher call is not blocked by (or racing with) the disposal.
        if (running is null)
        {
            source.Dispose();
            return;
        }

        _ = running.ContinueWith(
            static (_, state) => ((CancellationTokenSource)state!).Dispose(),
            source,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }
}