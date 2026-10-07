namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class NetworkAdapterServiceTests
{
    private const long OneGigabit = 1_000_000_000;
    private const long TwoPointFiveGigabit = 2_500_000_000;

    private IReadOnlyList<NetworkAdapterSnapshot> snapshots = [];

    [StaFact]
    public async Task RefreshAsync_LoopbackAdapter_IsListedOnlyWhenIncluded()
    {
        using var service = CreateService();
        snapshots = [Ethernet(OneGigabit, "Ethernet", OperationalStatus.Up), Loopback()];

        await service.RefreshAsync();
        var withoutLoopback = service.Adapters.Count;
        service.IncludeLoopback = true;
        await service.RefreshAsync();

        Assert.Equal(1, withoutLoopback);
        Assert.Equal(2, service.Adapters.Count);
        Assert.Contains(service.Adapters, a => a.IsLoopback);
    }

    [StaFact]
    public async Task RefreshAsync_AdapterRemoved_IsMarkedDisconnected()
    {
        using var service = CreateService();
        snapshots = [Ethernet(OneGigabit, "Ethernet", OperationalStatus.Up)];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();

        Assert.Equal(DeviceState.Disconnected, Assert.Single(service.Adapters).State);
    }

    [StaFact]
    public async Task RefreshAsync_OperationalStatusChanged_UpdatesTheAdapterInPlace()
    {
        using var service = CreateService();
        snapshots = [Ethernet(OneGigabit, "Ethernet", OperationalStatus.Up)];
        await service.RefreshAsync();
        var adapter = service.Adapters[0];

        snapshots = [Ethernet(OneGigabit, "Ethernet", OperationalStatus.Down)];
        await service.RefreshAsync();

        Assert.Same(adapter, Assert.Single(service.Adapters));
        Assert.Equal(OperationalStatus.Down, adapter.OperationalStatus);
    }

    [StaFact]
    public async Task RefreshAsync_LinkSpeedAndNameChanged_UpdatesTheAdapterInPlace()
    {
        using var service = CreateService();
        snapshots = [Ethernet(OneGigabit, "Ethernet", OperationalStatus.Up)];
        await service.RefreshAsync();
        var adapter = service.Adapters[0];

        snapshots = [Ethernet(TwoPointFiveGigabit, "Office LAN", OperationalStatus.Up)];
        await service.RefreshAsync();

        Assert.Same(adapter, Assert.Single(service.Adapters));
        Assert.Equal(TwoPointFiveGigabit, adapter.Speed);
        Assert.Equal("Office LAN", adapter.Name);
    }

    private static NetworkAdapterSnapshot Ethernet(
        long speed,
        string name,
        OperationalStatus status)
        => new("{ETH-1}", name, "Intel(R) Ethernet", NetworkInterfaceType.Ethernet, "001122334455", speed, status);

    private static NetworkAdapterSnapshot Loopback()
        => new("{LOOP}", "Loopback", "Software Loopback Interface 1", NetworkInterfaceType.Loopback, string.Empty, null, OperationalStatus.Up);

    private NetworkAdapterService CreateService()
        => new(enumerate: () => snapshots);
}