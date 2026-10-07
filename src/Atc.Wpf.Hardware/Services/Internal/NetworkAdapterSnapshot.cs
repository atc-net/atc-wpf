namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// One polled observation of a network adapter.
/// </summary>
internal sealed record NetworkAdapterSnapshot(
    string Id,
    string Name,
    string Description,
    System.Net.NetworkInformation.NetworkInterfaceType AdapterType,
    string MacAddress,
    long? Speed,
    System.Net.NetworkInformation.OperationalStatus OperationalStatus);