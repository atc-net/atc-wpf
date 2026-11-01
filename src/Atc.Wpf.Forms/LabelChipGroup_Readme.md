# 🏷️ LabelChipGroup

A labeled group of toggle chips (`ChipGroup`) with a mandatory marker, information text and validation.

## 🔍 Overview

`LabelChipGroup` wraps `ChipGroup` in the standard label layout of the form controls (`LabelText`, `InformationText`, `ValidationText`, label `Orientation`, `IsMandatory`). Use it for quick choices in a form: work days, a status, a size, log levels.

- `IsMandatory` means at least one key is selected.
- With `MaxSelectionCount`, more keys than the limit (e.g. set from outside) make `IsValid()` false ("Select at most 2").
- Supported by `LabelControlsForm` (`GetKeyValues` returns the selected keys) and `LabelControlFactory.CreateLabelChipGroup`.

Set `InputDataType` to a `[Flags]` enum type to have `GetKeyValues()` return the selected keys as one combined flags value instead of a list. See the type mapping in [Readme.md](Readme.md#forms-generated-from-a-model).

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## 🚀 Usage

```xml
<atc:LabelChipGroup
    IsMandatory="True"
    Items="{Binding Path=WorkDays}"
    LabelText="Work days"
    SelectedKeys="{Binding Path=SelectedWorkDays}"
    ShowAllChip="True" />

<atc:LabelChipGroup
    ChipVariant="Outlined"
    Items="{Binding Path=Statuses}"
    LabelText="Status"
    SelectedKeys="{Binding Path=SelectedStatus}"
    SelectionMode="Single" />
```

```csharp
var control = LabelControlFactory.CreateLabelChipGroup(
    groupIdentifier: "Filter",
    labelText: "LogLevels",
    isReadOnly: false,
    isMandatory: true,
    items: logLevels,
    selectedKeys: ["error"]);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation` of the label, …):

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Items` | `Dictionary<string, string>` | empty | Key → display value. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The selected keys. Two-way by default; updated in place. |
| `SelectionMode` | `ChipSelectionMode` | `Multiple` | `Multiple`, `Single` or `SingleOrNone`. |
| `SelectNewItems` | `bool` | `false` | New keys in `Items` are selected (`Multiple` only), up to `MaxSelectionCount`. |
| `ChipVariant` | `ChipGroupVariant` | `Filled` | `Filled` or `Outlined`. |
| `ChipSize` | `ChipSize` | `Medium` | The chip size. |
| `ShowCheckMark` | `bool` | `true` | A ✓ on selected chips. |
| `ChipOrientation` | `Orientation` | `Horizontal` | The direction of the chips (`Orientation` is the label position). |
| `Wrap` | `bool` | `true` | `false` = one line that scrolls. |
| `ItemSpacing` | `double` | `4` | Space between chips. |
| `ShowAllChip` | `bool` | `false` | A leading **All** chip. |
| `MaxSelectionCount` | `int?` | `null` | The most keys that can be selected. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. Counts the distinct keys that are in `Items` (a stale key neither counts nor satisfies `IsMandatory`); re-validated when the keys are set in code or `SelectedKeys` changes in place. |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Keys shown but not toggleable. |
| `ItemIcons` | `Dictionary<string, object>?` | `null` | Optional icon per key. |
| `ItemCounts` | `Dictionary<string, int>?` | `null` | Optional count per key. |

## 📡 Events

| Event | Args | When |
|-------|------|------|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The chips changed the selection and it is valid; `Identifier` is the control's identifier. |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The chips changed the selection and it fails validation (nothing selected while mandatory). |

## 📝 Notes

- See [ChipGroup_Readme.md](../Atc.Wpf.Controls/DataDisplay/ChipGroup_Readme.md) for keyboard, UI Automation and selection details.
- For more than about 15 options use `LabelMultiSelectComboBox` or `LabelCheckListBox`.

## 🔗 Related Controls

- **ChipGroup** - The chips without the label
- **LabelCheckListBox** - Always-visible check list for 5–1,000 items
- **LabelMultiSelectComboBox** - Drop-down with check boxes, for little space
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelChipGroup**.
