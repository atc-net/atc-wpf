namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// What an ONVIF camera tells about itself.
/// </summary>
/// <param name="Information">Manufacturer, model, firmware and serial number.</param>
/// <param name="Streams">Every media profile with its RTSP URL.</param>
/// <param name="NetworkInterfaces">The network interfaces; empty when the camera doesn't report them.</param>
public sealed record OnvifCameraDetails(
    OnvifDeviceInformation Information,
    IReadOnlyList<OnvifStreamInfo> Streams,
    IReadOnlyList<OnvifNetworkInterface> NetworkInterfaces);