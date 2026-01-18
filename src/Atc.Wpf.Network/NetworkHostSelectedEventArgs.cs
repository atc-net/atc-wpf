namespace Atc.Wpf.Network;

/// <summary>
/// Provides data for the <see cref="NetworkScannerViewModel.EntrySelected"/> event.
/// </summary>
/// <param name="selectedHost">The selected network host, or <c>null</c> if selection was cleared.</param>
public sealed class NetworkHostSelectedEventArgs(NetworkHostViewModel? selectedHost)
    : EventArgs
{
    /// <summary>
    /// Gets the selected network host.
    /// </summary>
    /// <value>The selected <see cref="NetworkHostViewModel"/>, or <c>null</c> if no host is selected.</value>
    public NetworkHostViewModel? SelectedHost { get; } = selectedHost;
}