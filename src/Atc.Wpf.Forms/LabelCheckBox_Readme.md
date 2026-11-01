# ☑️ LabelCheckBox

A labeled check box form field with an information popup.

## 🔍 Overview

`LabelCheckBox` combines a label area (label text and information popup) with a WPF `CheckBox`. It derives from `LabelControlBase`, not `LabelControl`, so it has no mandatory marker and no validation message. The label can sit to the left of the check box (the default) or to the right. It implements `ILabelCheckBox` and can be created with `LabelControlFactory.CreateLabelCheckBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelCheckBox LabelText="Remember me" IsChecked="{Binding RememberMe}" />
```

### Label to the Right

```xml
<atc:LabelCheckBox
    LabelPosition="Right"
    LabelText="I accept the terms"
    Orientation="Horizontal" />
```

### Vertical Layout with Help Text

```xml
<atc:LabelCheckBox
    InformationText="This is a help text.."
    LabelText="MyLabel"
    Orientation="Vertical" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelCheckBox(
    groupIdentifier: "Settings",
    labelText: "EnableLogging",
    isReadOnly: false,
    value: true);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsChecked` | `bool` | `false` | Whether the box is checked. Binds two-way by default |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `IsCheckedChanged` | `EventHandler<ValueChangedEventArgs<bool>>` | Raised when `IsChecked` changes, with the old and new value and the control's `Identifier` |

## 📝 Notes

- Setting `LabelPosition` also sets the label width in pixels: `25` for `Right` and `120` otherwise
- `IsChecked` is a plain `bool`; there is no indeterminate (three-state) value
- The factory passes `isReadOnly` on as `IsEnabled = !isReadOnly`, and a `null` value as unchecked

## 🔗 Related Controls

- **LabelToggleSwitch** - Labeled on/off switch
- **LabelCheckListBox** - Labeled list of check boxes for multiple selection

## 🎮 Sample Application

See the LabelCheckBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelCheckBox** for interactive examples.
