namespace Atc.Wpf.Hardware.Models;

public sealed partial class SerialPortInfo : ObservableObject, IDeviceInfo
{
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

    public string DeviceId { get; }

    public string PortName { get; }

    public string FriendlyName { get; }

    public string? VendorId { get; }

    public string? ProductId { get; }

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => string.IsNullOrEmpty(FriendlyName)
            ? PortName
            : $"{PortName} — {FriendlyName}";
}