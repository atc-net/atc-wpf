# 🏷️ LabelAudioInputPicker

Labeled wrapper around `AudioInputPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with an `AudioInputPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelAudioInputPicker
    LabelText="Microphone"
    Value="{Binding SelectedMicrophone, Mode=TwoWay}" />

<atc:LabelAudioInputPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory microphone" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`AudioInputPicker_Readme.md`](Pickers/AudioInputPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `AudioDeviceInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<AudioDeviceInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<AudioDeviceInfo?>` | Selection fails validation |

## Related Controls

- `AudioInputPicker` — bare picker without label/validation
- `LabelAudioOutputPicker`, `LabelUsbCameraPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelAudioInputPicker`