# 🖍️ LabelWellKnownColorSelector

A labeled drop-down of named colors with color indicator, mandatory indicator and validation display.

## 🔍 Overview

`LabelWellKnownColorSelector` puts a `WellKnownColorSelector` in the standard `LabelContent` layout used by the other `Label*` controls. The selected color is exposed as its name in `SelectedKey` (for example `Red`). When no `LabelText` is set, the label defaults to the localized "Color" text and follows UI-culture changes. It implements `ILabelWellKnownColorSelector` and can be created with `LabelControlFactory.CreateLabelWellKnownColorSelector(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelWellKnownColorSelector SelectedKey="{Binding ColorName}" />
```

### Hex Code and Custom Label

```xml
<atc:LabelWellKnownColorSelector
    LabelText="MyColor"
    SelectedKey="Red"
    ShowHexCode="True" />
```

### Mandatory, Basic Colors Only

```xml
<atc:LabelWellKnownColorSelector
    DropDownFirstItemType="PleaseSelect"
    IsMandatory="True"
    UseOnlyBasicColors="True" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelWellKnownColorSelector(
    groupIdentifier: "Style",
    labelText: "BorderColor",
    isReadOnly: false,
    isMandatory: true,
    inputDataType: typeof(string),
    defaultColorName: "Blue");
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Name of the selected color. Binds two-way by default and updates the source on lost focus |
| `DefaultColorName` | `string` | `""` | Color selected on load when `SelectedKey` is empty |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | Optional first item, such as `PleaseSelect` |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator (`None`, `Circle`, `Square`) |
| `ShowHexCode` | `bool` | `false` | Show the hex code next to the color name (the bare selector defaults to `true`) |
| `UseOnlyBasicColors` | `bool` | `false` | Limit the list to the basic colors |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is valid |
| `SelectorLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is invalid |

## 📝 Notes

- The first time `SelectedKey` gets a value after creation, no event is raised (it is treated as the initial value); later changes to a new key raise the events
- When mandatory, an empty key shows "Field is required" and the "Please select" item shows "Please select a color"
- `IsValid()` validates `SelectedKey`, or the inner selector's key when `SelectedKey` is empty, and does not raise the events
- A custom `LabelText` is kept on culture change; only the default translated text is replaced
- The factory sets `DropDownFirstItemType` to `PleaseSelect`

## 🔗 Related Controls

- **WellKnownColorSelector** - The color drop-down without the label
- **WellKnownColorPicker** - Grid of named colors to pick from
- **LabelColorPicker** - Labeled free color picker
- **LabelAccentColorSelector** - Labeled accent color selector for the theme

## 🎮 Sample Application

See the LabelWellKnownColorSelector sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Selectors > LabelWellKnownColorSelector** for interactive examples.
