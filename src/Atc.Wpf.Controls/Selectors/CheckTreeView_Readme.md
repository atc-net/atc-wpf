# 🌳 CheckTreeView

An inline tree with **tri-state check boxes**: ticking a parent ticks everything under it, and a parent shows a partial mark when only some of its children are ticked.

## 🔍 Overview

`CheckTreeView` is for selecting from a **hierarchy**: permissions (module › feature › action), folders, regions (country › region › site), product categories. The check state of every node is kept by the control from `SelectedKeys`, so a view model only binds keys, the same contract as the rest of the multi-select family.

- tri-state propagation: a parent is ticked when all its children are, partial when some are
- `CheckedKeysStrategy` decides which keys `SelectedKeys` holds: leaves, top-most fully ticked parents, or all
- `IsCheckLinked="False"` makes every node independent
- disabled nodes keep their state (a parent over a disabled, unticked child stays partial)
- a filter box that keeps the tree shape (matches with their ancestors, expanded) or lists matches flat with their path
- an optional **Select all** / **Clear** row with a count, and a selection limit
- lazy loading: `LoadChildrenAsync` loads children on the first expand, with a "Please wait…" row; children of a ticked node inherit the tick
- one `SelectedKeysChanged` per user action: ticking a parent with 500 children is one event
- virtualized with recycling: 10,000 nodes in 3 levels open and filter without a visible delay; propagation touches only the affected nodes
- UI Automation: each row is a tree item with `ExpandCollapse` and a three-state `Toggle`, named after the node text

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Basic

```xml
<atc:CheckTreeView
    Height="300"
    CheckedKeysStrategy="Parents"
    Nodes="{Binding Path=Permissions}"
    SelectedKeys="{Binding Path=PermissionKeys}" />
```

```csharp
public ObservableCollection<CheckTreeNode> Permissions { get; } =
[
    new CheckTreeNode("orders", "Orders")
    {
        Children =
        {
            new CheckTreeNode("orders.view", "View orders"),
            new CheckTreeNode("orders.approve", "Approve orders"),
        },
    },
];

// SelectedKeys binds two-way by default, so the property needs a setter.
public ObservableCollection<string> PermissionKeys { get; set; } = [];
```

### The item model

| Member | Type | Description |
|---|---|---|
| `Key` | `string` (init) | The selection key; unique in the tree (a second node with the same key throws an `InvalidOperationException`). |
| `Text` | `string` | The display text, UI Automation name and what the filter matches. |
| `Icon` | `object?` | Optional content shown before the text. |
| `IsEnabled` | `bool` | `false`: the node cannot be toggled and keeps its state. |
| `IsExpanded` | `bool` | Two-way with the tree. |
| `Children` | `ObservableCollection<CheckTreeNode>` | The child nodes; changes are tracked. |
| `HasUnloadedChildren` | `bool` | Lazy loading: children are loaded by `LoadChildrenAsync` on the first expand. |
| `IsChecked` | `bool?` (read-only) | `true`, `false` or `null` (partial); kept by the control. |
| `Tag` | `object?` | Any application value. |

A node belongs to one tree: give each control its own nodes.

### Checked keys strategy

| Value | `SelectedKeys` holds | Example: "Denmark" with all its 3 sites ticked |
|---|---|---|
| `Leaves` (default) | ticked leaf nodes only | `dk-aarhus`, `dk-cph`, `dk-odense` |
| `Parents` | a fully ticked branch collapses to its top-most fully ticked node | `dk` |
| `All` | every ticked node, parents and leaves | `dk`, `dk-aarhus`, `dk-cph`, `dk-odense` |

`Parents` suits permissions: "all of Denmark" stays "all of Denmark" when a site is added later (a node added under a parent key comes up ticked). With `IsCheckLinked="False"` the strategy is ignored and every ticked node is a key. Changing `CheckedKeysStrategy` or `IsCheckLinked` at runtime keeps the ticks and rewrites `SelectedKeys` in the new shape (one `SelectedKeysChanged`); when `SelectedKeys` is data-bound this waits for the bindings to settle and is skipped after a DataContext swap.

### Lazy loading

```xml
<atc:CheckTreeView
    CheckedKeysStrategy="Parents"
    ExpandDepth="0"
    LoadChildrenAsync="{Binding Path=LoadFolder}"
    Nodes="{Binding Path=Drives}"
    SelectedKeys="{Binding Path=FolderKeys}" />
```

```csharp
public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> LoadFolder
    => async (folder, cancellationToken) =>
    {
        var names = await fileService.GetSubFoldersAsync(folder.Key, cancellationToken);
        return names
            .Select(x => new CheckTreeNode(x.Path, x.Name) { HasUnloadedChildren = true })
            .ToList();
    };
```

Ticking a node whose children are not loaded ticks the node itself (with `Leaves` its own key is in `SelectedKeys` until the children arrive; then the keys become the children, one `SelectedKeysChanged`). A failing load leaves the node unloaded and collapsed, so the next expand tries again.

### Filter

```xml
<atc:CheckTreeView FilterMode="FlatResults" IsFilterVisible="True" Nodes="{Binding Path=Catalog}" />
```

- A node matches when its text contains the filter text (case-insensitive). A matching node keeps **everything under it**; its ancestors stay visible.
- `KeepAncestors` (default): the tree keeps its shape and the ancestors of matches are expanded (their expansion is restored when the filter is cleared).
- `FlatResults`: the matches are listed flat, each with its path ("Nordics › Denmark") as a sub-line.
- Ticking a parent while filtering ticks only its **shown** children, like Select all in a filtered list.

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `ObservableCollection<CheckTreeNode>` | empty | The root nodes. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys (see `CheckedKeysStrategy`). Two-way by default; updated in place; setting it from code recomputes every state in one pass, with no event. |
| `CheckedKeysStrategy` | `CheckedKeysStrategy` | `Leaves` | Which ticked nodes are keys. |
| `IsCheckLinked` | `bool` | `true` | `false`: every node is independent. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown when the tree has more than 20 nodes. |
| `FilterText` | `string` | `""` | The filter text (two-way). Debounced above 2,000 nodes. |
| `FilterMode` | `TreeFilterMode` | `KeepAncestors` | `KeepAncestors` or `FlatResults`. |
| `ShowSelectAllRow` | `bool` | `false` | The **Select all** / count / **Clear** row above the tree. |
| `MaxSelectionCount` | `int?` | `null` | The most keys in `SelectedKeys`; a tick beyond it is refused and at the limit the unticked nodes are disabled. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. |
| `LoadChildrenAsync` | `Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?` | `null` | Loads the children of a node with `HasUnloadedChildren` on its first expand. |
| `ExpandDepth` | `int` | `1` | Levels expanded when `Nodes` is set (1 = the roots are expanded). |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a row (data context: `CheckTreeNode`). |
| `IsAtSelectionLimit` | `bool` | `false` | Read-only: `SelectedKeys` holds `MaxSelectionCount` keys. |

## 🔧 Methods

| Method | Description |
|---|---|
| `FindNode(string key)` | Returns the node with a key, or `null`. |
| `ToggleNode(CheckTreeNode node)` | Toggles a node as a click does (one event). |
| `SetNodeChecked(string key, bool isChecked)` | Ticks or unticks a node (with its shown, enabled children when linked). |
| `SelectAll()` / `ClearSelection()` | Ticks every shown, enabled node / unticks every enabled node (one event each). |
| `ExpandAll()` / `CollapseAll()` | Expands every loaded node / collapses every node. |
| `ExpandSubtree(CheckTreeNode node)` | Expands a node and every loaded node under it (what `*` does). |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per user action (a tick, Select all, Clear), or when loaded or added children change the keys. Not raised when `SelectedKeys` is set from code. |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `↑` / `↓` | Move between rows |
| `→` / `←` | Expand or go to the first child / collapse or go to the parent |
| `Space` | Toggle the focused row |
| `*` (numeric keypad) | Expand everything under the focused row |
| `Ctrl+A` | Select all shown nodes |
| `Down` in the filter box | Move to the first row |

## 📝 Notes

- Bulk changes: the default `SelectedKeys` is an `ObservableCollectionEx<string>`, so Select all and Clear raise one `Reset`; a view-model `ObservableCollection<string>` gets one notification per key, but the control still updates its states and summary once per action. For a bulk change from code, assign a new collection or use `ObservableCollectionEx<string>.AddRange` (one notification): adding keys one by one updates the control once per key.
- A click anywhere on a row (check box or text) toggles it; the arrow expands or collapses.
- Ticking a **partial** parent ticks all its enabled children; a parent whose enabled (and shown) children are all ticked is unticked by a click, even when a disabled child keeps it partial.
- Keys in `SelectedKeys` that are not in the tree (for example children that are not loaded yet) are kept, so a saved selection survives until those nodes appear. **Select all** and **Clear** drop them, and unticking a node whose children are not loaded drops the kept keys of the nodes that later load under it (they come up unticked).
- The check-state engine is separate from the tree presentation, so other views of a tree (for example a column cascader) can reuse it.

## 🔗 Related Controls

- **TreeSelectComboBox** - The same tree in a drop-down
- **CheckListBox** - A flat, always-visible check list
- **MultiSelectComboBox** - A flat check list in a drop-down
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > CheckTreeView** for a permissions tree with the `Parents` strategy and an event log (strategy, linking, filter mode and the Select all row can be switched in the side panel), a lazily loaded file-system tree, and a 10,000-node catalog with a flat filter.
