# 🌈 HueSlider

A vertical slider for selecting a hue value (0-360) from the full color spectrum.

## 🔍 Overview

`HueSlider` displays the full HSV hue spectrum as a vertical gradient and allows the user to select a hue value by clicking or dragging. It renders a draggable adorner indicator showing the current position.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.ColorEditing;
```

## 🚀 Usage

```xml
<colorEditing:HueSlider Hue="{Binding SelectedHue}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Hue` | `double` | `0.0` | Selected hue value (0-360) |

## 📝 Notes

- Hue range is 0 to 360 degrees on the HSV color wheel
- The adorner position updates automatically when `Hue` changes programmatically

## 🔗 Related Controls

- **SaturationBrightnessPicker** - 2D picker for saturation and brightness
- **TransparencySlider** - Alpha channel slider
- **WellKnownColorPicker** - Named color list picker

## 🎮 Sample Application

See the HueSlider sample in the Atc.Wpf.Sample application under **Wpf.Controls > Colors** for interactive examples.
