namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Specifies which device interface classes a USB device list includes; flags can be combined.
/// </summary>
[Flags]
public enum UsbDeviceClassFilter
{
    /// <summary>
    /// No class filter: all USB device interfaces are listed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Human interface devices, such as keyboards, mice and game controllers.
    /// </summary>
    Hid = 1 << 0,

    /// <summary>
    /// Imaging devices, such as scanners and cameras.
    /// </summary>
    Imaging = 1 << 1,

    /// <summary>
    /// Audio devices.
    /// </summary>
    Audio = 1 << 2,

    /// <summary>
    /// Printers.
    /// </summary>
    Printer = 1 << 3,

    /// <summary>
    /// Mass storage devices, such as USB drives.
    /// </summary>
    MassStorage = 1 << 4,

    /// <summary>
    /// Communication devices exposed as serial (COM) ports.
    /// </summary>
    Communication = 1 << 5,
}