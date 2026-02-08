# 📝 InputFormDialogBox

A modal dialog that presents a dynamic form built from `ILabelControlsForm` data with validation support.

## 🔍 Overview

`InputFormDialogBox` displays a modal dialog with a dynamically generated form using `LabelInputFormPanel`. The form layout is driven by an `ILabelControlsForm` data model, supporting multiple control types, validation, and auto-sizing. On OK click, the form validates all fields before closing.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
```

## 🚀 Usage

### Basic Form Dialog

```csharp
var form = new LabelControlsForm();
form.AddRow(new LabelControlsFormRow(new LabelTextBox { LabelText = "Name", IsMandatory = true }));
form.AddRow(new LabelControlsFormRow(new LabelTextBox { LabelText = "Email" }));

var dialog = new InputFormDialogBox(ownerWindow, "Enter Details", form);
if (dialog.ShowDialog() == true)
{
    var data = dialog.Data;
    // Extract form values from data
}
```

### With Header

```csharp
var dialog = new InputFormDialogBox(
    ownerWindow,
    titleBarText: "User Registration",
    headerText: "Please fill in your details",
    labelControlsForm: form);

if (dialog.ShowDialog() == true)
{
    // Process form data
}
```

### With Custom Settings

```csharp
var settings = new DialogBoxSettings
{
    TitleBarText = "Settings",
    Width = 600,
    Height = 400,
};

var dialog = new InputFormDialogBox(ownerWindow, settings, form);
dialog.ShowDialog();
```

### Re-Rendering

```csharp
// Re-render the form after modifying the data model
dialog.ReRender();
```

## ⚙️ Properties

| Property | Type | Description |
|----------|------|-------------|
| `OwningWindow` | `Window` | The parent/owner window |
| `Settings` | `DialogBoxSettings` | Dialog configuration settings |
| `HeaderControl` | `ContentControl?` | Optional header content |
| `LabelInputFormPanel` | `LabelInputFormPanel` | The form panel control |
| `Data` | `ILabelControlsForm` | The form data model (read-only, from LabelInputFormPanel) |

## 📝 Notes

- OK button validates all form fields before closing; dialog stays open if validation fails
- Cancel button closes with `DialogResult = false`
- Dialog auto-sizes based on form content
- `ReRender()` can be called to refresh the form after data model changes

## 🔗 Related Controls

- **InfoDialogBox** - Simple information dialog
- **QuestionDialogBox** - Yes/No question dialog
- **InputDialogBox** - Single text input dialog
- **LabelInputFormPanel** - The form panel used inside this dialog

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs** for interactive examples.
