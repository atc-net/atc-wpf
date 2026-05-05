namespace Atc.Wpf.Hardware.Abstractions;

public interface ILabelUsbCameraPicker : ILabelControl
{
    UsbCameraInfo? Value { get; set; }

    bool ShowRefreshButton { get; set; }

    bool AutoRefreshOnDeviceChange { get; set; }

    bool ClearValueOnDisconnect { get; set; }

    bool AutoRebindOnReconnect { get; set; }

    bool AutoSelectFirstAvailable { get; set; }

    bool ShowLivePreview { get; set; }

    double PreviewHeight { get; set; }

    UsbCameraFormat? PreferredFormat { get; set; }

    string WatermarkText { get; set; }
}