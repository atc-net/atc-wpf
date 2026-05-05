# Hardware Pickers

Hardware picker controls for selecting connected serial ports, USB devices, USB cameras and audio endpoints with **live device-state detection** (connect, disconnect, in-use).

## Table of Contents

- [Overview](#overview)
- [Quick Reference](#quick-reference)
- [Device-State UX](#device-state-ux)
- [Common Properties](#common-properties)
- [Localization](#localization)
- [Target Framework](#target-framework)

## Overview

`Atc.Wpf.Hardware` provides seven picker controls, each with a labeled variant:

- `SerialPortPicker` / `LabelSerialPortPicker` — select a serial / COM port
- `UsbPortPicker` / `LabelUsbPortPicker` — select a connected USB device
- `UsbCameraPicker` / `LabelUsbCameraPicker` — select a video-capture device
- `AudioInputPicker` / `LabelAudioInputPicker` — select a microphone / capture endpoint
- `AudioOutputPicker` / `LabelAudioOutputPicker` — select speakers / a render endpoint
- `DrivePicker` / `LabelDrivePicker` — select a logical drive (fixed / removable / network / CD)
- `TimeZonePicker` / `LabelTimeZonePicker` — select a system time zone (no hot-plug)

The five hardware-backed pickers share infrastructure (`DeviceWatcherHost`) using `Windows.Devices.Enumeration.DeviceWatcher`. `DrivePicker` polls `System.IO.DriveInfo.GetDrives()` for hot-plug. `TimeZonePicker` is a static list. Hot-plug, disconnect and in-use changes update the picker live — the user never has to click *Refresh* and never picks a device that's silently unusable.

## Quick Reference

| Control | Selected Value Type | Source API |
|---------|--------------------|------------|
| `SerialPortPicker` | `SerialPortInfo?` | `Windows.Devices.SerialCommunication.SerialDevice` |
| `UsbPortPicker` | `UsbDeviceInfo?` | `DeviceInformation` filtered by USB device interface class (or `UsbDeviceClassFilter` flags → AQS) |
| `UsbCameraPicker` | `UsbCameraInfo?` | `DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)` |
| `AudioInputPicker` | `AudioDeviceInfo?` | `DeviceInformation.FindAllAsync(DeviceClass.AudioCapture)` |
| `AudioOutputPicker` | `AudioDeviceInfo?` | `DeviceInformation.FindAllAsync(DeviceClass.AudioRender)` |
| `DrivePicker` | `DiskDriveInfo?` | `System.IO.DriveInfo.GetDrives()` + 2 s polling |
| `TimeZonePicker` | `TimeZoneInfo?` | `TimeZoneInfo.GetSystemTimeZones()` (static) |

## Device-State UX

Each item in a picker dropdown has a coloured status dot:

| State | Dot | Trigger |
|-------|-----|---------|
| `Available` | 🟢 green | enumerated, free to use |
| `JustConnected` | 🟢 green | hot-plugged within last 3 s, briefly highlighted |
| `InUse` | 🟠 amber | held by another process / interface disabled |
| `Disconnected` | 🔴 red | was selected, now physically gone |

When a *bound* `Value` device disconnects:

- An inline warning appears under the dropdown ("Device disconnected").
- The picker raises `DeviceLost`.
- The `Value` is **kept** by default (preserves user intent through USB jiggle); set `ClearValueOnDisconnect="True"` to opt out.
- When the same device reappears, the picker auto-rebinds and raises `DeviceReconnected` (turn off via `AutoRebindOnReconnect="False"`).

## Common Properties

These properties exist on all three pickers (and their `Label*` wrappers):

| Property | Type | Default | Notes |
|----------|------|---------|-------|
| `Value` | `T?` | `null` | The selected device (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Always-visible 🔄 button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` events |
| `DetectInUseState` | `bool` | `false` | Opt-in active probe (Serial / USB only) |
| `ClearValueOnDisconnect` | `bool` | `false` | Preserve user intent by default |
| `AutoRebindOnReconnect` | `bool` | `true` | Match by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Avoid surprise rebinding |
| `ItemTemplate` | `DataTemplate?` | default | Override status dot + name layout |

## Localization

User-visible strings ("Refresh", "Available", "In use", "Disconnected", validation messages) ship as `.resx` triplets:

- `en-US` (invariant)
- `da-DK`
- `de-DE`

Set `Thread.CurrentThread.CurrentUICulture` before showing the picker to switch language.

## Target Framework

`Atc.Wpf.Hardware` targets `net10.0-windows10.0.19041.0` (Windows 10 May 2020 / 2004 and later, all Windows 11). This is required for `Windows.Devices.Enumeration.DeviceWatcher` and the WinRT projection. Consumers need the same TFM or higher.