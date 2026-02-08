# 🔐 AuthenticationButton

A toggle button for login/logout state with automatic icon defaults from Material Design.

## 🔍 Overview

`AuthenticationButton` provides a pre-configured `ImageToggledButton` for authentication flows. It switches between login and logout states with separate images, labels, and commands. If no custom images are provided, it defaults to Material Design login/logout icons.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Buttons;
```

## 🚀 Usage

### Basic Authentication Button

```xml
<buttons:AuthenticationButton
    IsAuthenticated="{Binding IsLoggedIn}"
    LoginCommand="{Binding LoginCommand}"
    LogoutCommand="{Binding LogoutCommand}" />
```

### Custom Labels

```xml
<buttons:AuthenticationButton
    IsAuthenticated="{Binding IsLoggedIn}"
    LoginContent="Sign In"
    LogoutContent="Sign Out"
    LoginCommand="{Binding LoginCommand}"
    LogoutCommand="{Binding LogoutCommand}" />
```

### Custom SVG Icons

```xml
<buttons:AuthenticationButton
    IsAuthenticated="{Binding IsLoggedIn}"
    LoginSvgImageSource="/MyApp;component/Assets/login.svg"
    LogoutSvgImageSource="/MyApp;component/Assets/logout.svg"
    LoginSvgImageOverrideColor="White"
    LogoutSvgImageOverrideColor="White"
    LoginCommand="{Binding LoginCommand}"
    LogoutCommand="{Binding LogoutCommand}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsAuthenticated` | `bool` | `false` | Current authentication state |
| `LoginContent` | `object?` | auto | Label when logged out |
| `LogoutContent` | `object?` | auto | Label when logged in |
| `LoginImageSource` | `ImageSource?` | `null` | Bitmap image for login |
| `LogoutImageSource` | `ImageSource?` | `null` | Bitmap image for logout |
| `LoginSvgImageSource` | `string` | `""` | SVG path for login |
| `LogoutSvgImageSource` | `string` | `""` | SVG path for logout |
| `LoginSvgImageOverrideColor` | `Color?` | `null` | SVG color for login icon |
| `LogoutSvgImageOverrideColor` | `Color?` | `null` | SVG color for logout icon |
| `LoginCommand` | `ICommand?` | `null` | Command to execute on login |
| `LogoutCommand` | `ICommand?` | `null` | Command to execute on logout |
| `ImageLocation` | `ImageLocation?` | `Left` | Icon position |
| `ImageWidth` | `int` | `16` | Image size |
| `ImageHeight` | `int` | `16` | Image size |
| `IsBusy` | `bool` | `false` | Show loading indicator |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `IsAuthenticatedChanged` | `RoutedEventHandler` | Raised when authentication state changes |

## 📝 Notes

- Defaults to Material Design login/logout icons if no custom images are provided
- Responds to theme changes and updates icons accordingly

## 🔗 Related Controls

- **ConnectivityButton** - Connect/disconnect toggle
- **ImageToggledButton** - Generic two-state toggle button
- **ImageButton** - Single-state image button

## 🎮 Sample Application

See the AuthenticationButton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Buttons > AuthenticationButton** for interactive examples.
