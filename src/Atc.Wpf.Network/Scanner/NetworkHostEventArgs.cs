namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Provides data for the network scanner's host events.
/// </summary>
/// <param name="host">The row of the host.</param>
/// <param name="scanResult">The scan result that was reported.</param>
public sealed class NetworkHostEventArgs(
    NetworkHostViewModel host,
    IPScanResult scanResult)
    : EventArgs
{
    /// <summary>
    /// Gets the row of the host.
    /// </summary>
    public NetworkHostViewModel Host { get; } = host;

    /// <summary>
    /// Gets the scan result that was reported, with everything the scanner found so far.
    /// </summary>
    public IPScanResult ScanResult { get; } = scanResult;
}