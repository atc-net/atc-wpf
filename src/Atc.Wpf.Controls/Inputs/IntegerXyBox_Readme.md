# 📐 IntegerXyBox

A compact input for an X / Y pair of integer values.

## 🔍 Overview

`IntegerXyBox` is a `UserControl` that places two `IntegerBox` controls side by side - one for X and one for Y. Both boxes share the same `Minimum`, `Maximum`, `HideUpDownButtons` and `SuffixText`, while each axis has its own prefix text. Use it for pixel positions, grid coordinates and other two-component whole-number values.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Example

```xml
<atc:IntegerXyBox
    ValueX="{Binding PositionX}"
    ValueY="{Binding PositionY}" />
```

### Prefix and Suffix

```xml
<atc:IntegerXyBox
    PrefixTextX="x: "
    PrefixTextY="y: "
    SuffixText=" px"
    ValueX="{Binding PositionX}"
    ValueY="{Binding PositionY}" />
```

### Range and Hidden Spin Buttons

```xml
<atc:IntegerXyBox
    Minimum="0"
    Maximum="3840"
    HideUpDownButtons="True"
    ValueX="{Binding PositionX}"
    ValueY="{Binding PositionY}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `int` | `0` | The X value. Two-way by default, `UpdateSourceTrigger=LostFocus` |
| `ValueY` | `int` | `0` | The Y value. Two-way by default, `UpdateSourceTrigger=LostFocus` |
| `Minimum` | `int` | `int.MinValue` | Lower bound applied to both boxes |
| `Maximum` | `int` | `int.MaxValue` | Upper bound applied to both boxes |
| `HideUpDownButtons` | `bool` | `false` | Hides the spin buttons of both boxes |
| `PrefixTextX` | `string` | `""` | Text shown before the X value |
| `PrefixTextY` | `string` | `""` | Text shown before the Y value |
| `SuffixText` | `string` | `""` | Text shown after both values |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueXChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the X box value changes |
| `ValueYChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the Y box value changes |
| `ValueXLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised when the `ValueX` property changes (committed on lost focus) |
| `ValueYLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised when the `ValueY` property changes (committed on lost focus) |

## 📝 Notes

- The two inner `IntegerBox` controls are laid out in a `GridEx` with `Columns="*,10,*"`
- `ValueXChanged` / `ValueYChanged` are not raised when the inner box value changes from or to `null`
- `ValueXLostFocus` / `ValueYLostFocus` carry the control identifier (via `ControlHelper.GetIdentifier`) plus the old and new value

## 🔗 Related Controls

- **IntegerBox** - The single-value integer input used for each axis
- **DecimalXyBox** - Decimal variant
- **PixelSizeBox** - Width / height variant
- **LabelIntegerXyBox** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the IntegerXyBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > IntegerXyBox** for interactive examples.
