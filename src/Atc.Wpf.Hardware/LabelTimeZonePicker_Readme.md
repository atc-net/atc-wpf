# 🏷️ LabelTimeZonePicker

Labeled wrapper around `TimeZonePicker` with mandatory validation and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `TimeZonePicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelTimeZonePicker
    LabelText="Time zone"
    Value="{Binding SelectedTimeZone, Mode=TwoWay}" />

<atc:LabelTimeZonePicker
    IsMandatory="True"
    LabelText="Mandatory time zone" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs:

| Property | Type | Default |
|----------|------|---------|
| `Value` | `TimeZoneInfo?` | `null` |
| `WatermarkText` | `string` | `""` |

## Validation

- `IsMandatory="True"` + `Value=null` → `"Device is required"`

Time zones don't transition state, so there is no "no longer available" check.

## Events

| Event | Args | Fires when |
|-------|------|------------|
| `LostFocusValid` | `ValueChangedEventArgs<TimeZoneInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<TimeZoneInfo?>` | Selection fails validation |

## Related Controls

- `TimeZonePicker` — bare picker without label/validation
- `LabelDrivePicker` — sibling labeled system-metadata picker

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelTimeZonePicker`