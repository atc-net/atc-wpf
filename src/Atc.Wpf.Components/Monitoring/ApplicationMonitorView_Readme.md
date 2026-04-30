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
