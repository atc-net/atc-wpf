namespace Atc.Wpf.Hardware.Abstractions;

public interface ILabelProcessPicker : ILabelControl
{
    RunningProcessInfo? Value { get; set; }

    bool ShowRefreshButton { get; set; }

    bool AutoRefreshOnDeviceChange { get; set; }

    bool ClearValueOnDisconnect { get; set; }

    bool AutoRebindOnReconnect { get; set; }

    bool AutoSelectFirstAvailable { get; set; }

    string WatermarkText { get; set; }
}