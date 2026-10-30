# 🔢 LabelDecimalBox

A labeled decimal form field with up/down buttons, decimal places, prefix/suffix text and a range.

## 🔍 Overview

`LabelDecimalBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a `DecimalBox` from `Atc.Wpf.Controls`. The value is a `decimal` limited by `Minimum` and `Maximum`, shown with `DecimalPlaces` decimals. Prefix and suffix texts can show a unit, and the up/down buttons can be moved or hidden. It implements `ILabelDecimalBox` and can be created with `LabelControlFactory.CreateLabelDecimalBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelDecimalBox LabelText="Weight" Value="{Binding Weight}" />
```

### Decimal Places and Unit

```xml
<atc:LabelDecimalBox
    DecimalPlaces="3"
    InformationText="This is a help text.."
    LabelText="Thickness"
    SuffixText="mm"
    Value="{Binding Thickness}" />
```

### Without Up/Down Buttons

```xml
<atc:LabelDecimalBox
    HideUpDownButtons="True"
    LabelText="Factor"
    Orientation="Vertical" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelDecimalBox(
    groupIdentifier: "Measurement",
    labelText: "Temperature",
    isReadOnly: false,
    isMandatory: false,
    watermarkText: string.Empty,
    inputDataType: typeof(decimal),
    value: 21.5m,
    minimum: -40m,
    maximum: 60m);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `decimal` | `0` | Current value. Binds two-way by default and updates the source when focus is lost |
| `Minimum` | `decimal` | `decimal.MinValue` | Lowest value. Binds two-way by default |
| `Maximum` | `decimal` | `decimal.MaxValue` | Highest value. Binds two-way by default |
| `DecimalPlaces` | `int` | `2` | Number of decimals shown. Binds two-way by default |
| `PrefixText` | `string` | `""` | Text shown before the value. Binds two-way by default |
| `SuffixText` | `string` | `""` | Text shown after the value, for example a unit. Binds two-way by default |
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

- `IsValid()` validates now and returns `true` when `ValidationText` is empty. A number always has a value, so `IsMandatory` only shows the asterisk
- The factory reads `[Range]` for `Minimum` / `Maximum` and `[Required]` for `IsMandatory` when you pass a `PropertyInfo`

## 🔗 Related Controls

- **DecimalBox** - The input box without the label
- **LabelCurrencyBox** - Labeled currency input
- **LabelIntegerBox** - Labeled whole-number input
- **LabelDecimalXyBox** - Labeled pair of decimals (X and Y)

## 🎮 Sample Application

See the LabelDecimalBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelDecimalBox** for interactive examples.
