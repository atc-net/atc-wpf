namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class JustConnectedTimerTests
{
    private static readonly TimeSpan ShortDuration = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan WellPastDuration = TimeSpan.FromMilliseconds(400);

    [StaFact]
    public void Elapsed_WhileStillJustConnected_TransitionsToAvailable()
    {
        var state = DeviceState.JustConnected;

        JustConnectedTimer.TransitionToAvailableAfter(() => state, s => state = s, ShortDuration);
        DispatcherPump.For(WellPastDuration);

        Assert.Equal(DeviceState.Available, state);
    }

    [StaFact]
    public void Elapsed_AfterDeviceWasUnplugged_StaysDisconnected()
    {
        var state = DeviceState.JustConnected;

        JustConnectedTimer.TransitionToAvailableAfter(() => state, s => state = s, ShortDuration);
        state = DeviceState.Disconnected;
        DispatcherPump.For(WellPastDuration);

        Assert.Equal(DeviceState.Disconnected, state);
    }

    [StaFact]
    public void BeforeElapsed_StaysJustConnected()
    {
        var state = DeviceState.JustConnected;

        JustConnectedTimer.TransitionToAvailableAfter(() => state, s => state = s, TimeSpan.FromSeconds(30));
        DispatcherPump.For(ShortDuration);

        Assert.Equal(DeviceState.JustConnected, state);
    }
}