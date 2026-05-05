# 🪟 WindowPicker

An inspection-style picker for selecting a top-level OS window.

## Overview

Enumerates top-level windows via the Win32 `EnumWindows` callback (P/Invoked from `user32.dll`) and polls every two seconds (configurable via `IWindowService.PollingInterval`). By default only visible windows with a non-empty title are surfaced — flip `IWindowService.OnlyVisibleWithTitle=false` to include hidden / titleless windows. Useful for screen-capture targets, focus / activate flows, accessibility tooling, or "send to window" automation.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:WindowPicker
    AutoRefreshOnDeviceChange="True"
    Value="{Binding SelectedWindow, Mode=TwoWay}" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `TopLevelWindowInfo?` | `null` | Selected window (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when the window is destroyed |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach by HWND (rare; HWNDs aren't stable) |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first window |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>` | Selected window changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>` | Bound window destroyed |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>` | Same HWND reappeared (rare) |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Tracked window state changed |

## Notes

- Inspection-style picker — polls via `EnumWindows`, no hot-plug events from the OS.
- HWNDs are not stable across window-destroy / window-create cycles; treat them as session-local handles. Persisting `Value.Handle` across app restarts is meaningless.
- `Value.ProcessName` is captured at first-sight; it does not refresh if the process exits and a new process inherits the HWND (which itself is rare).
- `Value.DeviceId` is the handle as an invariant string for automation-peer lookup; for actual interop, use `Value.Handle`.

## Related Controls

- `LabelWindowPicker` — labeled wrapper with mandatory + destruction-detection validation
- `ProcessPicker` — sibling inspection picker for OS processes

## Sample Application Path

`SamplesWpfHardware → Pickers → WindowPicker`