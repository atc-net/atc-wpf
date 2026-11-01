# 📐 LabelPixelSizeBox

A labeled width × height pixel input form field with up/down buttons and mandatory indicator.

## 🔍 Overview

`LabelPixelSizeBox` puts a `PixelSizeBox` (from `Atc.Wpf.Controls.Inputs`) in the standard `LabelContent` layout used by the other `Label*` controls. It edits two whole numbers, `ValueWidth` and `ValueHeight`, for image sizes, window sizes and similar dimensions. It derives from `LabelIntegerNumberControl`, implements `ILabelPixelSizeBox` and can be created with `LabelControlFactory.CreateLabelPixelSizeBox(...)`. The model-to-form extractor uses it for `Size` properties.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelPixelSizeBox
    LabelText="Image size"
    ValueHeight="{Binding ImageHeight}"
    ValueWidth="{Binding ImageWidth}" />
```

### Maximum and Hidden Spin Buttons

```xml
<atc:LabelPixelSizeBox
    HideUpDownButtons="True"
    InformationText="This is a help text.."
    LabelText="Resolution"
    Maximum="4096"
    Orientation="Vertical" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelPixelSizeBox(
    groupIdentifier: "Export",
    labelText: "ImageSize",
    isReadOnly: false,
    isMandatory: false,
    inputDataType: typeof(Size),
    valueWidth: 1920,
    valueHeight: 1080,
    minimum: null,
    maximum: 4096);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueWidth` | `int` | `0` | Width value. Binds two-way by default and updates the source on lost focus |
| `ValueHeight` | `int` | `0` | Height value. Binds two-way by default and updates the source on lost focus |
| `Maximum` | `int` | from `LabelIntegerNumberControl` | Upper bound for both boxes, forwarded to the inner `PixelSizeBox`. Binds two-way by default |
| `HideUpDownButtons` | `bool` | `false` | Hide the up/down buttons of both boxes |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueWidthChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the width box value changes |
| `ValueHeightChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the height box value changes |
| `ValueWidthLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit, when focus leaves the input box after `ValueWidth` changed |
| `ValueHeightLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit, when focus leaves the input box after `ValueHeight` changed |

## 📝 Notes

- Only `Maximum` and `HideUpDownButtons` are forwarded to the inner `PixelSizeBox`. The inherited `Minimum` and `ButtonsAlignment` have no effect; the inner boxes have a fixed minimum of `0`
- `IsValid()` only checks that `ValidationText` is empty; `IsMandatory` does not add a check of its own
- The two boxes themselves are described in `PixelSizeBox_Readme.md`

## 🔗 Related Controls

- **PixelSizeBox** - The width × height input without the label
- **LabelIntegerXyBox** - Labeled X/Y integer pair
- **LabelIntegerBox** - Labeled single integer input

## 🎮 Sample Application

See the LabelPixelSizeBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelPixelSizeBox** for interactive examples.
