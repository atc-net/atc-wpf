# 🖥️ DisplayPicker

A control for selecting a connected display / monitor.

## Overview

Enumerates monitors via the Win32 `EnumDisplayMonitors` callback (P/Invoked from `user32.dll`) and reads each one's `MONITORINFOEX` to capture device name, bounds, working area, and the primary-monitor flag. Polls every two seconds (configurable via `IDisplayService.PollingInterval`) to surface display hot-plug. The system primary monitor is rendered with a ★ in the dropdown alongside its resolution. Useful for "open on this display", "capture this monitor", or per-screen window placement flows.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:DisplayPicker
    Value="{Binding SelectedDisplay, Mode=TwoWay}" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `DisplayInfo?` | `null` | Selected display (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when display unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach by device name |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first display |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<DisplayInfo?>` | Selected display changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<DisplayInfo?>` | Bound display unplugged |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<DisplayInfo?>` | Display reappeared by device name |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Tracked display changed state |

## Notes

- `Value.Bounds` is the full virtual-screen rectangle (in device pixels at 100 % DPI as Windows reports them — *not* DIPs); `Value.WorkingArea` excludes the taskbar / docked AppBars.
- `Value.DeviceName` is the Win32 GDI device string (e.g. `\\.\DISPLAY1`) — stable enough across power-cycles for `AutoRebindOnReconnect` to work.
- `Value.IsPrimary` reflects the system primary monitor at the moment of enumeration; a poll-tick after the user changes the primary display refreshes it.
- `Value.Handle` is the HMONITOR handle. It's session-scoped — don't persist it.

## Related Controls

- `LabelDisplayPicker` — labeled wrapper with validation
- `WindowPicker` — sibling polling-based picker that uses similar `user32` P/Invoke

## Sample Application Path

`SamplesWpfHardware → Pickers → DisplayPicker`