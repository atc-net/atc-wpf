# Atc.Wpf.Components

Higher-level composite components built on `Atc.Wpf.Controls` and `Atc.Wpf.Forms`. Each component bundles multiple primitive controls into a feature-complete unit (dialogs, viewers, monitoring, notifications, settings panels, the zoom browser, …) so consumer applications can drop them in directly.

## What lives here

| Area | Path | Highlights |
|---|---|---|
| Dialogs | `Dialogs/` | `InfoDialogBox`, `QuestionDialogBox`, `InputDialogBox`, `InputFormDialogBox`, `BasicApplicationSettingsDialogBox` |
| Viewers | `Viewers/` | `JsonViewer`, `TerminalViewer`, `MarkdownViewer` |
| Monitoring | `Monitoring/` | `ApplicationMonitorView` (memory, CPU, GC counters; copy-to-clipboard context menu) |
| Notifications | `Notifications/` | `ToastNotification` system + manager |
| Progressing | `Progressing/` | `BusyIndicatorService`, `BusyIndicatorManager` |
| Printing | `Printing/` | `VisualPaginator` |
| Settings | `Settings/` | reusable settings panels |
| Zoom browser | `Zoom/` | `ZoomBrowser` (multi-zoom orchestration) |
| Flyouts | `Flyouts/` | `Flyout` controls |

## Install

```xml
<PackageReference Include="Atc.Wpf.Components" Version="2.*" />
```

## Notes

- `TerminalViewer` is `IDisposable` — call `Dispose()` when removing it from the tree (typically from your view's `Unloaded` handler) so its background pump can complete.
- Components that subscribe to `Atc.XamlToolkit.Messaging.Messenger` (`TerminalViewer`, `ApplicationMonitorView`, `ZoomBox`, …) register on `Loaded` and unregister on `Unloaded` / `Dispose`. Subscriptions are weak-rooted by `Messenger`, so missed unregister calls do not leak — but they may keep delivering messages until the next `Cleanup()` cycle.

## Dependencies

- [`Atc.Wpf.Forms`](../Atc.Wpf.Forms/README.md) — labelled form fields used inside dialogs
- [`Atc.Wpf.Controls`](../Atc.Wpf.Controls/README.md) — atomic controls
- [`Atc.Wpf.FontIcons`](../Atc.Wpf.FontIcons/README.md) — icons used in toolbars / menu items
