# 🪟 InWindowDialogService

An `IDialogService` that shows the standard dialogs inside the window, as centred modal flyouts, instead of as separate windows.

## 🔍 Overview

`InWindowDialogService` shows the same dialogs as `DialogService` (information, warning, error, confirmation, OK/Cancel, text input, colour picker), plus an input form and a font picker, but inside the window: each dialog is a centred, modal `Flyout` in the window's `FlyoutHost`. The dialogs keep the look and answers of the windowed ones (same `DialogBoxSettings`, buttons, icons and results), and view models await them through `IDialogService` exactly as before, so switching is a DI registration.

In-window dialogs stay with their window when it moves between monitors, work in kiosk and full-screen layouts where a second top-level window is unwanted, and dim only the window they belong to.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs;
using Atc.Wpf.Components.Flyouts;
```

## 🚀 Usage

### Put a FlyoutHost over the window content

```xml
<Grid>
    <!-- The window content -->
    <ContentControl Content="{Binding CurrentView}" />

    <!-- The dialogs are shown here, over the content -->
    <flyouts:FlyoutHost x:Name="DialogHost" />
</Grid>
```

### Register the services

```csharp
var flyoutService = new FlyoutService();
services.AddSingleton<IFlyoutService>(flyoutService);
services.AddSingleton<IDialogService>(sp => new InWindowDialogService(sp.GetRequiredService<IFlyoutService>()));

// When the main window has been created:
flyoutService.SetFlyoutHost(mainWindow.DialogHost);
```

### Await a dialog from a view model

```csharp
public sealed partial class DocumentViewModel(IDialogService dialogService) : ViewModelBase
{
    [RelayCommand]
    private async Task Delete()
    {
        if (await dialogService.ShowConfirmation("Delete file", "Do you want to delete 'report.pdf'?"))
        {
            // ...
        }
    }
}
```

### Input form and font picker

These are on the class (not on `IDialogService`):

```csharp
var name = new LabelTextBox { LabelText = "Name", IsMandatory = true };
var form = new LabelControlsForm();
form.AddColumn(new List<ILabelControlBase> { name });

if (await inWindowDialogService.ShowInputForm("New contact", form))
{
    // name.Text holds the entered value
}

var font = await inWindowDialogService.ShowFontPicker("Pick a font");
```

`DialogService` has the same `ShowInputForm` for windowed input forms.

## 🛠️ Methods

| Method | Returns | Buttons | Answer |
|--------|---------|---------|--------|
| `ShowInformation(title, message)` | `Task<bool>` | OK | `true` for OK |
| `ShowWarning(title, message)` | `Task<bool>` | OK | `true` for OK |
| `ShowError(title, message)` | `Task<bool>` | OK | `true` for OK |
| `ShowConfirmation(title, message)` | `Task<bool>` | Yes / No | `true` for Yes |
| `ShowOkCancel(title, message)` | `Task<bool>` | OK / Cancel | `true` for OK |
| `ShowInput(title, label, defaultValue)` | `Task<string?>` | OK / Cancel | The text, or `null` |
| `ShowColorPicker(title, initialColor)` | `Task<Color?>` | OK / Cancel | The colour, or `null` |
| `ShowFontPicker(title, initialFont)` | `Task<FontDescription?>` | OK / Cancel | The font, or `null` |
| `ShowInputForm(title, form)` | `Task<bool>` | OK / Cancel | `true` for OK; OK only closes a valid form |

Every method takes an optional `CancellationToken`: cancelling it closes the open dialog and cancels the task; an already cancelled token never shows the dialog.

## 📝 Notes

- The dialog is modal to the window: the overlay blocks the content behind it, a click outside does not close it, and the keyboard focus stays inside it (Tab cycles through the dialog). The default button, or the text box of a text input, gets the focus when the dialog has opened, and the focus returns to where it was when it closes.
- Escape and the flyout's close button cancel (the same answer as No or Cancel).
- The dialog sizes to its content and stays inside the host; long content scrolls.
- Several dialogs can be open at once; they stack, and each task gets its own answer.
- The dialog follows the window's `FlowDirection` (right-to-left).
- **No host:** when the `FlyoutService` has no host (neither `SetFlyoutHost` nor `SetHostPanel` was called), the dialogs are shown as windows by the fallback service (a `DialogService` unless another is passed to the constructor) and one warning is written to `System.Diagnostics.Trace`.
- `FlyoutService.IsHostConfigured` tells whether a host has been set.

## 🔗 Related Controls

- **DialogService** - the same dialogs as separate windows
- **FlyoutHost** / **FlyoutService** - the overlay the dialogs are shown in
- **InfoDialogBox**, **QuestionDialogBox**, **InputDialogBox**, **InputFormDialogBox** - the windowed dialogs

## 🎮 Sample Application

See the sample in the Atc.Wpf.Sample application under **Wpf.Components > Dialogs > In-window dialogs**.
