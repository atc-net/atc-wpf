# 🔄 UndoRedoService

An MVVM-friendly undo/redo service providing command-based history management with grouping, configurable limits, and change notifications.

## 🔍 Overview

`UndoRedoService` implements `IUndoRedoService` to provide a thread-safe, command-pattern-based undo/redo system. It maintains dual stacks (undo/redo), supports command grouping via `BeginGroup()` for atomic multi-step operations, enforces a configurable maximum history size, and raises events on state changes.

## 📍 Namespace

```csharp
using Atc.Wpf.UndoRedo;
```

## 🚀 Usage

### Basic Undo/Redo

```csharp
IUndoRedoService undoRedo = new UndoRedoService();

// Execute a command (runs it and records it)
var value = 0;
undoRedo.Execute(new UndoCommand(
    "Increment",
    () => value++,
    () => value--));

// Undo the last action
undoRedo.Undo();   // value == 0

// Redo the undone action
undoRedo.Redo();   // value == 1
```

### Property Change Command

```csharp
// Pre-built command for property value changes
var command = new PropertyChangeCommand<string>(
    "Change Name",
    v => person.Name = v,
    oldValue: person.Name,
    newValue: "Alice");

undoRedo.Execute(command);
```

### Record Without Executing

```csharp
// For actions already performed (e.g., user typed text)
person.Name = "Bob";
undoRedo.Add(new PropertyChangeCommand<string>(
    "Change Name",
    v => person.Name = v,
    oldValue: "Alice",
    newValue: "Bob"));
```

### Command Grouping

```csharp
// Group multiple commands as a single undo unit
using (undoRedo.BeginGroup("Move objects"))
{
    undoRedo.Execute(new UndoCommand("Move X", ...));
    undoRedo.Execute(new UndoCommand("Move Y", ...));
}
// Undo reverts the entire group in one step
undoRedo.Undo();
```

### Bulk Operations

```csharp
// Undo/redo all commands
undoRedo.UndoAll();
undoRedo.RedoAll();

// Undo/redo to a specific command
undoRedo.UndoTo(targetCommand);
undoRedo.RedoTo(targetCommand);
```

### Events

```csharp
undoRedo.ActionPerformed += (sender, e) =>
{
    Console.WriteLine($"{e.ActionType}: {e.Command.Description}");
};

undoRedo.StateChanged += (sender, e) =>
{
    // Update CanUndo/CanRedo bindings
};
```

### Rich Commands

```csharp
// Commands with metadata for designer-style applications
var command = new RichUndoCommand(
    "Move element",
    () => element.Move(newPos),
    () => element.Move(oldPos),
    image: myIcon,
    data: element,
    allowUserAction: true);

undoRedo.Execute(command);

// Internal/programmatic commands (skipped by user-action navigation)
undoRedo.Execute(new RichUndoCommand(
    "Auto-layout",
    () => Layout(),
    () => UndoLayout(),
    allowUserAction: false));
```

### Undo/Redo by Levels

```csharp
undoRedo.Undo(3);  // Undo 3 steps
undoRedo.Redo(2);  // Redo 2 steps
```

### User-Action Navigation

```csharp
// Undo/redo to the next user-initiated action boundary,
// skipping programmatic (AllowUserAction=false) commands.
undoRedo.UndoToLastUserAction();
undoRedo.RedoToLastUserAction();
```

### Executing State Inspection

```csharp
// Inside a command delegate, inspect what action is in progress
var command = new UndoCommand(
    "Example",
    () => {
        if (undoRedo.ExecutingAction == UndoRedoActionType.Redo)
        {
            // Handle redo differently
        }
    },
    () => { });
```

### Save Point Tracking

```csharp
undoRedo.MarkSaved();
bool dirty = undoRedo.HasUnsavedChanges;
```

## ⚙️ IUndoRedoService Members

| Member | Type | Description |
|--------|------|-------------|
| `UndoCommands` | `IReadOnlyList<IUndoCommand>` | Current undo stack (newest first) |
| `RedoCommands` | `IReadOnlyList<IUndoCommand>` | Current redo stack (newest first) |
| `CanUndo` | `bool` | Whether the undo stack has commands |
| `CanRedo` | `bool` | Whether the redo stack has commands |
| `IsExecuting` | `bool` | Whether a command is currently running |
| `ExecutingAction` | `UndoRedoActionType` | Action type in progress (None when idle) |
| `ExecutingCommand` | `IUndoCommand?` | Command currently being executed/undone/redone |
| `MaxHistorySize` | `int` | Maximum undo entries (default 100) |
| `HasUnsavedChanges` | `bool` | Whether current state differs from last saved point |
| `Execute` | `void` | Runs a command and pushes to undo stack |
| `Add` | `void` | Records a command without executing |
| `Undo()` | `bool` | Undoes the last command |
| `Undo(int)` | `bool` | Undoes the specified number of commands |
| `Redo()` | `bool` | Redoes the last undone command |
| `Redo(int)` | `bool` | Redoes the specified number of commands |
| `UndoAll` | `void` | Undoes all commands |
| `RedoAll` | `void` | Redoes all undone commands |
| `UndoToLastUserAction` | `bool` | Undoes to the next user-action boundary |
| `RedoToLastUserAction` | `bool` | Redoes to the next user-action boundary |
| `UndoTo` | `void` | Undoes up to a specific command |
| `RedoTo` | `void` | Redoes up to a specific command |
| `BeginGroup` | `IDisposable` | Starts a grouped command scope |
| `Clear` | `void` | Clears both stacks |
| `MarkSaved` | `void` | Marks current state as saved |
| `ActionPerformed` | `event` | Raised after execute, undo, or redo |
| `StateChanged` | `event` | Raised when stacks change |

## 📋 Command Types

| Type | Description |
|------|-------------|
| `IUndoCommand` | Interface: `Description`, `Execute()`, `UnExecute()` |
| `IRichUndoCommand` | Extended interface adding `Id`, `Image`, `Parameter`, `Data`, `AllowUserAction` |
| `UndoCommand` | Wraps execute/un-execute `Action` delegates |
| `RichUndoCommand` | Like `UndoCommand` with rich metadata (Id, Image, Parameter, Data, AllowUserAction) |
| `PropertyChangeCommand<T>` | Pre-built for property setter with old/new values |
| `RichPropertyChangeCommand<T>` | Like `PropertyChangeCommand<T>` with rich metadata |
| `UndoCommandGroup` | Groups commands; execute in order, un-execute in reverse |

## 🎹 Keyboard Behavior

`UndoRedoKeyBindingBehavior` is an attached behavior that maps standard keyboard shortcuts to an `IUndoRedoService`:

```xml
<i:Interaction.Behaviors>
    <atcBehaviors:UndoRedoKeyBindingBehavior UndoRedoService="{Binding UndoRedoService}" />
</i:Interaction.Behaviors>
```

| Shortcut | Action |
|----------|--------|
| Ctrl+Z | Undo |
| Ctrl+Shift+Z | Undo All |
| Ctrl+Y | Redo |
| Ctrl+Shift+Y | Redo All |

## 📝 Notes

- Thread-safe: all stack operations are protected by `Lock`
- `IsExecuting` guard prevents recursive command additions during Execute/UnExecute
- Nested `BeginGroup()` calls throw `InvalidOperationException`
- Empty groups (no commands added) are silently discarded
- `Execute()` and `Add()` both clear the redo stack
- Oldest commands are trimmed from the undo stack when `MaxHistorySize` is exceeded
- Plain `IUndoCommand` instances are treated as user actions by `UndoToLastUserAction`/`RedoToLastUserAction`
- `UndoToLastUserAction` groups trailing non-user-action commands with their user action for correct round-trip navigation

## 🔗 Related

- **UndoRedoActionType** — Enum: None, Execute, Undo, Redo
- **UndoRedoEventArgs** — Event data with ActionType and Command
- **UndoRedoHistoryView** — UI component for visualizing the history (`Atc.Wpf.Components.UndoRedo`)
- **UndoRedoKeyBindingBehavior** — Keyboard shortcut behavior (`Atc.Wpf.Behaviors`)

## 🎮 Sample Application

See the UndoRedoService sample in the Atc.Wpf.Sample application under **Wpf.Components > History > UndoRedoService** for interactive examples.
