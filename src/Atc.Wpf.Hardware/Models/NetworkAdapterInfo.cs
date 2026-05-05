namespace Atc.Wpf.Hardware.Models;

public sealed partial class NetworkAdapterInfo : ObservableObject, IDeviceInfo
{
    public NetworkAdapterInfo(
        string adapterId,
        string name,
        string description,
        System.Net.NetworkInformation.NetworkInterfaceType adapterType,
        string macAddress,
        long? speed,
        bool isLoopback)
    {
        DeviceId = adapterId;
        Name = name;
        Description = description;
        AdapterType = adapterType;
        MacAddress = macAddress;
        Speed = speed;
        IsLoopback = isLoopback;
    }

    public string DeviceId { get; }

    public string Name { get; }

    public string Description { get; }

    public string FriendlyName
        => string.IsNullOrEmpty(Description) ? Name : Description;

    public System.Net.NetworkInformation.NetworkInterfaceType AdapterType { get; }

    public string MacAddress { get; }

    public long? Speed { get; }

    public bool IsLoopback { get; }

    [ObservableProperty]
    private System.Net.NetworkInformation.OperationalStatus operationalStatus
        = System.Net.NetworkInformation.OperationalStatus.Unknown;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => $"{FriendlyName} ({AdapterType})";
}