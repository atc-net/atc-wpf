# 🔍 ZoomBox

A control that wraps content and provides zoom and pan functionality with undo/redo support.

## 🔍 Overview

`ZoomBox` is a zoom and pan control for WPF content. It includes `ZoomScrollViewer` for ScrollViewer integration and `ZoomMiniMap` for an overview minimap. Features include animated zooming, keyboard shortcuts, mouse wheel zoom, drag-to-zoom, pinch-to-zoom touch support, and undo/redo zoom history.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Zoom;
```

## 🚀 Usage

### Basic ZoomScrollViewer

```xml
<zoom:ZoomScrollViewer ZoomInitialPosition="FitScreen" UseAnimations="True">
    <Canvas Width="800" Height="600">
        <!-- Your content here -->
    </Canvas>
</zoom:ZoomScrollViewer>
```

### With Toolbar Commands

```xml
<Button Command="{Binding ElementName=MyZoom, Path=FitCommand}" Content="Fit" />
<Button Command="{Binding ElementName=MyZoom, Path=FillCommand}" Content="Fill" />
<Button Command="{Binding ElementName=MyZoom, Path=ZoomPercentCommand}" CommandParameter="100" Content="100%" />
<zoom:ZoomScrollViewer x:Name="MyZoom">
    <!-- Content -->
</zoom:ZoomScrollViewer>
```

### With MiniMap

```xml
<zoom:ZoomMiniMap
    DataContext="{Binding ElementName=MyZoom}"
    VisualElement="{Binding ElementName=MyZoom, Path=Content}" />
```

## ⚙️ ZoomBox Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `UseAnimations` | `bool` | `true` | Enable/disable zoom animations |
| `AnimationDuration` | `double` | `0.4` | Animation duration in seconds |
| `ZoomInitialPosition` | `ZoomInitialPositionType` | `Default` | Initial zoom position on load |
| `ViewportZoom` | `double` | `1.0` | Current zoom level |
| `MinimumZoom` | `double` | `0.1` | Minimum zoom level |
| `MaximumZoom` | `double` | `10.0` | Maximum zoom level |
| `MinimumZoomType` | `ZoomMinimumType` | `MinimumZoom` | How minimum zoom is calculated |
| `IsMouseWheelScrollingEnabled` | `bool` | `false` | Enable mouse wheel scrolling |
| `DragZoomThreshold` | `double` | `10.0` | Pixel threshold before drag-zoom activates |
| `IsTouchEnabled` | `bool` | `true` | Enable touch gestures |
| `ContentOffsetX` | `double` | `0.0` | Horizontal content offset |
| `ContentOffsetY` | `double` | `0.0` | Vertical content offset |
| `MousePosition` | `Point?` | `null` | Current mouse position in content coordinates |

## 🎮 Commands

| Command | Description |
|---------|-------------|
| `ZoomFillCommand` | Zoom to fill the viewport |
| `ZoomFitCommand` | Zoom to fit content in viewport |
| `ZoomPercentCommand` | Zoom to percentage (parameter: double) |
| `ZoomRatioFromMinimumCommand` | Zoom by ratio from minimum |
| `ZoomInCommand` | Zoom in by 10% |
| `ZoomOutCommand` | Zoom out by ~10% |
| `UndoZoomCommand` | Undo last zoom change |
| `RedoZoomCommand` | Redo last undone zoom |

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl` + `+` | Zoom in |
| `Ctrl` + `-` | Zoom out |
| `Ctrl+Alt` + `8` | Zoom to fill |
| `Ctrl+Alt` + `9` | Zoom to fit |
| `Ctrl+Alt` + `0` | Zoom to 100% |

## 🖱️ Mouse Interactions

| Interaction | Action |
|-------------|--------|
| `Ctrl` + Scroll wheel | Zoom in/out at cursor |
| `Ctrl` + Left drag | Pan content |
| `Ctrl+Shift` + Left drag | Drag-zoom rectangle |
| `Ctrl+Shift` + Left click | Zoom in at point |
| `Ctrl+Shift` + Right click | Zoom out at point |
| Double-click | Snap to point |

## 👆 Touch Gestures

| Gesture | Action |
|---------|--------|
| Pinch | Zoom in/out at gesture center |
| Two-finger drag | Pan content |

## 📊 Enumerations

### ZoomInitialPositionType

| Value | Description |
|-------|-------------|
| `Default` | No initial positioning |
| `FitScreen` | Zoom to fit content |
| `FillScreen` | Zoom to fill viewport |
| `OneHundredPercentCentered` | 100% zoom, centered |

### ZoomMinimumType

| Value | Description |
|-------|-------------|
| `FitScreen` | Minimum is fit-to-screen zoom |
| `FillScreen` | Minimum is fill-screen zoom |
| `MinimumZoom` | Use explicit MinimumZoom value |

## 📝 Notes

- `ZoomScrollViewer` wraps `ZoomBox` with a `ScrollViewer` for scrollbar integration
- `ZoomMiniMap` requires its `DataContext` to be set to the `ZoomBox` or `ZoomScrollViewer`
- Use `TryHandleKeyDown()` to forward keyboard events from the parent window
- The control uses MVVM Messenger for cross-component communication via `ZoomCommandMessage`

## 🔗 Related Controls

- **ZoomScrollViewer** - ScrollViewer wrapper with zoom commands
- **ZoomMiniMap** - Minimap overview of zoomed content

## 🎮 Sample Application

See the ZoomBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Zoom > ZoomBox** for interactive examples.
