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
| `IsSpacebarPanEnabled` | `bool` | `true` | Enable Space+drag temporary pan mode |
| `IsShiftScrollHorizontalPanEnabled` | `bool` | `true` | Enable Shift+Scroll for horizontal pan |
| `ZoomPresets` | `IList<double>?` | `null` | Custom preset zoom levels (null uses defaults: 10%–1000%) |
| `ConstraintMode` | `ZoomConstraintMode` | `Inside` | Viewport constraint mode (Free, Inside, Contain) |
| `ZoomKeyBindings` | `IList<ZoomKeyBinding>?` | `null` | Custom keyboard bindings (null uses defaults) |
| `IsEdgeScrollingEnabled` | `bool` | `false` | Auto-pan when mouse near viewport edge during drag |
| `EdgeScrollZoneWidth` | `double` | `40.0` | Edge scroll activation zone width in pixels |
| `EdgeScrollSpeed` | `double` | `5.0` | Edge scroll speed in content units per tick |
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
| `ZoomToSelectionCommand` | Zoom to fit a selection rectangle (parameter: `Rect`) |
| `ZoomToNextPresetCommand` | Zoom to the next higher preset level |
| `ZoomToPreviousPresetCommand` | Zoom to the next lower preset level |
| `SaveViewBookmarkCommand` | Save current viewport as named bookmark (parameter: `string`) |
| `RestoreViewBookmarkCommand` | Restore a saved bookmark (parameter: `ViewBookmark`) |
| `RemoveViewBookmarkCommand` | Remove a bookmark (parameter: `ViewBookmark`) |
| `UndoZoomCommand` | Undo last zoom change |
| `RedoZoomCommand` | Redo last undone zoom |
| `ZoomPreviousCommand` | Navigate to previous viewport (alias for UndoZoomCommand) |
| `ZoomNextCommand` | Navigate to next viewport (alias for RedoZoomCommand) |

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
| `Space` + Left drag | Pan content (temporary hand tool) |
| `Shift` + Scroll wheel | Pan horizontally |
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

### ZoomConstraintMode

| Value | Description |
|-------|-------------|
| `Free` | No constraints — viewport can pan past content edges |
| `Inside` | Viewport clamped to content bounds (default) |
| `Contain` | Content must fill viewport — no empty space visible |

## 📝 Notes

- `CurrentViewportState` returns a `ViewportState` record for serialization (zoom, offsetX, offsetY)
- `RestoreViewportState(state, animate)` restores a previously captured viewport state
- `EnsureVisible(Rect, marginX, marginY)` scrolls the minimum distance to make a rect visible without changing zoom
- `AnimatedEnsureVisible(Rect, marginX, marginY)` is the animated version of `EnsureVisible`
- `ZoomLevelChanged` event provides old/new zoom values and a normalized `LevelOfDetail` (0.0–1.0) for zoom-dependent rendering
- Cursor automatically changes to hand during pan and crosshair during zoom modes
- `ViewBookmarks` collection stores named viewport bookmarks; use `SaveViewBookmark(name)` to save and `RestoreViewBookmarkCommand` to restore
- `ZoomKeyBindings` allows customizing keyboard shortcuts; set to `null` for defaults or provide a custom `IList<ZoomKeyBinding>`
- `ZoomGridOverlay.Attach(zoomBox)` adds a zoom-adaptive grid overlay adorner
- `ZoomViewportLink.Create(primary, secondary, mode)` links two ZoomBox viewports (Mirror or FollowPan)
- `ZoomScrollViewer` wraps `ZoomBox` with a `ScrollViewer` for scrollbar integration
- `ZoomMiniMap` requires its `DataContext` to be set to the `ZoomBox` or `ZoomScrollViewer`
- Use `TryHandleKeyDown()` to forward keyboard events from the parent window — uses `ZoomKeyBindings` when set
- The control uses MVVM Messenger for cross-component communication via `ZoomCommandMessage`

## 🔗 Related Controls

- **ZoomScrollViewer** — ScrollViewer wrapper with zoom commands
- **ZoomMiniMap** — Minimap overview of zoomed content
- **ZoomGridOverlay** — Zoom-adaptive grid adorner
- **ZoomViewportLink** — Links two ZoomBox viewports (Mirror/FollowPan)
- **ZoomBrowserView** — Composite component with toolbar + minimap + status bar (`Atc.Wpf.Components.Zoom`)

## 🎮 Sample Application

See the ZoomBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Zoom > ZoomBox** for interactive examples.
