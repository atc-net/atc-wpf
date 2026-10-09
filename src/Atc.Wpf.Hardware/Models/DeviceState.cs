namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Specifies the live state of a device or resource shown in a picker.
/// </summary>
public enum DeviceState
{
    /// <summary>
    /// The state has not been determined yet.
    /// </summary>
    Unknown,

    /// <summary>
    /// The device is present and free to use.
    /// </summary>
    Available,

    /// <summary>
    /// The device was connected a moment ago and is briefly highlighted.
    /// </summary>
    JustConnected,

    /// <summary>
    /// The device is held by another process or its interface is disabled.
    /// </summary>
    InUse,

    /// <summary>
    /// The device is no longer present.
    /// </summary>
    Disconnected,
}