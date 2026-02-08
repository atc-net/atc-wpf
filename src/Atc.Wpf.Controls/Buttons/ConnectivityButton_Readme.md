# 🔌 ConnectivityButton

A toggle button for connect/disconnect state with automatic icon defaults from Material Design.

## 🔍 Overview

`ConnectivityButton` provides a pre-configured toggle button for connectivity flows. It switches between connected and disconnected states with separate images, labels, and commands. If no custom images are provided, it defaults to Material Design icons.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Buttons;
```

## 🚀 Usage

### Basic Connectivity Button

```xml
<buttons:ConnectivityButton
    IsConnected="{Binding IsOnline}"
    ConnectCommand="{Binding ConnectCommand}"
    DisconnectCommand="{Binding DisconnectCommand}" />
```

### Custom Labels and Icons

```xml
<buttons:ConnectivityButton
    IsConnected="{Binding IsConnected}"
    ConnectContent="Go Online"
    DisconnectContent="Go Offline"
    ConnectSvgImageSource="/MyApp;component/Assets/wifi-on.svg"
    DisconnectSvgImageSource="/MyApp;component/Assets/wifi-off.svg"
    ConnectCommand="{Binding ConnectCommand}"
    DisconnectCommand="{Binding DisconnectCommand}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsConnected` | `bool` | `false` | Current connectivity state |
| `ConnectContent` | `object?` | auto | Label when disconnected |
| `DisconnectContent` | `object?` | auto | Label when connected |
| `ConnectImageSource` | `ImageSource?` | `null` | Bitmap image for connect |
| `DisconnectImageSource` | `ImageSource?` | `null` | Bitmap image for disconnect |
| `ConnectSvgImageSource` | `string` | `""` | SVG path for connect |
| `DisconnectSvgImageSource` | `string` | `""` | SVG path for disconnect |
| `ConnectSvgImageOverrideColor` | `Color?` | `null` | SVG color for connect |
| `DisconnectSvgImageOverrideColor` | `Color?` | `null` | SVG color for disconnect |
| `ConnectCommand` | `ICommand?` | `null` | Command to execute on connect |
| `DisconnectCommand` | `ICommand?` | `null` | Command to execute on disconnect |
| `ImageLocation` | `ImageLocation?` | `Left` | Icon position |
| `ImageWidth` | `int` | `16` | Image size |
| `ImageHeight` | `int` | `16` | Image size |
| `IsBusy` | `bool` | `false` | Show loading indicator |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `IsConnectedChanged` | `RoutedEventHandler` | Raised when connectivity state changes |

## 📝 Notes

- Defaults to Material Design icons if no custom images are provided
- Responds to theme changes and updates icons accordingly

## 🔗 Related Controls

- **AuthenticationButton** - Login/logout toggle
- **ImageToggledButton** - Generic two-state toggle button

## 🎮 Sample Application

See the ConnectivityButton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Buttons > ConnectivityButton** for interactive examples.
