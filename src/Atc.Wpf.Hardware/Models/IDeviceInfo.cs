namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Defines the common members of a device or resource listed by a picker.
/// </summary>
public interface IDeviceInfo
{
    /// <summary>
    /// Gets the identifier that uniquely identifies the device.
    /// </summary>
    string DeviceId { get; }

    /// <summary>
    /// Gets the display name of the device.
    /// </summary>
    string FriendlyName { get; }

    /// <summary>
    /// Gets the current state of the device.
    /// </summary>
    DeviceState State { get; }
}