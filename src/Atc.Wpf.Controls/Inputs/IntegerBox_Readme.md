# 🔢 IntegerBox

A numeric input box restricted to whole numbers.

## 🔍 Overview

`IntegerBox` is a specialization of `NumericBox` for integer input. The class itself adds no new members; its default style pre-configures the inherited `NumericBox` properties for whole numbers: current UI culture, `NumericInputMode="Numbers"`, an `Int32`-based range and a starting value of `0`.

All behaviour (spin buttons, keyboard / mouse-wheel stepping, prefix / suffix text, routed events) is inherited from `NumericBox` - see `NumericBox_Readme.md`.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Example

```xml
<atc:IntegerBox Value="{Binding Quantity, Mode=TwoWay}" />
```

### Range and Step

```xml
<atc:IntegerBox
    Value="{Binding Port, Mode=TwoWay}"
    Minimum="1"
    Maximum="65535"
    Interval="1" />
```

### Spin Button Placement

```xml
<atc:IntegerBox ButtonsAlignment="Right" SwitchUpDownButtons="False" />
<atc:IntegerBox ButtonsAlignment="Right" SwitchUpDownButtons="True" />
<atc:IntegerBox ButtonsAlignment="Left" SwitchUpDownButtons="False" />
<atc:IntegerBox ButtonsAlignment="Left" SwitchUpDownButtons="True" />
```

### Without Spin Buttons

```xml
<atc:IntegerBox HideUpDownButtons="True" />
```

## ⚙️ Properties

`IntegerBox` declares no properties of its own.

### Values Set by the Default Style

The `AtcApps.Styles.IntegerBox` style sets these inherited `NumericBox` properties:

| Property | Value |
|----------|-------|
| `Culture` | `CultureInfo.CurrentUICulture` |
| `DefaultValue` | `0` |
| `Minimum` | `Int32.MinValue` |
| `Maximum` | `Int32.MaxValue` |
| `NumericInputMode` | `Numbers` |
| `Value` | `0` |

For the full list of inherited properties (`Value`, `Minimum`, `Maximum`, `Interval`, `ButtonsAlignment`, `HideUpDownButtons`, `PrefixText`, `SuffixText`, ...) and routed events (`ValueChanged`, `ValueIncremented`, `ValueDecremented`, `MaximumReached`, `MinimumReached`, `DelayChanged`), see **NumericBox**.

## 📝 Notes

- `Value` is a `double?` (inherited from `NumericBox`); bind it to a `double` / `double?` property or use a converter for `int`
- `NumericInputMode="Numbers"` restricts typed input to whole numbers
- `IntegerBox` is the building block of `IntegerXyBox`, `PixelSizeBox` and `ThicknessBox`

## 🔗 Related Controls

- **NumericBox** - The base numeric input control
- **DecimalBox** - Decimal specialization with `DecimalPlaces`
- **IntegerXyBox** - Pair of `IntegerBox` controls for X / Y values
- **PixelSizeBox** - Pair of `IntegerBox` controls for width / height
- **LabelIntegerBox** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the IntegerBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > IntegerBox** for interactive examples.
