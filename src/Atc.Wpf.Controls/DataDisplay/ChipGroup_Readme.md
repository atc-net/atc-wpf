# 🏷️ ChipGroup

A row of toggle chips, all visible, bound to a key/value `Items` dictionary and a `SelectedKeys` collection.

## 🔍 Overview

`ChipGroup` turns a handful of options into filter chips: "Error · Warning · Info · Debug", "Open · In progress · Done", product facets on a search page. It is faster than a drop-down when there are few options, and the state is visible at a glance. Each item is shown as a [`Chip`](Chip_Readme.md); the group owns the selection, so `SelectionMode`, the selection limit and disabled keys apply to every click, key press and UI Automation call.

Highlights:

- `Multiple`, `Single` (a radio group) or `SingleOrNone` (click the selected chip to clear it) selection
- `Filled` (selected = accent fill) or `Outlined` (selected = accent border and tint) chips, in the `Chip` sizes, with an optional check mark
- Wrapping rows, one scrolling line, or a vertical column
- An optional leading **All** chip that selects or clears every enabled key in one change
- Optional icon and count per key ("Error 12") for facets
- A selection limit (`MaxSelectionCount`) and disabled keys
- One `SelectedKeysChanged` event per action; `SelectedKeys` is updated in place

For more than about 15 options use `MultiSelectComboBox` (little space) or `CheckListBox` (a side panel) instead.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## 🚀 Usage

### Log-level filter with counts and an All chip

```xml
<atc:ChipGroup
    ItemCounts="{Binding LogLevelCounts}"
    Items="{Binding LogLevels}"
    SelectedKeys="{Binding SelectedLogLevels}"
    ShowAllChip="True" />
```

```csharp
public Dictionary<string, string> LogLevels { get; } = new()
{
    ["error"] = "Error",
    ["warning"] = "Warning",
    ["info"] = "Info",
    ["debug"] = "Debug",
};

public Dictionary<string, int> LogLevelCounts { get; } = new() { ["error"] = 12, ["warning"] = 48 };

public ObservableCollection<string> SelectedLogLevels { get; set; } = ["error", "warning"];
```

### Status filter (outlined, single selection)

```xml
<atc:ChipGroup
    ChipVariant="Outlined"
    Items="{Binding Statuses}"
    SelectedKeys="{Binding SelectedStatuses}"
    SelectionMode="Single" />
```

### One line that scrolls, or a vertical column

```xml
<atc:ChipGroup Items="{Binding Facets}" Wrap="False" />

<atc:ChipGroup
    DisabledKeys="{Binding SoldOut}"
    Items="{Binding Toppings}"
    MaxSelectionCount="3"
    Orientation="Vertical" />
```

### Reacting to changes

```csharp
chipGroup.SelectedKeysChanged += (_, e) =>
{
    // e.OldValue and e.NewValue are the keys before and after the change.
};
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Items` | `Dictionary<string, string>` | empty | Key → display value. Changing it drops keys no longer present from `SelectedKeys` (one event). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The selected keys. Two-way by default; updated in place; a new instance from the binding is picked up. |
| `SelectionMode` | `ChipSelectionMode` | `Multiple` | `Multiple`, `Single` (radio group) or `SingleOrNone` (click the selected chip to clear). |
| `SelectNewItems` | `bool` | `false` | Keys that appear in `Items` are selected automatically (`Multiple` only), up to `MaxSelectionCount`. |
| `ChipVariant` | `ChipGroupVariant` | `Filled` | `Filled`: selected = accent fill. `Outlined`: selected = accent border and a light accent tint. |
| `ChipSize` | `ChipSize` | `Medium` | `Small`, `Medium` or `Large`, as `Chip`. |
| `ShowCheckMark` | `bool` | `true` | A ✓ before the text of a selected chip. |
| `Orientation` | `Orientation` | `Horizontal` | The direction the chips are laid out in. |
| `Wrap` | `bool` | `true` | `false` keeps the chips on one line that scrolls. |
| `ItemSpacing` | `double` | `4` | Space between two chips and between two lines. |
| `ShowAllChip` | `bool` | `false` | A leading **All** chip (`Multiple` only): selected when every enabled key is; clicking it selects them all or clears them in one change. |
| `MaxSelectionCount` | `int?` | `null` | The most keys that can be selected (`Multiple`); at the limit the other chips are disabled, not hidden. `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Keys whose chips are shown but cannot be toggled. |
| `ItemIcons` | `Dictionary<string, object>?` | `null` | Optional icon per key (font icon, image, any content). |
| `ItemCounts` | `Dictionary<string, int>?` | `null` | Optional count per key, shown after the text ("Error 12"). |

## 📡 Events

| Event | Args | When |
|-------|------|------|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Each toggle; the **All** chip, an arrow key in a single-select group and an `Items` change that drops or adds keys are one event each. Changes made to `SelectedKeys` from outside raise none. |

## 🧩 Methods

| Method | Description |
|--------|-------------|
| `SelectAll()` | Selects every enabled chip in one change (`Multiple` only; nothing happens when the limit has no room). |
| `ClearSelection()` | Clears every enabled chip in one change. |

## ⌨️ Keyboard and UI Automation

- `Tab` enters and leaves the group as one stop.
- `Multiple`: arrow keys move the focus, `Space` / `Enter` toggles the focused chip, `Ctrl+A` selects all.
- `Single` / `SingleOrNone`: the group is a radio group; arrow keys move **and** select. Disabled chips are skipped.
- The group is a **list**. In `Multiple` each chip is a **check box** with the Toggle pattern; in the single modes each chip is a **radio button** with the SelectionItem pattern and the group has the Selection pattern. A chip's name is its display value.

## 📝 Notes

- When `SelectedKeys` is data-bound, the keys dropped (or ticked) because `Items` changed are written once the bindings have settled (one dispatcher pass later), so a DataContext swap never writes into the previous view model's collection; unbound, it happens at once.
- Only the keys of items that disappear from `Items` are dropped; keys that were never among the items are kept, so a selection restored before the items load (an empty `Items` first, the loaded items later) survives. The whole family follows this rule.
- Material guidance: a chip set is either single- or multi-select, never mixed — so `ShowAllChip`, `MaxSelectionCount` and `SelectNewItems` only apply in `Multiple` mode.
- Switching from `Multiple` to a single mode keeps only the most recently selected key (one `SelectedKeysChanged`; with a data-bound `SelectedKeys`, once the bindings have settled), like `Segmented`.
- In the single modes a `SelectedKeys` set from outside with several keys is shown as is; the next click or arrow key leaves one key.
- An icon element in `ItemIcons` can only be on one chip; use one element per key.
- Theme brushes only; the chips follow Light, Dark and right-to-left (arrow keys follow the flow direction).

## 🔗 Related Controls

- **Chip** - A single chip; `ChipGroup` uses it for every item
- **LabelChipGroup** - The same group with a label, mandatory marker and validation
- **MultiSelectComboBox** - Drop-down with check boxes, for 3–1,000 items and little space
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **Segmented** - 2–6 very short options as a joined button row
- **CheckListBox** - Always-visible check list for 5–1,000 items
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Data Display > ChipGroup**.
