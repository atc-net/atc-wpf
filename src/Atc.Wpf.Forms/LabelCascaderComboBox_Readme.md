# 🏷️ LabelCascaderComboBox

A labeled multi-select **cascader** (`CascaderComboBox`): a drop-down with one column per hierarchy level and tri-state check boxes, with a mandatory marker and validation text.

## 🔍 Overview

`LabelCascaderComboBox` is the form field for picking from a deep, uniform hierarchy (country › region › city, category › sub-category › product). It has the label layout of `LabelComboBox` (label, information text, validation text, horizontal or vertical) and passes the column, filter and summary options to the inner `CascaderComboBox`.

- `IsMandatory`: at least one key must be ticked
- `MaxSelectionCount`: keys set from code beyond the limit are kept, but `IsValid()` is `false`
- `SelectedKeysChanged` for a valid change, `SelectedKeysChangedInvalid` for a change that fails validation
- ⚠️ `CheckedKeysStrategy` defaults to **`Parents`**, as in `CascaderComboBox`, while `LabelTreeSelectComboBox` defaults to **`Leaves`**; set it explicitly when the two must agree
- supported by `LabelControlsForm` (the ticked keys are the value) and `LabelControlFactory.CreateLabelCascaderComboBox`

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelCascaderComboBox
    DisplayMode="Chips"
    IsMandatory="True"
    LabelText="Sales regions"
    Nodes="{Binding Path=Regions}"
    SelectedKeys="{Binding Path=RegionKeys}"
    ShowFullPath="True"
    WatermarkText="Pick regions" />
```

```csharp
var field = LabelControlFactory.CreateLabelCascaderComboBox(
    groupIdentifier: "Sales",
    labelText: "Regions",
    isReadOnly: false,
    isMandatory: true,
    nodes: regions,
    selectedKeys: ["dk-jut"]);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `ObservableCollection<CheckTreeNode>` | empty | The roots (the first column). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys (two-way). |
| `CheckedKeysStrategy` | `CheckedKeysStrategy` | `Parents` | `Parents`, `Leaves` or `All`. |
| `IsCheckLinked` | `bool` | `true` | `false`: every node is independent. |
| `ExpandTrigger` | `CascaderExpandTrigger` | `Click` | `Click` or `Hover`. |
| `ColumnWidth` | `double` | `180` | The width of each column. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 20 nodes. |
| `FilterText` | `string` | `""` | The filter text (two-way). |
| `MaxSelectionCount` | `int?` | `null` | The most keys; beyond it `IsValid()` is `false`. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. Counts the distinct keys; re-validated when the keys are set in code or `SelectedKeys` changes in place. |
| `LoadChildrenAsync` | `Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?` | `null` | Lazy loading of children when a row becomes active. |
| `PathSeparator` | `string` | `" / "` | Between the texts of a path. |
| `ShowFullPath` | `bool` | `false` | Summary names and chips show the full path. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a pop-up row. |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` / `ChipsWrap` / `ChipTemplate` | | | Chips options, as `CascaderComboBox`. |
| `MaxNamesInSummary` / `AllText` / `NoneText` / `CountFormat` | | | Summary texts, as `CascaderComboBox`. |
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

- **CascaderComboBox** - The inner drop-down
- **LabelTreeSelectComboBox** - The same hierarchy as an indented tree (better for wide, shallow hierarchies)
- **CheckTreeView** - The indented tree, always visible (wrap it in `LabelContent`)
- **LabelMultiSelectComboBox** - A flat multi-select drop-down
- **MultiColumnComboBox** - A drop-down with a check-box grid, for comparing attributes
- **LabelCheckListBox** - A flat, always-visible check list
- **LabelTokenBox** - Large or open-ended sets, typing is the main way in
- **LabelChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelCascaderComboBox** for the interactive field with an event log (mandatory, label and orientation in the side panel), a mandatory field, chips with full paths, a vertical product picker, and a limit of 3 cities with a count summary.
