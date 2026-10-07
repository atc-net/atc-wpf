# 🔢 DecimalBox

A numeric input box for decimal values with a configurable number of decimal places.

## 🔍 Overview

`DecimalBox` is a specialization of `NumericBox` for decimal input. It adds a single property, `DecimalPlaces`, which drives the display format (`StringFormat` is kept in sync as `N{DecimalPlaces}`). Its default style pre-configures the box for decimal input: current UI culture, `NumericInputMode="Decimal"`, decimal-point correction, a `decimal`-based range and a starting value of `0`.

All other behaviour (spin buttons, keyboard / mouse-wheel stepping, prefix / suffix text, routed events) is inherited from `NumericBox` - see `NumericBox_Readme.md`.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Example

```xml
<atc:DecimalBox Value="{Binding Price, Mode=TwoWay}" />
```

### Decimal Places

```xml
<atc:DecimalBox DecimalPlaces="0" />
<atc:DecimalBox DecimalPlaces="2" />
<atc:DecimalBox DecimalPlaces="4" />
```

### Range, Step and Prefix / Suffix

```xml
<atc:DecimalBox
    Value="{Binding Temperature, Mode=TwoWay}"
    DecimalPlaces="1"
    Minimum="-40"
    Maximum="60"
    Interval="0.5"
    SuffixText=" °C" />
```

## ⚙️ Properties

### DecimalBox Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DecimalPlaces` | `int` | `2` | Number of decimal places shown. Coerced into the range `0`-`15`. Setting it updates `StringFormat` to `N{DecimalPlaces}` |

### Values Set by the Default Style

The `AtcApps.Styles.DecimalBox` style sets these inherited `NumericBox` properties:

| Property | Value |
|----------|-------|
| `Culture` | `CultureInfo.CurrentUICulture` |
| `DecimalPointCorrection` | `Number` |
| `DefaultValue` | `0` |
| `Minimum` | `Decimal.MinValue` |
| `Maximum` | `Decimal.MaxValue` |
| `NumericInputMode` | `Decimal` |
| `StringFormat` | `N2` |
| `Value` | `0` |

For the full list of inherited properties (`Value`, `Minimum`, `Maximum`, `Interval`, `HideUpDownButtons`, `PrefixText`, `SuffixText`, ...) and routed events (`ValueChanged`, `ValueIncremented`, `ValueDecremented`, `MaximumReached`, `MinimumReached`, `DelayChanged`), see **NumericBox**.

## 📝 Notes

- `Value` is a `double?` (inherited from `NumericBox`); bind it to a `double` or `double?` property, or use a converter for `decimal`
- When the control is loaded, `StringFormat` is re-synchronised to `N{DecimalPlaces}`, so use `DecimalPlaces` rather than `StringFormat` to control precision
- Values of `DecimalPlaces` below `0` are coerced to `0`, values above `15` to `15`

## 🔗 Related Controls

- **NumericBox** - The base numeric input control
- **IntegerBox** - Integer-only specialization
- **CurrencyBox** - Currency-formatted specialization
- **DecimalXyBox** - Pair of `DecimalBox` controls for X / Y values
- **LabelDecimalBox** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the DecimalBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > DecimalBox** for interactive examples.
