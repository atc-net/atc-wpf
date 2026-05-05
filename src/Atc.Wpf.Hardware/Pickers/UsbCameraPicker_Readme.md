# 📷 UsbCameraPicker

A control for selecting a video-capture device (webcam, capture card, etc.).

## Overview

Enumerates video-capture devices via WinRT `DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)` and listens for hot-plug events through a shared `DeviceWatcherHost`. The selected `DeviceId` round-trips cleanly to `MediaCaptureInitializationSettings.VideoDeviceId` for downstream `MediaCapture` use.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:UsbCameraPicker
    Value="{Binding SelectedCamera, Mode=TwoWay}" />

<atc:UsbCameraPicker
    AutoSelectFirstAvailable="True"
    WatermarkText="Select camera…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `UsbCameraInfo?` | `null` | Selected camera (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when camera unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first camera on first appear |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<UsbCameraInfo?>` | Selected camera changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<UsbCameraInfo?>` | Bound camera disconnected |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<UsbCameraInfo?>` | Lost camera reappeared |

## Notes

- `Value.Panel` reports physical orientation (Front / Back / External) when the OS provides it.
- `Value.DeviceId` is the WinRT device ID — pass to `MediaCaptureInitializationSettings.VideoDeviceId` to open the camera.
- Live preview, resolution/FPS pickers, and active "Test camera" probing are deferred to v2 — opening `MediaCapture` is heavyweight and only worth doing on user request.

## Related Controls

- `LabelUsbCameraPicker` — labeled wrapper with validation
- `UsbPortPicker` — generic USB device picker
- `SerialPortPicker` — serial / COM port picker

## Sample Application Path

`SamplesWpfHardware → Pickers → UsbCameraPicker`