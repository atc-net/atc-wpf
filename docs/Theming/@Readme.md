# 🎨 Theming Controls

The `Atc.Wpf.Theming` library provides window infrastructure, theme switching, icon rendering, and enhanced base controls for building themed WPF applications.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Windows;    // NiceWindow, WindowCommands
using Atc.Wpf.Theming.Controls.Selectors;  // ThemeSelector, AccentColorSelector
using Atc.Wpf.Theming.Controls.Icons;      // FontIcon, PathIcon
using Atc.Wpf.Theming.Controls.Images;     // MultiFrameImage
using Atc.Wpf.Theming.Controls;            // TransitioningContentControl, ContentControlEx
```

---

## 🪟 NiceWindow

An enhanced WPF window with customizable title bar, window commands, overlay dimming, dialog hosting, flyout integration, and window placement persistence.

### Usage

```xml
<theming:NiceWindow
    x:Class="MyApp.MainWindow"
    xmlns:theming="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    Title="My Application"
    TitleBarHeight="30"
    ShowIconOnTitleBar="True"
    SaveWindowPosition="True">

    <theming:NiceWindow.LeftWindowCommands>
        <theming:WindowCommands>
            <Button Content="Menu" />
        </theming:WindowCommands>
    </theming:NiceWindow.LeftWindowCommands>

    <Grid>
        <!-- Main content -->
    </Grid>
</theming:NiceWindow>
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowTitleBar` | `bool` | `true` | Show/hide the title bar |
| `TitleBarHeight` | `int` | `30` | Title bar height in pixels |
| `TitleCharacterCasing` | `CharacterCasing` | `Normal` | Title text casing |
| `TitleAlignment` | `HorizontalAlignment` | `Stretch` | Title text alignment |
| `ShowIconOnTitleBar` | `bool` | `true` | Show window icon |
| `IconWidth` / `IconHeight` | `double` | `20` | Icon dimensions |
| `ShowMinButton` | `bool` | `true` | Show minimize button |
| `ShowMaxRestoreButton` | `bool` | `true` | Show maximize/restore button |
| `ShowCloseButton` | `bool` | `true` | Show close button |
| `SaveWindowPosition` | `bool` | `false` | Persist window placement |
| `WindowTransitionsEnabled` | `bool` | `false` | Enable content transitions |
| `IsWindowDraggable` | `bool` | `true` | Allow window dragging |
| `UseNoneWindowStyle` | `bool` | `false` | Hide title bar entirely |
| `OverlayBrush` | `Brush` | - | Dialog overlay brush |
| `OverlayOpacity` | `double` | `0.7` | Overlay opacity |

### Overlay Methods

```csharp
// Show dimming overlay (for dialogs)
await myWindow.ShowOverlayAsync();

// Hide overlay
await myWindow.HideOverlayAsync();

// Focus management
myWindow.StoreFocus(currentElement);
myWindow.RestoreFocus();
```

---

## 🔄 TransitioningContentControl

Animates content changes with configurable transition effects.

### Usage

```xml
<theming:TransitioningContentControl
    Transition="Left"
    Content="{Binding CurrentView}" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Transition` | `TransitionType` | `Default` | Animation type |
| `IsTransitioning` | `bool` | `false` | Read-only: currently animating |
| `RestartTransitionOnContentChange` | `bool` | `false` | Restart if same content set again |

### TransitionType Values

| Value | Description |
|-------|-------------|
| `Default` | Default behavior |
| `Normal` | Cross-fade |
| `Up` | Slide up |
| `Down` | Slide down |
| `Left` | Slide left |
| `Right` | Slide right |
| `LeftReplace` | Slide left with replacement |
| `RightReplace` | Slide right with replacement |
| `Custom` | User-defined visual states |

### Events

| Event | Description |
|-------|-------------|
| `TransitionCompleted` | Fires when transition animation finishes |

---

## 🎨 ThemeSelector

Dropdown selector for switching between Light and Dark themes.

### Usage

```xml
<theming:ThemeSelector
    RenderColorIndicatorType="Square" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape (Square or Circle) |
| `SelectedKey` | `string` | - | Currently selected theme name |

---

## 🌈 AccentColorSelector

Dropdown selector for switching accent colors.

### Usage

```xml
<theming:AccentColorSelector
    RenderColorIndicatorType="Circle" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape |
| `SelectedKey` | `string` | - | Currently selected accent color name |

---

## 🎨 WellKnownColorSelector

ComboBox with the full WPF named color palette. Supports basic or extended colors.

### Usage

```xml
<theming:WellKnownColorSelector
    UseOnlyBasicColors="False"
    ShowHexCode="True"
    SelectedKey="{Binding SelectedColor, Mode=TwoWay}" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | First item: None, Blank, PleaseSelect, IncludeAll |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape |
| `ShowHexCode` | `bool` | `true` | Display hex codes |
| `UseOnlyBasicColors` | `bool` | `false` | Show 16 basic colors vs. extended palette |
| `DefaultColorName` | `string` | `""` | Default color selection |
| `SelectedKey` | `string` | - | Two-way bindable selected color name |

### Events

| Event | Description |
|-------|-------------|
| `SelectorChanged` | Fires when color selection changes |

---

## 🔤 FontIcon

Renders a glyph from a font family as an icon.

### Usage

```xml
<theming:FontIcon Glyph="&#xE001;" FontFamily="Segoe MDL2 Assets" />
```

---

## 📐 PathIcon

Renders a vector path (WPF Geometry) as an icon.

### Usage

```xml
<theming:PathIcon Data="M10,10 L20,20 L10,30 Z" Foreground="Blue" />
```

---

## 🖼️ MultiFrameImage

Intelligently selects the best frame from multi-frame images (ICO, TIF) based on rendering size.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MultiFrameImageMode` | `MultiFrameImageMode` | `ScaleDownLargerFrame` | Frame selection strategy |

---

## 💬 NiceDialogBox

Preconfigured dialog window (centered, no taskbar, no min/max buttons) inheriting from NiceWindow.

### Usage

```csharp
var dialog = new NiceDialogBox
{
    Content = new MyDialogContent(),
    Width = 400,
    Height = 300
};
dialog.ShowDialog();
```

---

## 🔧 WindowCommands

Specialized toolbar for hosting action buttons in the window title bar.

### Usage

```xml
<theming:NiceWindow.RightWindowCommands>
    <theming:WindowCommands ShowSeparators="True">
        <Button Content="Settings" />
        <Button Content="Help" />
    </theming:WindowCommands>
</theming:NiceWindow.RightWindowCommands>
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Theme` | `string` | `"Light"` | Active theme |
| `ShowSeparators` | `bool` | `true` | Show dividers between items |
| `ShowLastSeparator` | `bool` | `true` | Show separator after last item |
| `SeparatorHeight` | `double` | `15` | Separator height in pixels |

---

## 📝 Notes

- All controls automatically respond to theme changes via `ThemeManager`
- Selectors respond to `CultureManager.UiCultureChanged` for localization
- NiceWindow integrates with the Flyout system from `Atc.Wpf.Controls`
- Standard WPF controls (Button, TextBox, etc.) are automatically themed when `Atc.Wpf.Theming/Styles/Default.xaml` is merged

## 🎮 Sample Application

See the theming samples in the Atc.Wpf.Sample application under **Wpf.Theming** for interactive examples of all themed controls, NiceWindow configurations, and color selectors.
