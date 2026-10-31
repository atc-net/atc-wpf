# 🔽 TreeSelectComboBox

A drop-down with a **tri-state check tree**. Closed, it looks like a `ComboBox` and shows a summary of the ticked nodes; open, it shows a `CheckTreeView`.

## 🔍 Overview

`TreeSelectComboBox` is the hierarchical multi-select field for little space: "Sales regions" in a filter bar, "Folders to back up" in a form row. It hosts a `CheckTreeView` in its pop-up, so the tree behaviour (tri-state propagation, `CheckedKeysStrategy`, filter, lazy loading, limit, virtualization, keyboard) is the same.

- closed: the themed `ComboBox` look and height, with a summary in one of three display modes (`MultiSelectDisplayMode`, shared with `MultiSelectComboBox`): names, removable chips with "+N", or a count
- open: ticking a node does **not** close the pop-up
- one `SelectedKeysChanged` per action (a parent with all its children is one tick)
- ⚠️ `CheckedKeysStrategy` defaults to **`Leaves`** (like `CheckTreeView`), while `CascaderComboBox` defaults to **`Parents`**: a tree drop-down usually feeds a query by its leaves (cities, folders), a cascader usually picks whole branches by path. Set `CheckedKeysStrategy` explicitly when the two must agree.
- UI Automation: a combo box with `ExpandCollapse` and the summary as its `Value`; while open, the tree rows are tree items with `ExpandCollapse` and a three-state `Toggle`

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Basic

```xml
<atc:TreeSelectComboBox
    Width="260"
    atc:TextBoxHelper.Watermark="Sales regions"
    Nodes="{Binding Path=Regions}"
    SelectedKeys="{Binding Path=RegionKeys}" />
```

```csharp
public ObservableCollection<CheckTreeNode> Regions { get; } = CreateRegions();

// SelectedKeys binds two-way by default, so the property needs a setter.
public ObservableCollection<string> RegionKeys { get; set; } = [];
```

### Chips with whole branches

```xml
<atc:TreeSelectComboBox
    Width="320"
    CheckedKeysStrategy="Parents"
    DisplayMode="Chips"
    Nodes="{Binding Path=Regions}"
    SelectedKeys="{Binding Path=RegionKeys}" />
```

With `Parents`, ticking all of Denmark gives one chip, "Denmark". Removing a chip unticks its node (with everything under it) without opening the pop-up.

### Summary

| Ticked | Summary | Property |
|---|---|---|
| none | `NoneText` in the watermark style; when empty, `TextBoxHelper.Watermark` | `NoneText` |
| every leaf | "All" (translated) | `AllText` |
| 1 … `MaxNamesInSummary` keys | the node texts of `SelectedKeys`: "Copenhagen, Stockholm" (or "Denmark" with `Parents`) | `MaxNamesInSummary` |
| more | "3 of 12 selected": ticked leaves of all leaves (translated) | `CountFormat` (`{0}` = ticked leaves, `{1}` = leaves) |

`DisplayMode="Count"` always shows the count; `DisplayMode="Chips"` shows a chip per key in `SelectedKeys`.

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `ObservableCollection<CheckTreeNode>` | empty | The root nodes. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys. Two-way by default; updated in place. |
| `CheckedKeysStrategy` | `CheckedKeysStrategy` | `Leaves` | `Leaves`, `Parents` or `All` (see `CheckTreeView`). Note: `CascaderComboBox` defaults to `Parents`. |
| `IsCheckLinked` | `bool` | `true` | `false`: every node is independent. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 20 nodes. |
| `FilterText` | `string` | `""` | The filter text (two-way). |
| `FilterMode` | `TreeFilterMode` | `KeepAncestors` | `KeepAncestors` or `FlatResults`. |
| `ShowSelectAllRow` | `bool` | `false` | **Select all** / count / **Clear** above the tree. |
| `MaxSelectionCount` | `int?` | `null` | The most keys; a tick beyond it is refused. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. |
| `LoadChildrenAsync` | `Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?` | `null` | Lazy loading of children on the first expand. |
| `ExpandDepth` | `int` | `1` | Levels expanded when `Nodes` is set. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a pop-up row (data context: `CheckTreeNode`). |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` | `int?` | `null` | `Chips` only: `null` = as many as fit, then "+N". |
| `ChipsWrap` | `bool` | `false` | `Chips` only: wrap over several lines (no "+N"). |
| `ChipTemplate` | `DataTemplate?` | `null` | `Chips` only (data context: `MultiSelectChipItem`). |
| `MaxNamesInSummary` | `int` | `2` | The most names before the count. |
| `AllText` | `string?` | `null` | The summary when every leaf is ticked. |
| `NoneText` | `string` | `""` | The summary when nothing is ticked. |
| `CountFormat` | `string?` | `null` | The count format; `null` = "{0} of {1} selected". |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height; the tree scrolls (virtualized) beyond it. |
| `IsDropDownOpen` | `bool` | `false` | Whether the pop-up is open (two-way). |
| `SummaryText` | `string` | `""` | Read-only: the closed-box text (also the UI Automation value). |

## 🔧 Methods

| Method | Description |
|---|---|
| `FindNode(string key)` | Returns the node with a key, or `null`. |
| `ToggleNode(CheckTreeNode node)` | Toggles a node as a click does (one event). |
| `SelectAll()` / `ClearSelection()` | One event each. |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per tick, Select all, Clear, chip removal, or loaded children that change the keys. |
| `DropDownOpened` | `EventArgs` | The pop-up opened. |
| `DropDownClosed` | `EventArgs` | The pop-up closed. |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `Alt+Down` / `Alt+Up` / `F4` | Open or close the pop-up |
| `Esc` | Clear the filter text if there is any, otherwise close the pop-up |
| `Enter` | Close the pop-up |
| `Backspace` | `Chips` mode, closed box: remove the last chip |
| `↑` `↓` `→` `←` `Space` `*` | Tree navigation, toggle, expand all under the row (see `CheckTreeView`) |

When the pop-up opens, focus goes to the filter box (if shown) or the first row.

## 📝 Notes

- Bulk changes: the default `SelectedKeys` is an `ObservableCollectionEx<string>`, so Select all and Clear raise one `Reset`; a view-model `ObservableCollection<string>` gets one notification per key, but the control still updates its states and summary once per action. For a bulk change from code, assign a new collection or use `ObservableCollectionEx<string>.AddRange` (one notification): adding keys one by one updates the control once per key.
- A node belongs to one tree: give each control its own nodes.
- There is no deferred (OK / Cancel) commit mode; ticks apply right away.
- The drop-down shell (pop-up, mouse capture, keyboard, automation peer) is kept in its own files so a single-select tree drop-down can reuse it.

## 🔗 Related Controls

- **CheckTreeView** - The same tree, always visible
- **[CascaderComboBox](CascaderComboBox_Readme.md)** - The same tree as one column per level, for deep, uniform hierarchies where an indented tree gets too tall
- **LabelTreeSelectComboBox** - The form field around this control
- **MultiSelectComboBox** - A flat check list in a drop-down
- **CheckListBox** - A flat, always-visible check list
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > TreeSelectComboBox** for regions with the `Leaves` strategy and an event log (strategy, display mode, filter mode and the Select all row in the side panel), chips with the `Parents` strategy, a count summary with a flat filter, a lazily loaded file system and a disabled box.
