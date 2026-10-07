# 🔔 ToastNotificationArea

A container control that positions, stacks and auto-expires toast notifications.

## 🔍 Overview

`ToastNotificationArea` is the visual host for `ToastNotification` items. It registers itself with `ToastNotificationManager` when constructed, so notifications shown through `IToastNotificationService` / `ToastNotificationManager` are routed to it. The area aligns its notifications to one of the four corners, stacks them (newest nearest the corner for bottom positions), limits how many are shown at once, and closes each notification when its expiration time elapses.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Notifications;
```

## 🚀 Usage

### Basic Example

Place the area at the root of a window so it overlays the other content:

```xml
<Window>
    <Grid>
        <!-- Main content -->
        <ContentControl Content="{Binding CurrentView}" />

        <!-- Notification area -->
        <atc:ToastNotificationArea
            x:Name="WindowArea"
            MaxItems="3"
            Position="TopLeft" />
    </Grid>
</Window>
```

### Targeting a Named Area

The `x:Name` of the area is used as the area name when showing notifications:

```csharp
toastNotificationService.ShowInformation(
    "Saved",
    "Your changes have been saved.",
    areaName: "WindowArea");
```

### Showing Content Directly on the Area

```csharp
var content = new ToastNotificationContent(
    ToastNotificationType.Success,
    "Saved",
    "Your changes have been saved.");

WindowArea.Show(
    content,
    TimeSpan.FromSeconds(5),
    onClick: () => { /* clicked */ },
    onClose: () => { /* closed */ });

// Or await until the notification is closed / expired
await WindowArea.ShowAsync(content, TimeSpan.MaxValue, onClick: null, onClose: null);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Position` | `ToastNotificationPosition` | `BottomRight` | Corner where notifications appear (`TopLeft`, `TopRight`, `BottomLeft`, `BottomRight`) |
| `MaxItems` | `int` | `int.MaxValue` | Maximum number of simultaneously visible notifications |

## 🔧 Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Show(ContentControl, TimeSpan, Action?, Action?)` | `void` | Fire-and-forget wrapper for `ShowAsync` |
| `ShowAsync(ContentControl, TimeSpan, Action?, Action?)` | `Task` | Shows the content as a toast; completes when the toast expires or is closed. Use `TimeSpan.MaxValue` to disable auto-close |

## 📝 Notes

- The area registers itself with `ToastNotificationManager` in its constructor; the manager routes a notification to the area(s) whose `Name` matches the requested area name, and falls back to all non-desktop areas when no name matches
- When `useDesktop` is requested, notifications go to a full-screen overlay window that hosts an area named `DesktopArea`
- Clicking a toast invokes `onClick` (when provided) and closes it; `onClose` is invoked when the toast closes
- When more than `MaxItems` notifications are open, a notification beyond the limit is closed automatically
- Nothing is shown until the area is loaded and attached to a window
- The default template uses a `ReversibleStackPanel` named `PART_Items`; bottom positions reverse the stacking order
- The `ToastNotificationManager` default expiration time is 5 seconds

## 🔗 Related Controls

- **ToastNotification** - The individual notification and the overall notification system (see `ToastNotification_Readme.md`)
- **IToastNotificationService / ToastNotificationService** - MVVM-friendly API for showing notifications
- **ToastNotificationManager** - Routes notifications to registered areas

## 🎮 Sample Application

See the ToastNotification samples in the Atc.Wpf.Sample application under **Wpf.Components > Notifications > ToastNotification** and **Wpf.Components > Notifications > ToastNotificationService** for interactive examples.
