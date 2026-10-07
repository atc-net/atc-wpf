# 🏷️ LabelAccentColorSelector

A labeled form control that wraps `AccentColorSelector` for switching the application's accent color.

## 🔍 Overview

`LabelAccentColorSelector` derives from `LabelControlBase` and composes a `LabelContent` (label area and information popup) around an `AccentColorSelector`. It gives the accent drop-down the same label layout, width and orientation options as the other `Label*` form controls. When no `LabelText` is set, the label defaults to the localized "Accent" text and follows UI-culture changes.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelAccentColorSelector />
```

### Vertical Layout with Help Text

```xml
<atc:LabelAccentColorSelector
    InformationText="This is a help text.."
    Orientation="Vertical" />
```

### Circle Indicator

```xml
<atc:LabelAccentColorSelector RenderColorIndicatorType="Circle" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Indicator shape forwarded to the inner `AccentColorSelector` (`None`, `Circle`, `Square`) |

### Inherited from LabelControlBase

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LabelText` | `string` | `""` (set to localized "Accent" when empty) | Label text |
| `LabelPosition` | `LabelPosition` | `Left` | Position of the label |
| `Orientation` | `Orientation` | `Horizontal` | Label/content layout direction |
| `LabelWidthNumber` | `int` | `120` | Label width |
| `LabelWidthSizeDefinition` | `SizeDefinitionType` | `Pixel` | Unit for `LabelWidthNumber` |
| `HideAreas` | `LabelControlHideAreasType` | `None` | Areas of the label layout to hide |
| `InformationText` | `string` | `""` | Help text shown in the information popup |
| `InformationContent` | `object?` | `null` | Custom information content |
| `InformationColor` | `Color` | `DodgerBlue` | Color of the information icon |

## 📝 Notes

- The selected accent is applied application-wide by the inner `AccentColorSelector`; there is no `Value`/`SelectedKey` property on the labeled control
- Derives from `LabelControlBase` (not `LabelControl`), so there are no mandatory/validation properties
- A custom `LabelText` is kept on culture change; only the default translated text is replaced

## 🔗 Related Controls

- **AccentColorSelector** - The bare selector without label
- **LabelThemeSelector** - Labeled base theme selector
- **LabelThemeAndAccentColorSelectors** - Theme and accent selectors combined
- **LabelWellKnownColorSelector** - Labeled selector for well-known color values

## 🎮 Sample Application

See the LabelAccentColorSelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelAccentColorSelector** for interactive examples.
