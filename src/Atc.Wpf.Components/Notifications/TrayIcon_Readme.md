# 🔔 TrayIcon

An icon in the Windows notification area (system tray) with a tooltip, click commands, a context menu and desktop toast notifications.

## 🔍 Overview

`TrayIcon` adds an icon to the notification area while its `Visibility` is `Visible`. It suits monitoring and background applications that keep running when the main window is hidden.

- Click and double-click raise events and execute commands
- Right-click (or Shift+F10 / the menu key) opens the element's `ContextMenu` at the mouse position
- `ShowNotification` shows a desktop toast through the toast notification system, with an optional click action such as restoring the main window
- The icon comes back on its own after Explorer restarts

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Notifications;
```

## 🚀 Usage

### In a window

```xml
<Window.Resources>
    <BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter" />
</Window.Resources>

<Grid>
    <atc:TrayIcon
        x:Name="AppTrayIcon"
        DoubleClickCommand="{Binding RestoreWindowCommand}"
        IconSource="/Assets/app.ico"
        ToolTipText="{Binding Status}"
        Visibility="{Binding IsTrayIconVisible, Converter={StaticResource BooleanToVisibilityConverter}}">
        <atc:TrayIcon.ContextMenu>
            <ContextMenu>
                <MenuItem Command="{Binding RestoreWindowCommand}" Header="Open" />
                <Separator />
                <MenuItem Command="{Binding ExitCommand}" Header="Exit" />
            </ContextMenu>
        </atc:TrayIcon.ContextMenu>
    </atc:TrayIcon>

    <!-- Window content -->
</Grid>
```

### Desktop notification

```csharp
AppTrayIcon.ShowNotification(
    "Backup finished",
    "All files were copied.",
    ToastNotificationType.Success,
    onClick: () => Activate());
```

### In code, for an application without a main window

```csharp
var trayIcon = new TrayIcon
{
    IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/app.ico")),
    ToolTipText = "My service monitor",
};

trayIcon.DoubleClick += (_, _) => ShowMainWindow();

// On exit:
trayIcon.Dispose();
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IconSource` | `ImageSource?` | `null` | The image shown in the notification area, scaled to the small-icon size; the application icon when `null` |
| `ToolTipText` | `string` | `""` | The tooltip; the shell shows at most 127 characters |
| `ClickCommand` | `ICommand?` | `null` | Executed when the icon is clicked or selected with the keyboard |
| `DoubleClickCommand` | `ICommand?` | `null` | Executed when the icon is double-clicked |
| `CommandParameter` | `object?` | `null` | Passed to both commands |
| `ContextMenu` | `ContextMenu?` | `null` | Opened on right-click; it inherits the `DataContext` of the `TrayIcon` |
| `Visibility` | `Visibility` | `Visible` | The icon is shown only while `Visible` |
| `IsCreated` | `bool` | `false` | Read-only; whether the icon is currently in the notification area |
| `NotificationService` | `IToastNotificationService?` | `null` | Used by `ShowNotification`; a `ToastNotificationService` when not set |

## 🔔 Events and methods

| Member | Description |
|--------|-------------|
| `Click` | The icon was clicked or selected with the keyboard; also raised for the first click of a double-click |
| `DoubleClick` | The icon was double-clicked |
| `ShowNotification(title, message, type, expirationTime, onClick)` | Shows a desktop toast (`useDesktop: true`) of the given `ToastNotificationType` |
| `Dispose()` | Removes the icon; the `TrayIcon` can't be shown again |

## 📝 Notes

- Windows 11 puts a new icon in the hidden overflow (the ^ arrow next to the clock) until the user turns it on under **Settings > Personalization > Taskbar > Other system tray icons**. Applications can't do that themselves, so `IsCreated` can be `true` while the icon isn't visible on the taskbar
- In the visual tree, the icon follows the element: unloading removes it and loading adds it again. Declared in resources or created in code, it stays until `Dispose()`
- The icon is also removed when the dispatcher shuts down, so it doesn't linger after the application exits
- In the designer no icon is created
- Each `TrayIcon` is identified by its own hidden window and id, not by a GUID, so Debug and Release builds of the same application don't clash

## 🔗 Related Controls

- **ToastNotification / ToastNotificationService** - The toast system behind `ShowNotification`
- **ToastNotificationArea** - In-window toasts

## 🎮 Sample Application

See **Wpf.Components > Notifications > TrayIcon** in the sample application.