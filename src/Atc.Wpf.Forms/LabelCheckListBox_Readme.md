# 🏷️ LabelCheckListBox

A labeled form field around `CheckListBox`: an always-visible check list with a label, mandatory marker, information text and validation.

## 🔍 Overview

`LabelCheckListBox` puts a `CheckListBox` in the standard `LabelContent` layout used by the other `Label*` controls. `IsMandatory` means at least one key must be ticked. It implements `ILabelCheckListBox`, so it works in `LabelControlsForm` (`GetKeyValues()` returns the ticked keys as a list, which `LabelControlsFormToModelWriter` writes back to a `List<string>` / `IList<string>` property) and can be created with `LabelControlFactory.CreateLabelCheckListBox(...)`.

Set `InputDataType` to a `[Flags]` enum type to have `GetKeyValues()` return the ticked keys as one combined flags value instead of a list. See the type mapping in [Readme.md](Readme.md#forms-generated-from-a-model).

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelCheckListBox
    InformationText="Tick at least one day."
    IsMandatory="True"
    Items="{Binding Path=Weekdays}"
    LabelText="Work days"
    SelectedKeys="{Binding Path=SelectedWorkdays}" />
```

```xml
<atc:LabelCheckListBox
    ItemStyle="Switch"
    Items="{Binding Path=Notifications}"
    LabelText="Notifications"
    MaxSelectionCount="2"
    Orientation="Vertical"
    ShowSelectAllRow="False" />
```

```csharp
var control = LabelControlFactory.CreateLabelCheckListBox(
    groupIdentifier: "Export",
    labelText: "Columns",
    isReadOnly: false,
    isMandatory: true,
    items: columns,
    selectedKeys: ["id", "name"]);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, `LabelPosition`, `LabelWidthNumber`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | Key → display value |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys; two-way by default, updated in place |
| `SelectNewItems` | `bool` | `false` | Tick keys that appear in `Items`, up to `MaxSelectionCount` |
| `ShowSelectAllRow` | `bool` | `true` | The tri-state Select all row with the count |
| `ShowClearButton` | `bool` | `false` | A Clear button in the Select all row |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = more than 10 items |
| `ItemGroups` | `Dictionary<string, string>?` | `null` | Key → group name |
| `IsGroupCheckable` | `bool` | `true` | Tri-state check boxes on group headers |
| `MaxSelectionCount` | `int?` | `null` | Selection limit. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. Counts the distinct keys that are in `Items` (a stale key neither counts nor satisfies `IsMandatory`); re-validated when the keys are set in code or `SelectedKeys` changes in place. |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Shown but cannot be toggled |
| `ItemStyle` | `CheckListItemStyle` | `CheckBox` | `CheckBox`, `Highlight` or `Switch` |
| `Columns` | `int` | `1` | Lay items out in N columns |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a row |
| `MaxListHeight` | `double` | `250` | The list scrolls (virtualized) beyond this height |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The ticked keys changed through the list and the field is valid |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The ticked keys changed through the list and the field is invalid (mandatory with nothing ticked) |

## 📝 Notes

- `IsValid()` validates now and sets `ValidationText` ("Field is required" when mandatory and nothing is ticked).
- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or use `Mode=OneWay`.
- All list behaviour (Select all, filter, groups, `Shift+click`, keyboard, UI Automation) comes from `CheckListBox`; see `CheckListBox_Readme.md`.

## 🔗 Related Controls

- **CheckListBox** - The list without the label
- **LabelComboBox** - Single selection by key
- **MultiSelectComboBox** - The same list in a drop-down, for little space
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelCheckListBox**.
