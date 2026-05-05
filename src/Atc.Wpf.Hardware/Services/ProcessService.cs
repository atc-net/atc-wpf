namespace Atc.Wpf.Hardware.Services;

public sealed class ProcessService : IProcessService
{
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public ProcessService()
    {
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
    private void EnumerateAndSync()
    {
        var processes = System.Diagnostics.Process.GetProcesses();
        var foundIds = new HashSet<int>();

        foreach (var p in processes)
        {
            try
            {
                var hasWindow = p.MainWindowHandle != IntPtr.Zero;
                if (OnlyWithMainWindow && !hasWindow)
                {
                    continue;
                }

                foundIds.Add(p.Id);
                UpsertFromProcess(p);
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

        for (var i = Processes.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Processes[i].ProcessId))
            {
                Processes[i].State = DeviceState.Disconnected;
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Module access can throw for protected processes.")]
    private void UpsertFromProcess(System.Diagnostics.Process process)
    {
        var existing = FindByProcessId(process.Id);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        string? modulePath = null;
        try
        {
            modulePath = process.MainModule?.FileName;
        }
        catch (Exception)
        {
            // Access denied — leave path null.
        }

        var info = new RunningProcessInfo(
            processId: process.Id,
            processName: process.ProcessName,
            mainWindowTitle: process.MainWindowTitle,
            mainModulePath: modulePath)
        {
            State = DeviceState.Available,
        };

        Processes.Add(info);
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