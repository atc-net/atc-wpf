namespace Atc.Wpf.Hardware.Models;

[Flags]
public enum UsbDeviceClassFilter
{
    None = 0,
    Hid = 1 << 0,
    Imaging = 1 << 1,
    Audio = 1 << 2,
    Printer = 1 << 3,
    MassStorage = 1 << 4,
    Communication = 1 << 5,
}