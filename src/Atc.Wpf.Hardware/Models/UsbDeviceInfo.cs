namespace Atc.Wpf.Hardware.Models;

public sealed partial class UsbDeviceInfo : ObservableObject, IDeviceInfo
{
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

    public string DeviceId { get; }

    public string FriendlyName { get; }

    public string? VendorId { get; }

    public string? ProductId { get; }

    public string? PnpClass { get; }

    public bool InterfaceEnabled { get; }

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
    {
        if (string.IsNullOrEmpty(VendorId) && string.IsNullOrEmpty(ProductId))
        {
            return FriendlyName;
        }

        return $"{FriendlyName} (VID:{VendorId} PID:{ProductId})";
    }
}