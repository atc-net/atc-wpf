# 🪟 NiceWindow

A themed WPF window with a customizable title bar, window commands, overlay dimming, dialog hosting, flyout support and window placement persistence.

## 🔍 Overview

`NiceWindow` derives from `WindowChromeWindow` and replaces the standard window chrome with a themed template. It adds a configurable title bar (icon, title, height, alignment, brushes), left/right `WindowCommands` areas, configurable minimize/maximize/close buttons, an overlay layer used to dim the window while a dialog is open, and a `WindowsSettingBehavior` that can save and restore the window position. The template also contains a flyout host area, so `Flyout` controls placed in the window content are displayed inside the window.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Windows;
```

## 🚀 Usage

### Basic Example

```xml
<atc:NiceWindow
    x:Class="MyApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    Title="My Application"
    Width="1024"
    Height="768"
    SaveWindowPosition="True">

    <Grid>
        <!-- Main content -->
    </Grid>
</atc:NiceWindow>
```

```csharp
public partial class MainWindow : NiceWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

### Title Bar Commands

```xml
<atc:NiceWindow.LeftWindowCommands>
    <atc:WindowCommands>
        <Button Content="Menu" />
    </atc:WindowCommands>
</atc:NiceWindow.LeftWindowCommands>

<atc:NiceWindow.RightWindowCommands>
    <atc:WindowCommands>
        <Button Content="Settings" />
    </atc:WindowCommands>
</atc:NiceWindow.RightWindowCommands>
```

### Title Bar Customization

```xml
<atc:NiceWindow
    Title="Tool"
    TitleBarHeight="40"
    TitleCharacterCasing="Upper"
    TitleAlignment="Left"
    ShowIconOnTitleBar="False"
    ShowMaxRestoreButton="False" />
```

### Overlay (e.g. while showing a dialog)

```csharp
await myWindow.ShowOverlayAsync();
// ... show dialog ...
await myWindow.HideOverlayAsync();

// Without animation
myWindow.ShowOverlay();
myWindow.HideOverlay();
```

### Hosting a Flyout

```csharp
var flyout = new Flyout
{
    Header = "Information Panel",
    FlyoutWidth = 400,
    Position = FlyoutPosition.Right,
};

var grid = new Grid();
grid.Children.Add(flyout);

var window = new NiceWindow
{
    Title = "NiceWindow - Flyout Demo",
    Content = grid,
};
```

### Windows 11 Backdrop (Mica, Acrylic, Tabbed)

```xml
<atc:NiceWindow
    xmlns:controlzEx="clr-namespace:ControlzEx.Theming;assembly=ControlzEx"
    BackdropType="{x:Static controlzEx:WindowBackdropType.Mica}">
    <!-- Leave the content background unset (transparent) so the backdrop shows through -->
</atc:NiceWindow>
```

While Windows applies the backdrop, `IsBackdropActive` is `true` and the default style makes the window background and title bar transparent, switches the title and window buttons to `AtcApps.Brushes.ThemeForeground`, and extends the frame into the client area (`GlassFrameThickness = -1`). Values you set locally on these properties win over the style.

## ⚙️ Properties

### Title Bar

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowTitleBar` | `bool` | `true` | Show the title bar (coerced to `false` when `UseNoneWindowStyle` is `true`) |
| `TitleBarHeight` | `int` | `30` | Height of the title bar |
| `TitleCharacterCasing` | `CharacterCasing` | `Normal` | Casing of the title text |
| `TitleAlignment` | `HorizontalAlignment` | `Stretch` (style sets `Center`) | Horizontal alignment of the title |
| `TitleForeground` | `Brush?` | `null` (style sets `AtcApps.Brushes.IdealForeground`) | Title text brush |
| `TitleTemplate` | `DataTemplate?` | `null` (style sets a trimmed `TextBlock`) | Template for the title |
| `WindowTitleBrush` | `Brush` | `Transparent` (style sets `AtcApps.Brushes.WindowTitle`) | Title bar background when active |
| `NonActiveWindowTitleBrush` | `Brush` | `Gray` (style sets `AtcApps.Brushes.WindowTitle.NonActive`) | Title bar background when inactive |
| `NonActiveBorderBrush` | `Brush` | `Gray` (style sets `AtcApps.Brushes.Border.NonActive`) | Border brush when inactive |
| `UseNoneWindowStyle` | `bool` | `false` | Hide the title bar entirely (set automatically when `WindowStyle="None"`) |
| `IsWindowDraggable` | `bool` | `true` | Allow dragging the window by the title bar |
| `ShowSystemMenu` | `bool` | `true` | Show the system menu when the title-bar icon is clicked (double-clicking the icon closes the window) |
| `ShowSystemMenuOnRightClick` | `bool` | `true` | Show the system menu when right-clicking the title bar |

### Icon

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowIconOnTitleBar` | `bool` | `true` | Show the window icon in the title bar |
| `IconTemplate` | `DataTemplate?` | `null` | Template used to render the icon (style provides a `MultiFrameImage` when `Icon` is set) |
| `IconWidth` | `double` | `20` | Icon width |
| `IconHeight` | `double` | `20` | Icon height |
| `IconMargin` | `Thickness` | `10,3,10,3` | Icon margin |
| `IconEdgeMode` | `EdgeMode` | `Aliased` | Edge mode used when rendering the icon |
| `IconBitmapScalingMode` | `BitmapScalingMode` | `HighQuality` | Bitmap scaling mode for the icon |
| `IconScalingMode` | `MultiFrameImageMode` | `ScaleDownLargerFrame` | Frame selection mode for multi-frame icons |
| `IconOverlayBehavior` | `OverlayBehavior` | `Never` | When it includes `HiddenTitleBar`, the icon stays visible while the title bar is hidden |

### Window Buttons and Commands

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowMinButton` | `bool` | `true` | Show the minimize button |
| `ShowMaxRestoreButton` | `bool` | `true` | Show the maximize/restore button |
| `ShowCloseButton` | `bool` | `true` | Show the close button |
| `IsMinButtonEnabled` | `bool` | `true` | Enable the minimize button |
| `IsMaxRestoreButtonEnabled` | `bool` | `true` | Enable the maximize/restore button |
| `IsCloseButtonEnabled` | `bool` | `true` | Enable the close button |
| `LeftWindowCommands` | `WindowCommands?` | `null` | Commands shown at the left of the title bar |
| `RightWindowCommands` | `WindowCommands?` | `null` | Commands shown at the right of the title bar |
| `WindowButtonCommands` | `WindowButtonCommands?` | `null` | The minimize/maximize/close button host |
| `LeftWindowCommandsOverlayBehavior` | `WindowCommandsOverlayBehaviorType` | `Never` | Whether left commands overlay a hidden title bar (`Never`, `HiddenTitleBar`) |
| `RightWindowCommandsOverlayBehavior` | `WindowCommandsOverlayBehaviorType` | `Never` | Whether right commands overlay a hidden title bar (`Never`, `HiddenTitleBar`) |
| `WindowButtonCommandsOverlayBehavior` | `OverlayBehavior` | `Always` | Overlay behavior of the window buttons (`Never`, `Overlays`, `HiddenTitleBar`, `Always`) |
| `OverrideDefaultWindowCommandsBrush` | `Brush?` | `null` | Foreground brush applied to the left/right window commands and the window button commands (theme default when `null`) |

### Overlay and Dialogs

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `OverlayBrush` | `Brush?` | `null` (style sets `AtcApps.Brushes.ThemeForeground`) | Brush of the dimming overlay |
| `OverlayOpacity` | `double` | `0.7` | Opacity of the overlay when shown |
| `OverlayFadeIn` | `Storyboard?` | `null` (style sets a fast fade-in) | Storyboard used by `ShowOverlayAsync` |
| `OverlayFadeOut` | `Storyboard?` | `null` (style sets a fast fade-out) | Storyboard used by `HideOverlayAsync` |
| `ShowDialogsOverTitleBar` | `bool` | `true` | Whether hosted dialogs cover the title bar |
| `IsAnyDialogOpen` | `bool` | `false` | Indicates a dialog is open; access keys are suppressed while `true` |

### Backdrop

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackdropType` | `WindowBackdropType` (ControlzEx) | `None` | System backdrop drawn behind the window: `None`, `Auto`, `Mica`, `Acrylic` or `Tabbed` |
| `IsBackdropActive` | `bool` (read-only) | `false` | `true` while Windows has applied `BackdropType` to the window |

### Window Placement and Transitions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SaveWindowPosition` | `bool` | `false` | Persist and restore the window placement |
| `WindowPlacementSettings` | `IWindowPlacementSettings?` | `null` | Custom placement storage (defaults to `WindowApplicationSettings`) |
| `WindowTransitionsEnabled` | `bool` | `false` | Enable content transitions |

## 🧩 Methods

| Method | Description |
|--------|-------------|
| `ShowOverlayAsync()` | Fades the overlay in using `OverlayFadeIn` |
| `HideOverlayAsync()` | Fades the overlay out using `OverlayFadeOut` |
| `ShowOverlay()` / `HideOverlay()` | Shows/hides the overlay immediately |
| `IsOverlayVisible()` | Returns `true` when the overlay is visible at `OverlayOpacity` |
| `StoreFocus(IInputElement?)` | Stores the element (or the currently focused one) to refocus after a dialog closes |
| `ResetStoredFocus()` | Clears the stored focus element |
| `GetWindowPlacementSettings()` | Returns `WindowPlacementSettings` or a new `WindowApplicationSettings`; virtual so it can be overridden |

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `WindowTransitionCompleted` | `RoutedEvent` | Raised when the content transition of the window finishes |

## 📝 Notes

- `ShowOverlayAsync`, `HideOverlayAsync` and `IsOverlayVisible` throw `InvalidOperationException` if called before the template is applied
- `WindowStyle="None"` sets `UseNoneWindowStyle` and `ResizeMode="NoResize"`; `WindowStyle="ToolWindow"` hides the minimize and maximize buttons
- The window's `DataContext` is passed on to `LeftWindowCommands`, `RightWindowCommands` and `WindowButtonCommands`
- `SaveWindowPosition` is handled by the `WindowsSettingBehavior` that is attached in the constructor
- Many brush and template defaults are set by the default style; the defaults listed first are the dependency property defaults
- `BackdropType` needs Windows 11 22H2 or later; on older Windows, with `AllowsTransparency="True"`, or when Windows refuses it, `IsBackdropActive` stays `false` and the window keeps its theme background. In high contrast the window also stays opaque
- Works with .NET's `Application.ThemeMode` (Fluent): WPF merges its Fluent dictionary ahead of the application's dictionaries, so the ATC control styles still win, and NiceWindow removes the Fluent `Window` style that WPF assigns to windows without a style (it would replace the NiceWindow template), also when the theme mode changes while the window is open. Keep `ThemeMode` and the ATC theme on the same light/dark: WPF sets the window's dark mode from `ThemeMode`, so a Dark `ThemeMode` with a Light ATC theme tints the backdrop dark under dark text
- The backdrop only shows where nothing opaque is drawn, so content with its own background (panels using `AtcApps.Brushes.ThemeBackground`, for example) covers it. Windows draws backdrops only while the window is active

## 🔗 Related Controls

- **NiceDialogBox** - Dialog window built on `NiceWindow`
- **WindowCommands** - Title bar command container
- **Flyout** - Slide-in panel that can be hosted inside a `NiceWindow`
- **MultiFrameImage** - Used to render the title bar icon

## 🎮 Sample Application

See the NiceWindow samples in the Atc.Wpf.Sample application under **Wpf.Theming > Window > NiceWindow**, **Wpf.Theming > Window > NiceWindow backdrop** and **Wpf.Theming > Window > NiceWindow with Flyout** for interactive examples.
