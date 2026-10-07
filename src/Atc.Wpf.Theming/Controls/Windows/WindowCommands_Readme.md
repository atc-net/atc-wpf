# 🧰 WindowCommands

A `ToolBar`-based container for buttons and other controls placed in a `NiceWindow` title bar.

## 🔍 Overview

`WindowCommands` hosts the items shown in the left or right part of a `NiceWindow` title bar (`LeftWindowCommands` / `RightWindowCommands`). Each item is wrapped in a `WindowCommandsItem` container that can show a separator after it. The container's visibility follows the visibility of the hosted element, and separators are recalculated whenever items or their visibility change. Light and dark control templates can be supplied and are swapped when `Theme` changes.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Windows;
```

## 🚀 Usage

### Left and Right Title Bar Commands

```xml
<atc:NiceWindow
    x:Class="MyApp.MainWindow"
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    Title="My Application">

    <atc:NiceWindow.LeftWindowCommands>
        <atc:WindowCommands>
            <Button Content="Menu" />
        </atc:WindowCommands>
    </atc:NiceWindow.LeftWindowCommands>

    <atc:NiceWindow.RightWindowCommands>
        <atc:WindowCommands ShowLastSeparator="False">
            <Button Content="Settings" />
            <Button Content="About" />
        </atc:WindowCommands>
    </atc:NiceWindow.RightWindowCommands>

    <Grid />
</atc:NiceWindow>
```

### Without Separators

```xml
<atc:WindowCommands ShowSeparators="False">
    <Button Content="Help" />
    <Button Content="Account" />
</atc:WindowCommands>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowSeparators` | `bool` | `true` | Show a separator after each item |
| `ShowLastSeparator` | `bool` | `true` | Show the separator after the last visible item (requires `ShowSeparators`) |
| `SeparatorHeight` | `double` | `15` | Height of the separators |
| `Theme` | `string` | `ThemeManager.BaseColorLight` | Current base color; switching to Light/Dark applies `LightTemplate` / `DarkTemplate` when set |
| `LightTemplate` | `ControlTemplate?` | `null` | Template used for the light theme |
| `DarkTemplate` | `ControlTemplate?` | `null` | Template used for the dark theme |
| `ParentWindow` | `Window?` | `null` | Read-only. The hosting window, resolved when the control is loaded |

## 📝 Notes

- Items that are not already a `WindowCommandsItem` are wrapped in one automatically
- When the hosted element's `Visibility` changes, the container's visibility follows and separators are re-evaluated
- On load the control copies `DockPanel.Dock` from its parent `ContentPresenter` and resolves `ParentWindow`
- `NiceWindow` passes its `DataContext` to `LeftWindowCommands` and `RightWindowCommands`, so items can bind to the window's view model
- Whether the commands overlay a hidden title bar is controlled by `NiceWindow.LeftWindowCommandsOverlayBehavior` / `RightWindowCommandsOverlayBehavior`

## 🔗 Related Controls

- **NiceWindow** - The themed window hosting the commands
- **WindowCommandsItem** - Item container with separator support

## 🎮 Sample Application

No dedicated sample yet. See **Wpf.Theming > Window > NiceWindow** for the window that hosts window commands.
