# 🎨 AccentColorSelector

A drop-down selector that switches the application's accent color scheme at runtime.

## 🔍 Overview

`AccentColorSelector` is a `UserControl` wrapping a `ComboBox` that lists every accent color scheme registered with `ThemeManager.Current` (schemes containing a `.` are excluded). Each entry shows a color indicator rendered with the scheme's showcase brush followed by the localized color name. Selecting an entry immediately calls `ThemeManager.Current.ChangeThemeColorScheme(Application.Current, ...)`, so the whole application changes accent while keeping the current base theme.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Selectors;
```

## 🚀 Usage

### Basic Example

```xml
<atc:AccentColorSelector />
```

### Circle Indicator

```xml
<atc:AccentColorSelector RenderColorIndicatorType="Circle" />
```

### Combined with ThemeSelector

```xml
<StackPanel Orientation="Horizontal">
    <atc:ThemeSelector Width="150" />
    <atc:AccentColorSelector Width="150" Margin="10,0,0,0" />
</StackPanel>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator shown before each name (`None`, `Circle`, `Square`) |
| `SelectedKey` | `string` | detected accent (falls back to `Blue`) | Name of the selected color scheme. Setting it changes the application accent. Empty values are ignored |
| `Items` | `IList<ColorItem>` | populated from `ThemeManager` | The available accent colors, sorted by display name |

> `SelectedKey` and `Items` are plain CLR properties raising `INotifyPropertyChanged`; only `RenderColorIndicatorType` is a dependency property.

## 📝 Notes

- On construction the control detects the current theme via `ThemeManager.Current.DetectTheme(this)` and pre-selects its color scheme
- Color names are translated through the `ColorNames` resources; untranslated names are shown as `#{Name}`
- The list is re-populated (and the selection kept) when `CultureManager.UiCultureChanged` fires
- The change is applied to `Application.Current`, not only to the containing window
- Picking an accent stops `WindowsThemeSync` from following the Windows accent color; while loaded, the control also shows accent changes made elsewhere. A runtime accent from the sync is not in the list, so no entry is selected

## 🔗 Related Controls

- **ThemeSelector** - Selects the base theme (Light / Dark)
- **LabelAccentColorSelector** - `AccentColorSelector` wrapped in a labeled form control
- **LabelThemeAndAccentColorSelectors** - Theme and accent selectors side by side
- **WellKnownColorSelector** - Selects a well-known color value (does not change the theme)

## 🎮 Sample Application

No dedicated sample yet. The control is shown through **Wpf.Forms > Label Controls > Selectors > LabelAccentColorSelector**.
