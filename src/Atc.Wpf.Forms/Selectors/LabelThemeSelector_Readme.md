# 🏷️ LabelThemeSelector

A labeled form control that wraps `ThemeSelector` for switching the application's base theme.

## 🔍 Overview

`LabelThemeSelector` derives from `LabelControlBase` and composes a `LabelContent` (label area and information popup) around a `ThemeSelector`. It gives the theme drop-down the same label layout, width and orientation options as the other `Label*` form controls. When no `LabelText` is set, the label defaults to the localized "Theme" text and follows UI-culture changes.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelThemeSelector />
```

### Vertical Layout with Help Text

```xml
<atc:LabelThemeSelector
    InformationText="Switches between the light and dark theme"
    Orientation="Vertical" />
```

### Custom Label and Indicator

```xml
<atc:LabelThemeSelector
    LabelText="Appearance"
    RenderColorIndicatorType="Circle" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape forwarded to the inner `ThemeSelector` (`None`, `Circle`, `Square`) |

### Inherited from LabelControlBase

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LabelText` | `string` | `""` (set to localized "Theme" when empty) | Label text |
| `LabelPosition` | `LabelPosition` | `Left` | Position of the label |
| `Orientation` | `Orientation` | `Horizontal` | Label/content layout direction |
| `LabelWidthNumber` | `int` | `120` | Label width |
| `LabelWidthSizeDefinition` | `SizeDefinitionType` | `Pixel` | Unit for `LabelWidthNumber` |
| `HideAreas` | `LabelControlHideAreasType` | `None` | Areas of the label layout to hide |
| `InformationText` | `string` | `""` | Help text shown in the information popup |
| `InformationContent` | `object?` | `null` | Custom information content |
| `InformationColor` | `Color` | `DodgerBlue` | Color of the information icon |

## 📝 Notes

- The selected theme is applied application-wide by the inner `ThemeSelector`; there is no `Value`/`SelectedKey` property on the labeled control
- Derives from `LabelControlBase` (not `LabelControl`), so there are no mandatory/validation properties
- A custom `LabelText` is kept on culture change; only the default translated text is replaced

## 🔗 Related Controls

- **ThemeSelector** - The bare selector without label
- **LabelAccentColorSelector** - Labeled accent color selector
- **LabelThemeAndAccentColorSelectors** - Theme and accent selectors combined

## 🎮 Sample Application

See the LabelThemeSelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelThemeSelector** for interactive examples.
