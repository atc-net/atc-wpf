# 🏷️ LabelThemeAndAccentColorSelectors

A composite form control showing a `LabelThemeSelector` and a `LabelAccentColorSelector` together.

## 🔍 Overview

`LabelThemeAndAccentColorSelectors` derives from `LabelControlBase` and places a `LabelThemeSelector` and a `LabelAccentColorSelector` in a `UniformSpacingPanel` (spacing `20`). The outer `Orientation` controls how the two selectors are stacked, while `LabelControlOrientation` controls the label/content layout inside each selector. Shared label settings (`HideAreas`, `LabelPosition`, `LabelWidthNumber`, `LabelWidthSizeDefinition`) and `RenderColorIndicatorType` are forwarded to both children.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelThemeAndAccentColorSelectors />
```

### Layout Combinations

```xml
<!-- Selectors side by side, labels to the left -->
<atc:LabelThemeAndAccentColorSelectors LabelControlOrientation="Horizontal" Orientation="Horizontal" />

<!-- Selectors side by side, labels above -->
<atc:LabelThemeAndAccentColorSelectors LabelControlOrientation="Vertical" Orientation="Horizontal" />

<!-- Selectors stacked, labels to the left -->
<atc:LabelThemeAndAccentColorSelectors LabelControlOrientation="Horizontal" Orientation="Vertical" />

<!-- Selectors stacked, labels above -->
<atc:LabelThemeAndAccentColorSelectors LabelControlOrientation="Vertical" Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LabelControlOrientation` | `Orientation` | `Horizontal` | Label/content orientation of each inner selector |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape forwarded to both selectors (`None`, `Circle`, `Square`) |

### Inherited from LabelControlBase (forwarded to both selectors)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Horizontal` | How the two selectors are arranged (outer panel) |
| `LabelPosition` | `LabelPosition` | `Left` | Position of the labels |
| `LabelWidthNumber` | `int` | `120` | Label width |
| `LabelWidthSizeDefinition` | `SizeDefinitionType` | `Pixel` | Unit for `LabelWidthNumber` |
| `HideAreas` | `LabelControlHideAreasType` | `None` | Areas of the label layout to hide |

## 📝 Notes

- `Orientation` is used by the outer panel, not by the individual selectors; use `LabelControlOrientation` for those
- `LabelText` and the information properties are not forwarded; each inner selector uses its own localized default label ("Theme" / "Accent")
- Both selectors apply their changes application-wide

## 🔗 Related Controls

- **LabelThemeSelector** - Labeled base theme selector
- **LabelAccentColorSelector** - Labeled accent color selector
- **ThemeSelector** / **AccentColorSelector** - The bare selectors

## 🎮 Sample Application

See the LabelThemeAndAccentColorSelectors sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelThemeAndAccentColorSelectors** for interactive examples.
