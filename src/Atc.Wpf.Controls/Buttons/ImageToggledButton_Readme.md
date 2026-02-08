# 🔄 ImageToggledButton

A button that toggles between two visual states with separate images, labels, and commands.

## 🔍 Overview

`ImageToggledButton` provides a toggle button with distinct image, content, and command for each state (on/off). It supports bitmap and SVG images, configurable icon placement, and a busy loading indicator. Useful for play/pause, show/hide, or any binary state toggle.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Buttons;
```

## 🚀 Usage

### Basic Toggle Button

```xml
<buttons:ImageToggledButton
    IsToggled="{Binding IsPlaying}"
    OnContent="Pause"
    OffContent="Play"
    OnSvgImageSource="/MyApp;component/Assets/pause.svg"
    OffSvgImageSource="/MyApp;component/Assets/play.svg"
    ImageLocation="Left"
    OnCommand="{Binding PauseCommand}"
    OffCommand="{Binding PlayCommand}" />
```

### With Bitmap Images

```xml
<buttons:ImageToggledButton
    IsToggled="{Binding IsVisible}"
    OnContent="Hide"
    OffContent="Show"
    OnImageSource="/Assets/eye-off.png"
    OffImageSource="/Assets/eye.png"
    ImageLocation="Left" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsToggled` | `bool` | `false` | Current toggle state |
| `OnContent` | `object?` | `null` | Content when toggled on |
| `OffContent` | `object?` | `null` | Content when toggled off |
| `OnImageSource` | `ImageSource?` | `null` | Bitmap image when on |
| `OffImageSource` | `ImageSource?` | `null` | Bitmap image when off |
| `OnSvgImageSource` | `string` | `""` | SVG path when on |
| `OffSvgImageSource` | `string` | `""` | SVG path when off |
| `OnSvgImageOverrideColor` | `Color?` | `null` | SVG color override when on |
| `OffSvgImageOverrideColor` | `Color?` | `null` | SVG color override when off |
| `OnCommand` | `ICommand?` | `null` | Command when toggled on |
| `OffCommand` | `ICommand?` | `null` | Command when toggled off |
| `ImageLocation` | `ImageLocation?` | `null` | Icon position |
| `ImageWidth` | `int` | `16` | Image width |
| `ImageHeight` | `int` | `16` | Image height |
| `ImageContentSpacing` | `double` | `10` | Space between image and content |
| `ImageBorderSpacing` | `double` | `0` | Space between image and border |
| `IsBusy` | `bool` | `false` | Show loading indicator |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `IsToggledChanged` | `RoutedEventHandler` | Raised when toggle state changes |

## 📝 Notes

- Each state has its own image, content, and command
- The button executes `OnCommand` or `OffCommand` based on the current state

## 🔗 Related Controls

- **ImageButton** - Single-state image button
- **AuthenticationButton** - Specialized login/logout toggle
- **ConnectivityButton** - Specialized connect/disconnect toggle
- **ToggleSwitch** - Switch-style toggle control

## 🎮 Sample Application

See the ImageToggledButton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Buttons > ImageToggledButton** for interactive examples.
