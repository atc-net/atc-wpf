# 🌍 CountrySelector

A dropdown selector for choosing countries with flag indicators and culture-aware display.

## 🔍 Overview

`CountrySelector` provides a ComboBox-style dropdown populated with countries, showing flag icons and localized country names. It supports filtering to only show application-supported countries, automatic UI culture updates, and configurable flag rendering styles.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

```xml
<selectors:CountrySelector
    SelectedKey="{Binding SelectedCountryCode}"
    UseOnlySupportedCountries="True"
    RenderFlagIndicatorType="Flat16" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Selected country code |
| `UseOnlySupportedCountries` | `bool` | `true` | Show only app-supported countries |
| `RenderFlagIndicatorType` | `RenderFlagIndicatorType` | `Flat16` | Flag icon style |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | First item type (None, Blank, PleaseSelect) |
| `DefaultCultureIdentifier` | `string` | `null` | Default culture for selection |
| `UpdateUiCultureOnChangeEvent` | `bool` | `false` | When true, selecting a country changes the application UI culture to that country's culture |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when selection changes |

## 📝 Notes

- Country names are localized based on the current UI culture
- Flag icons update when the theme changes

## 🔗 Related Controls

- **LanguageSelector** - Language/locale selector with flags
- **LabelCountrySelector** - Labeled form version

## 🎮 Sample Application

See the CountrySelector sample in the Atc.Wpf.Sample application under **Wpf.Controls > Selectors > CountrySelector** for interactive examples.
