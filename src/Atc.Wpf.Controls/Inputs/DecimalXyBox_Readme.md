# 📐 DecimalXyBox

A compact input for an X / Y pair of decimal values.

## 🔍 Overview

`DecimalXyBox` is a `UserControl` that places two `DecimalBox` controls side by side - one for X and one for Y. Both boxes share the same `Minimum`, `Maximum`, `DecimalPlaces`, `HideUpDownButtons` and `SuffixText`, while each axis has its own prefix text. Use it for coordinates, offsets, scale factors and other two-component decimal values.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Example

```xml
<atc:DecimalXyBox
    ValueX="{Binding OffsetX}"
    ValueY="{Binding OffsetY}" />
```

### Prefix, Suffix and Precision

```xml
<atc:DecimalXyBox
    DecimalPlaces="4"
    PrefixTextX="X: "
    PrefixTextY="Y: "
    SuffixText=" mm"
    ValueX="{Binding PositionX}"
    ValueY="{Binding PositionY}" />
```

### Range and Hidden Spin Buttons

```xml
<atc:DecimalXyBox
    Minimum="-180"
    Maximum="180"
    HideUpDownButtons="True"
    ValueX="{Binding Longitude}"
    ValueY="{Binding Latitude}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `decimal` | `0` | The X value. Two-way by default, `UpdateSourceTrigger=LostFocus` |
| `ValueY` | `decimal` | `0` | The Y value. Two-way by default, `UpdateSourceTrigger=LostFocus` |
| `Minimum` | `decimal` | `decimal.MinValue` | Lower bound applied to both boxes |
| `Maximum` | `decimal` | `decimal.MaxValue` | Upper bound applied to both boxes |
| `DecimalPlaces` | `int` | `2` | Number of decimal places shown in both boxes |
| `HideUpDownButtons` | `bool` | `false` | Hides the spin buttons of both boxes |
| `PrefixTextX` | `string` | `""` | Text shown before the X value |
| `PrefixTextY` | `string` | `""` | Text shown before the Y value |
| `SuffixText` | `string` | `""` | Text shown after both values |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueXChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<decimal>` | Raised when the X box value changes |
| `ValueYChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<decimal>` | Raised when the Y box value changes |
| `ValueXLostFocus` | `EventHandler<ValueChangedEventArgs<decimal?>>` | Raised when the `ValueX` property changes (committed on lost focus) |
| `ValueYLostFocus` | `EventHandler<ValueChangedEventArgs<decimal?>>` | Raised when the `ValueY` property changes (committed on lost focus) |

## 📝 Notes

- The two inner `DecimalBox` controls are laid out in a `GridEx` with `Columns="*,10,*"`
- `ValueXChanged` / `ValueYChanged` are not raised when the inner box value changes from or to `null`
- `ValueXLostFocus` / `ValueYLostFocus` carry the control identifier (via `ControlHelper.GetIdentifier`) plus the old and new value

## 🔗 Related Controls

- **DecimalBox** - The single-value decimal input used for each axis
- **IntegerXyBox** - Integer variant
- **PixelSizeBox** - Width / height variant
- **LabelDecimalXyBox** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the DecimalXyBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > DecimalXyBox** for interactive examples.
