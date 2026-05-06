# 📊 ApplicationMonitorView

A monitoring panel that displays application log entries with toolbar controls for search, auto-scroll, and clear.

## 🔍 Overview

`ApplicationMonitorView` provides a log viewer with a configurable toolbar showing clear, auto-scroll, and search functionality. It receives log entries via the MVVM Messenger pattern and displays them in a scrollable list.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Monitoring;
```

## 🚀 Usage

```xml
<monitoring:ApplicationMonitorView
    ShowToolbar="True"
    ShowClearInToolbar="True"
    ShowAutoScrollInToolbar="True"
    ShowSearchInToolbar="True"
    AreaColumnWidth="200"
    MessageColumnWidth="800" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowToolbar` | `bool` | `true` | Show the toolbar |
| `ShowClearInToolbar` | `bool` | `true` | Show clear button |
| `ShowAutoScrollInToolbar` | `bool` | `true` | Show auto-scroll toggle |
| `AutoScroll` | `bool` | `true` | Whether new entries scroll into view automatically. Two-way bindable; bridges to the underlying `ApplicationMonitorViewModel.AutoScroll`, so the toolbar toggle and this DP stay in sync. |
| `MaxEntries` | `int` | `10000` | Ring-buffer cap on the in-memory entry buffer. When new entries push the count past this value, the oldest entries (by insertion order) are dropped. Set to `0` for no cap (only safe for short-running apps). Two-way bindable and bridged to the VM. |
| `IsPaused` | `bool` | `false` | When `true`, suspends dispatch of incoming entries to the visible buffer while the ingest channel keeps capturing. The toolbar Pause button shows a badge with the buffered count. Resuming flushes the held entries. Two-way bindable and bridged to the VM. |
| `ShowPauseInToolbar` | `bool` | `true` | Show the Pause/Resume toggle in the toolbar. |
| `ShowExportInToolbar` | `bool` | `false` | Show the Export button (CSV / JSON / TXT) in the toolbar. Off by default — opt in with `ShowExportInToolbar="True"`. |
| `IsDetachedFromTail` | `bool` | `false` | (Read-only intent.) `true` while the user has manually scrolled away from the tail of the list. Auto-scroll is suppressed while detached; the `Jump to live` overlay is bound to this. |
| `NewSinceDetached` | `int` | `0` | (Read-only intent.) Count of entries received since detachment. Reset to `0` on tail re-attach or when `JumpToLive()` is called. |
| `ShowSearchInToolbar` | `bool` | `true` | Show search box |
| `AreaColumnWidth` | `double` | `150` | Width of the **Area** column (in DIPs). Combined at runtime with the VM's `ShowColumnArea` flag — when that flag is `false` the column collapses to `0` regardless of this value. Set to `double.NaN` to auto-size to content. |
| `MessageColumnWidth` | `double` | `400` | Width of the **Message** column (in DIPs). Increase for wider log lines. Set to `double.NaN` to auto-size to content. |

## 📝 Notes

- Uses `Messenger.Default` to receive `ApplicationMonitorScrollEvent` messages
- Auto-scroll keeps the latest entries visible
- Each `ApplicationEventEntry` displays a distinct icon per `LogCategoryType` via `LogCategoryTypeToResourceImageValueConverter`
- Supported log categories with unique icons: Critical, Error, Warning, Security, Audit, Service, UI, Information, Debug, Trace

## 🔗 Related Controls

- **TerminalViewer** - Terminal-style output with color coding

## 🎮 Sample Application

See the ApplicationMonitorView sample in the Atc.Wpf.Sample application under **Wpf.Components > Monitoring** for interactive examples.
