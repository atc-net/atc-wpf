# 🎨 LabelColorPicker

A labeled color picker form field with mandatory indicator, information text and validation display.

## 🔍 Overview

`LabelColorPicker` puts a `ColorPicker` (from `Atc.Wpf.Forms.BaseControls`) in the standard `LabelContent` layout used by the other `Label*` controls. The selected color is exposed both as a `Color` (`ColorValue`) and as a `SolidColorBrush` (`BrushValue`). The two properties are kept in sync: setting one updates the other. It implements `ILabelColorPicker`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelColorPicker LabelText="Color" ColorValue="Green" />
```

### Brush Value and Circle Indicator

```xml
<atc:LabelColorPicker
    BrushValue="DarkOrange"
    LabelText="Highlight"
    Orientation="Vertical"
    RenderColorIndicatorType="Circle" />
```

### Mandatory with Help Text

```xml
<atc:LabelColorPicker
    InformationText="Pick the background color."
    IsMandatory="True"
    LabelText="Background"
    BrushValue="Blue" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ColorValue` | `Color?` | `Black` | The selected color. Setting it also updates `BrushValue` |
| `BrushValue` | `SolidColorBrush?` | `Black` | The selected color as a brush. Setting it also updates `ColorValue` |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator, forwarded to the inner `ColorPicker` |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ColorChanged` | `EventHandler<ValueChangedEventArgs<Color>>` | Raised when a color is picked in the inner color picker, with the old and new color and the control's `Identifier` |

## 📝 Notes

- `ColorValue` and `BrushValue` do not bind two-way by default; use `Mode=TwoWay` to write the picked color back to the source
- `ColorChanged` is raised by picking in the inner `ColorPicker`, not when `ColorValue` or `BrushValue` is set in code
- `IsValid()` is not overridden and always returns `true`; `IsMandatory` only shows the asterisk
- The picker itself (popup, editing modes) is described in `ColorPicker_Readme.md`

## 🔗 Related Controls

- **ColorPicker** - The color picker without the label
- **LabelWellKnownColorSelector** - Labeled drop-down of named colors
- **LabelAccentColorSelector** - Labeled accent color selector for the theme
- **LabelFontPicker** - Labeled font picker with foreground and background colors

## 🎮 Sample Application

See the LabelColorPicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelColorPicker** for interactive examples.
