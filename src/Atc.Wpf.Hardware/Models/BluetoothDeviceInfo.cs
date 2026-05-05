namespace Atc.Wpf.Hardware.Models;

public sealed partial class BluetoothDeviceInfo : ObservableObject, IDeviceInfo
{
    public BluetoothDeviceInfo(
        string deviceId,
        string friendlyName,
        bool isPaired,
        bool isConnected)
    {
        DeviceId = deviceId;
        FriendlyName = friendlyName;
        IsPaired = isPaired;
        IsConnected = isConnected;
    }

    public string DeviceId { get; }

    public string FriendlyName { get; }

    public bool IsPaired { get; }

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => IsConnected
            ? $"{FriendlyName} ●"
            : FriendlyName;
}