# 🌍 LabelCountrySelector

A labeled country drop-down form field with flags, mandatory indicator and validation display.

## 🔍 Overview

`LabelCountrySelector` puts a `CountrySelector` in the standard `LabelContent` layout used by the other `Label*` controls. The selected country is exposed as a string key in `SelectedKey`. When no `LabelText` is set, the label defaults to the localized "Country" text and follows UI-culture changes. It implements `ILabelCountrySelector` and can be created with `LabelControlFactory.CreateLabelCountrySelector(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelCountrySelector SelectedKey="{Binding CountryKey}" />
```

### Mandatory with "Please select"

```xml
<atc:LabelCountrySelector
    DropDownFirstItemType="PleaseSelect"
    IsMandatory="True"
    RenderFlagIndicatorType="Shiny16" />
```

### All Countries with a Default

```xml
<atc:LabelCountrySelector
    DefaultCultureIdentifier="de-DE"
    Orientation="Vertical"
    RenderFlagIndicatorType="Shiny16"
    UseOnlySupportedCountries="False" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelCountrySelector(
    groupIdentifier: "Address",
    labelText: "Country",
    isReadOnly: false,
    isMandatory: true,
    inputDataType: typeof(string),
    defaultCultureIdentifier: "da-DK");
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Key of the selected country. Binds two-way by default and updates the source on lost focus |
| `DefaultCultureIdentifier` | `string` | `null` | Country selected by default, as a culture name (e.g. `de-DE`) or LCID (e.g. `1031`) |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | Optional first item, such as `PleaseSelect` |
| `RenderFlagIndicatorType` | `RenderFlagIndicatorType` | `Flat16` | Flag style: `None`, `Flat16` or `Shiny16` |
| `UseOnlySupportedCountries` | `bool` | `true` | Limit the list to the supported countries |
| `UpdateUiCultureOnChangeEvent` | `bool` | `false` | Change the application UI culture when the selection changes |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is valid |
| `SelectorLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is invalid |

## 📝 Notes

- When mandatory, an empty key shows "Field is required" and the "Please select" item shows "Please select a country"
- `GetKey()` returns `SelectedKey`, or the inner selector's key when `SelectedKey` is empty
- `IsValid()` validates now and sets `ValidationText` without raising the events
- A custom `LabelText` is kept on culture change; only the default translated text is replaced
- The factory sets `DropDownFirstItemType` to `PleaseSelect` and turns off `UseOnlySupportedCountries` and `UpdateUiCultureOnChangeEvent`

## 🔗 Related Controls

- **CountrySelector** - The country drop-down without the label
- **LabelLanguageSelector** - Labeled language drop-down
- **LabelTimeZonePicker** - Labeled time zone picker

## 🎮 Sample Application

See the LabelCountrySelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelCountrySelector** for interactive examples.
