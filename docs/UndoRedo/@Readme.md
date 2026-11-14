# Undo/Redo UI

WPF user interface for the [Atc.UndoRedo](https://www.nuget.org/packages/Atc.UndoRedo) service: a history view and a keyboard behavior that work with any `IUndoRedoService`.

## Table of Contents

- [Overview](#overview)
- [Setup](#setup)
- [Quick Reference](#quick-reference)
- [Sample Application](#sample-application)

## Overview

The undo/redo logic (`IUndoRedoService`, `UndoRedoService`, `PropertyChangeCommand<T>`, `UndoCommandGroup`, ...) lives in the platform-agnostic `Atc.UndoRedo` package, which `Atc.Wpf.UndoRedo` brings in. This package adds the WPF pieces:

- **`UndoRedoHistoryView`** shows the undo and redo history with a toolbar (Undo, Redo, UndoAll, RedoAll, MarkSaved, Clear); clicking a row moves the service to that point.
- **`UndoRedoHistoryViewModel`** binds the view to an `IUndoRedoService`.
- **`UndoRedoKeyBindingBehavior`** binds Ctrl+Z, Ctrl+Shift+Z, Ctrl+Y and Ctrl+Shift+Y to an `IUndoRedoService`.

## Setup

`UndoRedoHistoryView` uses the theme brushes, so merge the Theming dictionary into `App.xaml`:

```xml
<ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />
```

`Atc.Wpf.UndoRedo` has no XAML namespace mapping:

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

## Quick Reference

| Type | Namespace | Readme |
|------|-----------|--------|
| `UndoRedoHistoryView` | `Atc.Wpf.UndoRedo` | [UndoRedoHistoryView](../../src/Atc.Wpf.UndoRedo/UndoRedoHistoryView_Readme.md) |
| `UndoRedoHistoryViewModel` | `Atc.Wpf.UndoRedo` | [Atc.Wpf.UndoRedo overview](../../src/Atc.Wpf.UndoRedo/Readme.md) |
| `UndoRedoKeyBindingBehavior` | `Atc.Wpf.UndoRedo.Behaviors` | [Atc.Wpf.UndoRedo overview](../../src/Atc.Wpf.UndoRedo/Readme.md) |

## Sample Application

See **Wpf.UndoRedo** in the sample app (`sample/Atc.Wpf.Sample`): UndoRedoService and UndoRedoHistoryView.
