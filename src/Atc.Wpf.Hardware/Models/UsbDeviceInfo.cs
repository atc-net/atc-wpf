namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a USB device interface.
/// </summary>
public sealed partial class UsbDeviceInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDeviceInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The WinRT device ID of the interface.</param>
    /// <param name="friendlyName">The display name of the device.</param>
    /// <param name="vendorId">The USB vendor ID, if it could be parsed from the device ID.</param>
    /// <param name="productId">The USB product ID, if it could be parsed from the device ID.</param>
    /// <param name="pnpClass">The Plug and Play device class, if known.</param>
    /// <param name="interfaceEnabled">Whether the device interface is enabled.</param>
    public UsbDeviceInfo(
        string deviceId,
        string friendlyName,
        string? vendorId,
        string? productId,
        string? pnpClass,
        bool interfaceEnabled)
    {
        DeviceId = deviceId;
        FriendlyName = friendlyName;
        VendorId = vendorId;
        ProductId = productId;
        PnpClass = pnpClass;
        InterfaceEnabled = interfaceEnabled;
    }

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets the USB vendor ID (VID), or <see langword="null"/> when it could not be parsed from the device ID.
    /// </summary>
    public string? VendorId { get; }

    /// <summary>
    /// Gets the USB product ID (PID), or <see langword="null"/> when it could not be parsed from the device ID.
    /// </summary>
    public string? ProductId { get; }

    /// <summary>
    /// Gets the Plug and Play device class, or <see langword="null"/> when it is not known.
    /// </summary>
    public string? PnpClass { get; }

    /// <summary>
    /// Gets a value indicating whether the device interface is enabled.
    /// </summary>
    public bool InterfaceEnabled { get; }

    /// <summary>
    /// The current state of the device.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
    {
        if (string.IsNullOrEmpty(VendorId) && string.IsNullOrEmpty(ProductId))
        {
            return FriendlyName;
        }

        return $"{FriendlyName} (VID:{VendorId} PID:{ProductId})";
    }
}