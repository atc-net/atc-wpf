# 🏷️ LabelTreeSelectComboBox

A labeled drop-down with a **tri-state check tree** (`TreeSelectComboBox`), with a mandatory marker and validation text.

## 🔍 Overview

`LabelTreeSelectComboBox` is the form field for picking from a hierarchy (regions, permissions, folders). It has the label layout of `LabelComboBox` (label, information text, validation text, horizontal or vertical) and passes the tree and summary options to the inner `TreeSelectComboBox`.

- `IsMandatory`: at least one key must be ticked
- `MaxSelectionCount`: keys set from code beyond the limit are kept, but `IsValid()` is `false`
- `SelectedKeysChanged` for a valid change, `SelectedKeysChangedInvalid` for a change that fails validation
- supported by `LabelControlsForm` (the ticked keys are the value) and `LabelControlFactory.CreateLabelTreeSelectComboBox`

For an always-visible tree in a form, put a `CheckTreeView` inside a `LabelContent`; no separate wrapper is needed.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelTreeSelectComboBox
    CheckedKeysStrategy="Parents"
    IsMandatory="True"
    LabelText="Sales regions"
    Nodes="{Binding Path=Regions}"
    SelectedKeys="{Binding Path=RegionKeys}"
    WatermarkText="Pick regions" />
```

```csharp
var field = LabelControlFactory.CreateLabelTreeSelectComboBox(
    groupIdentifier: "Sales",
    labelText: "Regions",
    isReadOnly: false,
    isMandatory: true,
    nodes: regions,
    selectedKeys: ["dk"],
    checkedKeysStrategy: CheckedKeysStrategy.Parents);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `ObservableCollection<CheckTreeNode>` | empty | The root nodes. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys (two-way). |
| `CheckedKeysStrategy` | `CheckedKeysStrategy` | `Leaves` | `Leaves`, `Parents` or `All`. |
| `IsCheckLinked` | `bool` | `true` | `false`: every node is independent. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 20 nodes. |
| `FilterText` | `string` | `""` | The filter text (two-way). |
| `FilterMode` | `TreeFilterMode` | `KeepAncestors` | `KeepAncestors` or `FlatResults`. |
| `ShowSelectAllRow` | `bool` | `false` | Select all / count / Clear in the pop-up. |
| `MaxSelectionCount` | `int?` | `null` | The most keys; beyond it `IsValid()` is `false`. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. Counts the distinct keys; re-validated when the keys are set in code or `SelectedKeys` changes in place. |
| `LoadChildrenAsync` | `Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?` | `null` | Lazy loading of children. |
| `ExpandDepth` | `int` | `1` | Levels expanded when `Nodes` is set. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a pop-up row. |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` / `ChipsWrap` / `ChipTemplate` | | | Chips options, as `TreeSelectComboBox`. |
| `MaxNamesInSummary` / `AllText` / `NoneText` / `CountFormat` | | | Summary texts, as `TreeSelectComboBox`. |
| `WatermarkText` | `string` | `""` | The watermark when nothing is ticked. |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height. |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The keys changed through the pop-up and are valid. |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The keys changed through the pop-up and fail validation. |

## 📝 Notes

- A node belongs to one tree: give each control its own nodes.
- The validation messages ("Field is required", "Select at most {0}") come from the Forms `Validations` resources.

## 🔗 Related Controls

- **TreeSelectComboBox** - The inner drop-down
- **CheckTreeView** - The same tree, always visible (wrap it in `LabelContent`)
- **LabelMultiSelectComboBox** - A flat multi-select drop-down
- **LabelCheckListBox** - A flat, always-visible check list
- **LabelTokenBox** - Large or open-ended sets, typing is the main way in
- **LabelChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelTreeSelectComboBox** for the interactive field with an event log (mandatory, label and orientation in the side panel), mandatory permissions with the `Parents` strategy, chips, a vertical layout, a limit of 3 sites with a count summary, and a `CheckTreeView` inside a `LabelContent`.
