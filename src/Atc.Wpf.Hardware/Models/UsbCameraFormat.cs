namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a video format supported by a camera.
/// </summary>
/// <param name="Width">The frame width in pixels.</param>
/// <param name="Height">The frame height in pixels.</param>
/// <param name="FrameRate">The frame rate in frames per second.</param>
/// <param name="Subtype">The media encoding subtype, for example <c>MJPG</c> or <c>NV12</c>.</param>
public sealed record UsbCameraFormat(
    uint Width,
    uint Height,
    double FrameRate,
    string Subtype)
{
    /// <inheritdoc />
    public override string ToString()
        => $"{Width}×{Height} @ {FrameRate.ToString("0.#", CultureInfo.InvariantCulture)} fps ({Subtype})";
}