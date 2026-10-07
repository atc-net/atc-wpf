namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class DisplayServiceTests
{
    private static readonly IntPtr Monitor = new(0x10001);
    private static readonly Rect FullHd = new(0, 0, 1920, 1080);
    private static readonly Rect FullHdWork = new(0, 0, 1920, 1040);
    private static readonly Rect Qhd = new(0, 0, 2560, 1440);
    private static readonly Rect QhdWork = new(0, 0, 2560, 1400);

    private IReadOnlyList<DisplaySnapshot> snapshots = [];

    [StaFact]
    public async Task RefreshAsync_NewDisplay_IsAddedAsAvailable()
    {
        using var service = CreateService();
        snapshots = [new DisplaySnapshot(Monitor, @"\\.\DISPLAY1", FullHd, FullHdWork, IsPrimary: true)];

        await service.RefreshAsync();

        var display = Assert.Single(service.Displays);
        Assert.Equal(DeviceState.Available, display.State);
        Assert.Equal(FullHd, display.Bounds);
        Assert.True(display.IsPrimary);
    }

    [StaFact]
    public async Task RefreshAsync_DisplayUnpluggedAndPluggedBackIn_GoesDisconnectedThenAvailable()
    {
        using var service = CreateService();
        snapshots = [new DisplaySnapshot(Monitor, @"\\.\DISPLAY1", FullHd, FullHdWork, IsPrimary: true)];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();
        var whileUnplugged = service.Displays[0].State;
        snapshots = [new DisplaySnapshot(Monitor, @"\\.\DISPLAY1", FullHd, FullHdWork, IsPrimary: true)];
        await service.RefreshAsync();

        Assert.Equal(DeviceState.Disconnected, whileUnplugged);
        Assert.Equal(DeviceState.Available, Assert.Single(service.Displays).State);
    }

    [StaFact]
    public async Task RefreshAsync_ResolutionAndPrimaryChanged_UpdatesTheDisplayInPlace()
    {
        using var service = CreateService();
        snapshots = [new DisplaySnapshot(Monitor, @"\\.\DISPLAY1", FullHd, FullHdWork, IsPrimary: true)];
        await service.RefreshAsync();
        var display = service.Displays[0];

        snapshots = [new DisplaySnapshot(Monitor, @"\\.\DISPLAY1", Qhd, QhdWork, IsPrimary: false)];
        await service.RefreshAsync();

        Assert.Same(display, Assert.Single(service.Displays));
        Assert.Equal(Qhd, display.Bounds);
        Assert.Equal(QhdWork, display.WorkingArea);
        Assert.False(display.IsPrimary);
    }

    private DisplayService CreateService()
        => new(enumerate: () => snapshots);
}