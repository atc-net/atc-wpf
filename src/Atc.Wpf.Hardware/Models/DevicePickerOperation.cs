namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// The picker operation that failed, reported by the <c>DeviceError</c> routed event.
/// </summary>
public enum DevicePickerOperation
{
    /// <summary>
    /// Starting to watch for device changes failed; the list is not updated live.
    /// </summary>
    StartWatching,

    /// <summary>
    /// Reading the device list failed; the list may be empty or out of date.
    /// </summary>
    Refresh,
}