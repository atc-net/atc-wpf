# 🅱️ BluetoothDevicePicker

A control for selecting a paired Bluetooth (Classic) device.

## Overview

Enumerates paired Bluetooth devices via WinRT `BluetoothDevice.GetDeviceSelectorFromPairingState(true)` and listens for arrival/removal through the shared `DeviceWatcherHost`. The selected `DeviceId` round-trips cleanly to `BluetoothDevice.FromIdAsync` for downstream use.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:BluetoothDevicePicker
    Value="{Binding SelectedDevice, Mode=TwoWay}" />

<atc:BluetoothDevicePicker
    AutoSelectFirstAvailable="True"
    WatermarkText="Select Bluetooth device…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `BluetoothDeviceInfo?` | `null` | Selected paired device (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when device unpairs/leaves |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first device on first appear |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<BluetoothDeviceInfo?>` | Selected device changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<BluetoothDeviceInfo?>` | Bound device disappeared |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<BluetoothDeviceInfo?>` | Lost device reappeared |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Any tracked device's state changed |

## Notes

- v1 enumerates **paired classic Bluetooth** devices only. BLE-only and unpaired-discovery scenarios require additional permissions and are deferred to v2.
- `Value.IsConnected` reflects whether Windows considers the paired device currently in range / connected. Live updates depend on the OS surfacing them via `DeviceWatcher` events; consumers polling for connection status can refresh manually via `IBluetoothDeviceService.RefreshAsync()`.
- `Value.DeviceId` is the WinRT device id — pass to `BluetoothDevice.FromIdAsync` (or `BluetoothLEDevice.FromIdAsync` for BLE peers reachable via the same id).

## Related Controls

- `LabelBluetoothDevicePicker` — labeled wrapper with validation
- `UsbPortPicker` — wired USB device picker
- `AudioOutputPicker` — picks an audio render endpoint, which may itself be a connected Bluetooth headset

## Sample Application Path

`SamplesWpfHardware → Pickers → BluetoothDevicePicker`