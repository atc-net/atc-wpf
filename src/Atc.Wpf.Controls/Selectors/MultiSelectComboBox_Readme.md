# 🔽 MultiSelectComboBox

A drop-down where the user ticks **several** items. Closed, it looks like a `ComboBox` and shows a summary of what is ticked; open, it shows a check list with **Select all**, **Clear** and a filter.

## 🔍 Overview

`MultiSelectComboBox` is the multi-select field for little space: a toolbar or ribbon filter ("Source systems"), a form row, a column header. It hosts a `CheckListBox` in its pop-up, so the list behaviour (tri-state Select all with a count, filter, groups, limit, disabled keys, batching, virtualization, keyboard) is the same as in `CheckListBox`.

- closed: the themed `ComboBox` look and height (26 px), with a summary text: `"orders, payments"`, `"3 of 5 selected"`, `"All"`, or the watermark when nothing is ticked; or (`DisplayMode`) a removable chip per ticked item, or only the count
- a selection limit, OK / Cancel in the pop-up (`CommitMode="Deferred"`) and grouped items
- open: ticking an item does **not** close the pop-up
- Select all and Clear raise **one** `SelectedKeysChanged`
- 1,000 items open without a delay (the list is virtualized); `Items` changing while open keeps the scroll position and the ticks
- UI Automation: a combo box with `ExpandCollapse` and the summary as its `Value`; each row is a check box with `Toggle`

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Basic

```xml
<atc:MultiSelectComboBox
    Width="220"
    atc:TextBoxHelper.Watermark="Source systems"
    Items="{Binding Path=SourceSystems}"
    SelectedKeys="{Binding Path=SelectedSourceSystemKeys}" />
```

```csharp
public Dictionary<string, string> SourceSystems { get; } = new(StringComparer.Ordinal)
{
    ["orders"] = "Orders",
    ["payments"] = "Payments",
    ["shipping"] = "Shipping",
};

// SelectedKeys binds two-way by default, so the property needs a setter.
public ObservableCollection<string> SelectedSourceSystemKeys { get; set; } = [];
```

### Live data: tick new items automatically

```xml
<atc:MultiSelectComboBox
    Width="220"
    atc:TextBoxHelper.Watermark="Source systems"
    Items="{Binding Path=SourceSystems}"
    SelectNewItems="True"
    SelectedKeys="{Binding Path=SelectedSourceSystemKeys}" />
```

Replace `Items` with a new dictionary when a source system appears: the new key is ticked, keys that are gone are unticked (one `SelectedKeysChanged`), and the other ticks stay.

### In a toolbar

```xml
<ToolBar>
    <Button Content="Refresh" />
    <atc:MultiSelectComboBox
        Width="200"
        atc:TextBoxHelper.Watermark="Log levels"
        Items="{Binding Path=LogLevels}"
        SelectedKeys="{Binding Path=SelectedLogLevels}" />
</ToolBar>
```

### Summary texts

```xml
<atc:MultiSelectComboBox
    AllText="Every source system"
    CountFormat="{}{0}/{1} systems"
    Items="{Binding Path=SourceSystems}"
    MaxNamesInSummary="3"
    NoneText="No source system" />
```

| Ticked | Summary | Property |
|---|---|---|
| none | `NoneText`, shown in the watermark style; when empty, `TextBoxHelper.Watermark` | `NoneText` |
| all | "All" (translated) | `AllText` |
| 1 … `MaxNamesInSummary` | the display values in `Items` order, comma-separated: "Orders, Payments" | `MaxNamesInSummary` |
| more | "3 of 5 selected" (translated) | `CountFormat` (`{0}` = ticked, `{1}` = items) |

### Display modes

One control, three looks (`DisplayMode`):

| Mode | The closed box shows | Suits |
|---|---|---|
| `Text` (default) | "Orders, Payments", or "3 of 5 selected" beyond `MaxNamesInSummary`, "All" when all are ticked | toolbars, ribbons, narrow fields |
| `Chips` | a small removable chip per ticked item; the chips that do not fit become a "+N" chip | forms, where the user sees and removes picks without opening |
| `Count` | only "3 of 5 selected" (`CountFormat`), or `AllText` / `NoneText` at the ends | very narrow fields |

```xml
<!-- Text: "Information, Warning" -->
<atc:MultiSelectComboBox Width="240" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />

<!-- Chips: [Information ×] [Warning ×] [Error ×] [+1] -->
<atc:MultiSelectComboBox Width="240" DisplayMode="Chips" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />

<!-- Chips, at most two, then "+N" -->
<atc:MultiSelectComboBox Width="320" DisplayMode="Chips" MaxVisibleChips="2" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />

<!-- Chips on as many lines as needed (the box grows in height, no "+N") -->
<atc:MultiSelectComboBox Width="240" DisplayMode="Chips" ChipsWrap="True" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />

<!-- Count: "4 of 6 selected" -->
<atc:MultiSelectComboBox Width="160" DisplayMode="Count" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />
```

In `Chips` mode:

- each chip has a remove button that unticks the key **without opening** the pop-up (one `SelectedKeysChanged`); a ticked key in `DisabledKeys` gets a chip without one
- `MaxVisibleChips="{x:Null}"` (the default) is responsive: as many chips as fit on one line, then "+N"; a number shows at most that many
- hovering "+N" shows the hidden names; clicking it opens the pop-up; clicking a chip opens or closes the pop-up like the rest of the box
- `Backspace` on the closed box removes the last chip
- one-line chips keep the `ComboBox` height (they use `Chip` with `Variant="Input"` and `Size="Small"`, made a little more compact)
- `ChipTemplate` replaces the chip; its data context is a `MultiSelectChipItem` (`Key`, `DisplayValue`, `CanRemove`, `RemoveCommand`)

```xml
<atc:MultiSelectComboBox DisplayMode="Chips" Items="{Binding Path=Tags}" SelectedKeys="{Binding Path=SelectedTags}">
    <atc:MultiSelectComboBox.ChipTemplate>
        <DataTemplate>
            <Button Margin="0,0,3,0" Command="{Binding Path=RemoveCommand}" Content="{Binding Path=DisplayValue}" />
        </DataTemplate>
    </atc:MultiSelectComboBox.ChipTemplate>
</atc:MultiSelectComboBox>
```

### Selection limit

```xml
<atc:MultiSelectComboBox DisplayMode="Chips" MaxSelectionCount="3" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />
```

At the limit the unticked items are **disabled** (not hidden) and the pop-up header shows "3 of max 3". **Select all** (and a group header) is disabled when the shown items do not fit in the remaining room: it never ticks part of them. `SelectedKeys` set from code beyond the limit is kept; `LabelMultiSelectComboBox.IsValid()` then reports it.

### OK / Cancel (deferred commit)

```xml
<atc:MultiSelectComboBox CommitMode="Deferred" Items="{Binding Path=LogLevels}" SelectedKeys="{Binding Path=SelectedLogLevels}" />
```

With `CommitMode="Deferred"` the pop-up shows **OK** and **Cancel** in a footer. Ticks are a draft: `SelectedKeys` and `SelectedKeysChanged` change only on **OK** (or `Enter`), with one event. **Cancel**, `Esc` and a click outside discard the draft; the next opening starts from `SelectedKeys` again. Use it when each change triggers an expensive re-query.

### Groups

```xml
<atc:MultiSelectComboBox
    IsFilterVisible="True"
    ItemGroups="{Binding Path=FoodGroups}"
    Items="{Binding Path=Foods}"
    SelectedKeys="{Binding Path=SelectedFoods}" />
```

`ItemGroups` maps a key to a group name. Keys without a group are listed first; the groups follow in the order of first appearance, each under a header with a tri-state check box (`IsGroupCheckable`) that ticks or unticks the shown items of the group (one event). The filter hides groups without matches. Group headers and items are one virtualized list.

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | Key → display value. Changing it keeps the ticks of keys still present and drops the others from `SelectedKeys` (one event). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys. Two-way by default; updated in place; a new instance from the binding is picked up. |
| `SelectNewItems` | `bool` | `false` | Keys that appear in `Items` (including the first items) are ticked automatically, up to `MaxSelectionCount`. Set it before `Items`. |
| `ShowSelectAllRow` | `bool` | `true` | The pop-up's top row with the tri-state **Select all** check box and the count. |
| `ShowClearButton` | `bool` | `true` | A **Clear** button in the Select all row. |
| `IsFilterVisible` | `bool?` | `null` | The filter box at the top of the pop-up; `null` = shown when there are more than 10 items. |
| `FilterText` | `string` | `""` | The filter text (two-way). With a filter, Select all ticks only the shown items. |
| `MaxNamesInSummary` | `int` | `2` | The most names the summary lists before it shows the count. |
| `AllText` | `string?` | `null` | The summary when all items are ticked; `null` = the translated "All". |
| `NoneText` | `string` | `""` | The summary when nothing is ticked. |
| `CountFormat` | `string?` | `null` | The count format; `null` = the translated "{0} of {1} selected". |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height; the list scrolls (virtualized) beyond it. |
| `IsDropDownOpen` | `bool` | `false` | Whether the pop-up is open (two-way). |
| `SummaryText` | `string` | `""` | Read-only: the text the closed box shows (also the UI Automation value). |
| `ItemGroups` | `Dictionary<string, string>?` | `null` | Key → group name (passed to the pop-up list). |
| `IsGroupCheckable` | `bool` | `true` | Tri-state check boxes on group headers. |
| `MaxSelectionCount` | `int?` | `null` | Selection limit; at the limit the unticked items are disabled. |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Shown but cannot be toggled. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a pop-up row (data context: `CheckListItem`). |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`: what the closed box shows. |
| `MaxVisibleChips` | `int?` | `null` | `Chips` only. `null` = as many as fit on one line, then "+N"; a number = at most that many chips, then "+N". |
| `ChipsWrap` | `bool` | `false` | `Chips` only. The chips wrap over several lines and the box grows in height (no "+N"). |
| `ChipTemplate` | `DataTemplate?` | `null` | `Chips` only. A custom chip (data context: `MultiSelectChipItem`); `null` = a small input `Chip`. |
| `CommitMode` | `MultiSelectCommitMode` | `Immediate` | `Immediate`, or `Deferred`: OK / Cancel in the pop-up, ticks apply on OK. |

The closed box honours `atc:TextBoxHelper.Watermark`, `atc:ControlsHelper.CornerRadius`, `Padding`, `Background`, `BorderBrush` and `Foreground` like the themed `ComboBox`.

## 🔧 Methods

| Method | Description |
|---|---|
| `SelectAll()` | Ticks every shown, enabled item (one event). |
| `ClearSelection()` | Unticks every enabled item (one event). |
| `GetItem(string key)` | Returns the pop-up row of a key; setting its `IsChecked` ticks it as a click would. |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per tick, Select all, Clear, group tick, chip removal, OK in deferred mode, or `Items` change that drops or adds keys. Changes made to `SelectedKeys` from outside only update the ticks and the summary. |
| `DropDownOpened` | `EventArgs` | The pop-up opened. |
| `DropDownClosed` | `EventArgs` | The pop-up closed. |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `Alt+Down` / `Alt+Up` / `F4` | Open or close the pop-up |
| `Esc` / `Enter` | Close the pop-up (focus returns to the box); in deferred mode `Esc` discards the ticks and `Enter` is OK |
| `Backspace` | `Chips` mode, closed box: remove the last chip |
| Arrow keys | Move between rows; `Down` in the filter box moves to the first row |
| `Space` | Tick the focused row |
| `Ctrl+A` | Select all shown items |

When the pop-up opens, focus goes to the filter box (if shown) or the first row.

## 📝 Notes

- Bulk changes: the default `SelectedKeys` is an `ObservableCollectionEx<string>`, so Select all and Clear raise one `Reset`; a view-model `ObservableCollection<string>` gets one notification per key, but the control still updates its states and summary once per action. For a bulk change from code, assign a new collection or use `ObservableCollectionEx<string>.AddRange` (one notification): adding keys one by one updates the control once per key.
- When `SelectedKeys` is data-bound, the keys dropped (or ticked) because `Items` changed are written once the bindings have settled (one dispatcher pass later), so a DataContext swap never writes into the previous view model's collection; unbound, it happens at once.
- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or set `Mode=OneWay` for a get-only collection.
- The summary lists names in the order of `Items`, not in the order they were ticked, and counts only keys that are in `Items`.
- UI Automation: while the pop-up is open, its rows (and in deferred mode the **OK** and **Cancel** buttons) are children of the combo box element, so a FlaUI search scoped to the control finds them. In `Chips` mode each shown chip's remove button is a button named "Remove {display value}" and "+N" is a button named after the hidden items ("Item 3, Item 4"); group headers are check boxes with a three-state `Toggle` named after the group.
- In `Chips` mode `SummaryText` (the UI Automation value) lists all ticked names.
- `ObservableCollectionEx<string>` as `SelectedKeys` gets a single reset notification for a bulk change.

## 🔗 Related Controls

- **LabelMultiSelectComboBox** - The form field around this control (label, mandatory marker, validation)
- **LabelComboBox** - Single selection by key
- **CheckListBox** - The same list, always visible
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **MultiColumnComboBox** - The same drop-down with a sortable check-box grid, for comparing items on several attributes
- **CascaderComboBox** - Deep, uniform hierarchies with one column per level
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool (two lists with transfer buttons)
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > MultiSelectComboBox** for the interactive box with an event log (its display and commit modes can be switched in the side panel), a toolbar, the three display modes side by side, chips with remove buttons, a limit of 3, deferred commit, grouped items, live data with `SelectNewItems`, the summary texts and 1,000 items with a filter.