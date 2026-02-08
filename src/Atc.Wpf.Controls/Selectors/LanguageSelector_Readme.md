# 🌐 LanguageSelector

A dropdown selector for choosing languages/locales with flag indicators and culture-aware display.

## 🔍 Overview

`LanguageSelector` provides a ComboBox-style dropdown populated with languages, showing flag icons and localized language names. It supports filtering to only show application-supported languages, automatic UI culture switching on selection, and configurable flag rendering styles.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

```xml
<selectors:LanguageSelector
    SelectedKey="{Binding SelectedLanguageCode}"
    UseOnlySupportedLanguages="True"
    UpdateUiCultureOnChangeEvent="True" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Selected language/culture code |
| `UseOnlySupportedLanguages` | `bool` | `true` | Show only app-supported languages |
| `RenderFlagIndicatorType` | `RenderFlagIndicatorType` | `Flat16` | Flag icon style |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | First item type |
| `DefaultCultureIdentifier` | `string` | `""` | Default culture for selection |
| `UpdateUiCultureOnChangeEvent` | `bool` | `true` | Auto-switch UI culture on selection |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when selection changes |

## 📝 Notes

- When `UpdateUiCultureOnChangeEvent` is true, selecting a language changes the application's UI culture
- Language names are localized based on the current UI culture

## 🔗 Related Controls

- **CountrySelector** - Country selector with flags
- **LabelLanguageSelector** - Labeled form version

## 🎮 Sample Application

See the LanguageSelector sample in the Atc.Wpf.Sample application under **Wpf.Controls > Selectors > LanguageSelector** for interactive examples.
