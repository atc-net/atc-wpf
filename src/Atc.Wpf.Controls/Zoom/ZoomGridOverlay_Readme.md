# 🔲 ZoomGridOverlay

A zoom-adaptive grid adorner that renders over a `ZoomBox` with automatic density adjustment.

## 🔍 Overview

`ZoomGridOverlay` draws a grid that adapts to the current zoom level — coarse grid lines at low zoom, fine grid lines at high zoom. Every 5th line is drawn as a major line (thicker, darker). Attach to any `ZoomBox` via the static `Attach` method.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Zoom;
```

## 🚀 Usage

```csharp
// Attach a grid overlay
var grid = ZoomGridOverlay.Attach(myZoomBox);

// Customize appearance
grid.BaseGridSpacing = 100;
grid.MinorLineBrush = Brushes.LightGray;
grid.MajorLineBrush = Brushes.DarkGray;

// Remove when no longer needed
grid.Detach();
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BaseGridSpacing` | `double` | `50.0` | Base spacing in content coordinates |
| `MinorLineBrush` | `Brush` | Semi-transparent gray | Brush for minor grid lines |
| `MajorLineBrush` | `Brush` | Semi-transparent gray (darker) | Brush for major grid lines (every 5th) |
| `MinorLineThickness` | `double` | `0.5` | Thickness of minor lines |
| `MajorLineThickness` | `double` | `1.0` | Thickness of major lines |

## 📝 Notes

- The grid automatically adapts spacing: multiplies/divides by 5 to keep screen-space spacing between 20–200 pixels
- Grid lines below 4 pixels screen spacing are hidden for performance
- The overlay is non-hit-testable — mouse events pass through to the ZoomBox
- Use `Attach`/`Detach` for programmatic control, or manage the `AdornerLayer` directly

## 🔗 Related Controls

- **ZoomBox** — The zoom/pan control this grid overlays
- **ZoomRuler** — Position ruler synchronized with ZoomBox