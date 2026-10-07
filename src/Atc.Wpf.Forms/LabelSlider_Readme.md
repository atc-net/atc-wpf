# 🎚️ LabelSlider

A labeled integer slider form field with tick marks, value tooltip and mandatory indicator.

## 🔍 Overview

`LabelSlider` combines a label area (label text, mandatory asterisk, information popup and validation message) with a WPF `Slider` that works on whole numbers. The slider uses a small change of `1` and a large change of `10`, and the value can also be changed with the mouse wheel while hovering. The current value is shown in an automatic tooltip and as the slider's tooltip.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelSlider LabelText="Volume" Value="{Binding Volume}" />
```

### Custom Range and Ticks

```xml
<atc:LabelSlider
    LabelText="Percentage"
    Minimum="0"
    Maximum="200"
    TickFrequency="20"
    TickPlacement="Both"
    Value="{Binding Percentage}" />
```

### Vertical Layout with Help Text

```xml
<atc:LabelSlider
    InformationText="This is a help text.."
    IsMandatory="True"
    LabelText="MyLabel"
    Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `int` | `0` | Current value. Binds two-way by default and updates the source on every change |
| `Minimum` | `int` | `0` | Lowest value. Binds two-way by default |
| `Maximum` | `int` | `100` | Highest value. Binds two-way by default |
| `TickFrequency` | `int` | `5` | Distance between tick marks |
| `TickPlacement` | `TickPlacement` | `BottomRight` | Where tick marks are drawn. Binds two-way by default |
| `AutoToolTipPlacement` | `AutoToolTipPlacement` | `TopLeft` | Where the value tooltip is shown while dragging. Binds two-way by default |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueChanged` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised when `Value` changes, with the old and new value and the control's `Identifier` |

## 📝 Notes

- The slider's `SmallChange` is `1` and `LargeChange` is `10`; the mouse wheel changes the value by `LargeChange` while hovering
- `Minimum` and `Maximum` are integers and are converted to `double` for the inner `Slider`

## 🔗 Related Controls

- **RangeSlider** - Slider with a lower and upper value
- **LabelIntegerBox** - Labeled integer input with up/down buttons
- **LabelDecimalBox** - Labeled decimal input

## 🎮 Sample Application

See the LabelSlider sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelSlider** for interactive examples.
