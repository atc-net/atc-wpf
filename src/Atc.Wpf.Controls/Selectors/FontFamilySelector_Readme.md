# 🔤 FontFamilySelector

A dropdown selector for choosing installed font families.

## 🔍 Overview

`FontFamilySelector` provides a ComboBox-style dropdown populated with system-installed font families. Each item previews the font in its own typeface.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

```xml
<selectors:FontFamilySelector SelectedKey="{Binding SelectedFontFamily, Mode=TwoWay}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Selected font family name |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | First item type (None, Blank, PleaseSelect) |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when selection changes |

## 🔗 Related Controls

- **LabelFontFamilySelector** - Labeled form version
- **LanguageSelector** - Language selector

## 🎮 Sample Application

See the FontFamilySelector sample in the Atc.Wpf.Sample application under **Wpf.Controls > Selectors > FontFamilySelector** for interactive examples.
