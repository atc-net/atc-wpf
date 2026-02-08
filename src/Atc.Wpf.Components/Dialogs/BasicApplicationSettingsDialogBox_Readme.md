# ⚙️ BasicApplicationSettingsDialogBox

A pre-built settings dialog for common application preferences (theme, language, accent color).

## 🔍 Overview

`BasicApplicationSettingsDialogBox` provides a ready-to-use settings dialog with theme selection, accent color, and language preferences. It wraps the `BasicApplicationSettingsView` control and supports exporting settings as JSON.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
```

## 🚀 Usage

```csharp
var dialog = new BasicApplicationSettingsDialogBox();
dialog.ShowDialog();

// Or with a custom ViewModel
var viewModel = new BasicApplicationSettingsDialogBoxViewModel();
var dialog = new BasicApplicationSettingsDialogBox(viewModel);
dialog.ShowDialog();

// Export settings
string json = dialog.GetDataAsJson();
```

## 🔧 Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetDataAsJson()` | `string` | Export current settings as JSON |

## 📝 Notes

- Uses `BasicApplicationSettingsView` internally
- Supports custom ViewModels via `IBasicApplicationSettingsDialogBoxViewModel`

## 🔗 Related Controls

- **BasicApplicationSettingsView** - Embeddable settings panel
- **InfoDialogBox** - Simple information dialog
- **InputFormDialogBox** - Custom form dialog

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs** for interactive examples.
