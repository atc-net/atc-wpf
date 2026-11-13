namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class DeviceSessionCoordinatorTests
{
    private readonly List<string> log = [];
    private string? desiredDeviceId;
    private TaskCompletionSource? pendingStart;
    private string? runningDeviceId;

    [Fact]
    public async Task SyncAsync_WhenDeviceDesired_StartsIt()
    {
        var sut = CreateSut();
        desiredDeviceId = "cam-1";

        await sut.SyncAsync();

        Assert.Equal(["stop", "start:cam-1"], log);
        Assert.Equal("cam-1", runningDeviceId);
    }

    [Fact]
    public async Task SyncAsync_WhenNothingDesired_OnlyStops()
    {
        var sut = CreateSut();
        desiredDeviceId = null;

        await sut.SyncAsync();

        Assert.Equal(["stop"], log);
        Assert.Null(runningDeviceId);
    }

    [Fact]
    public async Task SyncAsync_UnloadedWhileStartInFlight_StopsTheDeviceOnceStartCompletes()
    {
        var sut = CreateSut();
        desiredDeviceId = "cam-1";
        pendingStart = new TaskCompletionSource();

        var first = sut.SyncAsync();

        // Control unloads while MediaCapture.InitializeAsync is still running.
        desiredDeviceId = null;
        var second = sut.SyncAsync();

        pendingStart.SetResult();
        await Task.WhenAll(first, second);

        Assert.Equal(["stop", "start:cam-1", "stop"], log);
        Assert.Null(runningDeviceId);
    }

    [Fact]
    public async Task SyncAsync_DeviceChangedWhileStartInFlight_SwitchesToTheNewDevice()
    {
        var sut = CreateSut();
        desiredDeviceId = "cam-1";
        pendingStart = new TaskCompletionSource();

        var first = sut.SyncAsync();

        desiredDeviceId = "cam-2";
        var second = sut.SyncAsync();

        var firstStart = pendingStart;
        pendingStart = null;
        firstStart.SetResult();
        await Task.WhenAll(first, second);

        Assert.Equal(["stop", "start:cam-1", "stop", "start:cam-2"], log);
        Assert.Equal("cam-2", runningDeviceId);
    }

    [Fact]
    public async Task SyncAsync_SeveralRequestsWhileStartInFlight_RunOneMoreCycleWithTheLatestDevice()
    {
        var sut = CreateSut();
        desiredDeviceId = "cam-1";
        pendingStart = new TaskCompletionSource();

        var first = sut.SyncAsync();

        desiredDeviceId = "cam-2";
        var second = sut.SyncAsync();
        desiredDeviceId = "cam-3";
        var third = sut.SyncAsync();

        var firstStart = pendingStart;
        pendingStart = null;
        firstStart.SetResult();
        await Task.WhenAll(first, second, third);

        Assert.Equal(["stop", "start:cam-1", "stop", "start:cam-3"], log);
        Assert.Equal("cam-3", runningDeviceId);
    }

    [Fact]
    public async Task SyncAsync_AfterAnAsynchronousSyncHasCompleted_RunsAgain()
    {
        var sut = CreateSut();
        desiredDeviceId = "cam-1";
        pendingStart = new TaskCompletionSource();

        var first = sut.SyncAsync();
        pendingStart.SetResult();
        await first;

        desiredDeviceId = null;
        await sut.SyncAsync();

        Assert.Equal(["stop", "start:cam-1", "stop"], log);
        Assert.Null(runningDeviceId);
    }

    // Controls start a sync from event handlers without awaiting it; a failure must not surface later as an
    // unobserved task exception (which ends the process when unobserved exceptions are configured to throw).
    [Fact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Finalizes the faulted task so an unobserved exception would be raised.")]
    public async Task Resync_with_a_failing_start_leaves_no_unobserved_exception()
    {
        var failure = new InvalidOperationException("camera gone");
        var unobserved = new List<Exception>();
        void OnUnobserved(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception.InnerExceptions.Contains(failure))
            {
                lock (unobserved)
                {
                    unobserved.Add(e.Exception);
                }
            }
        }

        TaskScheduler.UnobservedTaskException += OnUnobserved;
        try
        {
            var sut = new DeviceSessionCoordinator(
                startAsync: _ => Task.FromException(failure),
                stopAsync: () => Task.CompletedTask,
                getDesiredDeviceId: () => "cam-1");

            await sut.Resync();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Empty(unobserved);
        }
        finally
        {
            TaskScheduler.UnobservedTaskException -= OnUnobserved;
        }
    }

    private DeviceSessionCoordinator CreateSut()
        => new(
            startAsync: async deviceId =>
            {
                log.Add("start:" + deviceId);
                if (pendingStart is not null)
                {
                    await pendingStart.Task;
                }

                // Like MediaCapture: the device only exists once the start has completed.
                runningDeviceId = deviceId;
            },
            stopAsync: () =>
            {
                log.Add("stop");
                runningDeviceId = null;
                return Task.CompletedTask;
            },
            getDesiredDeviceId: () => desiredDeviceId);
}