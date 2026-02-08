# 📐 GridLines

A content control that renders a visual grid overlay for alignment and layout debugging.

## 🔍 Overview

`GridLines` wraps content and draws a configurable grid of horizontal and vertical lines on top. Useful for design-time alignment, layout debugging, or as a visual background pattern.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

```xml
<layouts:GridLines HorizontalStep="20" VerticalStep="20" LineBrush="LightGray">
    <Canvas>
        <!-- Your content here -->
    </Canvas>
</layouts:GridLines>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `HorizontalStep` | `double` | `20.0` | Spacing between horizontal lines |
| `VerticalStep` | `double` | `20.0` | Spacing between vertical lines |
| `LineBrush` | `Brush` | `DeepPink` | Color of grid lines |

## 📝 Notes

- Grid lines are redrawn on size change and property change
- Lines are rendered as child elements of the content control

## 🔗 Related Controls

- **AutoGrid** - Grid with automatic child positioning
- **GridEx** - Grid with string-based definitions

## 🎮 Sample Application

See the GridLines sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > GridLines** for interactive examples.
