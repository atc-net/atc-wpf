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
| `ShowLivePreview` | `bool` | `false` | Show a live preview pane below the dropdown for the selected camera |
| `PreviewHeight` | `double` | `240` | Height of the preview pane in DIPs (only meaningful when `ShowLivePreview="True"`) |
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
- **Live preview** (`ShowLivePreview="True"`) opens the selected camera via `MediaCapture` + `MediaFrameReader` and renders frames into a `WriteableBitmap` (no `AllowUnsafeBlocks` required). The first activation triggers the OS webcam-permission prompt; if the user denies access or the camera is held by another process, the preview pane shows a localized error ("Camera access denied" / "Camera preview unavailable") instead of crashing the picker. The preview restarts when `Value` changes and stops on `Unloaded` / `ShowLivePreview="False"` / `Dispose`.
- Resolution/FPS pickers and an explicit active "Test camera" probe remain v2 — opening `MediaCapture` at a non-default format is its own design discussion.

## Related Controls

- `LabelUsbCameraPicker` — labeled wrapper with validation
- `UsbPortPicker` — generic USB device picker
- `SerialPortPicker` — serial / COM port picker

## Sample Application Path

`SamplesWpfHardware → Pickers → UsbCameraPicker`