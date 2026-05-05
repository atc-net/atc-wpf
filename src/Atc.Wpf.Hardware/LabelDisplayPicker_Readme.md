# 🏷️ LabelDisplayPicker

Labeled wrapper around `DisplayPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `DisplayPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelDisplayPicker
    LabelText="Display"
    Value="{Binding SelectedDisplay, Mode=TwoWay}" />

<atc:LabelDisplayPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory display" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`DisplayPicker_Readme.md`](Pickers/DisplayPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `DisplayInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<DisplayInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<DisplayInfo?>` | Selection fails validation |

## Related Controls

- `DisplayPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelDisplayPicker`