namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a network adapter.
/// </summary>
public sealed partial class NetworkAdapterInfo : ObservableObject, IDeviceInfo
{
    private string name;
    private long? speed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkAdapterInfo"/> class.
    /// </summary>
    /// <param name="adapterId">The identifier of the adapter.</param>
    /// <param name="name">The connection name of the adapter.</param>
    /// <param name="description">The description of the adapter, typically the hardware name.</param>
    /// <param name="adapterType">The interface type of the adapter.</param>
    /// <param name="macAddress">The physical (MAC) address of the adapter.</param>
    /// <param name="speed">The link speed in bits per second, if known.</param>
    /// <param name="isLoopback">Whether the adapter is a loopback interface.</param>
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

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <summary>
    /// Gets the connection name of the adapter, for example "Ethernet" or "Wi-Fi".
    /// </summary>
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

    /// <summary>
    /// Gets the description of the adapter, typically the hardware name.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the display name of the adapter: <see cref="Description"/>, or <see cref="Name"/> when the description is empty.
    /// </summary>
    public string FriendlyName
        => string.IsNullOrEmpty(Description) ? Name : Description;

    /// <summary>
    /// Gets the interface type of the adapter, such as Ethernet or wireless.
    /// </summary>
    public System.Net.NetworkInformation.NetworkInterfaceType AdapterType { get; }

    /// <summary>
    /// Gets the physical (MAC) address of the adapter.
    /// </summary>
    public string MacAddress { get; }

    /// <summary>
    /// Gets the link speed in bits per second, or <see langword="null"/> when it is not known.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether the adapter is a loopback interface.
    /// </summary>
    public bool IsLoopback { get; }

    /// <summary>
    /// The operational status of the adapter, such as up or down.
    /// </summary>
    [ObservableProperty]
    private System.Net.NetworkInformation.OperationalStatus operationalStatus
        = System.Net.NetworkInformation.OperationalStatus.Unknown;

    /// <summary>
    /// The current state of the adapter.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => $"{FriendlyName} ({AdapterType})";
}