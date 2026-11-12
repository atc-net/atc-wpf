namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Provides data for a request to connect to a host, such as <see cref="NetworkScannerViewModel.VncRequested"/>.
/// </summary>
/// <param name="host">The host.</param>
/// <param name="port">The port to connect to.</param>
public sealed class NetworkHostConnectEventArgs(
    NetworkHostViewModel host,
    int port)
    : EventArgs
{
    /// <summary>
    /// Gets the host.
    /// </summary>
    public NetworkHostViewModel Host { get; } = host;

    /// <summary>
    /// Gets the port to connect to.
    /// </summary>
    public int Port { get; } = port;
}