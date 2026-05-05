# 🏷️ LabelNetworkAdapterPicker

Labeled wrapper around `NetworkAdapterPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `NetworkAdapterPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelNetworkAdapterPicker
    LabelText="NetworkAdapter"
    Value="{Binding SelectedAdapter, Mode=TwoWay}" />

<atc:LabelNetworkAdapterPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory network adapter" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`NetworkAdapterPicker_Readme.md`](Pickers/NetworkAdapterPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `NetworkAdapterInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<NetworkAdapterInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<NetworkAdapterInfo?>` | Selection fails validation |

## Related Controls

- `NetworkAdapterPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelNetworkAdapterPicker`