# ℹ️ InfoDialogBox

A modal information dialog for displaying messages to the user with an OK button.

## 🔍 Overview

`InfoDialogBox` provides a simple modal dialog for displaying informational messages. It supports customizable title bar text, an optional header section, and content text. The dialog returns `true` when the user clicks OK.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
```

## 🚀 Usage

### Basic Information Dialog

```csharp
var dialog = new InfoDialogBox(ownerWindow, "Operation completed successfully.");
dialog.ShowDialog();
```

### With Custom Title

```csharp
var dialog = new InfoDialogBox(ownerWindow, "Save Complete", "Your document has been saved.");
dialog.ShowDialog();
```

### With Header and Content

```csharp
var dialog = new InfoDialogBox(
    ownerWindow,
    titleBarText: "Application Info",
    headerText: "About This Feature",
    contentText: "This feature allows you to configure your preferences.");
dialog.ShowDialog();
```

### With Custom Settings

```csharp
var settings = new DialogBoxSettings
{
    TitleBarText = "Custom Dialog",
    Width = 500,
    Height = 300,
};

var dialog = new InfoDialogBox(ownerWindow, settings, "Custom content here.");
dialog.ShowDialog();
```

## 📝 Notes

- The dialog is modal — it blocks interaction with the owner window
- `ShowDialog()` returns `true` when OK is clicked
- Escape closes the dialog with the same result as OK, like a message box
- `DialogService` owns its dialogs by the window from its owner resolver, otherwise by the active window, otherwise by the main window
- The dialog inherits window chrome from the theming system

## 🔗 Related Controls

- **QuestionDialogBox** - Yes/No question dialog
- **InputDialogBox** - Dialog with a text input field
- **InputFormDialogBox** - Dialog with a full form layout
- **ToastNotification** - Non-blocking notification alternative

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs** for interactive examples.
