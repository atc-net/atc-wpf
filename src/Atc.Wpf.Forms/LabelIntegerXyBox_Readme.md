# 📐 LabelIntegerXyBox

A labeled form field with two whole-number boxes side by side, one for X and one for Y.

## 🔍 Overview

`LabelIntegerXyBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with an `IntegerXyBox` from `Atc.Wpf.Controls`. The two values are exposed as `ValueX` and `ValueY`. Both boxes share `Minimum`, `Maximum`, `HideUpDownButtons` and `SuffixText`, while each axis has its own prefix text. Use it for positions, offsets and grid coordinates. It implements `ILabelIntegerXyBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelIntegerXyBox
    LabelText="Position"
    ValueX="{Binding PositionX}"
    ValueY="{Binding PositionY}" />
```

### Axis Prefixes and Unit

```xml
<atc:LabelIntegerXyBox
    InformationText="This is a help text.."
    LabelText="Offset"
    PrefixTextX="x:"
    PrefixTextY="y:"
    SuffixText="mm" />
```

### Range without Up/Down Buttons

```xml
<atc:LabelIntegerXyBox
    HideUpDownButtons="True"
    LabelText="Pixel"
    Maximum="3840"
    Minimum="0"
    Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `int` | `0` | X value. Binds two-way by default and updates the source when focus is lost |
| `ValueY` | `int` | `0` | Y value. Binds two-way by default and updates the source when focus is lost |
| `Minimum` | `int` | `int.MinValue` | Lowest value for both boxes. Binds two-way by default |
| `Maximum` | `int` | `int.MaxValue` | Highest value for both boxes. Binds two-way by default |
| `PrefixTextX` | `string` | `""` | Text shown before the X value. Binds two-way by default |
| `PrefixTextY` | `string` | `""` | Text shown before the Y value. Binds two-way by default |
| `SuffixText` | `string` | `""` | Text shown after both values, for example a unit. Binds two-way by default |
| `HideUpDownButtons` | `bool` | `false` | Hides the up/down buttons in both boxes |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueXChanged` | `RoutedPropertyChangedEventHandler<int>` | Bubbling routed event, raised when the value in the inner X box changes |
| `ValueYChanged` | `RoutedPropertyChangedEventHandler<int>` | Bubbling routed event, raised when the value in the inner Y box changes |
| `ValueXLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit of `ValueX`: when focus leaves the box and the value differs from when it got focus |
| `ValueYLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit of `ValueY`: when focus leaves the box and the value differs from when it got focus |

## 📝 Notes

- Moving focus from the X box to the Y box ends the edit of X, so `ValueXLostFocus` is raised then
- The `*LostFocus` events are not raised for values set from code while no box has focus
- `ButtonsAlignment` is inherited but not passed on to the inner boxes
- `IsValid()` returns `true` when `ValidationText` is empty. Numbers always have a value, so `IsMandatory` only shows the asterisk
- There is no `LabelControlFactory` method for this control

## 🔗 Related Controls

- **IntegerXyBox** - The X/Y boxes without the label
- **LabelDecimalXyBox** - Labeled pair of decimals (X and Y)
- **LabelPixelSizeBox** - Labeled width and height in pixels
- **LabelIntegerBox** - Labeled single whole-number input

## 🎮 Sample Application

See the LabelIntegerXyBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelIntegerXyBox** for interactive examples.
