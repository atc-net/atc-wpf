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
    ShowSearchInToolbar="True" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowToolbar` | `bool` | `true` | Show the toolbar |
| `ShowClearInToolbar` | `bool` | `true` | Show clear button |
| `ShowAutoScrollInToolbar` | `bool` | `true` | Show auto-scroll toggle |
| `ShowSearchInToolbar` | `bool` | `true` | Show search box |

## 📝 Notes

- Uses `Messenger.Default` to receive `ApplicationMonitorScrollEvent` messages
- Auto-scroll keeps the latest entries visible

## 🔗 Related Controls

- **TerminalViewer** - Terminal-style output with color coding

## 🎮 Sample Application

See the ApplicationMonitorView sample in the Atc.Wpf.Sample application under **Wpf.Components > Monitoring** for interactive examples.
