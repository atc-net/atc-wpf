# Network Controls

Network controls provide specialized WPF controls for network scanning and host discovery, built on the [Atc.Network](https://github.com/atc-net/atc-network) library.

## Table of Contents

- [Network Controls](#network-controls)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Installation](#installation)
  - [Quick Start](#quick-start)
  - [Controls](#controls)
    - [NetworkScannerView](#networkscannerview)
      - [Features](#features)
      - [Properties](#properties)
      - [Events](#events)
      - [Usage Example](#usage-example)
    - [NetworkScannerSettingsView](#networkscannersettingsview)
      - [Features](#features-1)
      - [Usage Example](#usage-example-1)
      - [Complete Example with Both Controls](#complete-example-with-both-controls)
  - [ViewModels](#viewmodels)
    - [NetworkScannerViewModel](#networkscannerviewmodel)
      - [Properties](#properties-1)
      - [Commands](#commands)
      - [Events](#events-1)
    - [NetworkHostViewModel](#networkhostviewmodel)
      - [Properties](#properties-2)
    - [NetworkScannerColumnsViewModel](#networkscannercolumnsviewmodel)
      - [Properties](#properties-3)
    - [NetworkScannerFilterViewModel](#networkscannerfilterviewmodel)
      - [Properties](#properties-4)
  - [Value Converters](#value-converters)
    - [IpStatusToLocalizedDescriptionValueConverter](#ipstatustolocalizeddescriptionvalueconverter)
    - [NetworkQualityCategoryTypeToResourceImageValueConverter](#networkqualitycategorytypetoresourceimagevalueconverter)
  - [Configuration Examples](#configuration-examples)
    - [Basic Scanner Setup](#basic-scanner-setup)
    - [Custom Port Scanning](#custom-port-scanning)
    - [Handling Selection Changes](#handling-selection-changes)
    - [Column Visibility Control](#column-visibility-control)
    - [Filter Configuration](#filter-configuration)
  - [Localization](#localization)
  - [Theming](#theming)
  - [Summary](#summary)

## Overview

The `Atc.Wpf.Network` package provides a ready-to-use network scanner control that can:

- Scan IP address ranges for active hosts
- Perform ping tests with quality indicators
- Resolve hostnames via DNS
- Detect MAC addresses and vendor information
- Scan for open ports on discovered hosts
- Display results in a sortable, filterable grid

More controls in the package (each has its own readme next to it):

| Control | Namespace | What |
|---------|-----------|------|
| `ServiceDiscoveryView` | `Atc.Wpf.Network.Discovery` | Devices that announce themselves with mDNS, SSDP or ONVIF, without a range scan |
| `OnvifCameraView` | `Atc.Wpf.Network.Onvif` | Reads an ONVIF camera's model and RTSP streams; `StreamSelected` hands the stream and account to the app |
| `TracerouteView` | `Atc.Wpf.Network.Diagnostics` | Route to a host, hop by hop |
| `WakeOnLanView` | `Atc.Wpf.Network.Diagnostics` | Wake a device by its MAC address |
| `NtpTimeView` | `Atc.Wpf.Network.Diagnostics` | Server time and local clock offset |
| `WakeOnLanButton` | `Atc.Wpf.Network.Diagnostics` | A button that wakes the device with its `MacAddress` |
| `TcpClientView` / `UdpClientView` | `Atc.Wpf.Network.Clients` | TCP (TLS, framing) and UDP test clients with a terminal |
| `NetworkHostDetailsView` | `Atc.Wpf.Network.Scanner` | mDNS, SSDP, ONVIF and NetBIOS details of one host |
| `VncViewerView` | `Atc.Wpf.Network.Vnc` | VNC remote viewer |

## Installation

Add the NuGet package to your project:

```xml
<PackageReference Include="Atc.Wpf.Network" Version="2.*" />
```

## Quick Start

1. Add the namespace to your XAML:

```xml
xmlns:network="clr-namespace:Atc.Wpf.Network;assembly=Atc.Wpf.Network"
```

2. Add the control to your view:

```xml
<network:NetworkScannerView DataContext="{Binding NetworkScanner}" />
```

3. Create the ViewModel:

```csharp
public class MyViewModel : ViewModelBase
{
    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254",
        PortsNumbers = [22, 80, 443, 3389]
    };
}
```

## Controls

### NetworkScannerView

The main control for displaying network scan results in a ListView with sortable columns.

### Features

- Real-time progress reporting with cancellation support
- Sortable columns (click header to sort)
- Configurable column visibility
- Built-in filtering (success only, open ports only, reachable only) plus your own `HostFilter`
- Scanner options: ping, host name, NetBIOS, MAC address and vendor, mDNS/SSDP and ONVIF discovery, timeouts
- Discovery columns: device name, workgroup, model, services and ONVIF (camera or device)
- Ranges from a local network, a CIDR range, or a single IPv6 address; port presets including IP cameras
- Row actions: open in browser, connect with VNC (`VncRequested`), Wake-on-LAN, scan again, copy
- Export the shown rows to CSV or JSON, and save/restore the settings (`GetSettings` / `ApplySettings`)
- Ping quality visualization with color-coded icons
- Localization support (English, Danish, German)
- Busy overlay during scanning operations

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `DataContext` | `NetworkScannerViewModel` | The ViewModel providing scan functionality |

### Events

Events are exposed through the `NetworkScannerViewModel`:

| Event | Type | Description |
|-------|------|-------------|
| `EntrySelected` | `EventHandler<NetworkHostSelectedEventArgs>` | Fired when a host entry is selected |
| `HostFound`, `HostUpdated`, `HostCompleted` | `EventHandler<NetworkHostEventArgs>` | Fired with the row and the raw `IPScanResult` |
| `ScanStarted`, `ScanCompleted` | `EventHandler`, `EventHandler<NetworkScanCompletedEventArgs>` | Fired around each scan |

### Usage Example

```xml
<UserControl
    xmlns:network="clr-namespace:Atc.Wpf.Network;assembly=Atc.Wpf.Network">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <!-- Settings -->
        <StackPanel Grid.Row="0" Orientation="Horizontal">
            <atc:LabelTextBox
                LabelText="Start IP"
                Text="{Binding NetworkScanner.StartIpAddress, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
            <atc:LabelTextBox
                LabelText="End IP"
                Text="{Binding NetworkScanner.EndIpAddress, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
            <Button Content="Scan" Command="{Binding NetworkScanner.ScanCommand}" />
            <Button Content="Clear" Command="{Binding NetworkScanner.ClearCommand}" />
        </StackPanel>

        <!-- Scanner Control -->
        <network:NetworkScannerView
            Grid.Row="1"
            DataContext="{Binding NetworkScanner}" />

        <!-- Selected Entry Info -->
        <TextBlock
            Grid.Row="2"
            Text="{Binding NetworkScanner.SelectedEntry.Hostname}" />
    </Grid>
</UserControl>
```

### NetworkScannerSettingsView

A reusable settings panel control for configuring network scan parameters. This control provides input fields for IP range, ports, filter options, and action buttons.

#### Features

- Local network picker (one entry per adapter network) and a CIDR range box (`192.168.1.0/24`)
- Start IP / End IP input fields; the start can also be a single IPv6 address with no end
- Port preset picker (well-known, web, remote, file sharing, IP cameras, or your own) and a ports input field (comma-separated)
- Option checkboxes: ping, host name (DNS), NetBIOS name, MAC address, MAC vendor, services (mDNS, SSDP), cameras (ONVIF)
- "Show only success", "Show only with open ports" and "Show only reachable" filter checkboxes
- Scan / Clear action buttons
- Entries count display
- Elapsed time display
- Localization support (English, Danish, German)

#### Usage Example

```xml
<GroupBox Header="Settings" Padding="10">
    <network:NetworkScannerSettingsView DataContext="{Binding NetworkScanner}" />
</GroupBox>
```

The control expects a `NetworkScannerViewModel` as its DataContext. Configure initial values through the ViewModel:

```csharp
public class MyViewModel : ViewModelBase
{
    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254",
        PortsNumbers = [22, 80, 443]
    };
}
```

#### Complete Example with Both Controls

```xml
<UserControl xmlns:network="clr-namespace:Atc.Wpf.Network;assembly=Atc.Wpf.Network">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- Settings Panel -->
        <GroupBox Grid.Row="0" Header="Settings" Padding="10">
            <network:NetworkScannerSettingsView DataContext="{Binding NetworkScanner}" />
        </GroupBox>

        <!-- Scanner Results -->
        <GroupBox Grid.Row="1" Header="Results" Padding="10">
            <network:NetworkScannerView DataContext="{Binding NetworkScanner}" />
        </GroupBox>
    </Grid>
</UserControl>
```

## ViewModels

### NetworkScannerViewModel

The main ViewModel that controls the network scanning operation.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StartIpAddress` | `string?` | `null` | Starting IP address for the scan range |
| `EndIpAddress` | `string?` | `null` | Ending IP address for the scan range |
| `PortsNumbers` | `List<ushort>` | `[]` | List of port numbers to scan |
| `PortsNumbersText` | `string` | `""` | Port numbers as comma-separated text (for binding) |
| `PortPresets` | `ObservableCollection<NetworkPortPreset>` | built-in presets | Port presets; add your own |
| `SelectedPortPreset` / `SelectedPortPresetKey` | `NetworkPortPreset?` / `string?` | `null` | Selecting a preset sets `PortsNumbers` |
| `RangeCidr` | `string?` | `null` | A CIDR range such as `192.168.1.0/24`; a valid value sets the start and end address |
| `LocalNetworks` | `ObservableCollection<LocalNetwork>` | this machine's networks | IPv4 networks of the adapters that are up (no loopback or link-local) |
| `SelectedLocalNetworkKey` | `string?` | `null` | CIDR of the selected local network; selecting one sets the range |
| `SelectedEntry` | `NetworkHostViewModel?` | `null` | Currently selected host entry |
| `Entries` | `ObservableCollectionEx<NetworkHostViewModel>` | `[]` | Collection of discovered hosts |
| `Columns` | `NetworkScannerColumnsViewModel` | (default) | Column visibility settings |
| `Filter` | `NetworkScannerFilterViewModel` | (default) | Filter settings |
| `Options` | `NetworkScannerOptionsViewModel` | (default) | What the scanner checks, and its timeouts |
| `ConfigureScanner` | `Action<IPScannerConfig>?` | `null` | Adjusts the scanner configuration after `Options` is applied |
| `ScannerFactory` | `Func<IPScannerConfig, IIPScanner>` | `IPScanner` | Creates the scanner for each scan (replace it in tests) |
| `Logger` | `ILogger?` | `null` | Logger for the default scanner |
| `MacAddressVendorLookup` | `IMacAddressVendorLookup?` | `null` | Vendor lookup for the default scanner; `null` looks up online at macvendors.com |
| `HostFilter` | `Func<NetworkHostViewModel, bool>?` | `null` | Extra row filter; hosts it returns `false` for are hidden |
| `OpenUrl` | `Action<Uri>` | default browser | How "Open in browser" opens a web interface |
| `WakeOnLanSender` | `Func<string, CancellationToken, Task>` | `WakeOnLanHelper` broadcast | How "Wake-on-LAN" sends the magic packet |
| `IsBusy` | `bool` | `false` | Indicates if a scan is in progress |
| `BusyIndicatorMaximumValue` | `int` | `0` | Maximum value for progress indicator |
| `BusyIndicatorCurrentValue` | `int` | `0` | Current value for progress indicator |
| `BusyIndicatorPercentageValue` | `string` | `""` | Percentage text for progress indicator |
| `CompletionTime` | `string?` | `null` | Time taken to complete the scan |
| `EntryCountInfo` | `string` | `"0 / 0"` | Display string showing filtered/total count |

#### Commands

| Command | Description |
|---------|-------------|
| `ScanCommand` | Starts the network scan (supports cancellation); `ScanAsync(CancellationToken)` does the same from code |
| `ScanCancelCommand` | Cancels an in-progress scan |
| `ClearCommand` | Clears all scan results |
| `SortCommand` | Sorts the results by clicked column |
| `FilterChangeCommand` | Applies current filter settings |
| `OpenInBrowserCommand` | Opens the selected host's first web interface (`WebUrls`) |
| `ConnectWithVncCommand` | Raises `VncRequested` with the selected host's VNC port (5900-5903) |
| `WakeOnLanCommand` | Sends a Wake-on-LAN packet to the selected host's MAC address |
| `RescanHostCommand` | Scans the selected host again and updates its row |
| `CopyAllEntriesCommand` | Copies the shown rows as CSV |
| `ExportCommand` | Saves the shown rows as CSV or JSON (`NetworkScanExport`) |

#### Events

| Event | Type | Description |
|-------|------|-------------|
| `EntrySelected` | `EventHandler<NetworkHostSelectedEventArgs>` | Fired when selection changes |
| `HostFound` | `EventHandler<NetworkHostEventArgs>` | The first report for an address; its row was added |
| `HostUpdated` | `EventHandler<NetworkHostEventArgs>` | A later report for an address |
| `HostCompleted` | `EventHandler<NetworkHostEventArgs>` | The scanner is done with an address (discovery data may still follow, see below) |
| `ScanStarted` | `EventHandler` | A scan started |
| `ScanCompleted` | `EventHandler<NetworkScanCompletedEventArgs>` | A scan ended: `ScanResults`, `IsCancelled`, `ErrorMessage`. Every report is applied to the rows first |
| `VncRequested` | `EventHandler<NetworkHostConnectEventArgs>` | The user asked to connect with VNC (`Host`, `Port`); open a `VncViewerView` |

### NetworkHostViewModel

Represents a single discovered network host.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IpAddress` | `IPAddress` | The IP address of the host |
| `IpAddressSortable` | `string` | Zero-padded IP for proper sorting |
| `PingStatus` | `PingStatusResult?` | Ping result including status and latency |
| `PingQualityCategoryToolTip` | `string?` | Tooltip text for ping quality icon |
| `Hostname` | `string?` | Resolved hostname |
| `MacAddress` | `string?` | MAC address (if available) |
| `MacVendor` | `string?` | Vendor name from MAC address lookup |
| `OpenPortNumbers` | `IEnumerable<ushort>` | List of open ports discovered |
| `OpenPortNumbersAsCommaList` | `string` | Open ports as comma-separated string |
| `TimeDiff` | `string?` | Time taken to scan this host |
| `HasConnection` | `bool` | Whether the host answered in any way (ping, host name, MAC, open port, NetBIOS, discovery) |
| `DeviceName` | `string?` | DNS host name, else mDNS host name, else NetBIOS name, else ONVIF name |
| `Workgroup` | `string?` | NetBIOS workgroup |
| `Model` | `string?` | ONVIF hardware, else mDNS `ty`/`md`/`model`, else the mDNS instance name |
| `Services` / `ServicesAsCommaList` | `IReadOnlyList<string>` / `string` | mDNS service types, plus `UPnP` when SSDP answered |
| `IsOnvifDevice` / `IsOnvifCamera` | `bool` | ONVIF answered / it is a camera or video encoder |
| `OnvifDeviceServiceUrl` | `Uri?` | ONVIF device service, for `OnvifClient` |
| `WebUrls` | `IReadOnlyList<Uri>` | Web interfaces: ports confirmed as HTTP/HTTPS, else open 80, 443, 8080, 8443 |
| `VncPort` | `int?` | First open VNC port (5900-5903) |
| `ScanResult` | `IPScanResult?` | The latest raw scan result, with mDNS, SSDP, ONVIF and NetBIOS data |

### NetworkScannerColumnsViewModel

Controls visibility of columns in the scanner grid.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowPingQualityCategory` | `bool` | `true` | Show/hide ping quality icon column |
| `ShowIpAddress` | `bool` | `true` | Show/hide IP address column |
| `ShowIpStatus` | `bool` | `true` | Show/hide IP status column |
| `ShowHostname` | `bool` | `true` | Show/hide hostname column |
| `ShowMacAddress` | `bool` | `true` | Show/hide MAC address column |
| `ShowMacVendor` | `bool` | `true` | Show/hide MAC vendor column |
| `ShowOpenPortNumbers` | `bool` | `true` | Show/hide open ports column |
| `ShowTotalInMs` | `bool` | `true` | Show/hide total time column |
| `ShowDeviceName` | `bool` | `true` | Show/hide device name column |
| `ShowWorkgroup` | `bool` | `false` | Show/hide NetBIOS workgroup column |
| `ShowModel` | `bool` | `true` | Show/hide model column |
| `ShowServices` | `bool` | `true` | Show/hide services column |
| `ShowOnvif` | `bool` | `true` | Show/hide ONVIF column |

### NetworkScannerFilterViewModel

Controls filtering of scan results.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowOnlySuccess` | `bool` | `true` | Show only hosts with successful ping |
| `ShowOnlyWithOpenPorts` | `bool` | `true` | Show only hosts with open ports |
| `ShowOnlyReachable` | `bool` | `false` | Show only hosts that answered in any way (`HasConnection`), including those found only by discovery |

### NetworkScannerOptionsViewModel

What the scanner checks on each address. The defaults keep the behaviour of earlier versions.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IcmpPing` | `bool` | `true` | Ping each address |
| `ResolveHostName` | `bool` | `true` | Reverse DNS. It has no timeout, so on networks without it a /24 can take minutes; turn it off and use NetBIOS or discovery for names |
| `ResolveNetBiosName` | `bool` | `false` | NetBIOS computer name and workgroup |
| `ResolveMacAddress` | `bool` | `true` | MAC address from the ARP table |
| `ResolveVendorFromMacAddress` | `bool` | `true` | Vendor of the MAC address (online unless `MacAddressVendorLookup` is set) |
| `DiscoverServices` | `bool` | `false` | mDNS and SSDP services (printers, cameras, speakers, …) |
| `DiscoverOnvif` | `bool` | `false` | ONVIF devices (IP cameras) |
| `TreatOpenPortsAsWebServices` | `IPServicePortExaminationLevel` | `WellKnown` | How open ports are checked for web services |
| `TimeoutPingInMs`, `TimeoutTcpInMs`, `TimeoutHttpInMs`, `TimeoutDiscoveryInMs`, `TimeoutNetBiosInMs` | `int` | Atc.Network defaults | Timeouts |

### NetworkHostDetailsView

Shows what discovery found for one host: mDNS services (type, instance, port), SSDP devices (type, server, description URL), ONVIF devices (name, hardware, device service URL), NetBIOS names and open ports. Its DataContext is a `NetworkHostViewModel`:

```xml
<network:NetworkHostDetailsView DataContext="{Binding NetworkScanner.SelectedEntry}" />
```

## Value Converters

### IpStatusToLocalizedDescriptionValueConverter

Converts `IPStatus` enum values to localized description strings.

```xml
<TextBlock Text="{Binding PingStatus.Status,
    Converter={StaticResource IpStatusToLocalizedDescriptionValueConverter}}" />
```

### NetworkQualityCategoryTypeToResourceImageValueConverter

Converts `NetworkQualityCategoryType` to the appropriate quality indicator image.

```xml
<Image Source="{Binding PingStatus.QualityCategory,
    Converter={StaticResource QualityCategoryImageConverter}}" />
```

Quality categories and their indicators:
- **Perfect** (< 10ms) - Green
- **Excellent** (< 30ms) - Light green
- **VeryGood** (< 50ms) - Yellow-green
- **Good** (< 100ms) - Yellow
- **Fair** (< 200ms) - Orange
- **Poor** (< 500ms) - Red-orange
- **VeryPoor** (>= 500ms) - Red

## Configuration Examples

### Basic Scanner Setup

```csharp
public class MainViewModel : ViewModelBase
{
    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254"
    };
}
```

### Custom Port Scanning

```csharp
// Scan specific ports
NetworkScanner.PortsNumbers = [22, 80, 443, 3389, 8080];

// Or use the text property for binding
NetworkScanner.PortsNumbersText = "22, 80, 443, 3389, 8080";
```

### Handling Selection Changes

```csharp
public MainViewModel()
{
    NetworkScanner.EntrySelected += OnEntrySelected;
}

private void OnEntrySelected(object? sender, NetworkHostSelectedEventArgs e)
{
    if (e.SelectedHost is not null)
    {
        Console.WriteLine($"Selected: {e.SelectedHost.IpAddress} - {e.SelectedHost.Hostname}");
    }
}
```

### Scanner Options and Offline Vendor Lookup

```csharp
NetworkScanner.Options.ResolveHostName = false;      // fast: no reverse DNS
NetworkScanner.Options.ResolveNetBiosName = true;
NetworkScanner.Options.DiscoverServices = true;      // mDNS and SSDP
NetworkScanner.Options.DiscoverOnvif = true;         // IP cameras

// Look up MAC vendors offline (IEEE files) instead of at macvendors.com
NetworkScanner.MacAddressVendorLookup = IeeeOuiMacAddressVendorLookup.FromFiles("oui.csv");
NetworkScanner.Logger = logger;

// Anything the options don't cover
NetworkScanner.ConfigureScanner = config => config.Timeout = TimeSpan.FromSeconds(5);
```

### Raw Results and Your Own Filter

mDNS, SSDP and ONVIF data is attached by Atc.Network once per scan, after the last address is done. The rows are updated with it when the scan returns: each row gets one more `HostUpdated`, before `ScanCompleted`. Read discovery data in `HostUpdated` or `ScanCompleted`, not only in `HostCompleted`.

```csharp
NetworkScanner.HostCompleted += (_, e) =>
{
    // e.ScanResult is the Atc.Network IPScanResult: MdnsServices, SsdpDevices, OnvifDevices, NetBiosStatus, ...
    if (e.ScanResult.OnvifDevices.Count > 0)
    {
        Console.WriteLine($"ONVIF device at {e.ScanResult.IPAddress}");
    }
};

// Show only the hosts your app cares about; the control has no domain logic of its own.
NetworkScanner.HostFilter = host => host.ScanResult?.OnvifDevices.Count > 0;

await NetworkScanner.ScanAsync(cancellationToken);
```

### Ranges and Port Presets

```csharp
NetworkScanner.RangeCidr = "192.168.1.0/24";                 // sets StartIpAddress and EndIpAddress
NetworkScanner.SelectedLocalNetworkKey = "192.168.1.0/24";   // one of NetworkScanner.LocalNetworks
NetworkScanner.StartIpAddress = "fe80::1";                   // a single IPv6 address (leave the end empty)

NetworkScanner.SelectedPortPresetKey = NetworkPortPreset.IpCamerasKey;
NetworkScanner.PortPresets.Add(new NetworkPortPreset("modbus", "Modbus", [502]));
```

### Saving the Settings

```csharp
// Save
var json = JsonSerializer.Serialize(NetworkScanner.GetSettings());

// Restore
NetworkScanner.ApplySettings(JsonSerializer.Deserialize<NetworkScannerSettings>(json)!);
```

### Column Visibility Control

```csharp
// Hide MAC-related columns
NetworkScanner.Columns.ShowMacAddress = false;
NetworkScanner.Columns.ShowMacVendor = false;
```

```xml
<!-- Or bind in XAML -->
<CheckBox
    Content="Show MAC Address"
    IsChecked="{Binding NetworkScanner.Columns.ShowMacAddress}" />
```

### Filter Configuration

```csharp
// Show all results (including failed pings and no open ports)
NetworkScanner.Filter.ShowOnlySuccess = false;
NetworkScanner.Filter.ShowOnlyWithOpenPorts = false;
```

```xml
<!-- Filter checkboxes -->
<CheckBox
    Content="Show only successful"
    IsChecked="{Binding NetworkScanner.Filter.ShowOnlySuccess}" />
<CheckBox
    Content="Show only with open ports"
    IsChecked="{Binding NetworkScanner.Filter.ShowOnlyWithOpenPorts}" />
```

## Localization

The control supports the following languages:

| Language | Culture Code |
|----------|--------------|
| English | en-US (default) |
| Danish | da-DK |
| German | de-DE |

Resource files are located in:
- `Resources/Resources.resx` (English)
- `Resources/Resources.da-DK.resx` (Danish)
- `Resources/Resources.de-DE.resx` (German)
- `Resources/EnumResources.resx` (Enum descriptions)

## Theming

The control integrates with `Atc.Wpf.Theming` for Light and Dark mode support:

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

## Summary

The Network Controls library provides:

- **NetworkScannerView** - A complete network scanner control with real-time updates
- **MVVM-ready** - Full ViewModel support with commands and data binding
- **Flexible configuration** - Column visibility, filtering, and port customization
- **Rich data display** - IP, hostname, MAC, vendor, ports, and ping quality
- **Cancellation support** - Long-running scans can be cancelled
- **Localization** - Built-in support for English, Danish, and German
- **Theme support** - Integrates with Light/Dark theming

All controls follow WPF best practices and support data binding, commanding, and MVVM patterns.
