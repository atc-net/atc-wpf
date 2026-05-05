# 🏷️ LabelWindowPicker

Labeled wrapper around `WindowPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `WindowPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelWindowPicker
    LabelText="Window"
    Value="{Binding SelectedWindow, Mode=TwoWay}" />

<atc:LabelWindowPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory window" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`WindowPicker_Readme.md`](Pickers/WindowPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `TopLevelWindowInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<TopLevelWindowInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<TopLevelWindowInfo?>` | Selection fails validation |

## Related Controls

- `WindowPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelWindowPicker`