namespace Atc.Wpf.Sample.SamplesWpfHardware.Demos;

public partial class HotPlugInUseDemoView
{
    private readonly HotPlugInUseDemoViewModel viewModel;

    public HotPlugInUseDemoView()
    {
        InitializeComponent();

        viewModel = new HotPlugInUseDemoViewModel();
        DataContext = viewModel;

        SerialPicker.AddHandler(
            SerialPortPicker.ValueChangedEvent,
            new RoutedPropertyChangedEventHandler<SerialPortInfo?>(OnSerialValueChanged));
        SerialPicker.AddHandler(
            SerialPortPicker.DeviceLostEvent,
            new RoutedPropertyChangedEventHandler<SerialPortInfo?>(OnSerialDeviceLost));
        SerialPicker.AddHandler(
            SerialPortPicker.DeviceReconnectedEvent,
            new RoutedPropertyChangedEventHandler<SerialPortInfo?>(OnSerialDeviceReconnected));
        SerialPicker.AddHandler(
            SerialPortPicker.DeviceStateChangedEvent,
            new EventHandler<DeviceStateChangedRoutedEventArgs>(OnSerialDeviceStateChanged));

        UsbPicker.AddHandler(
            UsbPortPicker.ValueChangedEvent,
            new RoutedPropertyChangedEventHandler<UsbDeviceInfo?>(OnUsbValueChanged));
        UsbPicker.AddHandler(
            UsbPortPicker.DeviceLostEvent,
            new RoutedPropertyChangedEventHandler<UsbDeviceInfo?>(OnUsbDeviceLost));
        UsbPicker.AddHandler(
            UsbPortPicker.DeviceReconnectedEvent,
            new RoutedPropertyChangedEventHandler<UsbDeviceInfo?>(OnUsbDeviceReconnected));
        UsbPicker.AddHandler(
            UsbPortPicker.DeviceStateChangedEvent,
            new EventHandler<DeviceStateChangedRoutedEventArgs>(OnUsbDeviceStateChanged));

        CameraPicker.AddHandler(
            UsbCameraPicker.ValueChangedEvent,
            new RoutedPropertyChangedEventHandler<UsbCameraInfo?>(OnCameraValueChanged));
        CameraPicker.AddHandler(
            UsbCameraPicker.DeviceLostEvent,
            new RoutedPropertyChangedEventHandler<UsbCameraInfo?>(OnCameraDeviceLost));
        CameraPicker.AddHandler(
            UsbCameraPicker.DeviceReconnectedEvent,
            new RoutedPropertyChangedEventHandler<UsbCameraInfo?>(OnCameraDeviceReconnected));
        CameraPicker.AddHandler(
            UsbCameraPicker.DeviceStateChangedEvent,
            new EventHandler<DeviceStateChangedRoutedEventArgs>(OnCameraDeviceStateChanged));
    }

    private void OnClearClick(
        object sender,
        RoutedEventArgs e)
        => viewModel.Clear();

    private void OnSerialValueChanged(
        object? sender,
        RoutedPropertyChangedEventArgs<SerialPortInfo?> e)
        => viewModel.Log($"[Serial] ValueChanged: {Format(e.OldValue)} → {Format(e.NewValue)}");

    private void OnSerialDeviceLost(
        object? sender,
        RoutedPropertyChangedEventArgs<SerialPortInfo?> e)
        => viewModel.Log($"[Serial] DeviceLost: {Format(e.NewValue)}");

    private void OnSerialDeviceReconnected(
        object? sender,
        RoutedPropertyChangedEventArgs<SerialPortInfo?> e)
        => viewModel.Log($"[Serial] DeviceReconnected: {Format(e.NewValue)}");

    private void OnSerialDeviceStateChanged(
        object? sender,
        DeviceStateChangedRoutedEventArgs e)
        => viewModel.Log($"[Serial] StateChanged: {e.OldState} → {e.NewState} ({Short(e.DeviceId)})");

    private void OnUsbValueChanged(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbDeviceInfo?> e)
        => viewModel.Log($"[USB]    ValueChanged: {Format(e.OldValue)} → {Format(e.NewValue)}");

    private void OnUsbDeviceLost(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbDeviceInfo?> e)
        => viewModel.Log($"[USB]    DeviceLost: {Format(e.NewValue)}");

    private void OnUsbDeviceReconnected(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbDeviceInfo?> e)
        => viewModel.Log($"[USB]    DeviceReconnected: {Format(e.NewValue)}");

    private void OnUsbDeviceStateChanged(
        object? sender,
        DeviceStateChangedRoutedEventArgs e)
        => viewModel.Log($"[USB]    StateChanged: {e.OldState} → {e.NewState} ({Short(e.DeviceId)})");

    private void OnCameraValueChanged(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
        => viewModel.Log($"[Camera] ValueChanged: {Format(e.OldValue)} → {Format(e.NewValue)}");

    private void OnCameraDeviceLost(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
        => viewModel.Log($"[Camera] DeviceLost: {Format(e.NewValue)}");

    private void OnCameraDeviceReconnected(
        object? sender,
        RoutedPropertyChangedEventArgs<UsbCameraInfo?> e)
        => viewModel.Log($"[Camera] DeviceReconnected: {Format(e.NewValue)}");

    private void OnCameraDeviceStateChanged(
        object? sender,
        DeviceStateChangedRoutedEventArgs e)
        => viewModel.Log($"[Camera] StateChanged: {e.OldState} → {e.NewState} ({Short(e.DeviceId)})");

    private static string Format(SerialPortInfo? port)
        => port is null ? "null" : port.PortName;

    private static string Format(UsbDeviceInfo? device)
        => device is null ? "null" : device.FriendlyName;

    private static string Format(UsbCameraInfo? camera)
        => camera is null ? "null" : camera.FriendlyName;

    private static string Short(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            return string.Empty;
        }

        return deviceId.Length > 40
            ? "…" + deviceId[^39..]
            : deviceId;
    }
}