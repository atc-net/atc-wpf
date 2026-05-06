# 💾 DrivePicker

A control for selecting a logical drive (`C:\`, `D:\`, USB sticks, network shares, CD/DVD, etc.).

## Overview

Enumerates drives via `System.IO.DriveInfo.GetDrives()` and polls every two seconds (configurable via `IDriveService.PollingInterval`) to pick up hot-plugged removable drives. The selected `DeviceId` is the drive's root path (e.g. `E:\`), ready to be combined with a relative path or passed to `Path.Combine`.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:DrivePicker
    Value="{Binding SelectedDrive, Mode=TwoWay}" />

<atc:DrivePicker
    AutoSelectFirstAvailable="True"
    WatermarkText="Select drive…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `DiskDriveInfo?` | `null` | Selected drive (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when drive unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by drive root |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first drive on first appear |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<DiskDriveInfo?>` | Selected drive changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<DiskDriveInfo?>` | Bound drive removed |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<DiskDriveInfo?>` | Removed drive reappeared |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Any tracked drive changed state |

## Notes

- `Value.DriveType` exposes the `System.IO.DriveType` (Fixed, Removable, CDRom, Network, Ram).
- `Value.IsReady`, `Value.TotalSize`, `Value.AvailableFreeSpace` are populated only when the drive has media; CD/DVD without media reports `IsReady=false` and null sizes.
- Polling rather than `WqlEventQuery` keeps the service free of WMI dependencies; if you need sub-second hot-plug latency, drop `PollingInterval` to a smaller value.

## Related Controls

- `LabelDrivePicker` — labeled wrapper with validation
- `UsbPortPicker` — generic USB device picker (firmware-level)
- `TimeZonePicker` (in `Atc.Wpf.Forms`) — sibling system-metadata picker

## Sample Application Path

`SamplesWpfHardware → Pickers → DrivePicker`