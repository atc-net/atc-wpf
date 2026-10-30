# Atc.Wpf.Controls

Atomic WPF controls: numeric and XY inputs, toggle switch, date/time pickers, buttons, color editing, data display (Badge, Card, Chip, ChipGroup, Timeline, ...), selectors including the multi-select `CheckListBox`, `MultiSelectComboBox`, `TokenBox`, `CheckTreeView` and `TreeSelectComboBox`, progress indicators and the `ZoomBox` family.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Controls
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

Merge the styles into `App.xaml`:

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

```xml
<Window xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <atc:IntegerBox Value="{Binding Port, Mode=TwoWay}" Minimum="1" Maximum="65535" />
</Window>
```

## Documentation

- [Atc.Wpf.Controls overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Controls/README.md)
- [Data display controls](https://github.com/atc-net/atc-wpf/blob/main/docs/DataDisplay/%40Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
