namespace Atc.Wpf.Hardware.Services;

public sealed class NetworkAdapterService : INetworkAdapterService
{
    private readonly Func<IReadOnlyList<NetworkAdapterSnapshot>> enumerate;
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public NetworkAdapterService()
        : this(EnumerateAdapters)
    {
    }

    internal NetworkAdapterService(
        Func<IReadOnlyList<NetworkAdapterSnapshot>> enumerate)
    {
        this.enumerate = enumerate;
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Adapter property access can throw on transient device state changes.")]
    private static IReadOnlyList<NetworkAdapterSnapshot> EnumerateAdapters()
    {
        var snapshots = new List<NetworkAdapterSnapshot>();

        foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            var mac = string.Empty;
            long? speed = null;
            var status = System.Net.NetworkInformation.OperationalStatus.Unknown;

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

            try
            {
                status = ni.OperationalStatus;
            }
            catch (Exception)
            {
                // Status read can throw transiently.
            }

            snapshots.Add(new NetworkAdapterSnapshot(
                ni.Id,
                ni.Name,
                ni.Description,
                ni.NetworkInterfaceType,
                mac,
                speed,
                status));
        }

        return snapshots;
    }

    private void EnumerateAndSync()
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in enumerate())
        {
            if (!IncludeLoopback &&
                snapshot.AdapterType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
            {
                continue;
            }

            foundIds.Add(snapshot.Id);
            Upsert(snapshot);
        }

        for (var i = Adapters.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Adapters[i].DeviceId))
            {
                Adapters[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(NetworkAdapterSnapshot snapshot)
    {
        var existing = FindByDeviceId(snapshot.Id);

        if (existing is not null)
        {
            existing.OperationalStatus = snapshot.OperationalStatus;

            // Link speed (e.g. Wi-Fi) and the user-chosen adapter name can change while the adapter stays present.
            existing.Speed = snapshot.Speed;
            existing.Name = snapshot.Name;

            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        var info = new NetworkAdapterInfo(
            adapterId: snapshot.Id,
            name: snapshot.Name,
            description: snapshot.Description,
            adapterType: snapshot.AdapterType,
            macAddress: snapshot.MacAddress,
            speed: snapshot.Speed,
            isLoopback: snapshot.AdapterType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
        {
            OperationalStatus = snapshot.OperationalStatus,
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