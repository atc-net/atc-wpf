# ✏️ LabelTextBox

A labeled single-line text form field with length, character, regex and format validation.

## 🔍 Overview

`LabelTextBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a single-line `TextBox`. It validates the text against `IsMandatory`, `MinLength`, `MaxLength`, `CharactersNotAllowed`, `RegexPattern` and `ValidationFormat`, and shows the first failing rule as the validation message. By default validation runs when the field loses focus. It implements `ILabelTextBox` and can be created with `LabelControlFactory.CreateLabelTextBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelTextBox LabelText="Name" Text="{Binding Name}" />
```

### Mandatory with Watermark and Format Validation

```xml
<atc:LabelTextBox
    InformationText="This is a help text.."
    IsMandatory="True"
    LabelText="Email"
    ShowClearTextButton="True"
    ValidationFormat="Email"
    WatermarkText="Email address" />
```

### Regex Pattern

```xml
<atc:LabelTextBox
    IsMandatory="True"
    LabelText="Regex [a-z0-9]"
    RegexPattern="^[a-z0-9]+$" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelTextBox(
    groupIdentifier: "Customer",
    labelText: "Name",
    isReadOnly: false,
    isMandatory: true,
    watermarkText: "Full name",
    inputDataType: typeof(string),
    value: null,
    minLength: 2,
    maxLength: 50,
    regexPattern: null);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | The text. Binds two-way by default and updates the source on every change |
| `WatermarkText` | `string` | `""` | Hint text shown while the box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |
| `MinLength` | `uint` | `0` | Minimum number of characters |
| `MaxLength` | `uint` | `100` | Maximum number of characters; also limits typing in the inner `TextBox` |
| `CharactersNotAllowed` | `string` | `""` | Characters that are not allowed (case-insensitive) |
| `RegexPattern` | `string?` | `null` | Regular expression the text must match (case-insensitive) |
| `ValidationFormat` | `TextBoxValidationRuleType` | `None` | Built-in format rule, for example `Email`, `HttpOrHttps`, `IPv4Address`, `Hostname` or `OpcTcp` |
| `ShowClearTextButton` | `bool` | `true` | Shows a button that clears the text |
| `TriggerOnlyOnLostFocus` | `bool` | `true` | Validate on lost focus only. While a validation message is shown, it re-validates on every change |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `TextChanged` | `RoutedPropertyChangedEventHandler<string>` | Bubbling routed event, raised every time `Text` changes |
| `TextLostFocusValid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when focus leaves the field and the text is valid, with the text from before and after the edit |
| `TextLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when focus leaves the field and the text is invalid |

## 📝 Notes

- `IsValid()` validates now and returns `true` when `ValidationText` is empty
- A mandatory field with only white space shows "Field is required"; an empty optional field is always valid
- Clearing the text with the clear button does not show a validation message
- The rules are checked in this order: mandatory, `MinLength`, `MaxLength`, `CharactersNotAllowed`, `RegexPattern`, `ValidationFormat`

## 🔗 Related Controls

- **LabelPasswordBox** - Labeled password input
- **LabelTextInfo** - Labeled read-only text
- **LabelEndpointBox** - Labeled protocol, host and port input
- **LabelComboBox** - Labeled single selection by key

## 🎮 Sample Application

See the LabelTextBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelTextBox** for interactive examples.
