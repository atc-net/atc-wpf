# Atc.Wpf.Controls.Sample

Building blocks for a WPF control-explorer application: the tabbed `SampleViewerView` (Sample / XAML / CodeBehind / ViewModel / Readme), `SampleTreeViewItem`, `SampleSidePanel`, `SamplePropertyController` and `SampleDataController`. The `Atc.Wpf.Sample` demo application is built from them.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Controls.Sample
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

```xml
<Window xmlns:sampleControls="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample">
    <sampleControls:SampleViewerView />
</Window>
```

## Documentation

- [Atc.Wpf.Controls.Sample overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Controls.Sample/Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
