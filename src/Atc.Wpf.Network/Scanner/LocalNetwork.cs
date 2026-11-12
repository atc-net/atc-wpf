namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// An IPv4 network this machine is on: one per network adapter address.
/// </summary>
public sealed class LocalNetwork
{
    private static readonly string[] VirtualAdapterMarkers =
    [
        "Virtual",
        "vEthernet",
        "Hyper-V",
        "VMware",
        "VirtualBox",
        "TAP-Windows",
        "WireGuard",
        "Tailscale",
        "ZeroTier",
        "VPN",
    ];

    private LocalNetwork(
        string interfaceName,
        string cidr,
        IPAddress startIpAddress,
        IPAddress endIpAddress)
    {
        InterfaceName = interfaceName;
        Cidr = cidr;
        StartIpAddress = startIpAddress;
        EndIpAddress = endIpAddress;
    }

    /// <summary>
    /// Gets the name of the network adapter.
    /// </summary>
    public string InterfaceName { get; }

    /// <summary>
    /// Gets the network in CIDR notation, such as <c>192.168.1.0/24</c>.
    /// </summary>
    public string Cidr { get; }

    /// <summary>
    /// Gets a value indicating whether the adapter is virtual (Hyper-V, WSL, VMware, VirtualBox, VPN, …).
    /// </summary>
    public bool IsVirtual { get; private init; }

    /// <summary>
    /// Gets the first host address of the network.
    /// </summary>
    public IPAddress StartIpAddress { get; }

    /// <summary>
    /// Gets the last host address of the network.
    /// </summary>
    public IPAddress EndIpAddress { get; }

    /// <summary>
    /// Creates the network of an adapter address and its subnet mask.
    /// </summary>
    /// <param name="interfaceName">The name of the network adapter.</param>
    /// <param name="address">The adapter's IPv4 address.</param>
    /// <param name="mask">The subnet mask.</param>
    /// <param name="isVirtual">Whether the adapter is virtual (Hyper-V, WSL, VMware, VPN, …).</param>
    /// <returns>The network.</returns>
    public static LocalNetwork FromAddressAndMask(
        string interfaceName,
        IPAddress address,
        IPAddress mask,
        bool isVirtual = false)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(mask);

        var maskValue = ToUInt32(mask);
        var network = ToUInt32(address) & maskValue;
        var broadcast = network | ~maskValue;
        var prefixLength = BitOperations.PopCount(maskValue);
        var cidr = $"{ToIPAddress(network)}/{prefixLength}";

        // A /31 or /32 has no network and broadcast address to leave out.
        return prefixLength <= 30
            ? new LocalNetwork(interfaceName, cidr, ToIPAddress(network + 1), ToIPAddress(broadcast - 1)) { IsVirtual = isVirtual }
            : new LocalNetwork(interfaceName, cidr, ToIPAddress(network), ToIPAddress(broadcast)) { IsVirtual = isVirtual };
    }

    /// <summary>
    /// Gets whether an adapter address is worth scanning: IPv4, not loopback and not link-local (169.254.x.x).
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns><c>true</c> when the address is worth scanning.</returns>
    public static bool IsScannable(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork ||
            IPAddress.IsLoopback(address))
        {
            return false;
        }

        return address.GetAddressBytes() is not [169, 254, _, _];
    }

    /// <summary>
    /// Gets the networks of the network adapters that are up.
    /// </summary>
    /// <returns>The networks, without duplicates.</returns>
    public static IReadOnlyList<LocalNetwork> GetAll()
        => NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up &&
                        x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .SelectMany(x => x
                .GetIPProperties()
                .UnicastAddresses
                .Where(a => IsScannable(a.Address) &&
                            a.IPv4Mask is not null &&
                            !a.IPv4Mask.Equals(IPAddress.Any))
                .Select(a => FromAddressAndMask(x.Name, a.Address, a.IPv4Mask, IsVirtualAdapter(x.Name, x.Description))))
            .DistinctBy(x => x.Cidr, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Gets whether a network adapter is virtual or a VPN tunnel, from its name and description.
    /// </summary>
    /// <param name="name">The adapter's name, such as <c>vEthernet (WSL)</c>.</param>
    /// <param name="description">The adapter's description, such as <c>Hyper-V Virtual Ethernet Adapter</c>.</param>
    /// <returns><c>true</c> for a virtual adapter.</returns>
    public static bool IsVirtualAdapter(
        string name,
        string description)
        => VirtualAdapterMarkers.Any(marker =>
            (name ?? string.Empty).Contains(marker, StringComparison.OrdinalIgnoreCase) ||
            (description ?? string.Empty).Contains(marker, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public override string ToString()
        => IsVirtual
            ? string.Format(CultureInfo.CurrentCulture, Resources.Resources.VirtualNetworkFormat1, $"{InterfaceName} - {Cidr}")
            : $"{InterfaceName} - {Cidr}";

    private static uint ToUInt32(IPAddress address)
        => BinaryPrimitives.ReadUInt32BigEndian(address.GetAddressBytes());

    private static IPAddress ToIPAddress(uint value)
    {
        var bytes = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
        return new IPAddress(bytes);
    }
}