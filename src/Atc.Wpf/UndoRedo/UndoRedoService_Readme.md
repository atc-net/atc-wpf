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

### Command Coalescing

```csharp
// Commands with the same MergeId are coalesced into a single undo step
public class SliderChangeCommand : IMergeableUndoCommand
{
    private double oldValue;
    private double newValue;

    public SliderChangeCommand(double oldValue, double newValue)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public string Description => $"Slider: {newValue:F1}";
    public int MergeId => 1; // Same ID = same logical operation

    public bool TryMergeWith(IUndoCommand other)
    {
        if (other is SliderChangeCommand next)
        {
            newValue = next.newValue; // Absorb the new value
            return true;
        }
        return false;
    }

    public void Execute() => SetSlider(newValue);
    public void UnExecute() => SetSlider(oldValue);
}

// 10 rapid slider changes become 1 undo step
undoRedo.Execute(new SliderChangeCommand(0, 10));
undoRedo.Execute(new SliderChangeCommand(10, 20));
undoRedo.Execute(new SliderChangeCommand(20, 30));
// UndoCommands.Count == 1 (all merged)
```

### Obsolete Command Detection

```csharp
// Commands that result in no change are auto-discarded
public class MoveCommand : IUndoCommand
{
    private readonly Point from;
    private readonly Point to;

    public string Description => $"Move to ({to.X}, {to.Y})";
    public bool IsObsolete => from == to; // No actual change

    public void Execute() => element.Position = to;
    public void UnExecute() => element.Position = from;
}

// Move from (10,10) to (10,10) — auto-discarded, no undo entry
undoRedo.Execute(new MoveCommand(new(10, 10), new(10, 10)));
// UndoCommands.Count == 0
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

### Suspend Recording

```csharp
using (undoRedoService.SuspendRecording())
{
    // Bulk operations — no undo commands recorded
    foreach (var item in items)
        undoRedoService.Execute(new ImportCommand(item));
}
// Recording resumes. Nesting is supported.
```

### Memory-Based Limits

```csharp
undoRedoService.MaxHistoryMemory = 50 * 1024 * 1024; // 50 MB budget
// Commands implementing IMemoryAwareUndoCommand report their size.
// Oldest commands are trimmed when the budget is exceeded.
```

### Persistent History

```csharp
// Save
using var stream = File.Create("history.bin");
undoRedoService.SaveHistory(stream);

// Load
using var stream = File.OpenRead("history.bin");
undoRedoService.LoadHistory(stream, myDeserializer);
```

### Audit Logging

```csharp
undoRedoService.AuditLogger = new UndoRedoAuditLogger(maxEntries: 500);
// All operations are now logged with timestamps
var entries = undoRedoService.AuditLogger.Entries;
```

### Auto-Tracking

```csharp
var tracker = new UndoRedoPropertyTracker(undoRedoService);
tracker.Track(myViewModel); // All INPC property changes become undo commands
tracker.Track(otherVm, "Name", "Age"); // Track specific properties only
```

### Non-Linear History

```csharp
undoRedoService.AllowNonLinearHistory = true;
// Undo some commands, then execute new ones
// Old redo path is saved as a branch instead of being lost
var branches = undoRedoService.RedoBranches;
undoRedoService.SwitchRedoBranch(0); // Restore old branch
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
| `MaxHistoryMemory` | `long` | Maximum memory budget in bytes (0 = disabled) |
| `CurrentHistoryMemory` | `long` | Current estimated memory of undo stack |
| `HasUnsavedChanges` | `bool` | Whether current state differs from last saved point |
| `IsRecordingSuspended` | `bool` | Whether undo recording is currently suspended |
| `AllowNonLinearHistory` | `bool` | Whether branching redo history is enabled |
| `RedoBranches` | `IReadOnlyList<IReadOnlyList<IUndoCommand>>` | Saved redo branches (non-linear mode) |
| `AuditLogger` | `IUndoRedoAuditLogger?` | Optional audit logger for operation tracking |
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
| `SuspendRecording` | `IDisposable` | Suspends undo recording (supports nesting) |
| `SwitchRedoBranch` | `void` | Restores a saved redo branch by index |
| `SaveHistory` | `void` | Persists serializable commands to a stream |
| `LoadHistory` | `void` | Loads history from a stream via a deserializer |
| `Clear` | `void` | Clears both stacks |
| `MarkSaved` | `void` | Marks current state as saved |
| `ActionPerformed` | `event` | Raised after execute, undo, or redo |
| `StateChanged` | `event` | Raised when stacks change |

## 📋 Command Types

| Type | Description |
|------|-------------|
| `IUndoCommand` | Interface: `Description`, `Execute()`, `UnExecute()` |
| `IRichUndoCommand` | Extended interface adding `Id`, `Image`, `Parameter`, `Data`, `AllowUserAction` |
| `IMergeableUndoCommand` | Interface: extends `IUndoCommand` with `MergeId` and `TryMergeWith()` for command coalescing |
| `UndoCommand` | Wraps execute/un-execute `Action` delegates |
| `RichUndoCommand` | Like `UndoCommand` with rich metadata (Id, Image, Parameter, Data, AllowUserAction) |
| `PropertyChangeCommand<T>` | Pre-built for property setter with old/new values |
| `RichPropertyChangeCommand<T>` | Like `PropertyChangeCommand<T>` with rich metadata |
| `IMemoryAwareUndoCommand` | Interface: `EstimatedMemoryBytes` for memory-budget trimming |
| `ISerializableUndoCommand` | Interface: `TypeId` + `Serialize()` for persistent history |
| `IUndoCommandDeserializer` | Interface: reconstructs commands from serialized data |
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
- `IMergeableUndoCommand` enables coalescing: consecutive commands with the same `MergeId` merge into a single undo step
- Commands with `IsObsolete == true` after execution are auto-discarded from the undo stack
- `IRichUndoCommand.Timestamp` records when a command was created (UTC)
- `SuspendRecording` supports nesting — recording resumes when all scopes are disposed
- `SaveHistory` skips non-serializable commands; `LoadHistory` clears existing state
- `UndoRedoPropertyTracker` ignores changes during undo/redo (no recursive recording)

## 🔗 Related

- **UndoRedoActionType** — Enum: None, Execute, Undo, Redo
- **UndoRedoEventArgs** — Event data with ActionType and Command
- **UndoRedoHistoryView** — UI component for visualizing the history (`Atc.Wpf.Components.UndoRedo`)
- **UndoRedoKeyBindingBehavior** — Keyboard shortcut behavior (`Atc.Wpf.Behaviors`)

## 🎮 Sample Application

See the UndoRedoService sample in the Atc.Wpf.Sample application under **Wpf.Components > History > UndoRedoService** for interactive examples.
