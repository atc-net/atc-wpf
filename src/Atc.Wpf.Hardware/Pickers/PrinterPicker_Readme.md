# 🖨️ PrinterPicker

A control for selecting an installed printer / print queue.

## Overview

Enumerates local and connected print queues via `System.Printing.LocalPrintServer.GetPrintQueues(Local | Connections)` and polls every two seconds (configurable via `IPrinterService.PollingInterval`) to pick up queue additions, removals, and the user changing their default printer. The system default printer is rendered with a ★ in the dropdown.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:PrinterPicker
    Value="{Binding SelectedPrinter, Mode=TwoWay}" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `PrinterInfo?` | `null` | Selected printer (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when queue removed |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach by queue full name |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first printer |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<PrinterInfo?>` | Selected printer changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<PrinterInfo?>` | Bound queue removed |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<PrinterInfo?>` | Queue reappeared by full name |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Tracked printer changed state |

## Notes

- `Value.IsDefault` flags the system default printer at the moment of enumeration; a poll-tick after the user changes the default will refresh it.
- `Value.IsShared`, `Value.IsLocal`, and `Value.QueueStatus` are sourced from `System.Printing.PrintQueue`. `QueueStatus` is the `PrintQueueStatus` flags converted to its string form (e.g. `None`, `Offline`, `PaperJam`).
- `Value.DeviceId` is the queue's `FullName` (`PrinterName` for local queues, `\\server\share` for shared/networked queues), suitable for round-tripping back through `PrintQueue` lookup.
- Print spooler hangs / WMI permission issues are caught silently; offending queues are simply skipped for the tick.

## Related Controls

- `LabelPrinterPicker` — labeled wrapper with validation
- `NetworkAdapterPicker` — sibling system-resource picker

## Sample Application Path

`SamplesWpfHardware → Pickers → PrinterPicker`