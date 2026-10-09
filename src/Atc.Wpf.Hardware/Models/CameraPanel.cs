namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Specifies where a camera is mounted on the device enclosure.
/// </summary>
public enum CameraPanel
{
    /// <summary>
    /// The mounting location is not known.
    /// </summary>
    Unknown,

    /// <summary>
    /// The camera is on the front panel.
    /// </summary>
    Front,

    /// <summary>
    /// The camera is on the back panel.
    /// </summary>
    Back,

    /// <summary>
    /// The camera is on the top panel.
    /// </summary>
    Top,

    /// <summary>
    /// The camera is on the bottom panel.
    /// </summary>
    Bottom,

    /// <summary>
    /// The camera is on the left panel.
    /// </summary>
    Left,

    /// <summary>
    /// The camera is on the right panel.
    /// </summary>
    Right,

    /// <summary>
    /// The camera is an external device, such as a USB webcam.
    /// </summary>
    External,
}