# 🎤 AudioInputPicker

A control for selecting an audio-capture device (microphone, line-in, USB headset mic, etc.).

## Overview

Enumerates audio-capture endpoints via WinRT `DeviceInformation.FindAllAsync(DeviceClass.AudioCapture)` and listens for hot-plug events through a shared `DeviceWatcherHost`. The selected `DeviceId` round-trips cleanly to `MediaCaptureInitializationSettings.AudioDeviceId` for downstream `MediaCapture` use, or to NAudio / `MMDevice` consumers via the WASAPI endpoint id.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:AudioInputPicker
    Value="{Binding SelectedMicrophone, Mode=TwoWay}" />

<atc:AudioInputPicker
    AutoSelectFirstAvailable="True"
    WatermarkText="Select audio input…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `AudioDeviceInfo?` | `null` | Selected audio input (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when device unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first device on first appear |
| `ShowLivePreview` | `bool` | `false` | Show a live waveform + peak-level pane below the dropdown for the selected microphone |
| `PreviewHeight` | `double` | `120` | Height of the preview pane in DIPs (only meaningful when `ShowLivePreview="True"`) |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<AudioDeviceInfo?>` | Selected device changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<AudioDeviceInfo?>` | Bound device disconnected |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<AudioDeviceInfo?>` | Lost device reappeared |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Any tracked device's state changed |

## Notes

- `Value.IsDefault` reflects the system's current default capture endpoint when the OS marks it so; the default is rendered with a ★ in the dropdown's `ToString()`.
- `Value.DeviceId` is the WinRT device ID — pass to `MediaCaptureInitializationSettings.AudioDeviceId` to open the device for capture.
- Active in-use probing is not performed for audio (would require opening the endpoint, which is intrusive). Passive detection still maps a disabled interface (`IsEnabled == false`) to `DeviceState.InUse`.
- **Live preview** (`ShowLivePreview="True"`) opens the selected microphone via WinRT `AudioGraph` + `AudioDeviceInputNode` + `AudioFrameOutputNode` and renders a scrolling waveform + side peak-level bar at ~30 fps. The first activation triggers Windows' microphone-permission prompt; if the user denies access or the device is held by another process, the pane shows a localized inline message ("Microphone access denied" / "Audio preview unavailable") instead of crashing the picker. Restarts on `Value` changes; stops cleanly on `Unloaded` / `ShowLivePreview="False"` / `Dispose`.

## Related Controls

- `LabelAudioInputPicker` — labeled wrapper with validation
- `AudioOutputPicker` — playback / render endpoint picker
- `UsbCameraPicker` — video-capture picker

## Sample Application Path

`SamplesWpfHardware → Pickers → AudioInputPicker`