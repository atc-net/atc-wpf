namespace Atc.Wpf.Network;

/// <summary>
/// Provides design-time sample data for the network scanner controls.
/// </summary>
public static class DesignModeHelper
{
    /// <summary>
    /// Creates a sample list of network host view models for design-time display.
    /// </summary>
    /// <returns>A collection of sample <see cref="NetworkHostViewModel"/> instances with varied data.</returns>
    public static IEnumerable<NetworkHostViewModel> CreateNetworkHostInfoList()
        =>
        [
            new(IPAddress.Parse("192.168.1.1"))
            {
                Hostname = "router.local",
                MacAddress = "00:11:22:33:44:55",
                MacVendor = "Cisco Systems",
                PingStatus = new PingStatusResult(IPAddress.Parse("192.168.1.1"), IPStatus.Success, 1),
                OpenPortNumbers = [22, 80, 443],
                TimeDiff = "12ms",
            },
            new(IPAddress.Parse("192.168.1.10"))
            {
                Hostname = "desktop-pc",
                MacAddress = "AA:BB:CC:DD:EE:FF",
                MacVendor = "Dell Inc.",
                PingStatus = new PingStatusResult(IPAddress.Parse("192.168.1.10"), IPStatus.Success, 5),
                OpenPortNumbers = [3389],
                TimeDiff = "45ms",
            },
            new(IPAddress.Parse("192.168.1.25"))
            {
                Hostname = "nas-storage",
                MacAddress = "11:22:33:44:55:66",
                MacVendor = "Synology Inc.",
                PingStatus = new PingStatusResult(IPAddress.Parse("192.168.1.25"), IPStatus.Success, 15),
                OpenPortNumbers = [80, 443, 5000, 5001],
                TimeDiff = "89ms",
            },
            new(IPAddress.Parse("192.168.1.50"))
            {
                PingStatus = new PingStatusResult(IPAddress.Parse("192.168.1.50"), IPStatus.TimedOut, 0),
                TimeDiff = "3000ms",
            },
            new(IPAddress.Parse("192.168.1.100"))
            {
                Hostname = "printer",
                MacAddress = "77:88:99:AA:BB:CC",
                MacVendor = "HP Inc.",
                PingStatus = new PingStatusResult(IPAddress.Parse("192.168.1.100"), IPStatus.Success, 120),
                OpenPortNumbers = [80, 9100],
                TimeDiff = "156ms",
            },
        ];
}