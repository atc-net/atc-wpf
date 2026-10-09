namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a serial (COM) port.
/// </summary>
public sealed partial class SerialPortInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SerialPortInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The WinRT device ID of the port.</param>
    /// <param name="portName">The port name, for example <c>COM3</c>.</param>
    /// <param name="friendlyName">The display name of the port.</param>
    /// <param name="vendorId">The USB vendor ID, if the port is a USB device.</param>
    /// <param name="productId">The USB product ID, if the port is a USB device.</param>
    public SerialPortInfo(
        string deviceId,
        string portName,
        string friendlyName,
        string? vendorId,
        string? productId)
    {
        DeviceId = deviceId;
        PortName = portName;
        FriendlyName = friendlyName;
        VendorId = vendorId;
        ProductId = productId;
    }

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <summary>
    /// Gets the port name, for example <c>COM3</c>.
    /// </summary>
    public string PortName { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets the USB vendor ID (VID), or <see langword="null"/> when the port is not a USB device.
    /// </summary>
    public string? VendorId { get; }

    /// <summary>
    /// Gets the USB product ID (PID), or <see langword="null"/> when the port is not a USB device.
    /// </summary>
    public string? ProductId { get; }

    /// <summary>
    /// The current state of the port.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => string.IsNullOrEmpty(FriendlyName)
            ? PortName
            : $"{PortName} — {FriendlyName}";
}