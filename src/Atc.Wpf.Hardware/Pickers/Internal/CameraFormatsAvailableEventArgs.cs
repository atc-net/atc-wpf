namespace Atc.Wpf.Hardware.Pickers.Internal;

internal sealed class CameraFormatsAvailableEventArgs(IReadOnlyList<UsbCameraFormat> formats) : EventArgs
{
    public IReadOnlyList<UsbCameraFormat> Formats { get; } = formats;
}