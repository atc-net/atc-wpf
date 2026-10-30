# 🔢 LabelIntegerBox

A labeled whole-number form field with up/down buttons, prefix/suffix text and a range.

## 🔍 Overview

`LabelIntegerBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with an `IntegerBox` from `Atc.Wpf.Controls`. The value is an `int` limited by `Minimum` and `Maximum`. Prefix and suffix texts can show a unit, and the up/down buttons can be moved or hidden. It implements `ILabelIntegerBox` and can be created with `LabelControlFactory.CreateLabelIntegerBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelIntegerBox LabelText="Quantity" Value="{Binding Quantity}" />
```

### Range and Unit

```xml
<atc:LabelIntegerBox
    InformationText="This is a help text.."
    LabelText="Length"
    Maximum="1000"
    Minimum="0"
    SuffixText="mm"
    Value="{Binding Length}" />
```

### Without Up/Down Buttons

```xml
<atc:LabelIntegerBox
    HideUpDownButtons="True"
    LabelText="Port"
    Orientation="Vertical" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelIntegerBox(
    groupIdentifier: "Connection",
    labelText: "Port",
    isReadOnly: false,
    isMandatory: true,
    watermarkText: string.Empty,
    inputDataType: typeof(int),
    value: 8080,
    minimum: 1,
    maximum: 65535);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `int` | `0` | Current value. Binds two-way by default and updates the source when focus is lost |
| `Minimum` | `int` | `int.MinValue` | Lowest value. Binds two-way by default |
| `Maximum` | `int` | `int.MaxValue` | Highest value. Binds two-way by default |
| `PrefixText` | `string` | `""` | Text shown before the value. Binds two-way by default |
| `SuffixText` | `string` | `""` | Text shown after the value, for example a unit. Binds two-way by default |
| `ButtonsAlignment` | `ButtonsAlignmentType` | `Right` | Where the up/down buttons are placed |
| `HideUpDownButtons` | `bool` | `false` | Hides the up/down buttons |
| `WatermarkText` | `string` | `""` | Hint text shown while the box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit: when focus leaves the input box and the value differs from when it got focus. Carries the control's `Identifier` and the old and new value |

## 📝 Notes

- `ValueLostFocus` is not raised for a value set from code while the box has no focus
- `IsValid()` returns `true` when `ValidationText` is empty. A number always has a value, so `IsMandatory` only shows the asterisk
- The factory reads `[Range]` for `Minimum` / `Maximum` and `[Required]` for `IsMandatory` when you pass a `PropertyInfo`

## 🔗 Related Controls

- **IntegerBox** - The input box without the label
- **LabelDecimalBox** - Labeled decimal input
- **LabelIntegerXyBox** - Labeled pair of whole numbers (X and Y)
- **LabelSlider** - Labeled integer slider

## 🎮 Sample Application

See the LabelIntegerBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelIntegerBox** for interactive examples.
