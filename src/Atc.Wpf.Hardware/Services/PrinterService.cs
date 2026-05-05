namespace Atc.Wpf.Hardware.Services;

public sealed class PrinterService : IPrinterService
{
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public PrinterService()
    {
        Printers = new ObservableCollection<PrinterInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<PrinterInfo> Printers { get; }

    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient print spooler errors.")]
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
            Debug.WriteLine($"PrinterService poll failed: {ex.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "PrintQueue access can throw transiently.")]
    private void EnumerateAndSync()
    {
        using var server = new System.Printing.LocalPrintServer();

        string? defaultName = null;
        try
        {
            defaultName = server.DefaultPrintQueue?.FullName;
        }
        catch (Exception)
        {
            // No default printer configured.
        }

        var queues = server.GetPrintQueues(new[]
        {
            System.Printing.EnumeratedPrintQueueTypes.Local,
            System.Printing.EnumeratedPrintQueueTypes.Connections,
        });

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var queue in queues)
        {
            try
            {
                foundIds.Add(queue.FullName);
                UpsertFromQueue(queue, defaultName);
            }
            catch (Exception)
            {
                // Skip queues that can't be inspected.
            }
            finally
            {
                queue.Dispose();
            }
        }

        for (var i = Printers.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Printers[i].DeviceId))
            {
                Printers[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void UpsertFromQueue(
        System.Printing.PrintQueue queue,
        string? defaultName)
    {
        var existing = FindByDeviceId(queue.FullName);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        var isDefault = !string.IsNullOrEmpty(defaultName) &&
            string.Equals(queue.FullName, defaultName, StringComparison.OrdinalIgnoreCase);

        var info = new PrinterInfo(
            name: queue.Name,
            fullName: queue.FullName,
            isLocal: !queue.IsShared || queue.HostingPrintServer.Name is null,
            isShared: queue.IsShared,
            isDefault: isDefault,
            queueStatus: queue.QueueStatus.ToString())
        {
            State = DeviceState.Available,
        };

        Printers.Add(info);
    }

    private PrinterInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Printers.Count; i++)
        {
            if (string.Equals(Printers[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Printers[i];
            }
        }

        return null;
    }
}