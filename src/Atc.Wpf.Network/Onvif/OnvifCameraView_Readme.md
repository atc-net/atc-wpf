# 📹 OnvifCameraView

Reads one ONVIF camera: its manufacturer, model, firmware, serial number and RTSP streams, so the user picks a stream instead of typing its path.

## 🔍 Overview

`OnvifCameraView` takes a camera's ONVIF device service URL and ONVIF account, reads the camera with Atc.Network's `OnvifClient` (`GetDeviceInformation`, `GetStreams`, `GetNetworkInterfaces`) and lists every media profile with its resolution, encoding, frame rate and stream URL. **Use stream** raises `StreamSelected` with the stream and the account, for the app to add the camera.

The control has no camera domain logic and never stores the credentials: they come in through the view model and go out with `StreamSelected`.

## 📍 Namespace

```xml
xmlns:onvif="clr-namespace:Atc.Wpf.Network.Onvif;assembly=Atc.Wpf.Network"
```

## 🚀 Usage

```xml
<onvif:OnvifCameraView DataContext="{Binding OnvifCamera}" />
```

```csharp
public OnvifCameraViewModel OnvifCamera { get; } = new();

public MyViewModel()
{
    OnvifCamera.StreamSelected += (_, e) =>
    {
        // e.Stream.StreamUri is the RTSP URL without credentials; play it with e.Credential.
        AddCamera(e.Model, e.Stream.StreamUri, e.Credential);
    };
}
```

### From the Network Scanner

Scan with `Options.DiscoverOnvif = true`; hosts that answered ONVIF have `IsOnvifDevice` and `OnvifDeviceServiceUrl`:

```csharp
OnvifCamera.UseHost(NetworkScanner.SelectedEntry!);   // fills DeviceServiceUrl
await OnvifCamera.ReadCommand.ExecuteAsync(null);
```

## ⚙️ Properties

`OnvifCameraViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DeviceServiceUrl` | `string?` | `null` | ONVIF device service URL (`http` or `https`), such as `http://192.168.1.132:2020/onvif/device_service` |
| `UserName` / `Password` | `string?` | `null` | The camera's ONVIF account; the password is never logged or shown in errors |
| `Manufacturer`, `Model`, `FirmwareVersion`, `SerialNumber` | `string?` | `null` | From `GetDeviceInformation` |
| `Streams` | `ObservableCollection<OnvifStreamInfo>` | empty | Every profile with its RTSP URL |
| `SelectedStream` | `OnvifStreamInfo?` | first stream | The chosen stream |
| `NetworkInterfaces` | `ObservableCollection<OnvifNetworkInterface>` | empty | The camera's interfaces (MAC, IPv4 addresses, Wi-Fi or not); empty when the camera refuses |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | Wrong account, no answer, or another ONVIF error |
| `IsBusy` | `bool` | `false` | `true` while reading |

### Commands

| Command | Description |
|---------|-------------|
| `ReadCommand` | Reads the camera; enabled for an `http`/`https` URL |
| `UseStreamCommand` | Raises `StreamSelected` for `SelectedStream` |
| `CopyStreamUriCommand` | Copies the selected stream URL |

### Methods

| Method | Description |
|--------|-------------|
| `UseHost(NetworkHostViewModel)` | Takes the ONVIF device service URL of a scanned host |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `StreamSelected` | `OnvifStreamSelectedEventArgs` | `DeviceServiceUrl`, `Stream` (profile and RTSP URL), `Credential`, `Manufacturer`, `Model` |

## 📝 Notes

- ONVIF must be on in the camera; many cameras ship with it off (Hikvision: *Configuration → Network → Advanced → Integration Protocol*). TP-Link Tapo uses port 2020 and its *Camera Account*
- Stream URLs carry no credentials; add the account when playing them
- Replace the reader in tests: `new OnvifCameraViewModel(IOnvifCameraReader)`; the default `OnvifCameraReader` uses `OnvifClient`
- Atc.Network reads Media service 1 with WS-Security; see its [ONVIF docs](https://github.com/atc-net/atc-network/blob/main/docs/onvif.md)

## 🔗 Related Controls

- **NetworkScannerView** - Finds the cameras (`DiscoverOnvif`, `IsOnvifCamera`, `OnvifDeviceServiceUrl`)
- **NetworkHostDetailsView** - Shows the ONVIF devices a host announced

## 🎮 Sample Application

See the OnvifCamera sample in the Atc.Wpf.Sample application under **Wpf.Network > Network > OnvifCamera**.