# 💬 NiceDialogBox

A preconfigured themed dialog window based on `NiceWindow`.

## 🔍 Overview

`NiceDialogBox` is a `NiceWindow` with dialog-friendly defaults: it opens centered on the screen, does not appear in the taskbar, hides the minimize, maximize/restore and close title-bar buttons, and starts at 350 x 200. Because it is a full `NiceWindow`, all title bar, window command, overlay and flyout features are available. It is the base class for the library dialogs such as `InfoDialogBox`, `QuestionDialogBox`, `InputDialogBox`, `InputFormDialogBox`, `BasicApplicationSettingsDialogBox`, `ColorPickerDialogBox` and `FontPickerDialogBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Themes.Dialogs;
```

```xml
xmlns:dialogs="clr-namespace:Atc.Wpf.Theming.Themes.Dialogs;assembly=Atc.Wpf.Theming"
```

## 🚀 Usage

### Basic Example (C#)

```csharp
var dialog = new NiceDialogBox
{
    Title = "Basic Dialog",
    Content = new TextBlock
    {
        Text = "This is a basic NiceDialogBox with default settings.",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(20),
    },
};

dialog.Show();
```

### Modal Dialog with Result

```csharp
var dialog = new NiceDialogBox
{
    Title = "Confirmation",
    Width = 400,
    Height = 180,
    ShowCloseButton = true,
};

var yesButton = new Button { Content = "Yes", Width = 80 };
yesButton.Click += (_, _) =>
{
    dialog.DialogResult = true;
    dialog.Close();
};

dialog.Content = yesButton;

var result = dialog.ShowDialog();
```

### Custom Dialog in XAML

Derive your own dialog by using `NiceDialogBox` as the XAML root element:

```xml
<dialogs:NiceDialogBox
    x:Class="MyApp.Dialogs.MyDialogBox"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:dialogs="clr-namespace:Atc.Wpf.Theming.Themes.Dialogs;assembly=Atc.Wpf.Theming"
    Title="My Dialog"
    Width="500"
    Height="300">

    <DockPanel>
        <!-- Dialog content and buttons -->
    </DockPanel>

</dialogs:NiceDialogBox>
```

```csharp
public partial class MyDialogBox
{
    public MyDialogBox()
    {
        InitializeComponent();
    }
}
```

## ⚙️ Properties

`NiceDialogBox` declares no new properties. Its constructor sets these defaults on inherited properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `WindowStartupLocation` | `WindowStartupLocation` | `CenterScreen` | Dialog opens centered on the screen |
| `ShowInTaskbar` | `bool` | `false` | Dialog is not shown in the taskbar |
| `ShowMinButton` | `bool` | `false` | Minimize button hidden |
| `ShowMaxRestoreButton` | `bool` | `false` | Maximize/restore button hidden |
| `ShowCloseButton` | `bool` | `false` | Close button hidden (set to `true` to allow closing from the title bar) |
| `Width` | `double` | `350` | Initial width |
| `Height` | `double` | `200` | Initial height |

All other properties (title bar, window commands, overlay, flyouts, etc.) are inherited from `NiceWindow`.

## 📝 Notes

- Because the close button is hidden by default, make sure the dialog content provides a way to close it (or set `ShowCloseButton = true`)
- Use `ShowDialog()` for modal dialogs and set `DialogResult` before closing to return a result
- `NiceDialogBox` does not override `DefaultStyleKey`, so it uses the `NiceWindow` default style; a separate `AtcApps.Styles.NiceDialogBox` style exists in `Themes/Dialogs/NiceDialogBox.xaml` and can be applied explicitly

## 🔗 Related Controls

- **NiceWindow** - The themed window `NiceDialogBox` derives from
- **InfoDialogBox / QuestionDialogBox / InputDialogBox / InputFormDialogBox** - Ready-made dialogs built on `NiceDialogBox` (in `Atc.Wpf.Components`)
- **IDialogService** - MVVM-friendly way to show dialogs
- **Flyout** - Slide-in panels, also usable inside dialogs

## 🎮 Sample Application

See the NiceDialogBox samples in the Atc.Wpf.Sample application under **Wpf.Theming > Window > NiceDialogBox** and **Wpf.Theming > Window > NiceDialogBox with Flyout** for interactive examples.
