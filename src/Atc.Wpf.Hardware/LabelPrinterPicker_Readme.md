# 🏷️ LabelPrinterPicker

Labeled wrapper around `PrinterPicker` with validation, mandatory marker, and `LostFocusValid` / `LostFocusInvalid` events.

## Overview

Composes a `LabelContent` (label area, mandatory asterisk, validation message, info popup) with a `PrinterPicker` content area. Mirrors the `LabelFilePicker` pattern from `Atc.Wpf.Forms`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:LabelPrinterPicker
    LabelText="Printer"
    Value="{Binding SelectedPrinter, Mode=TwoWay}" />

<atc:LabelPrinterPicker
    AutoSelectFirstAvailable="True"
    IsMandatory="True"
    LabelText="Mandatory printer" />
```

## Properties

Inherits all `LabelControl` properties plus the forwarded picker DPs documented in [`PrinterPicker_Readme.md`](Pickers/PrinterPicker_Readme.md):

| Property | Type | Default |
|----------|------|---------|
| `Value` | `PrinterInfo?` | `null` |
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
| `LostFocusValid` | `ValueChangedEventArgs<PrinterInfo?>` | Selection passes validation |
| `LostFocusInvalid` | `ValueChangedEventArgs<PrinterInfo?>` | Selection fails validation |

## Related Controls

- `PrinterPicker` — bare picker without label/validation
- `LabelSerialPortPicker`, `LabelUsbPortPicker` — sibling labeled pickers

## Sample Application Path

`SamplesWpfHardware → LabelControls → LabelPrinterPicker`