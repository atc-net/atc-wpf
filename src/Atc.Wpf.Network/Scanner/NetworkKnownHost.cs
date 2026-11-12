namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// A host an earlier scan found with its MAC address, kept so it can be woken when it stops answering.
/// </summary>
/// <param name="IpAddress">The address the host had.</param>
/// <param name="MacAddress">The host's MAC address.</param>
/// <param name="Name">The host's device name, when known.</param>
/// <param name="LastSeen">When the host last answered.</param>
public sealed record NetworkKnownHost(
    IPAddress IpAddress,
    string MacAddress,
    string? Name,
    DateTime LastSeen);