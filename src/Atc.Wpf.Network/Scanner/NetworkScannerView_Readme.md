# 📡 NetworkScannerView

A view that lists the hosts found by scanning an IPv4 address range, with ping status, hostname, MAC address/vendor and open ports.

## 🔍 Overview

`NetworkScannerView` is a `UserControl` that renders the results of a `NetworkScannerViewModel` in a sortable `ListView`. The view itself has no properties of its own; all state lives in the view model, which uses `IPScanner` from `Atc.Network` to scan the range between `StartIpAddress` and `EndIpAddress`. It shows a progress indicator while scanning, supports cancellation, and offers context-menu commands for copying host details. The companion `NetworkScannerSettingsView` binds to the same view model and provides the range/port inputs and the scan/clear buttons.

For the complete library guide see `src/Atc.Wpf.Network/Readme.md`.

## 📍 Namespace

```csharp
using Atc.Wpf.Network.Scanner;
```

## 🚀 Usage

### Settings and Result Views

```xml
<UserControl
    xmlns:network="clr-namespace:Atc.Wpf.Network.Scanner;assembly=Atc.Wpf.Network">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <network:NetworkScannerSettingsView DataContext="{Binding NetworkScanner}" />
        <network:NetworkScannerView Grid.Row="1" DataContext="{Binding NetworkScanner}" />
    </Grid>
</UserControl>
```

### View Model

```csharp
public NetworkScannerViewModel NetworkScanner { get; } = new()
{
    StartIpAddress = "192.168.1.1",
    EndIpAddress = "192.168.1.254",
    PortsNumbers = [22, 80, 443],
};
```

### Reacting to Selection

```csharp
NetworkScanner.EntrySelected += (sender, e) =>
{
    var host = e.SelectedHost; // NetworkHostViewModel? (null when cleared)
};
```

### Hiding Columns

```csharp
NetworkScanner.Columns.ShowMacAddress = false;
NetworkScanner.Columns.ShowMacVendor = false;
```

## ⚙️ Properties

The view is driven by `NetworkScannerViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StartIpAddress` | `string?` | `null` | First IP address of the scan range |
| `EndIpAddress` | `string?` | `null` | Last IP address of the scan range |
| `PortsNumbers` | `List<ushort>` | `[]` | Ports to examine; when empty the scanner configuration's default ports are used |
| `PortsNumbersText` | `string` | `""` | Port numbers as text (comma, semicolon or space separated; invalid values are ignored) |
| `PortPresets` / `SelectedPortPresetKey` | `ObservableCollection<NetworkPortPreset>` / `string?` | built-in / `null` | Port presets (well-known, web, remote, file sharing, IP cameras); selecting one sets the ports |
| `RangeCidr` | `string?` | `null` | CIDR range such as `192.168.1.0/24`; sets the start and end address |
| `LocalNetworks` / `SelectedLocalNetworkKey` | `ObservableCollection<LocalNetwork>` / `string?` | this machine's / `null` | Adapter networks; selecting one sets the range |
| `Entries` | `ObservableCollectionEx<NetworkHostViewModel>` | empty | Discovered hosts |
| `SelectedEntry` | `NetworkHostViewModel?` | `null` | Selected host; raises `EntrySelected` when changed |
| `Columns` | `NetworkScannerColumnsViewModel` | all `true` except `ShowWorkgroup` | Column visibility: `ShowPingQualityCategory`, `ShowIpAddress`, `ShowIpStatus`, `ShowDeviceName`, `ShowHostname`, `ShowWorkgroup`, `ShowMacAddress`, `ShowMacVendor`, `ShowOpenPortNumbers`, `ShowModel`, `ShowServices`, `ShowOnvif`, `ShowTotalInMs` |
| `Filter` | `NetworkScannerFilterViewModel` | see description | Result filters: `ShowOnlySuccess` and `ShowOnlyWithOpenPorts` (`true`), `ShowOnlyReachable` (`false`) |
| `Options` | `NetworkScannerOptionsViewModel` | earlier behaviour | Ping, host name, NetBIOS, MAC address and vendor, mDNS/SSDP and ONVIF discovery, timeouts |
| `ConfigureScanner` | `Action<IPScannerConfig>?` | `null` | Adjusts the scanner configuration after `Options` is applied |
| `ScannerFactory` | `Func<IPScannerConfig, IIPScanner>` | `IPScanner` | Creates the scanner for each scan |
| `Logger` | `ILogger?` | `null` | Logger for the default scanner |
| `MacAddressVendorLookup` | `IMacAddressVendorLookup?` | `null` | Vendor lookup for the default scanner, such as `IeeeOuiMacAddressVendorLookup` (offline); `null` looks up at macvendors.com |
| `HostFilter` | `Func<NetworkHostViewModel, bool>?` | `null` | Extra row filter for your own rules (for example "only cameras") |
| `BusyIndicatorMaximumValue` | `int` | `0` | Progress maximum |
| `BusyIndicatorCurrentValue` | `int` | `0` | Progress value |
| `BusyIndicatorPercentageValue` | `string` | `""` | Progress text |
| `EntryCountInfo` | `string` | `"0 / 0"` | Count of shown entries versus addresses in the range |
| `CompletionTime` | `string?` | `null` | Duration of the last completed scan |
| `ErrorMessage` | `string?` | `null` | Error from the last scan (invalid range, insufficient permissions, socket error) |
| `HasError` | `bool` | `false` | `true` when `ErrorMessage` is set |

### Commands

| Command | Description |
|---------|-------------|
| `ScanCommand` | Scans the range; enabled when both IP addresses parse. `ScanAsync(CancellationToken)` scans from code |
| `ScanCancelCommand` | Cancels a running scan |
| `ClearCommand` | Clears the results |
| `FilterChangeCommand` | Re-applies the result filters |
| `SortCommand` | Sorts by the clicked column header |
| `CopyIpAddressCommand` / `CopyHostnameCommand` / `CopyMacAddressCommand` / `CopyAllInfoCommand` | Copy details of the selected entry to the clipboard |
| `OpenInBrowserCommand` / `ConnectWithVncCommand` / `WakeOnLanCommand` / `RescanHostCommand` | Row actions for the selected entry |
| `CopyAllEntriesCommand` / `ExportCommand` | Copy the shown rows as CSV / save them as CSV or JSON |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `EntrySelected` | `NetworkHostSelectedEventArgs` | Raised by the view model when `SelectedEntry` changes; `SelectedHost` is `null` when the selection is cleared |
| `HostFound` / `HostUpdated` / `HostCompleted` | `NetworkHostEventArgs` | A row was added / updated / is done; `ScanResult` is the raw Atc.Network `IPScanResult` |
| `ScanStarted` | `EventArgs` | A scan started |
| `ScanCompleted` | `NetworkScanCompletedEventArgs` | A scan ended (`ScanResults`, `IsCancelled`, `ErrorMessage`), after every report was applied to the rows |
| `VncRequested` | `NetworkHostConnectEventArgs` | The user asked to connect with VNC; `Host` and `Port` |

## 📝 Notes

- `NetworkScannerView` and `NetworkScannerSettingsView` must share the same `NetworkScannerViewModel` instance
- The view model implements `IDisposable`; dispose it when the hosting view is discarded
- `GetSettings()` / `ApplySettings(NetworkScannerSettings)` save and restore range, ports, options, filters and columns
- mDNS, SSDP and ONVIF data arrives once per scan, after the last address is done; rows get one more `HostUpdated` with it before `ScanCompleted`
- A single IPv6 address can be scanned (start address, no end); ranges and CIDR are IPv4 only
- Reverse DNS (`Options.ResolveHostName`) has no timeout in Atc.Network 2.0.0; on networks without it a /24 takes minutes. Turn it off and turn on `ResolveNetBiosName` or `DiscoverServices` for fast device names
- Without `MacAddressVendorLookup`, MAC addresses are sent to macvendors.com for the vendor column
- The library has no XAML namespace mapping, so use a `clr-namespace:Atc.Wpf.Network.Scanner;assembly=Atc.Wpf.Network` prefix

## 🔗 Related Controls

- **NetworkScannerSettingsView** - Range, ports and scan buttons for the same view model
- **NetworkHostDetailsView** - mDNS, SSDP, ONVIF and NetBIOS details of one host (`SelectedEntry`)
- **VncViewerView** - VNC remote viewer from the same library
- **NetworkAdapterPicker** - Pick a local network adapter (Atc.Wpf.Hardware)
- Atc.Network docs: [IP scanning](https://github.com/atc-net/atc-network/blob/main/docs/ip-scanning.md) and [discovery](https://github.com/atc-net/atc-network/blob/main/docs/discovery.md)

## 🎮 Sample Application

See the NetworkScanner sample in the Atc.Wpf.Sample application under **Wpf.Network > Network > NetworkScanner** for interactive examples.
