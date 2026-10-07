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