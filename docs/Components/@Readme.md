# 🧩 Composite Components

The `Atc.Wpf.Components` library provides **higher-level composite components** built on top of the primitives in `Atc.Wpf.Controls` and the labelled fields in `Atc.Wpf.Forms`. These are feature-complete units you drop into an application — dialogs, viewers, monitoring panels, notifications, settings, and the zoom browser.

## 📍 Namespace

```csharp
using Atc.Wpf.Components;
```

## 🔍 What's in the box

| Category | Highlights | Path |
|---|---|---|
| **Dialogs** | `InfoDialogBox`, `QuestionDialogBox`, `InputDialogBox`, `InputFormDialogBox`, `BasicApplicationSettingsDialogBox` | [`Dialogs/`](../../src/Atc.Wpf.Components/Dialogs) |
| **Viewers** | `JsonViewer`, `TerminalViewer`, `MarkdownViewer` | [`Viewers/`](../../src/Atc.Wpf.Components/Viewers) |
| **Monitoring** | `ApplicationMonitorView` (memory, CPU, GC counters with copy-to-clipboard context menu) | [`Monitoring/`](../../src/Atc.Wpf.Components/Monitoring) |
| **Notifications** | `ToastNotification` system + manager | [`Notifications/`](../../src/Atc.Wpf.Components/Notifications) |
| **Progressing** | `BusyIndicatorService`, `BusyIndicatorManager` | [`Progressing/`](../../src/Atc.Wpf.Components/Progressing) |
| **Printing** | `VisualPaginator` for paginated print output | [`Printing/`](../../src/Atc.Wpf.Components/Printing) |
| **Settings** | Reusable settings panels | [`Settings/`](../../src/Atc.Wpf.Components/Settings) |
| **Zoom** | `ZoomBrowser` (multi-zoom orchestration around `ZoomBox`) | [`Zoom/`](../../src/Atc.Wpf.Components/Zoom) |
| **Flyouts** | `Flyout` controls | [`Flyouts/`](../../src/Atc.Wpf.Components/Flyouts) |

## 🚀 Common patterns

### Dialog box

```csharp
var dialog = new InfoDialogBox(
    Application.Current.MainWindow!,
    new DialogBoxSettings(DialogBoxType.Ok),
    "Hello to InfoDialogBox method");

dialog.Show();
```

### Toast notification

```csharp
ToastNotificationManager.Show(
    title: "Saved",
    message: "Your settings were saved.",
    type: ToastNotificationType.Success);
```

### Application monitor

```xml
<components:ApplicationMonitorView />
```

The view ships with a context menu (right-click) that copies a single counter or the full snapshot to the clipboard.

## 🧹 Lifetime & cleanup

Several components subscribe to `Atc.XamlToolkit.Messaging.Messenger` and/or hold timers:

- `TerminalViewer` is `IDisposable` — call `Dispose()` from your view's `Unloaded` handler so the background pump can complete cleanly.
- `ApplicationMonitorView` and `ZoomBox` register on `Loaded` and unregister on `Unloaded` automatically; you don't need to do anything.
- `Messenger` holds recipients via `WeakReference`, so a missed unregister is not a classic GC leak — but the recipient may keep receiving messages until the next `Messenger.Cleanup()` cycle.

## 📚 See also

- Per-component readme files live next to the source: e.g. [`InputDialogBox_Readme.md`](../../src/Atc.Wpf.Components/Dialogs/InputDialogBox_Readme.md), [`ApplicationMonitorView_Readme.md`](../../src/Atc.Wpf.Components/Monitoring/ApplicationMonitorView_Readme.md), [`ToastNotification_Readme.md`](../../src/Atc.Wpf.Components/Notifications/ToastNotification_Readme.md).
- Underlying form fields: [`docs/Forms/@Readme.md`](../Forms/@Readme.md)
- Underlying primitives: [`src/Atc.Wpf.Controls/`](../../src/Atc.Wpf.Controls)
