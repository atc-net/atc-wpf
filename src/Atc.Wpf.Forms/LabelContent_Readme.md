# 🏷️ LabelContent

A labeled container that wraps any content with the standard form-field layout: label, mandatory indicator, information icon and validation message.

## 🔍 Overview

`LabelContent` is the layout used by all `Label*` form controls. It places a label next to (`Orientation="Horizontal"`) or above (`Orientation="Vertical"`) its content, and adds a mandatory asterisk, an information icon with tooltip and a validation message line. Use it directly to give any control (a `TextBox`, a custom control, a group of controls) the same look as the built-in labeled controls.

`LabelContent` has no members of its own; everything is configured through the label properties it inherits from `LabelControlBase` and `LabelControl`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelContent LabelText="Name">
    <TextBox Text="{Binding Name}" />
</atc:LabelContent>
```

### Mandatory with Help Text

```xml
<atc:LabelContent
    InformationText="This is a help text.."
    IsMandatory="True"
    LabelText="Email">
    <TextBox Text="{Binding Email}" />
</atc:LabelContent>
```

### Vertical Layout

```xml
<atc:LabelContent LabelText="Notes" Orientation="Vertical">
    <TextBox AcceptsReturn="True" Height="80" />
</atc:LabelContent>
```

### Label on the Right, No Validation Area

```xml
<atc:LabelContent
    HideAreas="Validation"
    LabelPosition="Right"
    LabelText="Remember me">
    <CheckBox />
</atc:LabelContent>
```

## ⚙️ Properties

All properties are inherited from `LabelControlBase` and `LabelControl`.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LabelText` | `string` | `""` | Label text, shown with a trailing colon when the label is on the left or on top |
| `LabelPosition` | `LabelPosition` | `Left` | `Left` or `Right` of the content (horizontal layout) |
| `Orientation` | `Orientation` | `Horizontal` | `Horizontal` puts the label beside the content, `Vertical` above it |
| `LabelWidthNumber` | `int` | `120` | Width of the label column |
| `LabelWidthSizeDefinition` | `SizeDefinitionType` | `Pixel` | Unit for `LabelWidthNumber`: `None`, `Pixel`, `Percentage`, `Auto` |
| `HideAreas` | `LabelControlHideAreasType` | `None` | Areas to hide: `None`, `Asterisk`, `Information`, `Validation` or combinations (`All`, ...) |
| `ContentMinHeight` | `double` | `26` | Minimum height of the content area |
| `InformationText` | `string` | `""` | Text shown in the information icon tooltip |
| `InformationContent` | `object?` | `null` | Custom content shown in the information icon tooltip |
| `InformationColor` | `Color` | `DodgerBlue` | Color of the information icon |
| `IsMandatory` | `bool` | `false` | Marks the field as mandatory |
| `ShowAsteriskOnMandatory` | `bool` | `true` | Show the mandatory indicator when `IsMandatory` is `true` |
| `MandatoryColor` | `SolidColorBrush` | `Red` | Color of the mandatory indicator |
| `ValidationText` | `string` | `""` | Validation message shown below the content |
| `ValidationColor` | `SolidColorBrush` | theme validation brush | Color of the validation message (`AtcApps.Brushes.Control.Validation`, falling back to red) |
| `GroupIdentifier` | `string?` | `null` | Optional group name used by `GetFullIdentifier()` |
| `InputDataType` | `Type?` | `null` | Optional data type of the input |

## 📝 Notes

- The information icon is only shown when `InformationText` or `InformationContent` is set and `HideAreas` does not include `Information`
- The control has a `MinWidth` of `200`
- `Identifier` (read-only) is derived from `LabelText`; `GetFullIdentifier()` prefixes it with `GroupIdentifier` when set
- `IsValid()` always returns `true` for `LabelContent`; set `ValidationText` yourself to show a validation message

## 🔗 Related Controls

- **LabelTextBox**, **LabelComboBox**, **LabelCheckBox** - Ready-made labeled inputs built on the same layout
- **LabelTextInfo** - Labeled read-only text
- **LabelInputFormPanel** - Builds a full form of labeled controls

## 🎮 Sample Application

See the LabelContent sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelContent** for interactive examples.
