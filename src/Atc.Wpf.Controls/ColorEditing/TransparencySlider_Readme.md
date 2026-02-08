# 🔲 TransparencySlider

A vertical slider for selecting an alpha (transparency) value for a given color.

## 🔍 Overview

`TransparencySlider` displays a transparency gradient from fully opaque to fully transparent for the specified color, allowing the user to select an alpha byte value (0-255). A checkerboard pattern shows through the transparent areas.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.ColorEditing;
```

## 🚀 Usage

```xml
<colorEditing:TransparencySlider
    Color="{Binding CurrentColor}"
    Alpha="{Binding SelectedAlpha}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Color` | `Color` | Red | Base color for the transparency gradient |
| `Alpha` | `byte` | `0` | Selected alpha value (0 = transparent, 255 = opaque) |

## 📝 Notes

- The gradient updates when `Color` changes
- Adorner position updates when `Alpha` changes programmatically

## 🔗 Related Controls

- **HueSlider** - Hue selection slider
- **SaturationBrightnessPicker** - 2D saturation/brightness picker
- **WellKnownColorPicker** - Named color list picker

## 🎮 Sample Application

See the TransparencySlider sample in the Atc.Wpf.Sample application under **Wpf.Controls > Colors** for interactive examples.
