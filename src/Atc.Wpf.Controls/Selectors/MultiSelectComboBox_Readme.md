# 🔽 MultiSelectComboBox

A drop-down where the user ticks **several** items. Closed, it looks like a `ComboBox` and shows a summary of what is ticked; open, it shows a check list with **Select all**, **Clear** and a filter.

## 🔍 Overview

`MultiSelectComboBox` is the multi-select field for little space: a toolbar or ribbon filter ("Source systems"), a form row, a column header. It hosts a `CheckListBox` in its pop-up, so the list behaviour (tri-state Select all with a count, filter, groups, limit, disabled keys, batching, virtualization, keyboard) is the same as in `CheckListBox`.

- closed: the themed `ComboBox` look and height (26 px), with a summary text: `"orders, payments"`, `"3 of 5 selected"`, `"All"`, or the watermark when nothing is ticked
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

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | Key → display value. Changing it keeps the ticks of keys still present and drops the others from `SelectedKeys` (one event). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys. Two-way by default; updated in place; a new instance from the binding is picked up. |
| `SelectNewItems` | `bool` | `false` | Keys that appear in `Items` (including the first items) are ticked automatically. Set it before `Items`. |
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
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per tick, Select all, Clear, group tick, or `Items` change that drops or adds keys. Changes made to `SelectedKeys` from outside only update the ticks and the summary. |
| `DropDownOpened` | `EventArgs` | The pop-up opened. |
| `DropDownClosed` | `EventArgs` | The pop-up closed. |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `Alt+Down` / `Alt+Up` / `F4` | Open or close the pop-up |
| `Esc` / `Enter` | Close the pop-up (focus returns to the box) |
| Arrow keys | Move between rows; `Down` in the filter box moves to the first row |
| `Space` | Tick the focused row |
| `Ctrl+A` | Select all shown items |

When the pop-up opens, focus goes to the filter box (if shown) or the first row.

## 📝 Notes

- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or set `Mode=OneWay` for a get-only collection.
- The summary lists names in the order of `Items`, not in the order they were ticked, and counts only keys that are in `Items`.
- UI Automation: while the pop-up is open, its rows are children of the combo box element, so a FlaUI search scoped to the control finds them.
- `ObservableCollectionEx<string>` as `SelectedKeys` gets a single reset notification for a bulk change.

## 🔗 Related Controls

- **LabelMultiSelectComboBox** - The form field around this control (label, mandatory marker, validation)
- **LabelComboBox** - Single selection by key
- **CheckListBox** - The same list, always visible
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool (two lists with transfer buttons)
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > MultiSelectComboBox** for the interactive box with an event log, a toolbar, live data with `SelectNewItems`, the summary texts and 1,000 items with a filter.