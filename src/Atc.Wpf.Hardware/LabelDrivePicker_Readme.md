# 🏷️ LabelDrivePicker

Labeled wrapper around `DrivePicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `DrivePicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelDrivePicker
    LabelText="Drive"
    Value="{Binding SelectedDrive, Mode=TwoWay}" />

<atc:LabelDrivePicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory drive" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`DrivePicker_Readme.md`](Pickers/DrivePicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `DiskDriveInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<DiskDriveInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<DiskDriveInfo?>` | Selection fails validation |

## Related Controls

- `DrivePicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelDrivePicker`