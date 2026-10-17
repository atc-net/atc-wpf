namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class PollingServiceHostTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [WpfFact]
    public async Task InvokeOnUiAsync_CalledFromTheThreadPool_RunsOnTheUiThread()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        using var host = new PollingServiceHost("Test", () => Task.CompletedTask);
        int? actionThreadId = null;

        await Task.Run(() => host.InvokeOnUiAsync(() => { actionThreadId = Environment.CurrentManagedThreadId; }));
        var result = await Task.Run(() => host.InvokeOnUiAsync(() => Environment.CurrentManagedThreadId));

        Assert.Equal(uiThreadId, actionThreadId);
        Assert.Equal(uiThreadId, result);
    }

    [WpfFact]
    public void InvokeOnUiAsync_OnTheUiThread_RunsInline()
    {
        var host = new PollingServiceHost("Test", () => Task.CompletedTask);
        var ran = false;

        var task = host.InvokeOnUiAsync(() => { ran = true; });
        host.Dispose();

        Assert.True(ran);
        Assert.True(task.IsCompletedSuccessfully);
    }

    [WpfFact]
    public async Task RefreshAsync_RunsThePollOnTheThreadPool()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        int? pollThreadId = null;
        using var host = new PollingServiceHost(
            "Test",
            () =>
            {
                pollThreadId = Environment.CurrentManagedThreadId;
                return Task.CompletedTask;
            });

        await host.RefreshAsync();

        Assert.NotNull(pollThreadId);
        Assert.NotEqual(uiThreadId, pollThreadId);
    }

    [WpfFact]
    public async Task RefreshAsync_WhileAPollIsInFlight_SharesThatPoll()
    {
        var gate = new TaskCompletionSource();
        var polls = 0;
        using var host = new PollingServiceHost(
            "Test",
            () =>
            {
                Interlocked.Increment(ref polls);
                return gate.Task;
            });

        var first = host.RefreshAsync();
        var second = Task.Run(host.RefreshAsync);
        gate.SetResult();
        await Task.WhenAll(first, second);

        Assert.Equal(1, polls);
    }

    [WpfFact]
    public async Task Start_BuiltAndStartedOnTheThreadPool_TicksAndAppliesOnTheUiThread()
    {
        var uiDispatcher = Dispatcher.CurrentDispatcher;
        var applied = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        PollingServiceHost? host = null;

        host = await Task.Run(() =>
        {
            var created = new PollingServiceHost(
                "Test",
                () => host!.InvokeOnUiAsync(() => { applied.TrySetResult(uiDispatcher.CheckAccess()); }),
                uiDispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(20),
            };
            created.Start();
            return created;
        });

        using (host)
        {
            var completed = await Task.WhenAny(applied.Task, Task.Delay(Timeout));

            Assert.Same(applied.Task, completed);
            Assert.True(await applied.Task);
        }
    }

    [WpfFact]
    public async Task Stop_NoFurtherTicks()
    {
        var polls = 0;
        using var host = new PollingServiceHost(
            "Test",
            () =>
            {
                Interlocked.Increment(ref polls);
                return Task.CompletedTask;
            })
        {
            Interval = TimeSpan.FromMilliseconds(20),
        };

        host.Start();
        var deadline = DateTime.UtcNow + Timeout;
        while (Volatile.Read(ref polls) == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }

        host.Stop();
        await Task.Delay(100);
        var afterStop = Volatile.Read(ref polls);
        await Task.Delay(200);

        Assert.True(afterStop > 0);
        Assert.Equal(afterStop, Volatile.Read(ref polls));
    }

    [WpfFact]
    public async Task Dispose_RefreshAsyncNoLongerPolls()
    {
        var polls = 0;
        var host = new PollingServiceHost(
            "Test",
            () =>
            {
                Interlocked.Increment(ref polls);
                return Task.CompletedTask;
            });

        host.Dispose();
        await host.RefreshAsync();

        Assert.Equal(0, polls);
    }

    [WpfFact]
    public void Interval_Negative_Throws()
    {
        using var host = new PollingServiceHost("Test", () => Task.CompletedTask);

        Assert.Throws<ArgumentOutOfRangeException>(() => host.Interval = TimeSpan.FromSeconds(-1));
    }
}