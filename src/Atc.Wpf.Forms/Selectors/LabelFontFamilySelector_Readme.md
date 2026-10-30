# 🔠 LabelFontFamilySelector

A labeled font family drop-down form field with mandatory indicator and validation display.

## 🔍 Overview

`LabelFontFamilySelector` puts a `FontFamilySelector` in the standard `LabelContent` layout used by the other `Label*` controls. The selected font family is exposed as a string key in `SelectedKey`. When no `LabelText` is set, the label defaults to the localized "Font" text and follows UI-culture changes. It implements `ILabelFontFamilySelector`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelFontFamilySelector SelectedKey="{Binding FontFamilyName}" />
```

### Mandatory with "Please select"

```xml
<atc:LabelFontFamilySelector
    DropDownFirstItemType="PleaseSelect"
    InformationText="Font used in reports."
    IsMandatory="True" />
```

### Vertical Layout with Custom Label

```xml
<atc:LabelFontFamilySelector
    LabelText="Heading font"
    Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Key of the selected font family. Binds two-way by default and updates the source on lost focus |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | Optional first item, such as `PleaseSelect` |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is valid |
| `SelectorLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is invalid |

## 📝 Notes

- When mandatory, an empty key shows "Field is required" and the "Please select" item shows "Please select a font"
- `GetKey()` returns `SelectedKey`, or the inner selector's key when `SelectedKey` is empty
- `IsValid()` validates now and sets `ValidationText` without raising the events
- A custom `LabelText` is kept on culture change; only the default translated text is replaced
- There is no `LabelControlFactory` method for this control

## 🔗 Related Controls

- **FontFamilySelector** - The font family drop-down without the label
- **LabelFontPicker** - Labeled full font picker (family, size, weight, style, colors)

## 🎮 Sample Application

See the LabelFontFamilySelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelFontFamilySelector** for interactive examples.
