# ⚙️ BasicApplicationSettingsView

An embeddable settings panel for common application preferences (theme, language, accent color).

## 🔍 Overview

`BasicApplicationSettingsView` is a `UserControl` that provides a pre-built settings form for selecting application theme, accent color, and language. It can be embedded directly in your application or used inside `BasicApplicationSettingsDialogBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Settings;
```

## 🚀 Usage

```xml
<settings:BasicApplicationSettingsView />
```

## 📝 Notes

- Uses label selector controls from `Atc.Wpf.Forms` for theme and accent color
- Bind a ViewModel for persisting settings

## 🔗 Related Controls

- **BasicApplicationSettingsDialogBox** - Dialog wrapper for this view

## 🎮 Sample Application

See the settings samples in the Atc.Wpf.Sample application under **Wpf.Components > Settings** for interactive examples.
