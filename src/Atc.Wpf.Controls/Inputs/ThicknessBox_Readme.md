# 📐 ThicknessBox

A four-up numeric editor for `System.Windows.Thickness` values: separate `NumericBox`-based fields for `Left`, `Top`, `Right`, `Bottom`. The composed `Value` (and each side) is two-way bindable.

## 🔍 Overview

Use `ThicknessBox` to edit any property of type `Thickness` — most commonly `Margin`, `Padding`, or `BorderThickness`. The control keeps the four side properties and the composed `Value` in sync: setting `Value` populates the four sides; editing any side recomposes `Value`. Each side honours the shared `Minimum` / `Maximum` bounds.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic

```xml
<inputs:ThicknessBox
    Value="{Binding Margin, Mode=TwoWay}" />
```

### Bounded with up/down buttons hidden

```xml
<inputs:ThicknessBox
    Value="{Binding Padding, Mode=TwoWay}"
    Minimum="0"
    Maximum="40"
    HideUpDownButtons="True" />
```

### Edit individual sides

```xml
<inputs:ThicknessBox
    Left="{Binding LeftInset, Mode=TwoWay}"
    Top="{Binding TopInset, Mode=TwoWay}"
    Right="{Binding RightInset, Mode=TwoWay}"
    Bottom="{Binding BottomInset, Mode=TwoWay}"
    Minimum="0" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Value` | `Thickness` | `default` | The composed thickness (two-way) |
| `Left` | `double` | `0` | Left side (two-way; setter updates `Value`) |
| `Top` | `double` | `0` | Top side (two-way) |
| `Right` | `double` | `0` | Right side (two-way) |
| `Bottom` | `double` | `0` | Bottom side (two-way) |
| `Minimum` | `double` | `double.MinValue` | Lower bound applied to each side |
| `Maximum` | `double` | `double.MaxValue` | Upper bound applied to each side |
| `HideUpDownButtons` | `bool` | `false` | Hide the spin buttons on every side |

## 📡 Events

| Event | Type | When |
|---|---|---|
| `ValueChanged` (routed) | `RoutedPropertyChangedEventArgs<double?>` | Bubbles when any side changes |
| `ValueLeftChanged` | `EventHandler<ValueChangedEventArgs<double?>>` | After the `Left` value is committed |
| `ValueTopChanged` | `EventHandler<ValueChangedEventArgs<double?>>` | After the `Top` value is committed |
| `ValueRightChanged` | `EventHandler<ValueChangedEventArgs<double?>>` | After the `Right` value is committed |
| `ValueBottomChanged` | `EventHandler<ValueChangedEventArgs<double?>>` | After the `Bottom` value is committed |

## 📝 Notes

- Bind to `Value` for the most natural Thickness-shaped flow; use the per-side properties only when you genuinely need to wire each side to a separate ViewModel field.
- Internally the control suppresses re-entrant updates while syncing `Value` ↔ sides, so two-way binding stays stable.
- Each side is rendered with a `NumericBox` under the hood, so the keyboard / mouse-wheel / spin-button behaviour matches that control.

## 🔗 Related controls

- **`NumericBox`** — the underlying single-value input
- **`PixelSizeBox`** — width × height pair for pixel dimensions
- **`IntegerXyBox` / `DecimalXyBox`** — X/Y coordinate pairs

## 🎮 Sample application

See the `ThicknessBox` demo in `Atc.Wpf.Sample` under **Wpf.Controls > Inputs**.
