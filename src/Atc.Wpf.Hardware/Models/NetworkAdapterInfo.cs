namespace Atc.Wpf.Hardware.Models;

public sealed partial class NetworkAdapterInfo : ObservableObject, IDeviceInfo
{
    private string name;
    private long? speed;

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
        this.name = name;
        Description = description;
        AdapterType = adapterType;
        MacAddress = macAddress;
        this.speed = speed;
        IsLoopback = isLoopback;
    }

    public string DeviceId { get; }

    public string Name
    {
        get => name;
        internal set
        {
            if (string.Equals(value, name, StringComparison.Ordinal))
            {
                return;
            }

            name = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(FriendlyName));
        }
    }

    public string Description { get; }

    public string FriendlyName
        => string.IsNullOrEmpty(Description) ? Name : Description;

    public System.Net.NetworkInformation.NetworkInterfaceType AdapterType { get; }

    public string MacAddress { get; }

    public long? Speed
    {
        get => speed;
        internal set
        {
            if (value == speed)
            {
                return;
            }

            speed = value;
            RaisePropertyChanged();
        }
    }

    public bool IsLoopback { get; }

    [ObservableProperty]
    private System.Net.NetworkInformation.OperationalStatus operationalStatus
        = System.Net.NetworkInformation.OperationalStatus.Unknown;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => $"{FriendlyName} ({AdapterType})";
}