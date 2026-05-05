# 🔊 AudioOutputPicker

A control for selecting an audio-render device (speakers, headphones, USB headset, monitor speakers, etc.).

## Overview

Enumerates audio-render endpoints via WinRT `DeviceInformation.FindAllAsync(DeviceClass.AudioRender)` and listens for hot-plug events through a shared `DeviceWatcherHost`. The selected `DeviceId` is the WASAPI endpoint id, ready to be passed to `MediaPlayer`, `AudioGraph`, or NAudio's `MMDevice` lookup for downstream playback.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:AudioOutputPicker
    Value="{Binding SelectedSpeakers, Mode=TwoWay}" />

<atc:AudioOutputPicker
    AutoSelectFirstAvailable="True"
    WatermarkText="Select audio output…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `AudioDeviceInfo?` | `null` | Selected audio output (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribe to `DeviceWatcher` |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when device unplugs |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by `DeviceId` |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first device on first appear |
| `ShowLivePreview` | `bool` | `false` | Show a Test/Stop button + waveform pane below the dropdown for the selected speakers |
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

- `Value.IsDefault` reflects the system's current default render endpoint when the OS marks it so; the default is rendered with a ★ in the dropdown's `ToString()`.
- `Value.DeviceId` is the WASAPI endpoint id — pass to `MediaPlayer.AudioDeviceOutput`, `AudioGraphSettings.PrimaryRenderDevice`, or look up via `MMDeviceEnumerator` if you're using NAudio.
- Active in-use probing is not performed for audio (would require opening the endpoint, which is intrusive). Passive detection still maps a disabled interface (`IsEnabled == false`) to `DeviceState.InUse`.
- **Live preview** (`ShowLivePreview="True"`) shows a `Test` / `Stop` button next to a waveform pane. Pressing `Test` opens the selected speakers via `AudioGraph` (`PrimaryRenderDevice` set to the selected device) and feeds a 1 kHz sine through `AudioFrameInputNode` for ~3 seconds — 1 second on the left channel, 1 second on the right, 1 second stereo, with a 30 ms fade in/out at every segment boundary to suppress click artefacts. The same buffer feeds the waveform display, so the picker visualises exactly what's being sent to the device. Errors (device held by another process, format mismatch, etc.) surface as a localized inline "Audio preview unavailable" message; output rendering doesn't trigger a permission prompt.

## Related Controls

- `LabelAudioOutputPicker` — labeled wrapper with validation
- `AudioInputPicker` — capture / microphone picker
- `UsbCameraPicker` — video-capture picker

## Sample Application Path

`SamplesWpfHardware → Pickers → AudioOutputPicker`