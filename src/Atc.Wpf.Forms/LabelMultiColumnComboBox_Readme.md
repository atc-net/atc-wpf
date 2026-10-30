# 🏷️ LabelMultiColumnComboBox

A labeled multi-column multi-select drop-down (`MultiColumnComboBox`): a drop-down whose pop-up is a sortable grid with a check-box column, with a mandatory marker and validation text.

## 🔍 Overview

`LabelMultiColumnComboBox` is the form field for picking several objects by comparing their attributes (customer number + name + city). It has the label layout of `LabelComboBox` (label, information text, validation text, horizontal or vertical) and passes the grid, filter and summary options to the inner `MultiColumnComboBox`.

- `IsMandatory`: at least one key must be ticked
- `MaxSelectionCount`: keys set from code beyond the limit are kept, but `IsValid()` is `false`
- `SelectedKeysChanged` for a valid change, `SelectedKeysChangedInvalid` for a change that fails validation
- `Columns` is the inner drop-down's own collection: the columns move into its pop-up grid
- supported by `LabelControlsForm` (the ticked keys are the value) and `LabelControlFactory.CreateLabelMultiColumnComboBox` (auto-generated columns)

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelMultiColumnComboBox
    DisplayMemberPath="Name"
    IsMandatory="True"
    ItemsSource="{Binding Path=Customers}"
    KeyMemberPath="Number"
    LabelText="Customers"
    SelectedKeys="{Binding Path=CustomerNumbers}"
    WatermarkText="Pick customers">
    <atc:LabelMultiColumnComboBox.Columns>
        <DataGridTextColumn Binding="{Binding Path=Number}" Header="No." />
        <DataGridTextColumn Binding="{Binding Path=Name}" Header="Name" />
        <DataGridTextColumn Binding="{Binding Path=City}" Header="City" />
    </atc:LabelMultiColumnComboBox.Columns>
</atc:LabelMultiColumnComboBox>
```

```csharp
var field = LabelControlFactory.CreateLabelMultiColumnComboBox(
    groupIdentifier: "Sales",
    labelText: "Customers",
    isReadOnly: false,
    isMandatory: true,
    itemsSource: customers,
    keyMemberPath: "Number",
    displayMemberPath: "Name",
    selectedKeys: ["1002"]);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | The objects shown as rows. |
| `KeyMemberPath` | `string` | `""` | The (dotted) property path of the key. |
| `DisplayMemberPath` | `string` | `""` | The (dotted) property path of the name. |
| `Columns` | `ObservableCollection<DataGridColumn>` | empty | The grid columns after the check-box column (get-only; XAML content). |
| `AutoGenerateColumns` | `bool` | `false` | One text column per public property when `Columns` is empty. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys (two-way). |
| `MaxSelectionCount` | `int?` | `null` | The most keys; beyond it `IsValid()` is `false`. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. Counts the distinct keys; re-validated when the keys are set in code or `SelectedKeys` changes in place. |
| `IsEnabledMemberPath` | `string` | `""` | The path of a `bool` property; `false` rows cannot be toggled. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 10 items. |
| `FilterText` | `string` | `""` | The filter text (two-way). |
| `FilterMemberPaths` | `string?` | `null` | Comma-separated paths to search instead of the visible columns. |
| `KeepSelectedOnTop` | `bool` | `false` | Ticked rows come first when the pop-up opens. |
| `DropDownWidth` | `double` | `NaN` | The pop-up width; `NaN` fits the columns. |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The grid scrolls beyond it. |
| `CommitMode` | `MultiSelectCommitMode` | `Immediate` | `Deferred`: OK / Cancel in the pop-up. |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` / `ChipsWrap` / `ChipTemplate` | | | As on `MultiColumnComboBox`. |
| `MaxNamesInSummary` / `AllText` / `NoneText` / `CountFormat` | | | The summary texts, as on `MultiColumnComboBox`. |
| `WatermarkText` | `string` | `""` | Shown in the closed box when nothing is ticked. |

## 📡 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | A change through the pop-up that passes validation. |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | A change through the pop-up that fails validation. |

## 📝 Notes

- A `DataGridColumn` can belong to one grid only: do not share a column instance between fields.
- The identifier in the event args is the control's `Identifier`.

## 🔗 Related Controls

- **MultiColumnComboBox** - The drop-down inside this field
- **LabelMultiSelectComboBox** - A flat check list in a drop-down
- **LabelCheckListBox** - An always-visible check list
- **LabelTokenBox** - Large or open-ended sets, typing is the main way in
- **LabelTreeSelectComboBox** / **LabelCascaderComboBox** - Hierarchies
- **LabelChipGroup** - 2–15 short options as toggle chips
- **CardSelector**, **Segmented**, **DualListSelector** - Visual choices, very short options, ordered lists

## 🎮 Sample Application

See the sample application under **Wpf.Forms > LabelMultiColumnComboBox** for an interactive field with an event log (label, orientation and mandatory in the side panel), auto-generated columns, a mandatory field, chips, and a mandatory field with at most three customers.
