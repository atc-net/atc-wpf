# 🔘 DialogBoxButtonBar

The row of dialog buttons used by the Atc dialog boxes, built from a `DialogBoxSettings`.

## 🔍 Overview

`DialogBoxButtonBar` creates its buttons from `DialogBoxSettings` whenever `Settings` changes:

- an optional copy button (`ShowCopyButton`), first in the row
- the affirmative button, always shown
- the negative button when `ShowNegativeButton` is set (the `OkCancel`, `YesNo` and `YesNoCancel` dialog types; in `OkCancel` it is the Cancel button)
- a separate Cancel button when `ShowCancelButton` is set (the `YesNoCancel` dialog type)

The button texts and colours come from the settings. The `DefaultButton` button (the first button when that one is not shown) is the default, so **Enter** clicks it. **Escape** clicks Cancel, or the negative button when there is no Cancel button.

`InfoDialogBox`, `QuestionDialogBox`, `InputDialogBox`, `InputFormDialogBox`, `ColorPickerDialogBox` and `FontPickerDialogBox` all use it. Use it directly when you build your own dialog window and want the same buttons and keyboard behaviour.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.Dialogs;
```

In XAML it is in the `https://github.com/atc-net/atc-wpf/tree/main/schemas` namespace (`atc` prefix).

## 🚀 Usage

```xml
<atc:DialogBoxButtonBar
    ButtonClicked="OnButtonClicked"
    CopyClicked="OnCopyClicked"
    Settings="{Binding Settings}" />
```

```csharp
Settings = DialogBoxSettings.Create(DialogBoxType.YesNoCancel);
Settings.DefaultButton = DialogBoxButton.Negative;

private void OnButtonClicked(object? sender, DialogBoxButtonClickedEventArgs e)
{
    DialogResult = e.Button switch
    {
        DialogBoxButton.Affirmative => true,
        DialogBoxButton.Negative => false,
        _ => null,
    };
}
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Settings` | `DialogBoxSettings?` | `null` | The buttons, their texts and colours and the default button; `null` shows no buttons |

The `DialogBoxSettings` members it reads:

| Member | Description |
|--------|-------------|
| `AffirmativeButtonText`, `AffirmativeButtonBackground`, `AffirmativeButtonForeground` | Text and colours of the affirmative button |
| `NegativeButtonText`, `NegativeButtonBackground`, `NegativeButtonForeground` | Text and colours of the negative button; the Cancel button uses the same colours |
| `CancelButtonText` | Text of the Cancel button |
| `ShowNegativeButton`, `ShowCancelButton` | Set by the dialog type |
| `ShowCopyButton` | Shows a copy button before the other buttons |
| `DefaultButton` | The button that Enter clicks (default `Affirmative`) |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ButtonClicked` | `EventHandler<DialogBoxButtonClickedEventArgs>` | A dialog button was clicked; `e.Button` is `Affirmative`, `Negative` or `Cancel` |
| `CopyClicked` | `EventHandler` | The copy button was clicked; the host copies its text. The button shows a check mark for 1.5 seconds |

## 🛠️ Methods

| Method | Description |
|--------|-------------|
| `GetButton(DialogBoxButton button)` | The button shown for `button`, or `null` when it is not shown |
| `GetCopyButton()` | The copy button, or `null` when it is not shown |

## ♿ UI Automation

| Constant | Automation id |
|----------|---------------|
| `AffirmativeButtonAutomationId` | `DialogBoxAffirmativeButton` |
| `NegativeButtonAutomationId` | `DialogBoxNegativeButton` |
| `CancelButtonAutomationId` | `DialogBoxCancelButton` |
| `CopyButtonAutomationId` | `DialogBoxCopyButton` |

## 📝 Notes

- The buttons are rebuilt when `Settings` is replaced, not when a property of the same settings object changes.
- The bar itself is not focusable and is not a tab stop.

## 🔗 Related Controls

- **InfoDialogBox**, **QuestionDialogBox**, **InputDialogBox**, **InputFormDialogBox** - the dialog boxes in Atc.Wpf.Components that host it
- **ColorPickerDialogBox**, **FontPickerDialogBox** - the picker dialogs in Atc.Wpf.Forms

## 🎮 Sample Application

See the dialog samples in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs**.
