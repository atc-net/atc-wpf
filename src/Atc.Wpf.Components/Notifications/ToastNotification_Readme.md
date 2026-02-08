# 🔔 ToastNotification

A notification system for displaying temporary, non-blocking toast messages with multiple severity levels.

## 🔍 Overview

The toast notification system consists of several components that work together:

- **`ToastNotification`** — Individual notification control with close animation
- **`ToastNotificationArea`** — Container that positions and manages notifications
- **`ToastNotificationManager`** — Manages notification routing to areas
- **`IToastNotificationService`** — MVVM-friendly service interface for showing notifications
- **`ToastNotificationService`** — Default implementation of the service

The notification pipeline flows: `IToastNotificationService` → `ToastNotificationManager` → `ToastNotificationArea` → `ToastNotification`.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Notifications;
```

## 🚀 Usage

### XAML: Place a Notification Area

```xml
<Window>
    <Grid>
        <!-- Main content -->
        <ContentControl Content="{Binding CurrentView}" />

        <!-- Notification area (typically at root of window) -->
        <notifications:ToastNotificationArea
            x:Name="NotificationArea"
            Position="BottomRight"
            MaxItems="5" />
    </Grid>
</Window>
```

### Service Registration (DI)

```csharp
// In your DI container setup
services.AddSingleton<IToastNotificationService, ToastNotificationService>();
```

### Showing Notifications via Service

```csharp
public class MyViewModel
{
    private readonly IToastNotificationService notificationService;

    public MyViewModel(IToastNotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    public void OnSaveCompleted()
    {
        notificationService.ShowSuccess("Saved", "Document saved successfully.");
    }

    public void OnError(Exception ex)
    {
        notificationService.ShowError("Error", ex.Message);
    }

    public void OnWarning(string message)
    {
        notificationService.ShowWarning("Warning", message, expirationTime: TimeSpan.FromSeconds(10));
    }

    public void OnInfo()
    {
        notificationService.ShowInformation("Info", "New updates available.",
            onClick: () => OpenUpdates(),
            onClose: () => LogDismissed());
    }
}
```

### Desktop Overlay Notifications

```csharp
// Show notification as a desktop overlay (above all windows)
notificationService.ShowInformation("Alert", "Background task completed.", useDesktop: true);
```

## ⚙️ Properties

### ToastNotificationArea Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Position` | `ToastNotificationPosition` | `BottomRight` | Corner where notifications appear |
| `MaxItems` | `int` | `int.MaxValue` | Maximum simultaneous notifications (oldest auto-closed) |

### ToastNotification Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CloseOnClick` | `bool` | `false` | Whether clicking the notification closes it |

## 📊 Enumerations

### ToastNotificationType

| Value | Description |
|-------|-------------|
| `Information` | Informational notification |
| `Success` | Success notification |
| `Warning` | Warning notification |
| `Error` | Error notification |

### ToastNotificationPosition

| Value | Description |
|-------|-------------|
| `TopLeft` | Top-left corner |
| `TopRight` | Top-right corner |
| `BottomLeft` | Bottom-left corner |
| `BottomRight` | Bottom-right corner (default) |

## 🔧 IToastNotificationService Methods

| Method | Description |
|--------|-------------|
| `ShowInformation(title, message, ...)` | Show an informational toast |
| `ShowSuccess(title, message, ...)` | Show a success toast |
| `ShowWarning(title, message, ...)` | Show a warning toast |
| `ShowError(title, message, ...)` | Show an error toast |

All methods accept optional parameters: `areaName`, `expirationTime` (default 5s), `useDesktop`, `onClick`, `onClose`.

## 📝 Notes

- Notifications auto-dismiss after `expirationTime` (default 5 seconds)
- When `MaxItems` is exceeded, the oldest notification is auto-closed
- The `ToastNotificationArea` registers itself with the manager on construction
- All components handle dispatcher marshalling for cross-thread safety
- `ToastNotification.CloseAsync()` plays a closing animation before removal
- Desktop notifications use a `ToastNotificationsOverlayWindow` that floats above all windows

## 🔗 Related Controls

- **InfoDialogBox** - Modal information dialog for important messages
- **BusyOverlay** - Loading overlay for async operations

## 🎮 Sample Application

See the Toast Notification sample in the Atc.Wpf.Sample application under **Wpf.Components > Notifications** for interactive examples.
