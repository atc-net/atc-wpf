# 🔍 ZoomBrowserView

A composite component that bundles a zoom toolbar, minimap, and status bar around a `ZoomScrollViewer`.

## 🔍 Overview

`ZoomBrowserView` provides a ready-to-use zoom browser for designer-style applications. It wraps a consumer-provided `ZoomScrollViewer` with a toolbar (100%, Fill, Fit, Zoom In, Zoom Out), an overlay minimap, and a status bar showing the current zoom percentage. Each section can be toggled independently via dependency properties.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Zoom;
```

## 🚀 Usage

### Basic Example

```xml
<atc:ZoomBrowserView ShowToolbar="True" ShowMiniMap="True" ShowStatusBar="True">
    <atc:ZoomBrowserView.ZoomContent>
        <zoom:ZoomScrollViewer ZoomInitialPosition="FitScreen">
            <Canvas Width="800" Height="600">
                <!-- Your zoomable content here -->
            </Canvas>
        </zoom:ZoomScrollViewer>
    </atc:ZoomBrowserView.ZoomContent>
</atc:ZoomBrowserView>
```

### Binding to ViewModel Properties

```xml
<atc:ZoomBrowserView
    ShowToolbar="{Binding ShowToolbar}"
    ShowMiniMap="{Binding ShowMiniMap}"
    ShowStatusBar="{Binding ShowStatusBar}">
    <atc:ZoomBrowserView.ZoomContent>
        <zoom:ZoomScrollViewer>
            <!-- Content -->
        </zoom:ZoomScrollViewer>
    </atc:ZoomBrowserView.ZoomContent>
</atc:ZoomBrowserView>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowToolbar` | `bool` | `true` | Controls visibility of the zoom toolbar |
| `ShowMiniMap` | `bool` | `true` | Controls visibility of the minimap overlay |
| `ShowStatusBar` | `bool` | `true` | Controls visibility of the zoom percentage status bar |
| `ZoomContent` | `ZoomScrollViewer?` | `null` | The `ZoomScrollViewer` to control and display |

## 🎮 Toolbar Buttons

| Button | Command | Description |
|--------|---------|-------------|
| 100% | `ZoomPercentCommand(100)` | Zoom to 100% |
| Fill | `FillCommand` | Zoom to fill viewport |
| Fit | `FitCommand` | Zoom to fit content |
| Zoom In | `ZoomInCommand` | Zoom in by 10% |
| Zoom Out | `ZoomOutCommand` | Zoom out by ~10% |

All toolbar buttons have localized tooltips (English, Danish, German).

## 📝 Notes

- The `ZoomContent` property accepts a `ZoomScrollViewer` (not a raw `ZoomBox`) because `ZoomMiniMap` binds to `ZoomScrollViewer` and it exposes all zoom commands as relay properties
- The minimap is overlaid in the bottom-right corner of the content area
- The status bar shows `ViewportZoom` formatted as a percentage
- Toolbar buttons delegate directly to the `ZoomScrollViewer` commands

## 🔗 Related Controls

- **ZoomBox** — Low-level zoom/pan control (`Atc.Wpf.Controls.Zoom`)
- **ZoomScrollViewer** — ScrollViewer wrapper with zoom commands (`Atc.Wpf.Controls.Zoom`)
- **ZoomMiniMap** — Minimap overview control (`Atc.Wpf.Controls.Zoom`)

## 🎮 Sample Application

See the ZoomBrowserView sample in the Atc.Wpf.Sample application under **Wpf.Components > Zoom > ZoomBrowserView** for an interactive demo.
