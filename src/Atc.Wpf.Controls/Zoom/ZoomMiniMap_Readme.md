# 🗺️ ZoomMiniMap

A minimap control that shows a scaled overview of `ZoomBox` content with a draggable viewport indicator.

## 🔍 Overview

`ZoomMiniMap` renders a live thumbnail of the content hosted by a `ZoomBox` (or `ZoomScrollViewer`) using a `VisualBrush`, and overlays a rectangle that represents the currently visible viewport. Dragging the rectangle pans the zoomed view, Shift+drag draws a new region to zoom to, and double-clicking snaps the view to the clicked point. It is the overview companion to `ZoomBox` and is used by `ZoomBrowserView`.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Zoom;
```

## 🚀 Usage

### Basic Example

The `DataContext` must be the `ZoomScrollViewer` (or `ZoomBox`) the minimap controls:

```xml
<atc:ZoomScrollViewer x:Name="MyZoom" ZoomInitialPosition="FitScreen">
    <Canvas Width="800" Height="600">
        <!-- Your content here -->
    </Canvas>
</atc:ZoomScrollViewer>

<atc:ZoomMiniMap
    Height="200"
    BorderBrush="{DynamicResource AtcApps.Brushes.Gray6}"
    BorderThickness="1"
    DataContext="{Binding ElementName=MyZoom}" />
```

### Explicit Visual Element

By default the minimap paints the `Content` of the `ContentControl` set as `DataContext`. Use `VisualElement` to point it at a specific element:

```xml
<atc:ZoomMiniMap
    DataContext="{Binding ElementName=MyZoom}"
    VisualElement="{Binding ElementName=MyZoom, Path=Content}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `VisualElement` | `FrameworkElement?` | `null` | Element rendered as the minimap background. When `null`, the `Content` of the `DataContext` (`ContentControl`) is used |
| `ViewportBorderBrush` | `Brush?` | `null` | Brush for the viewport indicator border (`null` = theme default) |
| `ViewportBorderThickness` | `double?` | `null` | Thickness of the viewport indicator border (`null` = default thickness) |

## 🖱️ Mouse Interaction

| Gesture | Action |
|---------|--------|
| Drag | Pans the viewport indicator (and the zoomed view) |
| Shift + Drag | Draws a sizing rectangle and animates the zoom to that region on release |
| Double-click | Snaps (animated) the view to the clicked point |

## 📝 Notes

- `DataContext` must be a `ZoomBox` or a `ZoomScrollViewer`; mouse interaction throws `InvalidOperationException` otherwise
- The zoom state is saved (`SaveZoom`) before each pan or drag-zoom, so minimap navigation participates in the zoom undo history
- The minimap background is a `VisualBrush` of the content and follows its size changes
- The default template is a `Viewbox` hosting a `Canvas` (`PART_Content`) with a dragging border (`PART_DraggingBorder`) and a hidden sizing border (`PART_SizingBorder`)
- The default template draws the viewport indicator with the `AtcApps.Brushes.Accent` brush; `ViewportBorderBrush` and `ViewportBorderThickness` are not referenced by the default template
- The control's `BorderThickness` is scaled to the content size and applied to the viewport indicator borders

## 🔗 Related Controls

- **ZoomBox** - The zoom and pan control the minimap navigates
- **ZoomScrollViewer** - ScrollViewer wrapper around `ZoomBox`, typically used as the minimap `DataContext`
- **ZoomRuler** - Rulers that follow the zoom/pan state
- **ZoomBrowserView** - Composite zoom browser that includes a minimap

## 🎮 Sample Application

See the ZoomBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Zoom > ZoomBox** (MiniMap panel) for interactive examples.
