namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a Bluetooth device.
/// </summary>
public sealed partial class BluetoothDeviceInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothDeviceInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The WinRT device ID of the device.</param>
    /// <param name="friendlyName">The display name of the device.</param>
    /// <param name="isPaired">Whether the device is paired with this computer.</param>
    /// <param name="isConnected">Whether the device is currently connected.</param>
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

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets a value indicating whether the device is paired with this computer.
    /// </summary>
    public bool IsPaired { get; }

    /// <summary>
    /// A value indicating whether the device is currently connected.
    /// </summary>
    [ObservableProperty]
    private bool isConnected;

    /// <summary>
    /// The current state of the device.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => IsConnected
            ? $"{FriendlyName} ●"
            : FriendlyName;
}