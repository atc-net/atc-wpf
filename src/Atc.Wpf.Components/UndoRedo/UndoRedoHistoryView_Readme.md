# 📜 UndoRedoHistoryView

A unified history view for visualizing and navigating an `IUndoRedoService` undo/redo stack.

## 🔍 Overview

`UndoRedoHistoryView` displays both undo and redo stacks in a single chronological list. The current position is highlighted with an accent background. Clicking a row navigates (undo/redo) to that position. An optional toolbar provides Undo, Redo, UndoAll, RedoAll, and Clear buttons.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.UndoRedo;
```

## 🚀 Usage

### Basic Example

```xml
<components:UndoRedoHistoryView
    DataContext="{Binding HistoryViewModel}"
    ShowToolbar="True"
    ShowMarkSaved="True" />
```

### ViewModel Setup

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly IUndoRedoService undoRedoService = new UndoRedoService();

    public UndoRedoHistoryViewModel HistoryViewModel { get; }

    public MyViewModel()
    {
        HistoryViewModel = new UndoRedoHistoryViewModel
        {
            UndoRedoService = undoRedoService,
        };
    }
}
```

### History List Layout

The unified list displays items in chronological order:

| Row | Description | Style |
|-----|-------------|-------|
| Root | "(Initial state)" | Bold when at initial state |
| Undo items | Chronological (oldest first) | Bold on most recent (current position) |
| Redo items | Next-to-redo first | Italic, subdued opacity |

Clicking any row navigates the undo/redo service to that position.

## ⚙️ Properties

### UndoRedoHistoryView (UserControl)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowToolbar` | `bool` | `true` | Controls visibility of the toolbar with undo/redo/clear buttons |
| `ShowClear` | `bool` | `true` | Controls visibility of the Clear button and its separator in the toolbar |
| `ShowMarkSaved` | `bool` | `false` | Controls visibility of the Mark Saved button and its separator in the toolbar |

### UndoRedoHistoryViewModel

| Property | Type | Description |
|----------|------|-------------|
| `UndoRedoService` | `IUndoRedoService?` | The service instance to visualize. Set this to bind the view to a service |
| `HistoryItems` | `ObservableCollectionEx<UndoRedoHistoryItem>` | Generated list of history rows |
| `SelectedItem` | `UndoRedoHistoryItem?` | Currently selected row |

### Commands

| Command | Description |
|---------|-------------|
| `UndoCommand` | Undo the most recent command |
| `RedoCommand` | Redo the most recently undone command |
| `UndoAllCommand` | Undo all commands |
| `RedoAllCommand` | Redo all commands |
| `ClearCommand` | Clear the entire history |
| `MarkSavedCommand` | Mark the current position as saved (disabled when no unsaved changes) |
| `NavigateToCommand` | Navigate to a specific history item |

## 📝 Notes

- The view automatically refreshes when the `IUndoRedoService.StateChanged` event fires
- The `UndoRedoService` property setter manages event subscriptions (unsubscribes from old, subscribes to new)
- Navigation via row click uses `UndoTo`/`RedoTo` for efficient multi-step jumps
- The root "(Initial state)" row navigates via `UndoAll()` when clicked

## 🔗 Related

- **IUndoRedoService** - The undo/redo service interface (`Atc.Wpf.UndoRedo`)
- **UndoRedoService** - Default service implementation
- **PropertyChangeCommand&lt;T&gt;** - Generic property change command

## 🎮 Sample Application

See the UndoRedoHistoryView sample in the Atc.Wpf.Sample application under **Wpf.Components > History > UndoRedoHistoryView** for an interactive demo.
