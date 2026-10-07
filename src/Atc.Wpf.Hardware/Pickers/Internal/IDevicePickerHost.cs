namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The picker-side surface that <see cref="DevicePickerController{TInfo}"/> drives: the selected value,
/// the behaviour switches, and the picker's routed events.
/// </summary>
internal interface IDevicePickerHost<TInfo>
    where TInfo : class, IDeviceInfo, INotifyPropertyChanged
{
    TInfo? Value { get; set; }

    bool AutoRefreshOnDeviceChange { get; }

    bool ClearValueOnDisconnect { get; }

    bool AutoRebindOnReconnect { get; }

    bool AutoSelectFirstAvailable { get; }

    void SetSelectedStateMessage(string message);

    void RaiseValueChanged(
        TInfo? oldValue,
        TInfo? newValue);

    void RaiseDeviceLost(TInfo device);

    void RaiseDeviceReconnected(TInfo device);

    void RaiseDeviceStateChanged(
        string deviceId,
        DeviceState oldState,
        DeviceState newState);
}