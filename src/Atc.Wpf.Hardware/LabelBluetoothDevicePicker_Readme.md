# 🏷️ LabelBluetoothDevicePicker

Labeled wrapper around `BluetoothDevicePicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `BluetoothDevicePicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelBluetoothDevicePicker
    LabelText="Bluetooth device"
    Value="{Binding SelectedDevice, Mode=TwoWay}" />

<atc:LabelBluetoothDevicePicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory Bluetooth device" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`BluetoothDevicePicker_Readme.md`](Pickers/BluetoothDevicePicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `BluetoothDeviceInfo?` | `null` |
| `WatermarkText` | `string` | `""` |
| `ShowRefreshButton` | `bool` | `true` |
| `AutoRefreshOnDeviceChange` | `bool` | `true` |
| `ClearValueOnDisconnect` | `bool` | `false` |
| `AutoRebindOnReconnect` | `bool` | `true` |
| `AutoSelectFirstAvailable` | `bool` | `false` |

## Validation

- `IsMandatory="True"` + `Value=null` → `"Device is required"`
- `Value.State == Disconnected` → `"Device is no longer available"`

Validation messages are localised (`en-US` / `da-DK` / `de-DE`).

## Events

| Event | Args | Fires when |
|-------|------|------------|
| `LostFocusValid` | `ValueChangedEventArgs<BluetoothDeviceInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<BluetoothDeviceInfo?>` | Selection fails validation |

## Related Controls

- `BluetoothDevicePicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelBluetoothDevicePicker`