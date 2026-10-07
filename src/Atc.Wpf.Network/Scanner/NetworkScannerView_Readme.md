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
| `Entries` | `ObservableCollectionEx<NetworkHostViewModel>` | empty | Discovered hosts |
| `SelectedEntry` | `NetworkHostViewModel?` | `null` | Selected host; raises `EntrySelected` when changed |
| `Columns` | `NetworkScannerColumnsViewModel` | all `true` | Column visibility: `ShowPingQualityCategory`, `ShowIpAddress`, `ShowIpStatus`, `ShowHostname`, `ShowMacAddress`, `ShowMacVendor`, `ShowOpenPortNumbers`, `ShowTotalInMs` |
| `Filter` | `NetworkScannerFilterViewModel` | all `true` | Result filters: `ShowOnlySuccess`, `ShowOnlyWithOpenPorts` |
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
| `ScanCommand` | Scans the range; enabled when both IP addresses parse |
| `ScanCancelCommand` | Cancels a running scan |
| `ClearCommand` | Clears the results |
| `FilterChangeCommand` | Re-applies the result filters |
| `SortCommand` | Sorts by the clicked column header |
| `CopyIpAddressCommand` / `CopyHostnameCommand` / `CopyMacAddressCommand` / `CopyAllInfoCommand` | Copy details of the selected entry to the clipboard |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `EntrySelected` | `NetworkHostSelectedEventArgs` | Raised by the view model when `SelectedEntry` changes; `SelectedHost` is `null` when the selection is cleared |

## 📝 Notes

- `NetworkScannerView` and `NetworkScannerSettingsView` must share the same `NetworkScannerViewModel` instance
- The view model implements `IDisposable`; dispose it when the hosting view is discarded
- The library has no XAML namespace mapping, so use a `clr-namespace:Atc.Wpf.Network.Scanner;assembly=Atc.Wpf.Network` prefix

## 🔗 Related Controls

- **NetworkScannerSettingsView** - Range, ports and scan buttons for the same view model
- **VncViewerView** - VNC remote viewer from the same library
- **NetworkAdapterPicker** - Pick a local network adapter (Atc.Wpf.Hardware)

## 🎮 Sample Application

See the NetworkScanner sample in the Atc.Wpf.Sample application under **Wpf.Network > Network > NetworkScanner** for interactive examples.
