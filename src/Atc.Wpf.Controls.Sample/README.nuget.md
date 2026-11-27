# Atc.Wpf.Controls.Sample

Building blocks for a WPF control-explorer application: a ready-made shell (`SampleApplication`, `SampleMainWindow`, `SampleExplorer` with filter, category tabs and sample trees), the tabbed `SampleViewerView` (Sample / XAML / CodeBehind / ViewModel / Readme), `SampleTreeViewItem`, `SampleSidePanel`, `SamplePropertyController` and `SampleDataController`. The `Atc.Wpf.Sample` demo application is built from them.

> For building sample / control-explorer applications only; it is not meant for production UI.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Controls.Sample
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

A gallery is an `App.xaml`, a `MainWindow.xaml` that lists its categories, and its sample trees and pages.
`SampleApplication` merges the Atc style dictionaries, so `App.xaml` needs none.

```xml
<!-- App.xaml -->
<atcSample:SampleApplication
    x:Class="MyGallery.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    Culture="en-US"
    StartupUri="MainWindow.xaml"
    Theme="Dark.Blue" />

<!-- MainWindow.xaml -->
<atcSample:SampleMainWindow
    x:Class="MyGallery.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    xmlns:local="clr-namespace:MyGallery"
    Title="My control explorer">
    <atcSample:SampleCategory Key="Wpf" Header="Wpf" TreeType="{x:Type local:SamplesWpfTreeView}" />
</atcSample:SampleMainWindow>
```

## Documentation

- [Atc.Wpf.Controls.Sample overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Controls.Sample/Readme.md)
- [SampleExplorer: build a sample gallery](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Controls.Sample/SampleExplorer_Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
