namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The behaviour every hardware picker shares: tracking device state changes, handling a selected device
/// that disconnects (DeviceLost, optional clearing, auto-rebind when it returns), auto-selecting the first
/// available device, the selected-state message, and the watch/refresh lifecycle.
/// </summary>
/// <remarks>
/// Each picker keeps its own public dependency properties and routed events (WPF XAML roots cannot be
/// generic) and delegates to one instance of this class through <see cref="IDevicePickerHost{TInfo}"/>.
/// All subscriptions to the service collection and its items are weak, so a picker built on a shared,
/// long-lived service can still be garbage-collected.
/// </remarks>
internal sealed class DevicePickerController<TInfo>
    where TInfo : class, IDeviceInfo, INotifyPropertyChanged
{
    private readonly IDevicePickerHost<TInfo> host;
    private readonly Action startWatching;
    private readonly Action stopWatching;
    private readonly Func<Task> refreshAsync;
    private readonly Dictionary<string, DeviceState> lastKnownStates = new(StringComparer.OrdinalIgnoreCase);
    private string? lostDeviceId;
    private bool isLoaded;

    public DevicePickerController(
        IDevicePickerHost<TInfo> host,
        ObservableCollection<TInfo> items,
        Action startWatching,
        Action stopWatching,
        Func<Task> refreshAsync)
    {
        ArgumentNullException.ThrowIfNull(items);

        this.host = host;
        this.startWatching = startWatching;
        this.stopWatching = stopWatching;
        this.refreshAsync = refreshAsync;

        foreach (var item in items)
        {
            HookItem(item);
        }

        CollectionChangedEventManager.AddHandler(items, OnItemsCollectionChanged);
    }

    /// <summary>
    /// Call from the picker's Loaded handler.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "UI initialisation must not crash on hardware probe failure.")]
    public async Task LoadedAsync()
    {
        isLoaded = true;

        if (host.AutoRefreshOnDeviceChange)
        {
            startWatching();
        }

        try
        {
            await refreshAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{typeof(TInfo).Name} picker initial refresh failed: {ex.Message}");
        }

        UpdateSelectedStateMessage();
    }

    /// <summary>
    /// Call from the picker's Unloaded handler.
    /// </summary>
    public void Unloaded()
    {
        isLoaded = false;
        stopWatching();
    }

    /// <summary>
    /// Call when the picker's AutoRefreshOnDeviceChange property changes.
    /// </summary>
    public void AutoRefreshOnDeviceChangeChanged()
    {
        if (!isLoaded)
        {
            // Loaded applies the current setting.
            return;
        }

        if (host.AutoRefreshOnDeviceChange)
        {
            startWatching();
        }
        else
        {
            stopWatching();
        }
    }

    /// <summary>
    /// Call from the picker's refresh button.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-initiated refresh must not crash on hardware probe failure.")]
    public async Task RefreshAsync()
    {
        try
        {
            await refreshAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{typeof(TInfo).Name} picker refresh failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Call from the picker's Value property-changed callback.
    /// </summary>
    public void ValueChanged(
        TInfo? oldValue,
        TInfo? newValue)
    {
        if (oldValue is not null)
        {
            PropertyChangedEventManager.RemoveHandler(oldValue, OnValueStatePropertyChanged, nameof(IDeviceInfo.State));
        }

        if (newValue is not null)
        {
            PropertyChangedEventManager.AddHandler(newValue, OnValueStatePropertyChanged, nameof(IDeviceInfo.State));
            lostDeviceId = null;
        }

        UpdateSelectedStateMessage();
        host.RaiseValueChanged(oldValue, newValue);
    }

    public void UpdateSelectedStateMessage()
    {
        var value = host.Value;
        if (value is null)
        {
            host.SetSelectedStateMessage(string.Empty);
            return;
        }

        host.SetSelectedStateMessage(value.State switch
        {
            DeviceState.Disconnected => Miscellaneous.DeviceDisconnected,
            DeviceState.InUse => Miscellaneous.DeviceInUse,
            _ => string.Empty,
        });
    }

    private void HookItem(TInfo item)
    {
        lastKnownStates[item.DeviceId] = item.State;
        PropertyChangedEventManager.AddHandler(item, OnItemPropertyChanged, nameof(IDeviceInfo.State));
    }

    private void UnhookItem(TInfo item)
    {
        PropertyChangedEventManager.RemoveHandler(item, OnItemPropertyChanged, nameof(IDeviceInfo.State));
        lastKnownStates.Remove(item.DeviceId);
    }

    private void OnItemPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IDeviceInfo.State) ||
            sender is not TInfo item)
        {
            return;
        }

        var oldState = lastKnownStates.TryGetValue(item.DeviceId, out var previous)
            ? previous
            : DeviceState.Unknown;

        if (oldState == item.State)
        {
            return;
        }

        lastKnownStates[item.DeviceId] = item.State;
        host.RaiseDeviceStateChanged(item.DeviceId, oldState, item.State);
    }

    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IDeviceInfo.State))
        {
            return;
        }

        UpdateSelectedStateMessage();
    }

    private void OnItemsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is TInfo removed)
                {
                    UnhookItem(removed);
                }
            }
        }

        if (e.Action is NotifyCollectionChangedAction.Add &&
            e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is TInfo added)
                {
                    OnItemAdded(added);
                }
            }
        }

        if (host.Value is { State: DeviceState.Disconnected })
        {
            HandleSelectedDeviceLost();
        }
    }

    private void OnItemAdded(TInfo item)
    {
        HookItem(item);

        if (host.Value is not null)
        {
            return;
        }

        if (host.AutoRebindOnReconnect &&
            !string.IsNullOrEmpty(lostDeviceId) &&
            string.Equals(item.DeviceId, lostDeviceId, StringComparison.OrdinalIgnoreCase))
        {
            host.Value = item;
            host.RaiseDeviceReconnected(item);
            lostDeviceId = null;
        }
        else if (host.AutoSelectFirstAvailable &&
                 item.State is DeviceState.Available or DeviceState.JustConnected)
        {
            host.Value = item;
        }
    }

    private void HandleSelectedDeviceLost()
    {
        var lost = host.Value;
        if (lost is null)
        {
            return;
        }

        lostDeviceId = lost.DeviceId;
        host.RaiseDeviceLost(lost);

        if (host.ClearValueOnDisconnect)
        {
            host.Value = null;
        }
        else
        {
            UpdateSelectedStateMessage();
        }
    }
}