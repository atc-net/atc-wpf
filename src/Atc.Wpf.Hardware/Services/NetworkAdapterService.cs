namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the network adapters and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class NetworkAdapterService : INetworkAdapterService
{
    private readonly Func<IReadOnlyList<NetworkAdapterSnapshot>> enumerate;
    private readonly PollingServiceHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkAdapterService"/> class.
    /// </summary>
    public NetworkAdapterService()
        : this(EnumerateAdapters)
    {
    }

    internal NetworkAdapterService(
        Func<IReadOnlyList<NetworkAdapterSnapshot>> enumerate,
        Dispatcher? uiDispatcher = null)
    {
        this.enumerate = enumerate;
        Adapters = new ObservableCollection<NetworkAdapterInfo>();
        host = new PollingServiceHost(nameof(NetworkAdapterService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<NetworkAdapterInfo> Adapters { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => host.Interval;
        set => host.Interval = value;
    }

    /// <inheritdoc />
    public bool IncludeLoopback { get; set; }

    /// <inheritdoc />
    public void StartWatching()
        => host.Start();

    /// <inheritdoc />
    public void StopWatching()
        => host.Stop();

    /// <inheritdoc />
    public Task RefreshAsync()
        => host.RefreshAsync();

    /// <inheritdoc />
    public void Dispose()
        => host.Dispose();

    private async Task PollAsync()
    {
        var snapshots = enumerate();
        if (host.IsDisposed)
        {
            return;
        }

        // The collection is bound to the picker, so the sync has to run on the UI thread.
        await host
            .InvokeOnUiAsync(() =>
            {
                if (!host.IsDisposed)
                {
                    SyncWith(snapshots);
                }
            })
            .ConfigureAwait(false);
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

    private void SyncWith(IReadOnlyList<NetworkAdapterSnapshot> snapshots)
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in snapshots)
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