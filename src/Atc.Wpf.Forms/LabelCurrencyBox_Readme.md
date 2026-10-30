# 💰 LabelCurrencyBox

A labeled currency form field with up/down buttons, decimal places, prefix/suffix text and a range.

## 🔍 Overview

`LabelCurrencyBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a `CurrencyBox` from `Atc.Wpf.Controls`. The inner box formats the value with the .NET `"C"` (currency) format, so the currency symbol and separators follow its culture. The value is a `decimal` limited by `Minimum` and `Maximum`, shown with `DecimalPlaces` decimals. It implements `ILabelCurrencyBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelCurrencyBox LabelText="Price" Value="{Binding Price}" />
```

### Whole Amounts

```xml
<atc:LabelCurrencyBox
    DecimalPlaces="0"
    InformationText="This is a help text.."
    LabelText="Budget"
    Minimum="0"
    Value="{Binding Budget}" />
```

### Without Up/Down Buttons

```xml
<atc:LabelCurrencyBox
    HideUpDownButtons="True"
    LabelText="Amount"
    Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `decimal` | `0` | Current value. Binds two-way by default and updates the source when focus is lost |
| `Minimum` | `decimal` | `decimal.MinValue` | Lowest value. Binds two-way by default |
| `Maximum` | `decimal` | `decimal.MaxValue` | Highest value. Binds two-way by default |
| `DecimalPlaces` | `int` | `2` | Number of decimals shown. Binds two-way by default |
| `PrefixText` | `string` | `""` | Text shown before the value. Binds two-way by default |
| `SuffixText` | `string` | `""` | Text shown after the value. Binds two-way by default |
| `ButtonsAlignment` | `ButtonsAlignmentType` | `Right` | Where the up/down buttons are placed |
| `HideUpDownButtons` | `bool` | `false` | Hides the up/down buttons |
| `WatermarkText` | `string` | `""` | Hint text shown while the box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueChanged` | `EventHandler<ValueChangedEventArgs<decimal?>>` | Raised when `Value` changes, with the old and new value and the control's `Identifier` |

## 📝 Notes

- The currency symbol and number format come from the inner `CurrencyBox`; `LabelCurrencyBox` does not expose its culture
- `IsValid()` validates now and returns `true` when `ValidationText` is empty. A number always has a value, so `IsMandatory` only shows the asterisk
- There is no `LabelControlFactory` method for this control

## 🔗 Related Controls

- **CurrencyBox** - The input box without the label
- **LabelDecimalBox** - Labeled decimal input without currency formatting
- **LabelIntegerBox** - Labeled whole-number input

## 🎮 Sample Application

See the LabelCurrencyBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelCurrencyBox** for interactive examples.
