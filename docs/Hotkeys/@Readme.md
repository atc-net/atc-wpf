# Hotkey Manager

## Overview

The **Hotkey Manager** provides MVVM-friendly keyboard shortcut management for WPF applications. It supports both **global** (system-wide via Win32 `RegisterHotKey`) and **local** (window-scoped via WPF `InputBinding`) hotkeys, two-stroke **chord** sequences, and **conflict detection**.

## Quick Reference

| Type | Description |
|------|-------------|
| `IHotkeyService` | Main service interface for registering and managing hotkeys |
| `HotkeyService` | Default implementation using Win32 message-only window and WPF InputBindings |
| `IHotkeyRegistration` | Handle to a registered hotkey — dispose to unregister |
| `HotkeyScope` | Enum: `Local` (window) or `Global` (system-wide) |
| `HotkeyChord` | Two-stroke chord definition (e.g. Ctrl+K, Ctrl+C) |
| `HotkeyPressedEventArgs` | Event data when a hotkey is pressed |
| `HotkeyConflictEventArgs` | Event data when a registration conflicts with an existing one |
| `HotkeyBindingDefinition` | Serializable DTO for persisting hotkey bindings to JSON |

## Namespace

```
Atc.Wpf.Hotkeys
```

## Usage

### Basic Registration

```csharp
IHotkeyService hotkeyService = new HotkeyService();

// Start listening on a window
hotkeyService.StartListening(myWindow);

// Register a local hotkey (window-scoped)
var localReg = hotkeyService.Register(
    ModifierKeys.Control, Key.G,
    args => Debug.WriteLine("Ctrl+G pressed!"),
    description: "Go to line",
    scope: HotkeyScope.Local);

// Register a global hotkey (system-wide)
var globalReg = hotkeyService.Register(
    ModifierKeys.Control | ModifierKeys.Shift, Key.H,
    args => Debug.WriteLine("Global Ctrl+Shift+H pressed!"),
    description: "Show app",
    scope: HotkeyScope.Global);

// Unregister by disposing
localReg.Dispose();

// Or explicitly
hotkeyService.Unregister(globalReg);
```

### Chord Registration

```csharp
// Register a two-stroke chord: Ctrl+K followed by Ctrl+C
var chordReg = hotkeyService.RegisterChord(
    ModifierKeys.Control, Key.K,
    ModifierKeys.Control, Key.C,
    args => Debug.WriteLine("Chord Ctrl+K, Ctrl+C completed!"),
    description: "Comment selection");
```

### Conflict Detection

```csharp
hotkeyService.ConflictDetected += (sender, args) =>
{
    Debug.WriteLine(
        $"Conflict: '{args.Requested.Description}' conflicts with '{args.Existing.Description}'");
};
```

### Event Monitoring

```csharp
hotkeyService.HotkeyPressed += (sender, args) =>
{
    Debug.WriteLine($"Hotkey pressed: {args.Registration.Description}");
};
```

## Persistence

The `IHotkeyService` supports saving and loading hotkey bindings to/from JSON files. This allows applications to persist user-configured hotkeys and restore them on startup.

```csharp
// Save current bindings
hotkeyService.SaveBindings("hotkeys.json");

// Load bindings from file
hotkeyService.LoadBindings("hotkeys.json");
```

Loaded bindings fire only the `HotkeyPressed` event (callbacks cannot be serialized). Subscribe to the event and dispatch by `Registration.Description`:

```csharp
hotkeyService.HotkeyPressed += (sender, args) =>
{
    switch (args.Registration.Description)
    {
        case "Show app":
            ShowMainWindow();
            break;
        case "Comment selection":
            CommentSelection();
            break;
    }
};

hotkeyService.LoadBindings("hotkeys.json");
```

The JSON format uses enum names for readability. Chord bindings include `SecondModifiers` and `SecondKey`:

```json
[
  {
    "Modifiers": "Control, Shift",
    "Key": "H",
    "Description": "Show app",
    "Scope": "Global"
  },
  {
    "Modifiers": "Control",
    "Key": "K",
    "Description": "Comment selection",
    "Scope": "Local",
    "SecondModifiers": "Control",
    "SecondKey": "C"
  }
]
```

## Notes

- Global hotkeys use Win32 `RegisterHotKey` via a message-only `HwndSource` window
- Local hotkeys use WPF `InputBinding` + `KeyGesture` on the target window
- Chords are always local scope and use `PreviewKeyDown` with a 1.5-second timeout
- Call `StartListening(window)` before hotkeys will fire
- Call `Dispose()` to clean up all registrations and Win32 resources

## Sample Application

Navigate to **Wpf > Hotkeys > HotkeyService** in the sample application.
