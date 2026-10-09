# 📏 PixelSizeBox

A compact input for a width / height pair in pixels.

## 🔍 Overview

`PixelSizeBox` is a `UserControl` that places two `IntegerBox` controls side by side - one for width and one for height. Each box shows a localized width / height prefix and a `px` suffix, has a fixed minimum of `0` and shares a configurable `Maximum`. Use it for image sizes, window sizes, resolutions and similar dimensions.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Example

```xml
<atc:PixelSizeBox
    ValueWidth="{Binding ImageWidth}"
    ValueHeight="{Binding ImageHeight}" />
```

### Maximum and Hidden Spin Buttons

```xml
<atc:PixelSizeBox
    Maximum="4096"
    HideUpDownButtons="True"
    ValueWidth="{Binding ImageWidth}"
    ValueHeight="{Binding ImageHeight}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueWidth` | `int` | `0` | The width value. Two-way by default |
| `ValueHeight` | `int` | `0` | The height value. Two-way by default |
| `Maximum` | `int` | `int.MaxValue` | Upper bound applied to both boxes |
| `HideUpDownButtons` | `bool` | `false` | Hides the spin buttons of both boxes |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueWidthChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the width box value changes |
| `ValueHeightChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<int>` | Raised when the height box value changes |
| `ValueWidthLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit, when focus leaves the input box after `ValueWidth` changed |
| `ValueHeightLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised once per edit, when focus leaves the input box after `ValueHeight` changed |

## 📝 Notes

- The minimum of both boxes is fixed at `0`; there is no `Minimum` property
- The prefix texts come from the `PixelWidth` / `PixelHeight` resources in `Atc.Wpf.Controls.Resources.Miscellaneous`, and the suffix is always `px`
- Unlike `IntegerXyBox`, the value properties do not set `UpdateSourceTrigger=LostFocus`, so the binding default applies
- `ValueWidthChanged` / `ValueHeightChanged` are not raised when the inner box value changes from or to `null`

## 🔗 Related Controls

- **IntegerBox** - The single-value integer input used for each dimension
- **IntegerXyBox** - Generic X / Y integer pair
- **ThicknessBox** - Four-sided thickness input
- **LabelPixelSizeBox** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the PixelSizeBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > PixelSizeBox** for interactive examples.
