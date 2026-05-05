# 🏷️ LabelAudioOutputPicker

Labeled wrapper around `AudioOutputPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with an `AudioOutputPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelAudioOutputPicker
    LabelText="Speakers"
    Value="{Binding SelectedSpeakers, Mode=TwoWay}" />

<atc:LabelAudioOutputPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory speakers" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`AudioOutputPicker_Readme.md`](Pickers/AudioOutputPicker_Readme.md):

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

- `AudioOutputPicker` — bare picker without label/validation
- `LabelAudioOutputPicker`, `LabelUsbCameraPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelAudioOutputPicker`