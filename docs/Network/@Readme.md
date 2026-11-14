# Network Controls

WPF views for network scanning, discovery and diagnostics, built on the [Atc.Network](https://github.com/atc-net/atc-network) library (2.x).

## Table of Contents

- [Overview](#overview)
- [Setup](#setup)
- [Quick Reference](#quick-reference)
- [Sample Application](#sample-application)

## Overview

`Atc.Wpf.Network` contains ready-made MVVM views: each view binds to its own view model (for example `NetworkScannerView` to `NetworkScannerViewModel`), so an app adds a view, sets its `DataContext` and handles the events it cares about.

- **Scanner** - scan IPv4 ranges, CIDR blocks or the local networks for hosts, with ping quality, host names, MAC vendors, open ports, mDNS/SSDP/ONVIF/NetBIOS details, presets, export and row actions
- **Discovery** - live mDNS, SSDP and ONVIF devices without a range scan
- **ONVIF** - read one camera's device information, RTSP streams and network interfaces
- **Diagnostics** - traceroute, Wake-on-LAN and NTP time
- **Clients** - TCP and UDP test clients with a terminal
- **VNC** - a remote viewer with scale modes, view-only and a shared clipboard

## Setup

The views are built from the Atc form controls, so merge both dictionaries into `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

`Atc.Wpf.Network` has no XAML namespace mapping. Map each folder you use with a `clr-namespace`:

```xml
<UserControl
    xmlns:scanner="clr-namespace:Atc.Wpf.Network.Scanner;assembly=Atc.Wpf.Network"
    xmlns:diagnostics="clr-namespace:Atc.Wpf.Network.Diagnostics;assembly=Atc.Wpf.Network">
    <scanner:NetworkScannerView DataContext="{Binding NetworkScanner}" />
</UserControl>
```

## Quick Reference

| Control | Namespace | What | Readme |
|---------|-----------|------|--------|
| `NetworkScannerView` | `Atc.Wpf.Network.Scanner` | Range scan with a sortable, filterable host grid | [NetworkScannerView](../../src/Atc.Wpf.Network/Scanner/NetworkScannerView_Readme.md) |
| `NetworkScannerSettingsView` | `Atc.Wpf.Network.Scanner` | Scan options (range, ports, discovery options, timeouts, result filters) | [Atc.Wpf.Network overview](../../src/Atc.Wpf.Network/Readme.md) |
| `NetworkHostDetailsView` | `Atc.Wpf.Network.Scanner` | mDNS, SSDP, ONVIF and NetBIOS details of one host | [Atc.Wpf.Network overview](../../src/Atc.Wpf.Network/Readme.md) |
| `ServiceDiscoveryView` | `Atc.Wpf.Network.Discovery` | Devices that announce themselves with mDNS, SSDP or ONVIF | [ServiceDiscoveryView](../../src/Atc.Wpf.Network/Discovery/ServiceDiscoveryView_Readme.md) |
| `OnvifCameraView` | `Atc.Wpf.Network.Onvif` | An ONVIF camera's model and RTSP streams | [OnvifCameraView](../../src/Atc.Wpf.Network/Onvif/OnvifCameraView_Readme.md) |
| `TracerouteView` | `Atc.Wpf.Network.Diagnostics` | Route to a host, hop by hop | [TracerouteView](../../src/Atc.Wpf.Network/Diagnostics/TracerouteView_Readme.md) |
| `WakeOnLanView` / `WakeOnLanButton` | `Atc.Wpf.Network.Diagnostics` | Wake a device by its MAC address | [WakeOnLanView](../../src/Atc.Wpf.Network/Diagnostics/WakeOnLanView_Readme.md) |
| `NtpTimeView` | `Atc.Wpf.Network.Diagnostics` | Server time and local clock offset | [NtpTimeView](../../src/Atc.Wpf.Network/Diagnostics/NtpTimeView_Readme.md) |
| `TcpClientView` | `Atc.Wpf.Network.Clients` | TCP test client with TLS and message framing | [TcpClientView](../../src/Atc.Wpf.Network/Clients/TcpClientView_Readme.md) |
| `UdpClientView` | `Atc.Wpf.Network.Clients` | UDP test client | [UdpClientView](../../src/Atc.Wpf.Network/Clients/UdpClientView_Readme.md) |
| `VncViewerView` | `Atc.Wpf.Network.Vnc` | VNC remote viewer | [VncViewerView](../../src/Atc.Wpf.Network/Vnc/VncViewerView_Readme.md) |

Value converters (`Atc.Wpf.Network.ValueConverters`): `ConnectionStateToBrushValueConverter`, `ConnectionStateToColorValueConverter`, `ConnectionStateToTextValueConverter`, `IpStatusToLocalizedDescriptionValueConverter` and `NetworkQualityCategoryTypeToResourceImageValueConverter`.

## Sample Application

Every view has a sample under **Wpf.Network** in the sample app (`sample/Atc.Wpf.Sample`): NetworkScanner, OnvifCamera, ServiceDiscovery, VncViewer, Clients (TcpClient, UdpClient), Diagnostics (NtpTime, Traceroute, WakeOnLan) and ValueConverters.
