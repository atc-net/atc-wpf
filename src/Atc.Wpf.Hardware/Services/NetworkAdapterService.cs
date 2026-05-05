namespace Atc.Wpf.Hardware.Services;

public sealed class NetworkAdapterService : INetworkAdapterService
{
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public NetworkAdapterService()
    {
        Adapters = new ObservableCollection<NetworkAdapterInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<NetworkAdapterInfo> Adapters { get; }

    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    public bool IncludeLoopback { get; set; }

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient enumeration errors.")]
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
            Debug.WriteLine($"NetworkAdapterService poll failed: {ex.Message}");
        }
    }

    private void EnumerateAndSync()
    {
        var found = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var ni in found)
        {
            if (!IncludeLoopback &&
                ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
            {
                continue;
            }

            foundIds.Add(ni.Id);
            UpsertFromAdapter(ni);
        }

        for (var i = Adapters.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Adapters[i].DeviceId))
            {
                Adapters[i].State = DeviceState.Disconnected;
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Adapter property access can throw on transient device state changes.")]
    private void UpsertFromAdapter(
        System.Net.NetworkInformation.NetworkInterface ni)
    {
        var existing = FindByDeviceId(ni.Id);

        if (existing is not null)
        {
            try
            {
                existing.OperationalStatus = ni.OperationalStatus;
            }
            catch (Exception)
            {
                // Status read can throw transiently; skip this tick.
            }

            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        string mac = string.Empty;
        long? speed = null;

        try
        {
            mac = ni.GetPhysicalAddress().ToString();
        }
        catch (Exception)
        {
            // Some adapters don't expose a MAC.
        }

        try
        {
            speed = ni.Speed;
        }
        catch (Exception)
        {
            // Speed read can fail on virtual adapters.
        }

        var info = new NetworkAdapterInfo(
            adapterId: ni.Id,
            name: ni.Name,
            description: ni.Description,
            adapterType: ni.NetworkInterfaceType,
            macAddress: mac,
            speed: speed,
            isLoopback: ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
        {
            OperationalStatus = ni.OperationalStatus,
            State = DeviceState.Available,
        };

        Adapters.Add(info);
    }

    private NetworkAdapterInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Adapters.Count; i++)
        {
            if (string.Equals(Adapters[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Adapters[i];
            }
        }

        return null;
    }
}