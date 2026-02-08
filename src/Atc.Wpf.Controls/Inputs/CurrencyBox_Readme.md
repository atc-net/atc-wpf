# 💰 CurrencyBox

A locale-aware currency input control with formatting, thousand separators, and decimal precision.

## 🔍 Overview

`CurrencyBox` extends `NumericBox` to provide currency-specific formatting using the .NET `"C"` (currency) standard numeric format specifier. It automatically handles currency symbol placement, thousand separators, and decimal precision based on the configured `Culture`. The control supports spin buttons for increment/decrement, keyboard input, and configurable decimal places.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Currency Input

```xml
<inputs:CurrencyBox Value="{Binding Price}" />
```

### Hidden Spin Buttons

```xml
<inputs:CurrencyBox Value="{Binding Amount}" HideUpDownButtons="True" />
```

### Custom Decimal Places

```xml
<!-- Whole currency (no decimals) -->
<inputs:CurrencyBox Value="{Binding Total}" DecimalPlaces="0" />

<!-- Standard 2 decimals (default) -->
<inputs:CurrencyBox Value="{Binding Price}" DecimalPlaces="2" />

<!-- High precision (4 decimals) -->
<inputs:CurrencyBox Value="{Binding Rate}" DecimalPlaces="4" />
```

### Locale-Specific Currency

```xml
<!-- Euro formatting -->
<inputs:CurrencyBox Value="{Binding Price}" Culture="{x:Static globalization:CultureInfo.CurrentUICulture}" />
```

### With Min/Max Bounds

```xml
<inputs:CurrencyBox Value="{Binding Donation}" Minimum="1" Maximum="10000" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DecimalPlaces` | `int` | `2` | Number of decimal digits (0-15). Syncs with `StringFormat` as `"C{N}"` |
| `Value` | `double` | `0` | Current numeric value (inherited from NumericBox) |
| `Minimum` | `double` | `Decimal.MinValue` | Minimum allowed value |
| `Maximum` | `double` | `Decimal.MaxValue` | Maximum allowed value |
| `Culture` | `CultureInfo?` | `CurrentUICulture` | Culture for currency symbol and formatting |
| `HideUpDownButtons` | `bool` | `false` | Hide the spin buttons |
| `PrefixText` | `string` | `""` | Text displayed before the value |
| `SuffixText` | `string` | `""` | Text displayed after the value |
| `StringFormat` | `string` | `"C2"` | Format string (auto-synced with DecimalPlaces) |

## 📝 Notes

- The `"C"` format string automatically handles currency symbol placement (before or after the number) based on Culture
- Thousand separators are applied automatically via the Culture's number format settings
- `DecimalPlaces` and `StringFormat` are kept in sync; changing one updates the other
- `DecimalPointCorrection` is set to `Currency` by default, using the culture's currency decimal separator
- Inherits all NumericBox features: keyboard navigation, clipboard support, spin buttons, and validation

## 🔗 Related Controls

- **DecimalBox** - Numeric input with number formatting (`"N"` format)
- **IntegerBox** - Integer-only input without decimals
- **NumericBox** - Base numeric input control
- **LabelCurrencyBox** - Form field variant with label, validation, and mandatory indicator

## 🎮 Sample Application

See the CurrencyBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > CurrencyBox** for interactive examples including locale switching.