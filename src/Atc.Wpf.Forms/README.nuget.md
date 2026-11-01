# Atc.Wpf.Forms

Labeled form-field controls with validation display and mandatory markers (`LabelTextBox`, `LabelComboBox`, `LabelDatePicker`, `LabelIntegerBox`, ...), including the multi-select `LabelCheckListBox`, `LabelMultiSelectComboBox`, `LabelChipGroup`, `LabelTokenBox` and `LabelTreeSelectComboBox`, plus `TimeZonePicker`.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Forms
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
    <StackPanel>
        <atc:LabelTextBox LabelText="User name" IsMandatory="True" Text="{Binding UserName}" />
        <atc:LabelMultiSelectComboBox
            LabelText="Work days"
            Items="{Binding Weekdays}"
            SelectedKeys="{Binding SelectedWorkdays}" />
    </StackPanel>
</Window>
```

## Documentation

- [Form controls reference](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Forms/Readme.md)
- [Forms guide](https://github.com/atc-net/atc-wpf/blob/main/docs/Forms/%40Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
