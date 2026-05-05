# 🏷️ LabelUsbPortPicker

Labeled wrapper around `UsbPortPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `UsbPortPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelUsbPortPicker
    LabelText="USB device"
    Value="{Binding SelectedDevice, Mode=TwoWay}" />

<atc:LabelUsbPortPicker
    ClassFilter="Hid"
    IsMandatory="True"
    LabelText="HID device"
    Value="{Binding SelectedDevice, Mode=TwoWay}" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`UsbPortPicker_Readme.md`](Pickers/UsbPortPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `UsbDeviceInfo?` | `null` |
| `ClassFilter` | `UsbDeviceClassFilter` | `None` |
| `WatermarkText` | `string` | `""` |
| `ShowRefreshButton` | `bool` | `true` |
| `AutoRefreshOnDeviceChange` | `bool` | `true` |
| `DetectInUseState` | `bool` | `false` |
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
| `LostFocusValid` | `ValueChangedEventArgs<UsbDeviceInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<UsbDeviceInfo?>` | Selection fails validation |

## Related Controls

- `UsbPortPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbCameraPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelUsbPortPicker`