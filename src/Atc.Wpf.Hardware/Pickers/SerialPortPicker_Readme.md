# 🔌 SerialPortPicker

A control for selecting a serial / COM port from the connected hardware.

## Overview

Enumerates serial devices via WinRT `SerialDevice.GetDeviceSelector()` and listens for hot-plug events through a shared `DeviceWatcherHost`. The picker updates live as ports appear, disappear or change state.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:SerialPortPicker
    Value="{Binding SelectedPort, Mode=TwoWay}" />

<atc:SerialPortPicker
    AutoSelectFirstAvailable="True"
    DetectInUseState="True"
    WatermarkText="Select serial port…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `SerialPortInfo?` | `null` | Selected port (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `DetectInUseState` | `bool` | `false` | Opt-in `SerialDevice.FromIdAsync` probe |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when device unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first port on first appear |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<SerialPortInfo?>` | Selected port changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<SerialPortInfo?>` | Bound port disconnected |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<SerialPortInfo?>` | Lost port reappeared |

## Notes

- `Value.PortName` gives the friendly COM name (e.g. `"COM3"`).
- `Value.DeviceId` is the WinRT device ID — pass to `SerialDevice.FromIdAsync` to open the port.
- Use `LabelSerialPortPicker` when you need a label, validation, and mandatory marker.

## Related Controls

- `LabelSerialPortPicker` — labeled wrapper with validation
- `UsbPortPicker` — generic USB device picker
- `UsbCameraPicker` — video-capture device picker

## Sample Application Path

`SamplesWpfHardware → Pickers → SerialPortPicker`