# 🏷️ LabelUsbCameraPicker

Labeled wrapper around `UsbCameraPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `UsbCameraPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelUsbCameraPicker
    LabelText="Camera"
    Value="{Binding SelectedCamera, Mode=TwoWay}" />

<atc:LabelUsbCameraPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory camera" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`UsbCameraPicker_Readme.md`](Pickers/UsbCameraPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `UsbCameraInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<UsbCameraInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<UsbCameraInfo?>` | Selection fails validation |

## Related Controls

- `UsbCameraPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelUsbCameraPicker`