namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// A camera frame in BGRA32 layout handed from the capture thread to the UI thread.
/// </summary>
internal sealed record LatestFrame(
    byte[] Buffer,
    int Width,
    int Height);