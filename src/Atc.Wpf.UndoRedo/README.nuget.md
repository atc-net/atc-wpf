# Atc.Wpf.UndoRedo

Undo/redo UI for [Atc.UndoRedo](https://www.nuget.org/packages/Atc.UndoRedo): the `UndoRedoHistoryView` history list and the `UndoRedoKeyBindingBehavior` that wires the undo/redo keyboard shortcuts to an `IUndoRedoService`.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.UndoRedo
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

```xml
<UserControl
    xmlns:atcBehaviors="clr-namespace:Atc.Wpf.UndoRedo.Behaviors;assembly=Atc.Wpf.UndoRedo"
    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    xmlns:undoRedo="clr-namespace:Atc.Wpf.UndoRedo;assembly=Atc.Wpf.UndoRedo">
    <i:Interaction.Behaviors>
        <atcBehaviors:UndoRedoKeyBindingBehavior UndoRedoService="{Binding UndoRedoService}" />
    </i:Interaction.Behaviors>
    <undoRedo:UndoRedoHistoryView DataContext="{Binding HistoryViewModel}" />
</UserControl>
```

## Documentation

- [Atc.Wpf.UndoRedo overview](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.UndoRedo/Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
