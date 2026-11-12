namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// Provides data for the <see cref="OnvifCameraViewModel.StreamSelected"/> event.
/// </summary>
/// <param name="deviceServiceUrl">The camera's ONVIF device service URL.</param>
/// <param name="stream">The chosen stream: its profile and RTSP URL (without credentials).</param>
/// <param name="credential">The account the camera was read with; use it to play the stream.</param>
/// <param name="manufacturer">The camera's manufacturer, when known.</param>
/// <param name="model">The camera's model, when known.</param>
public sealed class OnvifStreamSelectedEventArgs(
    Uri deviceServiceUrl,
    OnvifStreamInfo stream,
    NetworkCredential credential,
    string? manufacturer,
    string? model)
    : EventArgs
{
    /// <summary>
    /// Gets the camera's ONVIF device service URL.
    /// </summary>
    public Uri DeviceServiceUrl { get; } = deviceServiceUrl;

    /// <summary>
    /// Gets the chosen stream: its profile and RTSP URL (without credentials).
    /// </summary>
    public OnvifStreamInfo Stream { get; } = stream;

    /// <summary>
    /// Gets the account the camera was read with; use it to play the stream.
    /// </summary>
    public NetworkCredential Credential { get; } = credential;

    /// <summary>
    /// Gets the camera's manufacturer, when known.
    /// </summary>
    public string? Manufacturer { get; } = manufacturer;

    /// <summary>
    /// Gets the camera's model, when known.
    /// </summary>
    public string? Model { get; } = model;
}