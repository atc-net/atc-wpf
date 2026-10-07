namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// One polled observation of a display monitor.
/// </summary>
internal sealed record DisplaySnapshot(
    IntPtr Handle,
    string DeviceName,
    Rect Bounds,
    Rect WorkingArea,
    bool IsPrimary);