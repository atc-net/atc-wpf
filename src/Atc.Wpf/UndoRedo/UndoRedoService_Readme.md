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

## ⚙️ IUndoRedoService Members

| Member | Type | Description |
|--------|------|-------------|
| `UndoCommands` | `IReadOnlyList<IUndoCommand>` | Current undo stack (newest first) |
| `RedoCommands` | `IReadOnlyList<IUndoCommand>` | Current redo stack (newest first) |
| `CanUndo` | `bool` | Whether the undo stack has commands |
| `CanRedo` | `bool` | Whether the redo stack has commands |
| `IsExecuting` | `bool` | Whether a command is currently running |
| `MaxHistorySize` | `int` | Maximum undo entries (default 100) |
| `Execute` | `void` | Runs a command and pushes to undo stack |
| `Add` | `void` | Records a command without executing |
| `Undo` | `bool` | Undoes the last command |
| `Redo` | `bool` | Redoes the last undone command |
| `UndoAll` | `void` | Undoes all commands |
| `RedoAll` | `void` | Redoes all undone commands |
| `UndoTo` | `void` | Undoes up to a specific command |
| `RedoTo` | `void` | Redoes up to a specific command |
| `BeginGroup` | `IDisposable` | Starts a grouped command scope |
| `Clear` | `void` | Clears both stacks |
| `ActionPerformed` | `event` | Raised after execute, undo, or redo |
| `StateChanged` | `event` | Raised when stacks change |

## 📋 Command Types

| Type | Description |
|------|-------------|
| `IUndoCommand` | Interface: `Description`, `Execute()`, `UnExecute()` |
| `UndoCommand` | Wraps execute/un-execute `Action` delegates |
| `UndoCommandGroup` | Groups commands; execute in order, un-execute in reverse |
| `PropertyChangeCommand<T>` | Pre-built for property setter with old/new values |

## 📝 Notes

- Thread-safe: all stack operations are protected by `Lock`
- `IsExecuting` guard prevents recursive command additions during Execute/UnExecute
- Nested `BeginGroup()` calls throw `InvalidOperationException`
- Empty groups (no commands added) are silently discarded
- `Execute()` and `Add()` both clear the redo stack
- Oldest commands are trimmed from the undo stack when `MaxHistorySize` is exceeded

## 🔗 Related

- **UndoRedoActionType** — Enum: None, Execute, Undo, Redo
- **UndoRedoEventArgs** — Event data with ActionType and Command

## 🎮 Sample Application

See the UndoRedoService sample in the Atc.Wpf.Sample application under **Wpf.Components > UndoRedo > UndoRedoService** for interactive examples.
