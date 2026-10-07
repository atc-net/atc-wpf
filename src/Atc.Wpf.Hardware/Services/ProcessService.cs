namespace Atc.Wpf.Hardware.Services;

public sealed class ProcessService : IProcessService
{
    private readonly Func<IReadOnlyList<ProcessSnapshot>> enumerate;
    private readonly Func<int, string?> resolveModulePath;
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public ProcessService()
        : this(EnumerateProcesses, ResolveModulePath)
    {
    }

    internal ProcessService(
        Func<IReadOnlyList<ProcessSnapshot>> enumerate,
        Func<int, string?> resolveModulePath)
    {
        this.enumerate = enumerate;
        this.resolveModulePath = resolveModulePath;
        Processes = new ObservableCollection<RunningProcessInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<RunningProcessInfo> Processes { get; }

    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    public bool OnlyWithMainWindow { get; set; } = true;

    public void StartWatching()
    {
        if (started)
        {
            return;
        }

        started = true;
        pollTimer.Start();
    }

    public void StopWatching()
    {
        if (!started)
        {
            return;
        }

        started = false;
        pollTimer.Stop();
    }

    public Task RefreshAsync()
    {
        EnumerateAndSync();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        pollTimer.Tick -= OnPollTick;
        pollTimer.Stop();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient process enumeration errors.")]
    private void OnPollTick(
        object? sender,
        EventArgs e)
    {
        try
        {
            EnumerateAndSync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ProcessService poll failed: {ex.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process metadata access can throw for protected processes.")]
    private static IReadOnlyList<ProcessSnapshot> EnumerateProcesses()
    {
        var snapshots = new List<ProcessSnapshot>();

        foreach (var p in System.Diagnostics.Process.GetProcesses())
        {
            try
            {
                var hasWindow = p.MainWindowHandle != IntPtr.Zero;
                snapshots.Add(new ProcessSnapshot(
                    p.Id,
                    p.ProcessName,
                    hasWindow ? p.MainWindowTitle : string.Empty,
                    hasWindow));
            }
            catch (Exception)
            {
                // Some processes (system / protected) deny metadata access.
            }
            finally
            {
                p.Dispose();
            }
        }

        return snapshots;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Module access can throw for protected processes.")]
    private static string? ResolveModulePath(int processId)
    {
        try
        {
            using var process = System.Diagnostics.Process.GetProcessById(processId);
            return process.MainModule?.FileName;
        }
        catch (Exception)
        {
            // Access denied or already exited — leave path null.
            return null;
        }
    }

    private void EnumerateAndSync()
    {
        var foundIds = new HashSet<int>();

        foreach (var snapshot in enumerate())
        {
            if (OnlyWithMainWindow && !snapshot.HasMainWindow)
            {
                continue;
            }

            foundIds.Add(snapshot.ProcessId);
            Upsert(snapshot);
        }

        for (var i = Processes.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Processes[i].ProcessId))
            {
                Processes[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(ProcessSnapshot snapshot)
    {
        var existing = FindByProcessId(snapshot.ProcessId);

        if (existing is not null &&
            string.Equals(existing.ProcessName, snapshot.ProcessName, StringComparison.OrdinalIgnoreCase))
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            existing.MainWindowTitle = snapshot.MainWindowTitle;
            return;
        }

        var info = new RunningProcessInfo(
            processId: snapshot.ProcessId,
            processName: snapshot.ProcessName,
            mainWindowTitle: snapshot.MainWindowTitle,
            mainModulePath: resolveModulePath(snapshot.ProcessId))
        {
            State = DeviceState.Available,
        };

        Processes.Add(info);

        // The PID was reused by another process: the old process is gone.
        if (existing is not null)
        {
            RemoveStale(existing);
        }
    }

    /// <summary>
    /// Retires an entry whose id now belongs to something else. Called after the fresh entry has been added,
    /// so a picker that still has the stale entry selected sees it disconnect and leave - and does not
    /// auto-rebind to the newcomer just because it reuses the same id.
    /// </summary>
    private void RemoveStale(RunningProcessInfo stale)
    {
        stale.State = DeviceState.Disconnected;
        Processes.Remove(stale);
    }

    private RunningProcessInfo? FindByProcessId(int processId)
    {
        for (var i = 0; i < Processes.Count; i++)
        {
            if (Processes[i].ProcessId == processId)
            {
                return Processes[i];
            }
        }

        return null;
    }
}