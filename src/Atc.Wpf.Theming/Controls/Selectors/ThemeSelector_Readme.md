# 🌗 ThemeSelector

A drop-down selector that switches the application's base theme (e.g. Light / Dark) at runtime.

## 🔍 Overview

`ThemeSelector` is a `UserControl` wrapping a `ComboBox` that lists every base color scheme registered with `ThemeManager.Current`. Each entry shows a small color indicator (rendered from the theme's `AtcApps.Brushes.ThemeBackground` / `AtcApps.Brushes.ThemeForeground` brushes) followed by the localized theme name. Selecting an entry immediately calls `ThemeManager.Current.ChangeThemeBaseColor(Application.Current, ...)`, so the whole application switches theme.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Selectors;
```

## 🚀 Usage

### Basic Example

```xml
<atc:ThemeSelector />
```

### Circle Indicator

```xml
<atc:ThemeSelector RenderColorIndicatorType="Circle" />
```

### Text Only

```xml
<atc:ThemeSelector RenderColorIndicatorType="None" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator shown before each theme name (`None`, `Circle`, `Square`) |
| `SelectedKey` | `string` | detected theme (falls back to `Light`) | Base color scheme name of the selected theme. Setting it changes the application theme. Empty values are ignored |
| `Items` | `IList<ThemeItem>` | populated from `ThemeManager` | The available themes (`Name`, `DisplayName`, `BorderColorBrush`, `ColorBrush`) |

> `SelectedKey` and `Items` are plain CLR properties raising `INotifyPropertyChanged`; only `RenderColorIndicatorType` is a dependency property.

## 📝 Notes

- On construction the control detects the current theme via `ThemeManager.Current.DetectTheme(this)` and pre-selects it
- Theme display names are translated through the `ColorNames` resources; untranslated names are shown as `#{Name}`
- The list is re-populated (and the selection kept) when `CultureManager.UiCultureChanged` fires
- The change is applied to `Application.Current`, not only to the containing window

## 🔗 Related Controls

- **AccentColorSelector** - Selects the accent color scheme
- **LabelThemeSelector** - `ThemeSelector` wrapped in a labeled form control
- **LabelThemeAndAccentColorSelectors** - Theme and accent selectors side by side

## 🎮 Sample Application

No dedicated sample yet. The control is shown through **Wpf.Forms > Label Controls > Selectors > LabelThemeSelector**.
