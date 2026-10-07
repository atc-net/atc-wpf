namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class DriveServiceTests
{
    private readonly List<string> volumeReads = [];
    private IReadOnlyList<DriveSnapshot> snapshots = [];

    [WpfFact]
    public async Task RefreshAsync_DriveRemoved_MarksEntryDisconnected()
    {
        using var service = CreateService();
        snapshots = [StickA(freeSpace: 500)];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();

        Assert.Single(service.Drives);
        Assert.Equal(DeviceState.Disconnected, service.Drives[0].State);
    }

    [WpfFact]
    public async Task RefreshAsync_OtherVolumeOnTheSameDriveLetter_ReplacesTheStaleEntry()
    {
        using var service = CreateService();
        snapshots = [StickA(freeSpace: 500)];
        await service.RefreshAsync();
        var stale = service.Drives[0];

        snapshots = [new DriveSnapshot(@"E:\", "STICK_B", DriveType.Removable, IsReady: true, TotalSize: 64_000, AvailableFreeSpace: 10_000)];
        await service.RefreshAsync();

        var current = Assert.Single(service.Drives);
        Assert.NotSame(stale, current);
        Assert.Equal("STICK_B", current.FriendlyName);
        Assert.Equal(64_000, current.TotalSize);
        Assert.Equal(DeviceState.Disconnected, stale.State);
    }

    [WpfFact]
    public async Task RefreshAsync_SameVolumeWithLessFreeSpace_UpdatesTheEntryInPlace()
    {
        using var service = CreateService();
        snapshots = [StickA(freeSpace: 500)];
        await service.RefreshAsync();
        var entry = service.Drives[0];

        snapshots = [StickA(freeSpace: 200)];
        await service.RefreshAsync();

        Assert.Same(entry, Assert.Single(service.Drives));
        Assert.Equal(200, entry.AvailableFreeSpace);
        Assert.Equal(DeviceState.Available, entry.State);
    }

    [WpfTheory]
    [InlineData(DriveType.Network)]
    [InlineData(DriveType.CDRom)]
    public async Task RefreshAsync_KnownSlowDrive_IsNotReReadOnEveryPoll(
        DriveType driveType)
    {
        // Volume metadata of an offline network share or a spinning-up optical drive can block for seconds;
        // it is read once, when the drive appears.
        using var service = CreateService();
        snapshots = [new DriveSnapshot(@"Z:\", "volume", driveType, IsReady: true, TotalSize: 1_000, AvailableFreeSpace: 900)];

        await service.RefreshAsync();
        await service.RefreshAsync();
        await service.RefreshAsync();

        Assert.Equal([@"Z:\"], volumeReads);
    }

    private static DriveSnapshot StickA(long freeSpace)
        => new(@"E:\", "STICK_A", DriveType.Removable, IsReady: true, TotalSize: 32_000, AvailableFreeSpace: freeSpace);

    private DriveService CreateService()
        => new(
            listDrives: () => snapshots.Select(s => (s.Name, s.DriveType)).ToList(),
            readVolume: (name, _) =>
            {
                volumeReads.Add(name);
                return snapshots.Single(s => s.Name == name);
            });
}