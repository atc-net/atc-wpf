namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class ProcessServiceTests
{
    private IReadOnlyList<ProcessSnapshot> snapshots = [];

    [WpfFact]
    public async Task RefreshAsync_ProcessExited_MarksEntryDisconnected()
    {
        using var service = CreateService();
        snapshots = [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();

        Assert.Single(service.Processes);
        Assert.Equal(DeviceState.Disconnected, service.Processes[0].State);
    }

    [WpfFact]
    public async Task RefreshAsync_PidReusedByAnotherProcess_ReplacesTheStaleEntry()
    {
        using var service = CreateService();
        snapshots = [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
        await service.RefreshAsync();
        var stale = service.Processes[0];

        snapshots = [new ProcessSnapshot(100, "chrome", "Google", HasMainWindow: true)];
        await service.RefreshAsync();

        var current = Assert.Single(service.Processes);
        Assert.NotSame(stale, current);
        Assert.Equal("chrome", current.ProcessName);
        Assert.Equal("Google", current.MainWindowTitle);
        Assert.Equal(DeviceState.Available, current.State);
        Assert.Equal(DeviceState.Disconnected, stale.State);
    }

    [WpfFact]
    public async Task RefreshAsync_SameProcessWithNewWindowTitle_UpdatesTheEntryInPlace()
    {
        using var service = CreateService();
        snapshots = [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
        await service.RefreshAsync();
        var entry = service.Processes[0];
        var changed = new List<string>();
        entry.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        snapshots = [new ProcessSnapshot(100, "notepad", "b.txt", HasMainWindow: true)];
        await service.RefreshAsync();

        Assert.Same(entry, Assert.Single(service.Processes));
        Assert.Equal("b.txt", entry.MainWindowTitle);
        Assert.Contains(nameof(RunningProcessInfo.MainWindowTitle), changed, StringComparer.Ordinal);
        Assert.Contains(nameof(RunningProcessInfo.FriendlyName), changed, StringComparer.Ordinal);
    }

    [WpfFact]
    public async Task RefreshAsync_EnumeratesAndResolvesOffTheUiThread_AndAppliesOnIt()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        int? enumerateThreadId = null;
        int? resolveThreadId = null;
        int? collectionChangedThreadId = null;
        using var service = new ProcessService(
            enumerate: () =>
            {
                enumerateThreadId = Environment.CurrentManagedThreadId;
                return [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
            },
            resolveModulePath: _ =>
            {
                resolveThreadId = Environment.CurrentManagedThreadId;
                return "notepad-module-path";
            });
        service.Processes.CollectionChanged += (_, _) => collectionChangedThreadId = Environment.CurrentManagedThreadId;

        await service.RefreshAsync();

        Assert.NotEqual(uiThreadId, enumerateThreadId);
        Assert.NotEqual(uiThreadId, resolveThreadId);
        Assert.Equal(uiThreadId, collectionChangedThreadId);
        Assert.Equal("notepad-module-path", Assert.Single(service.Processes).MainModulePath);
    }

    [WpfFact]
    public async Task RefreshAsync_WhileAPollIsInFlight_SharesThatPoll()
    {
        using var gate = new ManualResetEventSlim();
        var enumerateCalls = 0;
        using var service = new ProcessService(
            enumerate: () =>
            {
                Interlocked.Increment(ref enumerateCalls);
                gate.Wait(TimeSpan.FromSeconds(5));
                return [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
            },
            resolveModulePath: _ => null);

        var first = service.RefreshAsync();
        var second = service.RefreshAsync();
        gate.Set();
        await Task.WhenAll(first, second);

        Assert.Equal(1, enumerateCalls);
        Assert.Single(service.Processes);
    }

    private ProcessService CreateService()
        => new(
            enumerate: () => snapshots,
            resolveModulePath: _ => null);
}