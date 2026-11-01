# 🔽 LabelComboBox

A labeled drop-down form field for picking one item by key, with mandatory validation.

## 🔍 Overview

`LabelComboBox` combines a label area (label text, mandatory asterisk, information popup and validation message) with a WPF `ComboBox`. The items are a `Dictionary<string, string>` of key → display value, and the selection is exposed as `SelectedKey`. When `IsMandatory` is set, an empty key or a "please select" / blank first item counts as no selection. It implements `ILabelComboBox`, and `LabelControlFactory.CreateLabelEnumPicker(...)` creates one that lists the values of an enum type.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelComboBox
    Items="{Binding Countries}"
    LabelText="Country"
    SelectedKey="{Binding SelectedCountryKey}" />
```

### Items from an Enum

```xml
<atc:LabelComboBox
    InformationText="This is a help text..."
    IsMandatory="True"
    Items="{Binding Source={atc:EnumToKeyValueBindingSource {x:Type wpf:ControlSizeType}, KeyAsString=True, IncludeDefault=True}}"
    LabelText="Size"
    Orientation="Vertical" />
```

### Factory (Enum Picker)

```csharp
var control = LabelControlFactory.CreateLabelEnumPicker(
    groupIdentifier: "Settings",
    labelText: "ControlSize",
    isReadOnly: false,
    isMandatory: true,
    selectedKey: null,
    inputDataType: typeof(ControlSizeType));
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Items` | `Dictionary<string, string>` | `null` | Key → display value |
| `SelectedKey` | `string` | `""` | Key of the selected item. Binds two-way by default and updates the source when focus is lost |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectorChanged` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is valid, with the old and new key |
| `SelectorLostFocusInvalid` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when `SelectedKey` changes and the field is invalid (mandatory with no selection) |

## 📝 Notes

- `IsValid()` validates now and sets `ValidationText` ("Field is required" when mandatory and nothing real is selected)
- The validation message is updated as soon as the user picks an item, before `SelectedKey` is committed
- The enum picker adds a "please select" first item and skips enum values named `None`, `Unknown` and `Default`
- `ContentMinHeight` sets the minimum height of the drop-down

## 🔗 Related Controls

- **LabelMultiSelectComboBox** - Labeled drop-down with multiple selection
- **LabelMultiColumnComboBox** - Labeled multi-select drop-down with a sortable grid
- **LabelCascaderComboBox** - Labeled drop-down for a deep, uniform hierarchy
- **LabelTreeSelectComboBox** - Labeled drop-down with a tri-state check tree
- **LabelCheckListBox** - Labeled always-visible check list

## 🎮 Sample Application

See the LabelComboBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelComboBox** for interactive examples.
