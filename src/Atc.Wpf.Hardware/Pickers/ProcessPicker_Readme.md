# 🔍 ProcessPicker

An inspection-style picker for selecting a running OS process.

## Overview

Enumerates `System.Diagnostics.Process.GetProcesses()` and polls every two seconds (configurable via `IProcessService.PollingInterval`) so the list reflects the current OS state. By default the list shows only processes with a main window — flip `IProcessService.OnlyWithMainWindow=false` to surface every running process. Useful for hooking debuggers, automation, capture targets, or "attach to existing" flows.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:ProcessPicker
    AutoRefreshOnDeviceChange="True"
    Value="{Binding SelectedProcess, Mode=TwoWay}" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `RunningProcessInfo?` | `null` | Selected process (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when the process exits |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach by PID (rare; PIDs aren't reused immediately) |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first process |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<RunningProcessInfo?>` | Selected process changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<RunningProcessInfo?>` | Bound process exited |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<RunningProcessInfo?>` | Same PID reappeared (rare) |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Tracked process state changed |

## Notes

- This is an inspection-style picker, not a hardware picker. It polls; there is no DeviceWatcher.
- Some processes (system, protected, anti-malware, etc.) deny metadata access — those rows quietly skip module-path lookup.
- `Value.MainModulePath` may be `null` even for surfaced processes; do not assume it's always set.
- `Value.DeviceId` is the PID as an invariant string; consumers should look up the live `Process` via `Process.GetProcessById` rather than holding the cached snapshot.

## Related Controls

- `LabelProcessPicker` — labeled wrapper with mandatory + exit-detection validation
- `WindowPicker` — sibling inspection picker for top-level OS windows

## Sample Application Path

`SamplesWpfHardware → Pickers → ProcessPicker`