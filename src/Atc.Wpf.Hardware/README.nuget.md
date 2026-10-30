# Atc.Wpf.Hardware

WPF pickers for hardware devices and system resources with live device-state detection: serial ports, USB devices, cameras, audio input/output, Bluetooth devices, drives, displays, printers, network adapters, processes and windows. Every picker has a `Label*` form wrapper.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Hardware
```

Requires Windows 10 version 2004 (build 19041) or later; the package targets `net10.0-windows10.0.19041.0`.

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

```xml
<Window xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <atc:SerialPortPicker
        Value="{Binding SelectedPort, Mode=TwoWay}"
        WatermarkText="Select a COM port" />
</Window>
```

## Documentation

- [Hardware guide](https://github.com/atc-net/atc-wpf/blob/main/docs/Hardware/%40Readme.md)
- [Atc.Wpf.Hardware overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Hardware/Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
