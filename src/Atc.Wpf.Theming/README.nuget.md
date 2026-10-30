# Atc.Wpf.Theming

Light/Dark theme infrastructure: `ThemeManager` with accent colors, themed styles for the standard WPF controls, and the `NiceWindow` window chrome. Built on ControlzEx.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Theming
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

Merge the styles into `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

Switch theme and accent at runtime:

```csharp
ThemeManager.Current.ChangeTheme(Application.Current, "Dark", "Blue");
```

## Documentation

- [Theming guide](https://github.com/atc-net/atc-wpf/blob/main/docs/Theming/%40Readme.md)
- [Atc.Wpf.Theming overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Theming/README.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
