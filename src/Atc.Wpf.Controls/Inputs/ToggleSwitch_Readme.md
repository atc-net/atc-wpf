# 🔀 ToggleSwitch

A switch control for toggling between on and off states with animated knob and customizable labels.

## 🔍 Overview

`ToggleSwitch` provides a modern on/off toggle with a sliding knob animation, customizable on/off content labels, and full command support. It inherits from `HeaderedContentControl` and implements `ICommandSource`, supporting three command levels: a primary command on every toggle, plus separate `OnCommand` and `OffCommand` for state-specific actions. The control includes drag support, keyboard navigation, and accessibility via an automation peer.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Toggle

```xml
<inputs:ToggleSwitch Header="Enable notifications" IsOn="{Binding IsEnabled}" />
```

### Custom On/Off Labels

```xml
<inputs:ToggleSwitch
    Header="Dark mode"
    OnContent="Dark"
    OffContent="Light"
    IsOn="{Binding IsDarkMode}" />
```

### With Commands

```xml
<inputs:ToggleSwitch
    Header="WiFi"
    IsOn="{Binding IsWifiEnabled}"
    Command="{Binding ToggleWifiCommand}"
    OnCommand="{Binding EnableWifiCommand}"
    OffCommand="{Binding DisableWifiCommand}" />
```

### Content Direction

```xml
<!-- Label on the right (default) -->
<inputs:ToggleSwitch Header="Option A" ContentDirection="LeftToRight" />

<!-- Label on the left -->
<inputs:ToggleSwitch Header="Option B" ContentDirection="RightToLeft" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsOn` | `bool` | `false` | Current toggle state (two-way bindable) |
| `OnContent` | `object?` | localized "On" | Content displayed when toggle is on |
| `OffContent` | `object?` | localized "Off" | Content displayed when toggle is off |
| `OnContentTemplate` | `DataTemplate?` | `null` | Template for on content |
| `OffContentTemplate` | `DataTemplate?` | `null` | Template for off content |
| `ContentDirection` | `FlowDirection` | `LeftToRight` | Direction of content relative to switch |
| `ContentPadding` | `Thickness` | `0,1,0,0` | Padding around content area |
| `Command` | `ICommand?` | `null` | Command executed on every toggle |
| `OnCommand` | `ICommand?` | `null` | Command executed when toggled on |
| `OffCommand` | `ICommand?` | `null` | Command executed when toggled off |
| `CommandParameter` | `object?` | `null` | Parameter passed to commands |
| `CommandTarget` | `IInputElement?` | `null` | Target for routed commands |
| `IsPressed` | `bool` | `false` | Whether the control is currently pressed |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Toggled` | `RoutedEventHandler` | Raised when the toggle state changes |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Space` | Toggle the switch |

## 📝 Notes

- Default on/off labels are localized ("On"/"Off") and update automatically on culture change
- The knob can be dragged with the mouse for a slider-like interaction
- Switch size is 44x20 pixels with a 10px knob radius
- Animation uses 500ms exponential easing between states
- `IsOn` supports two-way binding and journaling
- Three-level command: `Command` fires on every toggle, then `OnCommand` or `OffCommand` fires based on new state

## 🔗 Related Controls

- **LabelToggleSwitch** - Labeled form version with validation support
- **Rating** - Another interactive input control
- **RangeSlider** - Slider-based range selection

## 🎮 Sample Application

See the ToggleSwitch sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > ToggleSwitch** for interactive examples.
