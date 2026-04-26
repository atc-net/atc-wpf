# 🔢 NumericBox

The base numeric input control behind `IntegerBox` and `DecimalBox`. Provides a `TextBox` paired with up/down `RepeatButton`s and rich behaviour around value parsing, range clamping, formatting, and keyboard / mouse-wheel input.

## 🔍 Overview

Most consumers use the typed wrappers (`IntegerBox`, `DecimalBox`, `CurrencyBox`) or the labeled form variants (`LabelIntegerBox`, `LabelDecimalBox`). Use `NumericBox` directly when you need full control over `StringFormat`, the spin buttons' content, hex input, or mixed positive/negative behaviour.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic

```xml
<inputs:NumericBox
    Value="{Binding Quantity, Mode=TwoWay}"
    Minimum="0"
    Maximum="100"
    Interval="1" />
```

### Decimal with precision and prefix / suffix

```xml
<inputs:NumericBox
    Value="{Binding Price, Mode=TwoWay}"
    StringFormat="N2"
    Minimum="0"
    Maximum="9999.99"
    Interval="0.05"
    PrefixText="$ "
    SuffixText=" USD" />
```

### Hexadecimal input

```xml
<inputs:NumericBox
    Value="{Binding ColorCode, Mode=TwoWay}"
    StringFormat="X4"
    NumericInputMode="Numbers"
    Minimum="0"
    Maximum="65535" />
```

### Hidden spin buttons

```xml
<inputs:NumericBox
    Value="{Binding Score, Mode=TwoWay}"
    HideUpDownButtons="True" />
```

### Snap to interval

```xml
<inputs:NumericBox
    Value="{Binding Volume, Mode=TwoWay}"
    Minimum="0"
    Maximum="100"
    Interval="5"
    SnapToMultipleOfInterval="True" />
```

## ⚙️ Properties

### Value & range

| Property | Type | Default | Description |
|---|---|---|---|
| `Value` | `double?` | `null` | The current value (two-way by default) |
| `DefaultValue` | `double?` | `null` | Value applied when the user clears the input |
| `Minimum` | `double` | `double.MinValue` | Lower bound (inclusive); `Value` is coerced into range |
| `Maximum` | `double` | `double.MaxValue` | Upper bound (inclusive); `Value` is coerced into range |
| `Interval` | `double` | `1` | Step size for spin buttons / arrow keys / wheel |
| `SnapToMultipleOfInterval` | `bool` | `false` | When true, `Value` snaps to the nearest multiple of `Interval` |

### Formatting & culture

| Property | Type | Default | Description |
|---|---|---|---|
| `StringFormat` | `string` | `""` | Standard or custom .NET numeric format (e.g. `N2`, `C`, `X4`, `{0:0.##}`) |
| `Culture` | `CultureInfo?` | `null` | Culture used for parsing/formatting; falls back to the framework `Language` |
| `PrefixText` | `string` | `""` | Static text shown before the value |
| `SuffixText` | `string` | `""` | Static text shown after the value |
| `NumericInputMode` | `NumericInput` | `All` | Restrict allowed characters (`Numbers`, `Decimal`, `All`) |
| `DecimalPointCorrection` | `DecimalPointCorrectionMode` | `Inherits` | Auto-corrects the decimal separator on paste/type |
| `ParsingNumberStyle` | `NumberStyles` | `Any` | `NumberStyles` flags used by `double.TryParse` |

### Input behaviour

| Property | Type | Default | Description |
|---|---|---|---|
| `Delay` | `int` | `500` | RepeatButton initial delay (ms) |
| `Speedup` | `bool` | `true` | Accelerate value change while spin button is held |
| `InterceptArrowKeys` | `bool` | `true` | Up / Down keys change `Value` by `Interval` |
| `InterceptMouseWheel` | `bool` | `true` | Mouse wheel changes `Value` by `Interval` |
| `InterceptManualEnter` | `bool` | `true` | Commit on `Enter`; revert on `Escape` |
| `TrackMouseWheelWhenMouseOver` | `bool` | `true` | Wheel works on hover (no focus required) |
| `IsReadOnly` | `bool` | `false` | Suppress edits and spin buttons |

### Spin buttons & layout

| Property | Type | Default | Description |
|---|---|---|---|
| `HideUpDownButtons` | `bool` | `false` | Hide the spin buttons |
| `ButtonsAlignment` | `ButtonsAlignmentType` | `Right` | Position the spin buttons (`Left`, `Right`) |
| `SwitchUpDownButtons` | `bool` | `false` | Swap which button increments / decrements |
| `UpDownButtonsWidth` | `double` | `20` | Width of each spin button |
| `UpDownButtonsMargin` | `Thickness` | `0,-0.5,-0.5,0` | Margin around the button stack |
| `UpDownButtonsFocusable` | `bool` | `true` | Whether spin buttons enter the tab order |
| `SpinButtonStyle` | `Style?` | `null` | Custom style (must target `ButtonBase`) |
| `ButtonUpContent` / `ButtonDownContent` | `object?` | `null` | Custom content for each button |
| `ButtonUpContentTemplate` / `ButtonDownContentTemplate` | `DataTemplate?` | `null` | Template for the custom content |
| `TextAlignment` | `TextAlignment` | `Right` | Alignment of the value text inside the box |

## 📡 Routed events

| Event | Args | When |
|---|---|---|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<double?>` | Whenever `Value` changes |
| `ValueIncremented` | `NumericBoxChangedRoutedEventArgs` | After a successful spin-up / arrow-up / wheel-up |
| `ValueDecremented` | `NumericBoxChangedRoutedEventArgs` | After a successful spin-down / arrow-down / wheel-down |
| `MaximumReached` | `RoutedEventArgs` | When the value would exceed `Maximum` and is clamped |
| `MinimumReached` | `RoutedEventArgs` | When the value would fall below `Minimum` and is clamped |
| `DelayChanged` | `RoutedEventArgs` | When the spin-button repeat `Delay` changes |

## 📝 Notes

- Prefer the typed wrappers (`IntegerBox`, `DecimalBox`, `CurrencyBox`) for typical scalar input — they pre-configure `StringFormat`, `NumericInputMode`, and value coercion.
- When `Value` is `null`, the `TextBox` is empty; setting `DefaultValue` causes a clear-then-blur to restore that value.
- `StringFormat` accepts both shorthand (`N2`, `C`, `X4`) and full `{0:...}` patterns; for hex, the box also accepts the hex chars `a–f`.
- `Culture` controls parsing of decimal/group separators and currency symbols. If unset, the framework `Language` is honored, so `xml:lang` on a parent works.
- Because the spin buttons use `RepeatButton`, holding them down accelerates when `Speedup="True"`.

## 🔗 Related controls

- **`IntegerBox`** — integer-only specialization
- **`DecimalBox`** — decimal-typed specialization
- **`CurrencyBox`** — currency-formatted specialization
- **`LabelIntegerBox` / `LabelDecimalBox`** — labelled form-control wrappers (`Atc.Wpf.Forms`)

## 🎮 Sample application

See the `NumericBox` family demos in `Atc.Wpf.Sample` under **Wpf.Controls > Inputs**.
