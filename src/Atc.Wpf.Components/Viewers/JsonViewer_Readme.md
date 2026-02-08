# 📋 JsonViewer

A tree-view control for displaying and navigating JSON data with syntax highlighting and theme support.

## 🔍 Overview

`JsonViewer` parses a JSON string and displays it as an expandable/collapsible tree view. It supports light and dark themes, an action bar with copy/expand/collapse buttons, and theme-aware syntax coloring. The viewer automatically responds to application theme changes.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Viewers;
```

## 🚀 Usage

### Basic Usage

```xml
<viewers:JsonViewer Data="{Binding JsonString}" />
```

### Start Collapsed

```xml
<viewers:JsonViewer Data="{Binding JsonData}" StartExpanded="False" />
```

### Without Action Bar

```xml
<viewers:JsonViewer
    Data="{Binding ResponseJson}"
    ShowActionAndInformationBar="False" />
```

### Suppress Errors

```xml
<!-- Don't show error messages for invalid JSON -->
<viewers:JsonViewer
    Data="{Binding RawData}"
    SuppressErrorMessages="True" />
```

### Programmatic Loading

```csharp
jsonViewer.Load(jsonString);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Data` | `string` | `null` | JSON string to display |
| `ShowActionAndInformationBar` | `bool` | `true` | Show the toolbar with copy/expand/collapse buttons |
| `SuppressErrorMessages` | `bool` | `false` | Suppress error display for invalid JSON |
| `StartExpanded` | `bool` | `true` | Whether the tree starts fully expanded |
| `ThemeMode` | `ThemeMode` | `Light` | Color theme (two-way bindable) |

## 🔧 Action Bar

When `ShowActionAndInformationBar` is true, the toolbar provides:

| Button | Action |
|--------|--------|
| Copy | Copy JSON to clipboard |
| Expand All | Expand all tree nodes |
| Collapse All | Collapse all tree nodes |

## 📝 Notes

- The viewer automatically responds to `ThemeManager.Current.ThemeChanged` events
- Changing `Data` or `ThemeMode` triggers a full re-render
- Invalid JSON displays an error message unless `SuppressErrorMessages` is true
- `ThemeMode` supports two-way binding and journaling

## 🔗 Related Controls

- **TerminalViewer** - Terminal-style output display with colored text

## 🎮 Sample Application

See the JsonViewer sample in the Atc.Wpf.Sample application under **Wpf.Components > Viewers > JsonViewer** for interactive examples.
