namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class WindowServiceTests
{
    private static readonly IntPtr Handle = new(0x1234);

    private readonly Dictionary<int, string> processNames = new()
    {
        [10] = "notepad",
        [20] = "chrome",
    };

    private IReadOnlyList<WindowSnapshot> snapshots = [];

    [StaFact]
    public async Task RefreshAsync_WindowClosed_MarksEntryDisconnected()
    {
        using var service = CreateService();
        snapshots = [new WindowSnapshot(Handle, "a.txt - Notepad", "Notepad", ProcessId: 10)];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();

        Assert.Single(service.Windows);
        Assert.Equal(DeviceState.Disconnected, service.Windows[0].State);
    }

    [StaFact]
    public async Task RefreshAsync_HandleReusedByAnotherProcess_ReplacesTheStaleEntry()
    {
        using var service = CreateService();
        snapshots = [new WindowSnapshot(Handle, "a.txt - Notepad", "Notepad", ProcessId: 10)];
        await service.RefreshAsync();
        var stale = service.Windows[0];

        snapshots = [new WindowSnapshot(Handle, "Google", "Chrome_WidgetWin_1", ProcessId: 20)];
        await service.RefreshAsync();

        var current = Assert.Single(service.Windows);
        Assert.NotSame(stale, current);
        Assert.Equal("chrome", current.ProcessName);
        Assert.Equal("Google", current.Title);
        Assert.Equal(DeviceState.Available, current.State);
        Assert.Equal(DeviceState.Disconnected, stale.State);
    }

    [StaFact]
    public async Task RefreshAsync_SameWindowWithNewTitle_UpdatesTheEntryInPlace()
    {
        using var service = CreateService();
        snapshots = [new WindowSnapshot(Handle, "a.txt - Notepad", "Notepad", ProcessId: 10)];
        await service.RefreshAsync();
        var entry = service.Windows[0];
        var changed = new List<string>();
        entry.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        snapshots = [new WindowSnapshot(Handle, "*a.txt - Notepad", "Notepad", ProcessId: 10)];
        await service.RefreshAsync();

        Assert.Same(entry, Assert.Single(service.Windows));
        Assert.Equal("*a.txt - Notepad", entry.Title);
        Assert.Contains(nameof(TopLevelWindowInfo.Title), changed, StringComparer.Ordinal);
        Assert.Contains(nameof(TopLevelWindowInfo.FriendlyName), changed, StringComparer.Ordinal);
    }

    private WindowService CreateService()
        => new(
            enumerate: _ => snapshots,
            resolveProcessName: pid => processNames[pid]);
}