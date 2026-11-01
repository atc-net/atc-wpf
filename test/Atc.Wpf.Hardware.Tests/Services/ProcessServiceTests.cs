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
    public async Task RefreshAsync_ExitedProcesses_AreRemovedOnceTheyHaveBeenShownDisconnectedForOnePoll()
    {
        using var service = CreateService();
        snapshots =
        [
            new ProcessSnapshot(1, "a", "A", HasMainWindow: true),
            new ProcessSnapshot(2, "b", "B", HasMainWindow: true),
            new ProcessSnapshot(3, "c", "C", HasMainWindow: true),
        ];
        await service.RefreshAsync();

        snapshots =
        [
            new ProcessSnapshot(4, "d", "D", HasMainWindow: true),
            new ProcessSnapshot(5, "e", "E", HasMainWindow: true),
        ];
        await service.RefreshAsync();

        Assert.Equal(5, service.Processes.Count);
        Assert.Equal(3, service.Processes.Count(p => p.State is DeviceState.Disconnected));

        await service.RefreshAsync();

        Assert.Equal([4, 5], service.Processes.Select(p => p.ProcessId).Order());
    }

    [WpfFact]
    public async Task RefreshAsync_ManyProcessesComeAndGo_TheListDoesNotGrow()
    {
        using var service = CreateService();

        for (var generation = 0; generation < 50; generation++)
        {
            snapshots = Generation(generation * 100, 100);
            await service.RefreshAsync();
        }

        Assert.True(service.Processes.Count <= 200, $"{service.Processes.Count} entries are kept");
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

    private static ProcessSnapshot[] Generation(
        int firstProcessId,
        int count)
    {
        var generation = new ProcessSnapshot[count];
        for (var i = 0; i < count; i++)
        {
            var pid = firstProcessId + i;
            generation[i] = new ProcessSnapshot(pid, "p" + pid.ToString(CultureInfo.InvariantCulture), string.Empty, HasMainWindow: true);
        }

        return generation;
    }

    private ProcessService CreateService()
        => new(
            enumerate: () => snapshots,
            resolveModulePath: _ => null);
}