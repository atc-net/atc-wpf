# 🏷️ LabelSerialPortPicker

Labeled wrapper around `SerialPortPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `SerialPortPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelSerialPortPicker
    LabelText="Serial port"
    Value="{Binding SelectedPort, Mode=TwoWay}" />

<atc:LabelSerialPortPicker
    IsMandatory="True"
    LabelText="Required port"
    ShowAsteriskOnMandatory="True"
    Value="{Binding SelectedPort, Mode=TwoWay}" />
```

## Properties

Inherits all `LabelControl` properties (`LabelText`, `LabelPosition`, `IsMandatory`, `InformationText`, `ValidationText`, `LabelWidthNumber`, `Orientation`, …) plus the same forwarded picker DPs documented in [`SerialPortPicker_Readme.md`](Pickers/SerialPortPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `SerialPortInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<SerialPortInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<SerialPortInfo?>` | Selection fails validation |

## Related Controls

- `SerialPortPicker` — bare picker without label/validation
- `LabelUsbPortPicker`, `LabelUsbCameraPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelSerialPortPicker`