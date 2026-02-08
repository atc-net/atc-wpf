# ✏️ InputDialogBox

A modal dialog with a single labeled input control for collecting a value from the user.

## 🔍 Overview

`InputDialogBox` displays a modal dialog containing a single `ILabelControlBase` input (e.g., `LabelTextBox`, `LabelComboBox`) and OK/Cancel buttons. The input data is accessible via the `Data` property after the dialog closes.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
```

## 🚀 Usage

```csharp
var input = new LabelTextBox { LabelText = "Enter your name", IsMandatory = true };
var dialog = new InputDialogBox(ownerWindow, "Name Required", input);

if (dialog.ShowDialog() == true)
{
    var data = dialog.Data;
    // Extract the value from data
}
```

## ⚙️ Properties

| Property | Type | Description |
|----------|------|-------------|
| `Data` | `ILabelControlBase` | The input control's data after dialog closes |

## 📝 Notes

- Accepts any `ILabelControlBase` implementation (LabelTextBox, LabelComboBox, etc.)
- OK validates the input before closing

## 🔗 Related Controls

- **InfoDialogBox** - Information-only dialog
- **QuestionDialogBox** - Yes/No question dialog
- **InputFormDialogBox** - Multi-field form dialog

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs** for interactive examples.
