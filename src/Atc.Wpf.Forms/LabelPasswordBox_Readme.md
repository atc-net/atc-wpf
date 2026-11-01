# 🔑 LabelPasswordBox

A labeled password form field with a reveal button, length validation and mandatory indicator.

## 🔍 Overview

`LabelPasswordBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a WPF `PasswordBox`. The inner box uses the `AtcApps.Styles.PasswordBox.Revealed` style, which has a button to show the typed password. The password is exposed as the bindable `Text` property and is validated against `IsMandatory`, `MinLength` and `MaxLength`. It implements `ILabelPasswordBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelPasswordBox LabelText="Password" Text="{Binding Password}" />
```

### Mandatory with Watermark

```xml
<atc:LabelPasswordBox
    InformationText="This is a help text.."
    IsMandatory="True"
    LabelText="Password"
    MinLength="8"
    ShowClearTextButton="True"
    WatermarkText="Fill out with text.." />
```

### Vertical Layout

```xml
<atc:LabelPasswordBox LabelText="Password" Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | The password. Binds two-way by default and updates the source when focus is lost |
| `WatermarkText` | `string` | `""` | Hint text shown while the box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |
| `MinLength` | `uint` | `0` | Minimum number of characters |
| `MaxLength` | `uint` | `100` | Maximum number of characters; also limits typing in the inner `PasswordBox` |
| `ShowClearTextButton` | `bool` | `true` | Shows a button that clears the password |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `TextChanged` | `RoutedPropertyChangedEventHandler<string>` | Bubbling routed event, raised when the user changes the password. The old value is always empty |
| `TextLostFocusValid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `Text` changes and the password is valid. The old value is always `null` |
| `TextLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `Text` changes and the password is invalid |

## 📝 Notes

- `IsValid()` validates now and returns `true` when `ValidationText` is empty
- A mandatory field that is empty or only white space shows "Field is required"
- The rules are checked in this order: mandatory, `MinLength`, `MaxLength`
- There is no format, strength or regex validation

## 🔗 Related Controls

- **LabelTextBox** - Labeled text input with more validation rules
- **LabelTextInfo** - Labeled read-only text

## 🎮 Sample Application

See the LabelPasswordBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelPasswordBox** for interactive examples.
