# 🏷️ LabelMultiSelectComboBox

A labeled form field around `MultiSelectComboBox`: a multi-select drop-down with a label, mandatory marker, information text and validation.

## 🔍 Overview

`LabelMultiSelectComboBox` puts a `MultiSelectComboBox` in the standard `LabelContent` layout used by the other `Label*` controls, at the same height as `LabelComboBox`. `IsMandatory` means at least one key must be ticked. It implements `ILabelMultiSelectComboBox`, so it works in `LabelControlsForm` (`GetKeyValues()` returns the ticked keys as a list, which `LabelControlsFormToModelWriter` writes back to a `List<string>` / `IList<string>` property) and can be created with `LabelControlFactory.CreateLabelMultiSelectComboBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelMultiSelectComboBox
    InformationText="Tick at least one day."
    IsMandatory="True"
    Items="{Binding Path=Weekdays}"
    LabelText="Work days"
    SelectedKeys="{Binding Path=SelectedWorkdays}"
    WatermarkText="Pick work days" />
```

```xml
<atc:LabelMultiSelectComboBox
    AllText="Every day"
    Items="{Binding Path=Weekdays}"
    LabelText="Meeting days"
    MaxSelectionCount="3"
    NoneText="No meetings"
    Orientation="Vertical" />
```

```csharp
var control = LabelControlFactory.CreateLabelMultiSelectComboBox(
    groupIdentifier: "Filter",
    labelText: "SourceSystems",
    isReadOnly: false,
    isMandatory: true,
    items: sourceSystems,
    selectedKeys: ["orders"]);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, `LabelPosition`, `LabelWidthNumber`, `ContentMinHeight`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | Key → display value |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys; two-way by default, updated in place |
| `SelectNewItems` | `bool` | `false` | Tick keys that appear in `Items`, up to `MaxSelectionCount` |
| `WatermarkText` | `string` | `""` | Shown in the closed box when nothing is ticked |
| `MaxNamesInSummary` | `int` | `2` | The most names the summary lists before it shows the count |
| `AllText` | `string?` | `null` | Summary when all are ticked; `null` = the translated "All" |
| `NoneText` | `string` | `""` | Summary when nothing is ticked (watermark style) |
| `CountFormat` | `string?` | `null` | Count format; `null` = the translated "{0} of {1} selected" |
| `ShowSelectAllRow` | `bool` | `true` | The tri-state Select all row with the count |
| `ShowClearButton` | `bool` | `true` | A Clear button in the Select all row |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = more than 10 items |
| `FilterText` | `string` | `""` | The filter text (two-way) |
| `ItemGroups` | `Dictionary<string, string>?` | `null` | Key → group name |
| `IsGroupCheckable` | `bool` | `true` | Tri-state check boxes on group headers |
| `MaxSelectionCount` | `int?` | `null` | Selection limit |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Shown but cannot be toggled |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a pop-up row |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` (removable chips with "+N") or `Count` |
| `MaxVisibleChips` | `int?` | `null` | Chips mode: `null` = as many as fit, then "+N" |
| `ChipsWrap` | `bool` | `false` | Chips mode: wrap the chips over several lines |
| `ChipTemplate` | `DataTemplate?` | `null` | Chips mode: a custom chip (data context: `MultiSelectChipItem`) |
| `CommitMode` | `MultiSelectCommitMode` | `Immediate` | `Deferred` shows OK / Cancel; ticks apply on OK |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The ticked keys changed through the pop-up (or an `Items` change) and the field is valid |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The ticked keys changed through the pop-up and the field is invalid (mandatory with nothing ticked, or more keys than `MaxSelectionCount`) |

## 📝 Notes

- `IsValid()` validates now and sets `ValidationText`: "Field is required" when mandatory and nothing is ticked, "Select at most {0}" (translated) when `SelectedKeys` set from code holds more keys of `Items` than `MaxSelectionCount` (the keys are kept; the pop-up itself never ticks beyond the limit).
- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or use `Mode=OneWay`.
- The summary, keyboard and UI Automation behaviour comes from `MultiSelectComboBox`; see `MultiSelectComboBox_Readme.md`.

## 🔗 Related Controls

- **MultiSelectComboBox** - The drop-down without the label
- **LabelComboBox** - Single selection by key
- **LabelCheckListBox** - The same list, always visible
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool (two lists with transfer buttons)
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelMultiSelectComboBox**, including mandatory fields.