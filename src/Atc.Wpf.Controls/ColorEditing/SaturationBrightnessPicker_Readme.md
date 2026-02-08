# 🎨 SaturationBrightnessPicker

A 2D picker for selecting saturation and brightness values at a given hue.

## 🔍 Overview

`SaturationBrightnessPicker` displays a gradient area where the X axis represents saturation and the Y axis represents brightness for the current hue. The user clicks or drags to select a color. A point adorner shows the current selection position.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.ColorEditing;
```

## 🚀 Usage

```xml
<colorEditing:SaturationBrightnessPicker
    Hue="{Binding SelectedHue}"
    Saturation="{Binding SelectedSaturation}"
    Brightness="{Binding SelectedBrightness}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Hue` | `double` | `0` | Input hue value (sets the base color) |
| `Saturation` | `double` | `0` | Selected saturation (0-1) |
| `Brightness` | `double` | `0` | Selected brightness (0-1) |
| `ColorValue` | `Color` | Black | Computed Color from HSB values |
| `BrushValue` | `Brush` | Black | Computed Brush from HSB values |

## 📝 Notes

- Drag or click anywhere in the area to set saturation and brightness
- `ColorValue` and `BrushValue` are computed outputs
- Uses a `PointPickerAdorner` for the selection indicator

## 🔗 Related Controls

- **HueSlider** - Hue selection slider
- **TransparencySlider** - Alpha channel slider
- **WellKnownColorPicker** - Named color list picker

## 🎮 Sample Application

See the SaturationBrightnessPicker sample in the Atc.Wpf.Sample application under **Wpf.Controls > Colors** for interactive examples.
