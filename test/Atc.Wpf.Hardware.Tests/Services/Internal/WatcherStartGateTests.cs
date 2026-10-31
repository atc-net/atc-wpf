namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class WatcherStartGateTests
{
    [Theory]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.Created, true)]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.Stopped, true)]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.Aborted, true)]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.Started, false)]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.EnumerationCompleted, false)]
    [InlineData(Windows.Devices.Enumeration.DeviceWatcherStatus.Stopping, false)]
    public void RequestStart_StartsNowOnlyFromAStartableStatus(
        Windows.Devices.Enumeration.DeviceWatcherStatus status,
        bool expected)
    {
        var sut = new WatcherStartGate();

        Assert.Equal(expected, sut.RequestStart(() => status));
    }

    [Fact]
    public void RequestStart_WhileStopping_RestartsWhenStopped()
    {
        var sut = new WatcherStartGate();

        sut.RequestStart(() => Windows.Devices.Enumeration.DeviceWatcherStatus.Stopping);

        Assert.True(sut.OnStopped());
        Assert.False(sut.OnStopped());
    }

    [Fact]
    public void RequestStart_WhileStopping_ThenRequestStop_DoesNotRestart()
    {
        var sut = new WatcherStartGate();

        sut.RequestStart(() => Windows.Devices.Enumeration.DeviceWatcherStatus.Stopping);
        sut.RequestStop();

        Assert.False(sut.OnStopped());
        Assert.False(sut.IsStartWanted);
    }

    [Fact]
    public void StoppedEventRaisedWhileTheStatusIsBeingRead_TheRestartIsNotLost()
    {
        var sut = new WatcherStartGate();
        var stoppedResult = false;
        Thread? stoppedThread = null;

        sut.RequestStart(() =>
        {
            // The Stopped event arrives on a WinRT thread right after the status was read as Stopping.
            stoppedThread = new Thread(() => stoppedResult = sut.OnStopped());
            stoppedThread.Start();
            Assert.False(stoppedThread.Join(TimeSpan.FromMilliseconds(100)));
            return Windows.Devices.Enumeration.DeviceWatcherStatus.Stopping;
        });

        Assert.True(stoppedThread!.Join(TimeSpan.FromSeconds(10)));
        Assert.True(stoppedResult);
    }
}