# 🗂️ CascaderComboBox

A multi-select drop-down for **deep, uniform hierarchies**: one column per level, side by side, each row with a tri-state check box.

## 🔍 Overview

`CascaderComboBox` is for hierarchies like Country › Region › City, Category › Sub-category › Product or Site › Building › Room, where an indented tree gets too tall and the user loses the path. Picking "Denmark" in the first column shows its regions in the second, picking "Jutland" shows its cities in the third, so the path is always in view.

- closed: the same box as `TreeSelectComboBox`: names, removable chips with "+N", or a count (`MultiSelectDisplayMode`)
- open: a row of columns; each row is `[☐] Text ›`, the `›` on rows with children (loaded or not); the **active** row of each column (the one whose children are shown next) is highlighted
- tri-state check boxes from the same engine as `CheckTreeView`: a parent is partial when only some of what is under it is ticked
- ⚠️ `CheckedKeysStrategy` defaults to **`Parents`**: ticking all of Jutland keeps the one key `dk-jut`. This differs from `CheckTreeView` and `TreeSelectComboBox`, which default to **`Leaves`**: a cascader usually picks whole branches by path, a tree drop-down usually feeds a query by its leaves. Set `CheckedKeysStrategy` explicitly when the two must agree.
- a filter across **all levels**: typing replaces the columns with one flat list of matches, each with its full path and the match highlighted
- lazy loading: `LoadChildrenAsync` loads the children of a row when it becomes active, with a busy row in the next column; activating another row cancels the load
- one `SelectedKeysChanged` per user action: ticking a node with 500 descendants is one event
- columns are virtualized: a column of 10,000 nodes opens without a visible delay; check propagation touches only the affected nodes
- UI Automation: the control is a combo box with `ExpandCollapse` and `Value` (the summary); each column is a list, each row a check box with a three-state `Toggle`, rows with children also `ExpandCollapse` (expanded = active)

For wide, shallow hierarchies (many roots, one or two levels) a `TreeSelectComboBox`, or a `CheckListBox` with groups, is the better fit.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Basic

```xml
<atc:CascaderComboBox
    Width="360"
    atc:TextBoxHelper.Watermark="Pick regions"
    DisplayMode="Chips"
    Nodes="{Binding Path=Regions}"
    SelectedKeys="{Binding Path=SelectedRegionKeys}"
    ShowFullPath="True" />
```

```csharp
public ObservableCollection<CheckTreeNode> Regions { get; } =
[
    new CheckTreeNode("dk", "Denmark")
    {
        Children =
        {
            new CheckTreeNode("dk-jut", "Jutland")
            {
                Children =
                {
                    new CheckTreeNode("dk-jut-aar", "Aarhus"),
                    new CheckTreeNode("dk-jut-aal", "Aalborg"),
                },
            },
            new CheckTreeNode("dk-zea", "Zealand")
            {
                Children = { new CheckTreeNode("dk-zea-cph", "Copenhagen") },
            },
        },
    },
];

// SelectedKeys binds two-way by default, so the property needs a setter.
public ObservableCollection<string> SelectedRegionKeys { get; set; } = [];
```

With `ShowFullPath="True"` the chip for Aarhus reads "Denmark / Jutland / Aarhus".

### Checked keys strategy

Ticking Aarhus, Aalborg (all of Jutland) and Copenhagen gives:

| Value | `SelectedKeys` |
|---|---|
| `Parents` (default) | `dk-jut`, `dk-zea-cph` |
| `Leaves` | `dk-jut-aar`, `dk-jut-aal`, `dk-zea-cph` |
| `All` | `dk-jut`, `dk-jut-aar`, `dk-jut-aal`, `dk-zea-cph` |

A parent key ticks the whole sub-tree under it, so a city added (or loaded) later under a ticked region comes up ticked.

### Hover mode

```xml
<atc:CascaderComboBox ExpandTrigger="Hover" Nodes="{Binding Path=Products}" />
```

Resting the mouse on a row with children makes it active after about 150 ms, so a diagonal mouse move to the next column does not open the rows it crosses. A click still works.

### Lazy loading

```xml
<atc:CascaderComboBox
    LoadChildrenAsync="{Binding Path=LoadSiteChildren}"
    Nodes="{Binding Path=Sites}"
    SelectedKeys="{Binding Path=SiteKeys}" />
```

```csharp
public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> LoadSiteChildren
    => async (site, cancellationToken) =>
    {
        var buildings = await siteService.GetBuildingsAsync(site.Key, cancellationToken);
        return buildings
            .Select(x => new CheckTreeNode(x.Id, x.Name) { HasUnloadedChildren = true })
            .ToList();
    };
```

A row with `HasUnloadedChildren` shows `›`; making it active calls the loader and shows "Please wait…" in the next column. Activating another row cancels the token. A failing load drops the busy column silently and the row loads again on the next activation. Children of a ticked row inherit the tick.

### Filter

- Shown automatically when the tree has more than 20 nodes (`IsFilterVisible="{x:Null}"`), or always / never with `True` / `False`.
- A node matches when its text contains the filter text (case-insensitive). Matches come from **every level**, in tree order, each with its full path (`Denmark / Jutland / Aarhus`, separator `PathSeparator`) and the match highlighted.
- Ticking a result ticks that node and, linked, everything under it, as in the columns.
- Clearing the filter returns to the columns with the previous active path.
- Children that are not loaded yet are **not** searched.

### Summary

As `TreeSelectComboBox`: `NoneText` (or the watermark) when nothing is ticked, "All" when every leaf is ticked, the names up to `MaxNamesInSummary`, then "3 of 12 selected" (`CountFormat`). With `ShowFullPath` the names and chips are full paths.

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `ObservableCollection<CheckTreeNode>` | empty | The roots (the first column). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys. Two-way by default; updated in place; setting it from code recomputes every state, with no event. |
| `CheckedKeysStrategy` | `CheckedKeysStrategy` | `Parents` | `Parents`, `Leaves` or `All`. Note: `CheckTreeView` and `TreeSelectComboBox` default to `Leaves`. |
| `IsCheckLinked` | `bool` | `true` | `false`: every node is independent. |
| `ExpandTrigger` | `CascaderExpandTrigger` | `Click` | `Click` or `Hover`. |
| `ColumnWidth` | `double` | `180` | The width of each column. |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height; each column scrolls (virtualized) beyond it. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 20 nodes. |
| `FilterText` | `string` | `""` | The filter text (two-way). Debounced above 2,000 nodes. |
| `MaxSelectionCount` | `int?` | `null` | The most keys in `SelectedKeys`; a tick beyond it is refused and at the limit the unticked rows are dimmed. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. |
| `LoadChildrenAsync` | `Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?` | `null` | Loads the children of a row with `HasUnloadedChildren` when it becomes active. |
| `PathSeparator` | `string` | `" / "` | Between the texts of a path (filter results, and the summary with `ShowFullPath`). |
| `ShowFullPath` | `bool` | `false` | Summary names and chips show the full path instead of the node text. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a row (data context: `CheckTreeNode`). |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` | `int?` | `null` | `Chips` only: `null` = as many as fit, then "+N". |
| `ChipsWrap` | `bool` | `false` | `Chips` only: wrap over several lines (no "+N"). |
| `ChipTemplate` | `DataTemplate?` | `null` | `Chips` only (data context: `MultiSelectChipItem`). |
| `MaxNamesInSummary` | `int` | `2` | The most names before the count. |
| `AllText` | `string?` | `null` | The summary when every leaf is ticked. |
| `NoneText` | `string` | `""` | The summary when nothing is ticked. |
| `CountFormat` | `string?` | `null` | The count format; `null` = "{0} of {1} selected". |
| `IsDropDownOpen` | `bool` | `false` | Whether the pop-up is open (two-way). |
| `SummaryText` | `string` | `""` | Read-only: the closed-box text (also the UI Automation value). |

## 🔧 Methods

| Method | Description |
|---|---|
| `FindNode(string key)` | Returns the node with a key, or `null`. |
| `ToggleNode(CheckTreeNode node)` | Toggles a node as a click on its check box does (one event). |
| `SelectAll()` / `ClearSelection()` | One event each. |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per user action (a tick, a chip removal), or when loaded children change the keys. Not raised when `SelectedKeys` is set from code. |
| `DropDownOpened` | `EventArgs` | The pop-up opened. |
| `DropDownClosed` | `EventArgs` | The pop-up closed. |

## 🖱️ Mouse

| Where | Action |
|---|---|
| The check box of any row | Tick or untick it (with everything under it when linked) |
| The text of a row with children | Make it active: its children open in the next column, deeper columns close |
| Anywhere on a leaf row | Tick or untick it |
| A filter result | Tick or untick it |
| Outside the pop-up | Close it |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `↑` / `↓` | Move within the column |
| `→` | Make the focused row active and move into the next column (loads lazily) |
| `←` | Back to the previous column |
| `Space` | Toggle the focused row |
| `Enter` | On a row with children: as `→`; on a leaf: toggle |
| `Esc` | Clear the filter text if there is any, otherwise close the pop-up |
| `Alt+Down` / `Alt+Up` / `F4` | Open or close the pop-up |
| `Backspace` | `Chips` mode, closed box: remove the last chip |
| `Down` in the filter box | Move to the first row |

Unlike the other drop-downs of the family, `Enter` on a row does not close the pop-up (it acts on the row); `Enter` in the filter box does. In right-to-left layouts `←` and `→` swap.

## 📝 Notes

- Bulk changes: the default `SelectedKeys` is an `ObservableCollectionEx<string>`, so Select all and Clear raise one `Reset`; a view-model `ObservableCollection<string>` gets one notification per key, but the control still updates its states and summary once per action. For a bulk change from code, assign a new collection or use `ObservableCollectionEx<string>.AddRange` (one notification): adding keys one by one updates the control once per key.
- A node belongs to one tree: give each control its own nodes.
- Keys in `SelectedKeys` that are not in the tree (for example cities that are not loaded yet) are kept, so a saved selection survives until those nodes appear. **Select all** and **Clear** drop them, and unticking a node whose children are not loaded drops the kept keys of the nodes that later load under it (they come up unticked).
- Disabled nodes keep their state; a parent over a disabled, unticked child stays partial.
- Ticks apply right away; there is no OK / Cancel mode.
- The column view is a separate internal element, so it could later be shown inline or reused for a single-select cascader.

## 🔗 Related Controls

- **TreeSelectComboBox** - The same tree as an indented tree in a drop-down (better for wide, shallow hierarchies)
- **CheckTreeView** - The indented tree, always visible
- **LabelCascaderComboBox** - The form field around this control
- **MultiSelectComboBox** - A flat check list in a drop-down
- **MultiColumnComboBox** - A drop-down with a check-box grid, for comparing attributes
- **CheckListBox** - A flat, always-visible check list (with groups for shallow hierarchies)
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > CascaderComboBox** for regions (country › region › city) in click mode with chips and full paths and an event log (strategy, expand trigger, display mode, full path and column width in the side panel), product categories in hover mode, a lazily loaded site › building › room tree, a limit of 3 keys with a count summary, and a disabled box.
