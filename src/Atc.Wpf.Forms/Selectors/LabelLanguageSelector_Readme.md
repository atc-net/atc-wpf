# 🗣️ LabelLanguageSelector

A labeled language drop-down form field with flags, mandatory indicator and validation display.

## 🔍 Overview

`LabelLanguageSelector` puts a `LanguageSelector` in the standard `LabelContent` layout used by the other `Label*` controls. The selected language is exposed as a string key in `SelectedKey`. By default, changing the selection also changes the application UI culture (`UpdateUiCultureOnChangeEvent` is `true`). When no `LabelText` is set, the label defaults to the localized "Language" text and follows UI-culture changes. It implements `ILabelLanguageSelector` and can be created with `LabelControlFactory.CreateLabelLanguageSelector(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example (switches the UI culture)

```xml
<atc:LabelLanguageSelector />
```

### Form Field Only

```xml
<atc:LabelLanguageSelector
    DropDownFirstItemType="PleaseSelect"
    IsMandatory="True"
    RenderFlagIndicatorType="Shiny16"
    SelectedKey="{Binding LanguageKey}"
    UpdateUiCultureOnChangeEvent="False" />
```

### All Languages with a Default

```xml
<atc:LabelLanguageSelector
    DefaultCultureIdentifier="de-DE"
    Orientation="Vertical"
    UpdateUiCultureOnChangeEvent="False"
    UseOnlySupportedLanguages="False" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelLanguageSelector(
    groupIdentifier: "Profile",
    labelText: "Language",
    isReadOnly: false,
    isMandatory: true,
    inputDataType: typeof(string),
    defaultCultureIdentifier: "en-US");
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Key of the selected language. Binds two-way by default and updates the source on lost focus |
| `DefaultCultureIdentifier` | `string` | `""` | Language selected by default, as a culture name (e.g. `de-DE`) or LCID (e.g. `1031`) |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | Optional first item, such as `PleaseSelect` |
| `RenderFlagIndicatorType` | `RenderFlagIndicatorType` | `Flat16` | Flag style: `None`, `Flat16` or `Shiny16` |
| `UseOnlySupportedLanguages` | `bool` | `true` | Limit the list to the supported languages |
| `UpdateUiCultureOnChangeEvent` | `bool` | `true` | Change the application UI culture when the selection changes |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is valid |
| `SelectorLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is invalid |

## 📝 Notes

- Set `UpdateUiCultureOnChangeEvent="False"` when the control is a data field and should not switch the application language
- When mandatory, an empty key shows "Field is required" and the "Please select" item shows "Please select a language"
- `GetKey()` returns `SelectedKey`, or the inner selector's key when `SelectedKey` is empty
- `IsValid()` validates now and sets `ValidationText` without raising the events
- A custom `LabelText` is kept on culture change; only the default translated text is replaced
- The factory sets `DropDownFirstItemType` to `PleaseSelect` and turns off `UseOnlySupportedLanguages` and `UpdateUiCultureOnChangeEvent`

## 🔗 Related Controls

- **LanguageSelector** - The language drop-down without the label
- **LabelCountrySelector** - Labeled country drop-down
- **LabelThemeSelector** - Labeled theme selector

## 🎮 Sample Application

See the LabelLanguageSelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelLanguageSelector** for interactive examples.
