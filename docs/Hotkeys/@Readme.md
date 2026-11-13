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

`ConflictDetected` covers clashes inside your own application. A **global** hotkey can also be
refused by Windows when another application already owns the same combination; the registration
then stays in `Registrations` but never fires, and `RegistrationFailed` is raised:

```csharp
hotkeyService.RegistrationFailed += (sender, args) =>
{
    // args.ErrorCode 1409 = ERROR_HOTKEY_ALREADY_REGISTERED
    Debug.WriteLine(
        $"'{args.Registration.Description}' could not be registered (Win32 error {args.ErrorCode})");
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

Loading merges the saved bindings with the hotkeys the app has already registered in code, matched by description:

- A saved binding whose description matches a registration with other keys moves that registration to the saved keys and keeps its callback (the user's rebinding). The registration returned by `Register` is replaced; find the new one in `Registrations`.
- A saved binding whose keys are already registered is skipped (no duplicate, no `ConflictDetected`).
- Any other saved binding is added. Its callback cannot be serialized, so it fires only the `HotkeyPressed` event.

Register the app's hotkeys first, then load. For bindings the app does not register in code, subscribe to `HotkeyPressed` and dispatch by `Registration.Description`:

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
- Local letter or digit hotkeys with no modifier or Shift only (which `KeyGesture` refuses) are matched in `PreviewKeyDown` instead, and do not fire while a `TextBox` or `PasswordBox` has focus
- `Register` and `Unregister` can be called from any thread; the Win32 and window work runs on the service's dispatcher
- Chords are always local scope and use `PreviewKeyDown` with a 1.5-second timeout
- Call `StartListening(window)` before hotkeys will fire
- Call `Dispose()` to clean up all registrations and Win32 resources

## Sample Application

Navigate to **Wpf > Hotkeys > HotkeyService** in the sample application.
