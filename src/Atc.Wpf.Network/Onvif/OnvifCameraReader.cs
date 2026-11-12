namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// Reads ONVIF cameras with <see cref="OnvifClient"/>.
/// </summary>
/// <remarks>
/// The network interfaces are optional: when the camera refuses <c>GetNetworkInterfaces</c>, the details have none.
/// </remarks>
public sealed class OnvifCameraReader : IOnvifCameraReader
{
    /// <inheritdoc />
    public async Task<OnvifCameraDetails> Read(
        Uri deviceServiceUrl,
        NetworkCredential credential,
        CancellationToken cancellationToken)
    {
        var client = new OnvifClient(deviceServiceUrl, credential);

        var information = await client.GetDeviceInformation(cancellationToken).ConfigureAwait(false);
        var streams = await client.GetStreams(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<OnvifNetworkInterface> networkInterfaces;
        try
        {
            networkInterfaces = await client.GetNetworkInterfaces(cancellationToken).ConfigureAwait(false);
        }
        catch (OnvifException)
        {
            networkInterfaces = [];
        }

        return new OnvifCameraDetails(information, streams, networkInterfaces);
    }
}