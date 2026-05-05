# 🏷️ LabelProcessPicker

Labeled wrapper around `ProcessPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `ProcessPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelProcessPicker
    LabelText="Process"
    Value="{Binding SelectedProcess, Mode=TwoWay}" />

<atc:LabelProcessPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory process" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`ProcessPicker_Readme.md`](Pickers/ProcessPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `RunningProcessInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<RunningProcessInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<RunningProcessInfo?>` | Selection fails validation |

## Related Controls

- `ProcessPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelProcessPicker`