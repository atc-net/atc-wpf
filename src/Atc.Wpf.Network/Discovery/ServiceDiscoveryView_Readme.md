# 📡 ServiceDiscoveryView

Lists the devices on the local network that announce themselves with mDNS, SSDP (UPnP) or ONVIF, without scanning an address range: printers, cameras, speakers, TVs, NAS boxes, smart-home hubs.

## 🔍 Overview

`ServiceDiscoveryView` asks the network once with the chosen protocols (at the same time, for `DurationInMs`) and lists one row per address as the answers arrive: device name, model, services and whether it is an ONVIF camera. The selected device's details (mDNS services, SSDP devices, ONVIF devices) are shown next to the list with `NetworkHostDetailsView`.

It finds devices on any subnet the machine's adapters are on, in a few seconds, and often names devices that reverse DNS doesn't know. It finds only devices that announce themselves; use `NetworkScannerView` to find everything in a range.

## 📍 Namespace

```xml
xmlns:discovery="clr-namespace:Atc.Wpf.Network.Discovery;assembly=Atc.Wpf.Network"
```

## 🚀 Usage

```xml
<discovery:ServiceDiscoveryView DataContext="{Binding ServiceDiscovery}" />
```

```csharp
public ServiceDiscoveryViewModel ServiceDiscovery { get; } = new()
{
    UseSsdp = false,       // only mDNS and ONVIF
    DurationInMs = 5000,
};

ServiceDiscovery.DeviceFound += (_, e) =>
{
    if (e.Host.IsOnvifCamera)
    {
        OnvifCamera.UseHost(e.Host);   // see OnvifCameraView
    }
};
```

## ⚙️ Properties

`ServiceDiscoveryViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `UseMdns` / `UseSsdp` / `UseOnvif` | `bool` | `true` | Which protocols to ask with |
| `DurationInMs` | `int` | `3000` | How long to listen for answers |
| `Devices` | `ObservableCollection<NetworkHostViewModel>` | empty | One row per address, sorted by address: `DeviceName`, `Model`, `Services`, `IsOnvifCamera`, `OnvifDeviceServiceUrl`, `ScanResult` with the raw answers |
| `SelectedDevice` | `NetworkHostViewModel?` | `null` | The selected device |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | A protocol that failed; the others still run |
| `IsBusy` | `bool` | `false` | `true` while listening |

### Commands

| Command | Description |
|---------|-------------|
| `DiscoverCommand` | Clears the list and asks again; enabled when a protocol is chosen |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `DeviceFound` | `NetworkHostEventArgs` | The first answer from an address; its row was added |
| `DiscoveryCompleted` | `EventArgs` | Listening ended |

## 📝 Notes

- mDNS devices are listed by their IPv4 addresses when they have any (one row per device), else by their IPv6 addresses
- Many cameras ship with ONVIF off; Hikvision cameras are still found through mDNS with their model
- Firewalls that block unicast replies to multicast queries hide the answers
- Replace the discovery in tests: `new ServiceDiscoveryViewModel(IServiceDiscoverer)`; the default `ServiceDiscoverer` uses Atc.Network's `MdnsDiscovery`, `SsdpDiscovery` and `OnvifDiscovery`

## 🔗 Related Controls

- **NetworkScannerView** - Scans an address range (`DiscoverServices` / `DiscoverOnvif` attach the same data to the scanned addresses)
- **NetworkHostDetailsView** - The details pane used here
- **OnvifCameraView** - Reads a camera's model and stream URLs

## 🎮 Sample Application

See the ServiceDiscovery sample in the Atc.Wpf.Sample application under **Wpf.Network > Network > ServiceDiscovery**.