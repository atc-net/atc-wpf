# Network Controls

Network controls provide specialized WPF controls for network scanning and host discovery, built on the [Atc.Network](https://github.com/atc-net/atc-network) library.

## Table of Contents

- [Network Controls](#network-controls)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Installation](#installation)
  - [Quick Start](#quick-start)
  - [NetworkScannerView](#networkscannerview)
    - [Features](#features)
    - [Properties](#properties)
    - [Events](#events)
    - [Usage Example](#usage-example)
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
    - [IpStatusToLocalizedDescriptionValueConverter](#ipstatustolocalizedescriptionvalueconverter)
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

The `Atc.Wpf.NetworkControls` package provides a ready-to-use network scanner control that can:

- Scan IP address ranges for active hosts
- Perform ping tests with quality indicators
- Resolve hostnames via DNS
- Detect MAC addresses and vendor information
- Scan for open ports on discovered hosts
- Display results in a sortable, filterable grid

## Installation

Add the NuGet package to your project:

```xml
<PackageReference Include="Atc.Wpf.NetworkControls" Version="2.*" />
```

## Quick Start

1. Add the namespace to your XAML:

```xml
xmlns:networkControls="clr-namespace:Atc.Wpf.NetworkControls;assembly=Atc.Wpf.NetworkControls"
```

2. Add the control to your view:

```xml
<networkControls:NetworkScannerView DataContext="{Binding NetworkScanner}" />
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

## NetworkScannerView

The main control for displaying network scan results in a ListView with sortable columns.

### Features

- Real-time progress reporting with cancellation support
- Sortable columns (click header to sort)
- Configurable column visibility
- Built-in filtering (success only, open ports only)
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

### Usage Example

```xml
<UserControl
    xmlns:networkControls="clr-namespace:Atc.Wpf.NetworkControls;assembly=Atc.Wpf.NetworkControls">

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
        <networkControls:NetworkScannerView
            Grid.Row="1"
            DataContext="{Binding NetworkScanner}" />

        <!-- Selected Entry Info -->
        <TextBlock
            Grid.Row="2"
            Text="{Binding NetworkScanner.SelectedEntry.Hostname}" />
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
| `SelectedEntry` | `NetworkHostViewModel?` | `null` | Currently selected host entry |
| `Entries` | `ObservableCollectionEx<NetworkHostViewModel>` | `[]` | Collection of discovered hosts |
| `Columns` | `NetworkScannerColumnsViewModel` | (default) | Column visibility settings |
| `Filter` | `NetworkScannerFilterViewModel` | (default) | Filter settings |
| `IsBusy` | `bool` | `false` | Indicates if a scan is in progress |
| `BusyIndicatorMaximumValue` | `int` | `0` | Maximum value for progress indicator |
| `BusyIndicatorCurrentValue` | `int` | `0` | Current value for progress indicator |
| `BusyIndicatorPercentageValue` | `string` | `""` | Percentage text for progress indicator |
| `CompletionTime` | `string?` | `null` | Time taken to complete the scan |
| `EntryCountInfo` | `string` | `"0 / 0"` | Display string showing filtered/total count |

#### Commands

| Command | Description |
|---------|-------------|
| `ScanCommand` | Starts the network scan (supports cancellation) |
| `ScanCancelCommand` | Cancels an in-progress scan |
| `ClearCommand` | Clears all scan results |
| `SortCommand` | Sorts the results by clicked column |
| `FilterChangeCommand` | Applies current filter settings |

#### Events

| Event | Type | Description |
|-------|------|-------------|
| `EntrySelected` | `EventHandler<NetworkHostSelectedEventArgs>` | Fired when selection changes |

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

### NetworkScannerFilterViewModel

Controls filtering of scan results.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowOnlySuccess` | `bool` | `true` | Show only hosts with successful ping |
| `ShowOnlyWithOpenPorts` | `bool` | `true` | Show only hosts with open ports |

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
