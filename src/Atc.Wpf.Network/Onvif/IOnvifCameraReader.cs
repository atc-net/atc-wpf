namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// Reads what an ONVIF camera tells about itself: device information, streams and network interfaces.
/// </summary>
public interface IOnvifCameraReader
{
    /// <summary>
    /// Reads the camera.
    /// </summary>
    /// <param name="deviceServiceUrl">The ONVIF device service URL, such as <c>http://192.168.1.132:2020/onvif/device_service</c>.</param>
    /// <param name="credential">The camera's ONVIF account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The camera details.</returns>
    Task<OnvifCameraDetails> Read(
        Uri deviceServiceUrl,
        NetworkCredential credential,
        CancellationToken cancellationToken);
}