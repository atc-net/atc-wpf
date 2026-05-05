# 🔌 UsbPortPicker

A control for selecting a connected USB device.

## Overview

Enumerates USB devices via the WinRT USB device-interface AQS query and listens for hot-plug events through a shared `DeviceWatcherHost`. Maps `InterfaceEnabled=false` to `DeviceState.InUse` so the picker can flag busy devices automatically.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:UsbPortPicker
    Value="{Binding SelectedDevice, Mode=TwoWay}" />

<atc:UsbPortPicker
    ClassFilter="Hid"
    WatermarkText="Select HID device…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `UsbDeviceInfo?` | `null` | Selected device (TwoWay) |
| `ClassFilter` | `UsbDeviceClassFilter` | `None` | Optional filter (HID, Audio, Printer, …) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `DetectInUseState` | `bool` | `false` | Opt-in active probe |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when device unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first device on first appear |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<UsbDeviceInfo?>` | Selected device changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<UsbDeviceInfo?>` | Bound device disconnected |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<UsbDeviceInfo?>` | Lost device reappeared |

## Notes

- `Value.VendorId` and `Value.ProductId` are 4-character hex strings (e.g. `"046D"`).
- `Value.DeviceId` is the WinRT device ID — usable as a stable identifier for persistence.
- `ClassFilter` is currently exposed but applies client-side; AQS-level translation is a v2 enhancement.

## Related Controls

- `LabelUsbPortPicker` — labeled wrapper with validation
- `SerialPortPicker` — serial / COM port picker
- `UsbCameraPicker` — video-capture device picker

## Sample Application Path

`SamplesWpfHardware → Pickers → UsbPortPicker`