# 📐 LabelDecimalXyBox

A labeled form field with two decimal boxes side by side, one for X and one for Y.

## 🔍 Overview

`LabelDecimalXyBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a `DecimalXyBox` from `Atc.Wpf.Controls`. The two values are exposed as `ValueX` and `ValueY`. Both boxes share `Minimum`, `Maximum`, `DecimalPlaces`, `HideUpDownButtons` and `SuffixText`, while each axis has its own prefix text. Use it for coordinates, scale factors and other two-component decimal values. It implements `ILabelDecimalXyBox` and can be created with `LabelControlFactory.CreateLabelDecimalXyBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelDecimalXyBox
    LabelText="Scale"
    ValueX="{Binding ScaleX}"
    ValueY="{Binding ScaleY}" />
```

### Axis Prefixes and Unit

```xml
<atc:LabelDecimalXyBox
    InformationText="This is a help text.."
    LabelText="Offset"
    PrefixTextX="x:"
    PrefixTextY="y:"
    SuffixText="mm" />
```

### Decimal Places

```xml
<atc:LabelDecimalXyBox
    DecimalPlaces="4"
    HideUpDownButtons="True"
    LabelText="Coordinate"
    Orientation="Vertical" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelDecimalXyBox(
    groupIdentifier: "Layout",
    labelText: "Offset",
    isReadOnly: false,
    isMandatory: false,
    inputDataType: typeof(decimal),
    valueX: 0m,
    valueY: 0m,
    minimum: -100m,
    maximum: 100m);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `decimal` | `0` | X value. Binds two-way by default and updates the source when focus is lost |
| `ValueY` | `decimal` | `0` | Y value. Binds two-way by default and updates the source when focus is lost |
| `Minimum` | `decimal` | `decimal.MinValue` | Lowest value for both boxes. Binds two-way by default |
| `Maximum` | `decimal` | `decimal.MaxValue` | Highest value for both boxes. Binds two-way by default |
| `DecimalPlaces` | `int` | `2` | Number of decimals shown in both boxes. Binds two-way by default |
| `PrefixTextX` | `string` | `""` | Text shown before the X value. Binds two-way by default |
| `PrefixTextY` | `string` | `""` | Text shown before the Y value. Binds two-way by default |
| `SuffixText` | `string` | `""` | Text shown after both values, for example a unit. Binds two-way by default |
| `HideUpDownButtons` | `bool` | `false` | Hides the up/down buttons in both boxes |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueXChanged` | `RoutedPropertyChangedEventHandler<decimal>` | Bubbling routed event, raised when the value in the inner X box changes |
| `ValueYChanged` | `RoutedPropertyChangedEventHandler<decimal>` | Bubbling routed event, raised when the value in the inner Y box changes |
| `ValueXLostFocus` | `EventHandler<ValueChangedEventArgs<decimal?>>` | Raised once per edit of `ValueX`: when focus leaves the box and the value differs from when it got focus |
| `ValueYLostFocus` | `EventHandler<ValueChangedEventArgs<decimal?>>` | Raised once per edit of `ValueY`: when focus leaves the box and the value differs from when it got focus |

## 📝 Notes

- Moving focus from the X box to the Y box ends the edit of X, so `ValueXLostFocus` is raised then
- The `*LostFocus` events are not raised for values set from code while no box has focus
- `ButtonsAlignment` is inherited but not passed on to the inner boxes
- `IsValid()` returns `true` when `ValidationText` is empty. Numbers always have a value, so `IsMandatory` only shows the asterisk
- The factory reads `[Range]` for `Minimum` / `Maximum` and `[Required]` for `IsMandatory` when you pass a `PropertyInfo`

## 🔗 Related Controls

- **DecimalXyBox** - The X/Y boxes without the label
- **LabelIntegerXyBox** - Labeled pair of whole numbers (X and Y)
- **LabelDecimalBox** - Labeled single decimal input

## 🎮 Sample Application

See the LabelDecimalXyBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelDecimalXyBox** for interactive examples.
