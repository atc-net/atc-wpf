namespace Atc.Wpf.Hardware.Abstractions;

public interface ILabelSerialPortPicker : ILabelControl
{
    SerialPortInfo? Value { get; set; }

    bool ShowRefreshButton { get; set; }

    bool AutoRefreshOnDeviceChange { get; set; }

    bool DetectInUseState { get; set; }

    bool ClearValueOnDisconnect { get; set; }

    bool AutoRebindOnReconnect { get; set; }

    bool AutoSelectFirstAvailable { get; set; }

    string WatermarkText { get; set; }
}