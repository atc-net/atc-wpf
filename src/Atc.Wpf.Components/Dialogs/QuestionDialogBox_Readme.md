# ❓ QuestionDialogBox

A modal Yes/No question dialog for confirming user actions.

## 🔍 Overview

`QuestionDialogBox` displays a modal dialog with a question and OK/Cancel buttons. It supports customizable title bar text, an optional header section, and content text. Returns `true` for OK and `false` for Cancel.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
```

## 🚀 Usage

```csharp
var dialog = new QuestionDialogBox(ownerWindow, "Are you sure you want to delete this item?");
if (dialog.ShowDialog() == true)
{
    // User confirmed
}

// With title and header
var dialog = new QuestionDialogBox(
    ownerWindow,
    titleBarText: "Confirm Delete",
    headerText: "Delete Item",
    contentText: "This action cannot be undone. Continue?");
```

## 📝 Notes

- OK returns `DialogResult = true`, Cancel returns `false`
- Supports the same constructor patterns as `InfoDialogBox`

## 🔗 Related Controls

- **InfoDialogBox** - Information-only dialog
- **InputDialogBox** - Dialog with text input
- **InputFormDialogBox** - Dialog with full form

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs** for interactive examples.
