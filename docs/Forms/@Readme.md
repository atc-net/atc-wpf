# 📝 Form Controls

The `Atc.Wpf.Forms` library provides 25+ **labeled form field controls** for building data-entry views with consistent layout, validation, mandatory indicators, and information tooltips.

Each control wraps a primitive from `Atc.Wpf.Controls` (e.g. `LabelTextBox` wraps a `TextBox`, `LabelIntegerBox` wraps `IntegerBox`) and adds the label, the asterisk for `IsMandatory`, and the validation message slot.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

XAML:

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## 🔍 What's in the box

| Category | Controls |
|---|---|
| Text input | `LabelTextBox`, `LabelPasswordBox` |
| Number input | `LabelIntegerBox`, `LabelDecimalBox`, `LabelIntegerXyBox`, `LabelDecimalXyBox`, `LabelPixelSizeBox` |
| Date / time | `LabelDatePicker`, `LabelTimePicker`, `LabelDateTimePicker` |
| Selection | `LabelCheckBox`, `LabelComboBox`, `LabelToggleSwitch` |
| Selectors | `LabelAccentColorSelector`, `LabelCountrySelector`, `LabelFontFamilySelector`, `LabelLanguageSelector`, `LabelThemeSelector`, `LabelThemeAndAccentColorSelectors`, `LabelWellKnownColorSelector` |
| Pickers | `LabelColorPicker`, `LabelDirectoryPicker`, `LabelFilePicker`, `LabelFontPicker` |
| Slider | `LabelSlider` |
| Network | `LabelEndpointBox` |
| Information display | `LabelContent`, `LabelTextInfo` |

The full per-control reference (features, properties, examples) lives in [`src/Atc.Wpf.Forms/README.md`](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Forms/README.md).

## 🧱 Common shape

All form controls inherit from `LabelControl` and expose this surface:

| Property | Type | Description |
|---|---|---|
| `LabelText` | `string` | The label text shown next to / above the input |
| `IsMandatory` | `bool` | Marks the field as required (asterisk affordance) |
| `ShowAsteriskOnMandatory` | `bool` | Toggles the asterisk indicator |
| `MandatoryColor` | `Brush` | Color of the mandatory indicator |
| `ValidationText` | `string` | Validation error text shown below the input |
| `ValidationColor` | `Brush` | Color of the validation message |
| `Identifier` / `GroupIdentifier` | `string` | Stable ids for grouping / lookup |
| `Orientation` | `Orientation` | `Horizontal` (label left) or `Vertical` (label above) |
| `HideAreas` | `LabelControlHideAreas` | Hide the label / info / validation slots |

## ✅ Validation pattern

Form controls use **deferred validation**: error messages are not shown until the user has interacted with the field (i.e. after the first focus loss). This avoids the "everything is red on first load" anti-pattern.

```xml
<atc:LabelTextBox
    LabelText="Email"
    Text="{Binding Email, Mode=TwoWay}"
    ValidationRule="Email"
    IsMandatory="True" />
```

## 🚀 Quick start

```xml
<StackPanel xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <atc:LabelTextBox LabelText="Name"  Text="{Binding Name, Mode=TwoWay}" IsMandatory="True" />
    <atc:LabelIntegerBox LabelText="Age" Value="{Binding Age, Mode=TwoWay}" MinimumValue="0" MaximumValue="150" />
    <atc:LabelDatePicker LabelText="Birth date" SelectedDate="{Binding BirthDate, Mode=TwoWay}" />
</StackPanel>
```

## 📚 See also

- Per-control reference: [`src/Atc.Wpf.Forms/README.md`](https://github.com/atc-net/atc-wpf/blob/main/src/Atc.Wpf.Forms/README.md)
- Underlying primitives: [`src/Atc.Wpf.Controls/`](https://github.com/atc-net/atc-wpf/tree/main/src/Atc.Wpf.Controls)
- Composite dialogs that use these forms: [`src/Atc.Wpf.Components/Dialogs/`](https://github.com/atc-net/atc-wpf/tree/main/src/Atc.Wpf.Components/Dialogs)
