# 📜 UndoRedoHistoryView

A unified history view for visualizing and navigating an `IUndoRedoService` undo/redo stack.

## 🔍 Overview

`UndoRedoHistoryView` displays both undo and redo stacks in a single chronological list. The current position is highlighted with an accent background. Clicking a row navigates (undo/redo) to that position. An optional toolbar provides Undo, Redo, UndoAll, RedoAll, and Clear buttons.

## 📍 Namespace

```csharp
using Atc.Wpf.UndoRedo;
```

## 🚀 Usage

### Basic Example

```xml
<undoRedo:UndoRedoHistoryView
    xmlns:undoRedo="clr-namespace:Atc.Wpf.UndoRedo;assembly=Atc.Wpf.UndoRedo"
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

### UndoRedoHistoryItem

| Property | Type | Description |
|----------|------|-------------|
| `Description` | `string` | Human-readable description for the row |
| `IsRedo` | `bool` | Whether this item is in the redo portion |
| `IsHighlighted` | `bool` | Whether this item is the current position |
| `IsSavePoint` | `bool` | Whether this item represents the save point |
| `Image` | `ImageSource?` | Optional icon from `IRichUndoCommand.Image` metadata |
| `Timestamp` | `DateTimeOffset?` | When the command was created (from `IRichUndoCommand.Timestamp`) |
| `Command` | `IUndoCommand?` | The underlying command (`null` for root "Initial state" row) |

### Commands

| Command | Description |
|---------|-------------|
| `UndoCommand` | Undo the most recent command |
| `RedoCommand` | Redo the most recently undone command |
| `UndoAllCommand` | Undo all commands |
| `RedoAllCommand` | Redo all commands |
| `UndoToLastUserActionCommand` | Undo to the next user-action boundary (skips programmatic commands) |
| `RedoToLastUserActionCommand` | Redo to the next user-action boundary (skips programmatic commands) |
| `ClearCommand` | Clear the entire history |
| `MarkSavedCommand` | Mark the current position as saved (disabled when no unsaved changes) |
| `NavigateToCommand` | Navigate to a specific history item |

## 📝 Notes

- The view automatically refreshes when the `IUndoRedoService.StateChanged` event fires
- The `UndoRedoService` property setter manages event subscriptions (unsubscribes from old, subscribes to new)
- Navigation via row click uses `UndoTo`/`RedoTo` for efficient multi-step jumps
- The root "(Initial state)" row navigates via `UndoAll()` when clicked

## 🔗 Related

- **IUndoRedoService** - Core undo/redo service interface (external `Atc.UndoRedo` NuGet package)
- **UndoRedoService** - Default service implementation (external `Atc.UndoRedo` NuGet package)
- **UndoRedoKeyBindingBehavior** - Attached behavior for Ctrl+Z/Ctrl+Y keyboard shortcuts (`Atc.Wpf.UndoRedo.Behaviors`)

## 🎮 Sample Application

See the UndoRedoHistoryView sample in the Atc.Wpf.Sample application under **Wpf.UndoRedo > UndoRedoHistoryView** for an interactive demo.
