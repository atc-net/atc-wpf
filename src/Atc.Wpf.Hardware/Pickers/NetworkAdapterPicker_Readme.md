# 🌐 NetworkAdapterPicker

A control for selecting a network interface (Ethernet, Wi-Fi, virtual, loopback).

## Overview

Enumerates network adapters via `System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()` and polls every two seconds (configurable via `INetworkAdapterService.PollingInterval`) so the picker reflects cable / Wi-Fi state changes live. Loopback adapters are hidden by default — flip `INetworkAdapterService.IncludeLoopback=true` to surface them. Useful for "bind to interface" / "send via this NIC" / multi-homed app flows.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:NetworkAdapterPicker
    Value="{Binding SelectedAdapter, Mode=TwoWay}" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `NetworkAdapterInfo?` | `null` | Selected adapter (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ShowRefreshButton` | `bool` | `true` | Show inline ↻ button |
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Start the polling loop |
| `ClearValueOnDisconnect` | `bool` | `false` | Drop `Value` when adapter disappears |
| `AutoRebindOnReconnect` | `bool` | `true` | Re-attach on reconnect by adapter id |
| `AutoSelectFirstAvailable` | `bool` | `false` | Auto-select first adapter |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<NetworkAdapterInfo?>` | Selected adapter changed |
| `DeviceLost` | `RoutedPropertyChangedEventArgs<NetworkAdapterInfo?>` | Bound adapter removed (e.g. VPN disconnect) |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<NetworkAdapterInfo?>` | Adapter id reappeared |
| `DeviceStateChanged` | `DeviceStateChangedRoutedEventArgs` | Tracked adapter changed state |

## Notes

- `Value.AdapterType` is the standard `System.Net.NetworkInformation.NetworkInterfaceType` (Ethernet, Wireless80211, Tunnel, Loopback…).
- `Value.OperationalStatus` is reactive (INPC) — bind to it if you want live up/down indication for the bound adapter without re-selecting.
- `Value.MacAddress` and `Value.Speed` may be empty/null on virtual adapters where the OS doesn't expose them.
- Polling rather than `NetworkChange.NetworkAvailabilityChanged` keeps the model simple and uniformly catches "adapter added/removed" alongside up/down transitions.

## Related Controls

- `LabelNetworkAdapterPicker` — labeled wrapper with validation
- `PrinterPicker` — sibling system-resource picker
- `NetworkScanner` (in `Atc.Wpf.Network`) — scans the network *behind* the selected adapter

## Sample Application Path

`SamplesWpfHardware → Pickers → NetworkAdapterPicker`