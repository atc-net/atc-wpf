# Atc.Wpf.Network

Network scanning and host discovery controls (`NetworkScannerView`, `NetworkScannerSettingsView` and their view models), built on [Atc.Network](https://github.com/atc-net/atc-network).

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Network
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

```xml
<UserControl xmlns:network="clr-namespace:Atc.Wpf.Network;assembly=Atc.Wpf.Network">
    <network:NetworkScannerView DataContext="{Binding NetworkScanner}" />
</UserControl>
```

## Documentation

- [Network controls reference](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Network/Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
