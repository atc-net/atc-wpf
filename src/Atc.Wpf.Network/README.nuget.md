# Atc.Wpf.Network

Network controls for WPF, built on [Atc.Network](https://github.com/atc-net/atc-network):

- **Scanner** - `NetworkScannerView` and `NetworkScannerSettingsView`: scan IPv4 ranges, CIDR blocks or local networks, with ping, host names, MAC vendors, open ports, mDNS/SSDP/ONVIF/NetBIOS details, port presets, export and row actions
- **Discovery** - `ServiceDiscoveryView`: live mDNS, SSDP and ONVIF devices without a range scan
- **ONVIF** - `OnvifCameraView`: device information, streams with RTSP URLs and network interfaces of one camera
- **Diagnostics** - `TracerouteView`, `WakeOnLanView` / `WakeOnLanButton` and `NtpTimeView`
- **Clients** - `TcpClientView` and `UdpClientView` for sending and receiving text or hex, with TLS and message framing for TCP
- **VNC** - `VncViewerView` with scale modes, view-only and a shared clipboard

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Network
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

```xml
<UserControl xmlns:scanner="clr-namespace:Atc.Wpf.Network.Scanner;assembly=Atc.Wpf.Network">
    <scanner:NetworkScannerView DataContext="{Binding NetworkScanner}" />
</UserControl>
```

## Documentation

- [Network controls reference](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Network/Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
