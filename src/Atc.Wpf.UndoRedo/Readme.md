# Atc.Wpf.UndoRedo

WPF undo/redo UI components layered on top of the platform-agnostic [`Atc.UndoRedo`](https://www.nuget.org/packages/Atc.UndoRedo) service package.

## 🔍 Overview

`Atc.Wpf.UndoRedo` contains the WPF-specific pieces that consume the `IUndoRedoService` from the `Atc.UndoRedo` NuGet package:

- **`UndoRedoHistoryView`** — UserControl that displays a unified undo/redo history with a toolbar (Undo, Redo, UndoAll, RedoAll, MarkSaved, Clear). Clicking a row navigates the service to that position.
- **`UndoRedoHistoryViewModel`** — MVVM-friendly wrapper that binds to an `IUndoRedoService` and exposes relay commands + an observable history collection.
- **`UndoRedoHistoryItem`** — Row model for the history list.
- **`UndoRedoKeyBindingBehavior`** — Attached behavior that binds Ctrl+Z / Ctrl+Shift+Z / Ctrl+Y / Ctrl+Shift+Y to an `IUndoRedoService`.

For the service API itself (`IUndoRedoService`, `IUndoCommand`, `UndoRedoService`, `PropertyChangeCommand<T>`, `UndoCommandGroup`, etc.) see the [`Atc.UndoRedo`](https://www.nuget.org/packages/Atc.UndoRedo) package and its [repository](https://github.com/atc-net/atc-undoredo).

## 📦 Installation

```xml
<PackageReference Include="Atc.Wpf.UndoRedo" Version="*" />
```

This transitively pulls in `Atc.UndoRedo`, so the core service types are available in consuming projects without an extra reference.

## 🚀 Quick Start

```csharp
using Atc.UndoRedo;       // IUndoRedoService, UndoRedoService, PropertyChangeCommand
using Atc.Wpf.UndoRedo;   // UndoRedoHistoryViewModel

public sealed partial class EditorViewModel : ViewModelBase
{
    private readonly IUndoRedoService undoRedo = new UndoRedoService();

    public EditorViewModel()
    {
        HistoryViewModel = new UndoRedoHistoryViewModel { UndoRedoService = undoRedo };
    }

    public IUndoRedoService UndoRedoService => undoRedo;

    public UndoRedoHistoryViewModel HistoryViewModel { get; }
}
```

```xml
<UserControl
    xmlns:atcBehaviors="clr-namespace:Atc.Wpf.UndoRedo.Behaviors;assembly=Atc.Wpf.UndoRedo"
    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    xmlns:undoRedo="clr-namespace:Atc.Wpf.UndoRedo;assembly=Atc.Wpf.UndoRedo">

    <i:Interaction.Behaviors>
        <atcBehaviors:UndoRedoKeyBindingBehavior UndoRedoService="{Binding UndoRedoService}" />
    </i:Interaction.Behaviors>

    <undoRedo:UndoRedoHistoryView
        DataContext="{Binding HistoryViewModel}"
        ShowMarkSaved="True" />
</UserControl>
```

## 📚 Component Readmes

- [UndoRedoHistoryView](./UndoRedoHistoryView_Readme.md)

## 🎮 Sample Application

See the **Wpf.UndoRedo** section in `Atc.Wpf.Sample` for live demos of the history view, keyboard behavior, command grouping, snapshots, and batch import.
