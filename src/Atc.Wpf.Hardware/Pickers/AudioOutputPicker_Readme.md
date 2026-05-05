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

## Related Controls

- `LabelAudioOutputPicker` — labeled wrapper with validation
- `AudioInputPicker` — capture / microphone picker
- `UsbCameraPicker` — video-capture picker

## Sample Application Path

`SamplesWpfHardware → Pickers → AudioOutputPicker`