# ☑️ CheckListBox

An always-visible list with a check box per item, a tri-state **Select all** row, an optional filter box, groups, a selection limit and three row styles.

## 🔍 Overview

`CheckListBox` is the multi-select list for settings pages ("Columns to export"), filter side panels and permission lists: the user sees what is ticked without opening anything. It handles the parts every hand-made `ListBox` + `CheckBox` gets slightly wrong:

- a tri-state **Select all** row with a "{0} of {1}" count (for **all** items, also while filtering)
- a filter box (shown automatically above 10 items, debounced above 2,000 items)
- groups with tri-state headers, a selection limit and disabled keys
- batching: Select all, Clear, a group tick and a `Shift+click` range raise **one** `SelectedKeysChanged`
- virtualization with container recycling (10,000 items scroll smoothly)
- keyboard (`Space`, `Ctrl+A`, arrows, `Home` / `End`) and UI Automation (each row is a check box with `Toggle`)

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Basic

```xml
<atc:CheckListBox
    Height="250"
    Items="{Binding Path=ExportColumns}"
    SelectedKeys="{Binding Path=SelectedExportColumns}" />
```

```csharp
public Dictionary<string, string> ExportColumns { get; } = new(StringComparer.Ordinal)
{
    ["id"] = "Id",
    ["name"] = "Name",
    ["email"] = "E-mail",
};

// SelectedKeys binds two-way by default, so the property needs a setter.
public ObservableCollection<string> SelectedExportColumns { get; set; } = ["id", "name"];
```

### Settings page with switches

```xml
<atc:CheckListBox
    ItemStyle="Switch"
    Items="{Binding Path=Notifications}"
    SelectedKeys="{Binding Path=SelectedNotifications}"
    ShowSelectAllRow="False" />
```

### Grouped permissions with locked keys

```xml
<atc:CheckListBox
    DisabledKeys="{Binding Path=LockedPermissions}"
    ItemGroups="{Binding Path=PermissionGroups}"
    Items="{Binding Path=Permissions}"
    SelectedKeys="{Binding Path=SelectedPermissions}" />
```

`ItemGroups` maps a key to a group name. Keys without a group are listed first; the groups follow in the order they first appear in `Items`, each under a header with a tri-state check box.

### Highlight rows, a limit and columns

```xml
<atc:CheckListBox ItemStyle="Highlight" Items="{Binding Path=Tags}" MaxSelectionCount="3" />

<atc:CheckListBox Columns="3" IsFilterVisible="False" Items="{Binding Path=Months}" />
```

### Custom text template

The data context of `ItemTemplate` is the row (`CheckListItem`: `Key`, `DisplayValue`, `GroupName`, `IsChecked`, `IsEnabled`).

```xml
<atc:CheckListBox Items="{Binding Path=Servers}">
    <atc:CheckListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Path=DisplayValue}" />
                <TextBlock Margin="6,0,0,0" Opacity="0.6" Text="{Binding Path=Key}" />
            </StackPanel>
        </DataTemplate>
    </atc:CheckListBox.ItemTemplate>
</atc:CheckListBox>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | Key → display value. Changing it keeps the ticks (and the scroll position) of keys still present and drops the others from `SelectedKeys` (one event). |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The ticked keys. Two-way by default; updated in place; a new instance from the binding is picked up. |
| `SelectNewItems` | `bool` | `false` | Keys that appear in `Items` (including the first items) are ticked automatically, up to `MaxSelectionCount`. Set it before `Items`. |
| `ShowSelectAllRow` | `bool` | `true` | The top row with the tri-state **Select all** check box and the count. |
| `ShowClearButton` | `bool` | `false` | A **Clear** button in the Select all row. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown when there are more than 10 items. |
| `FilterText` | `string` | `""` | The filter text (two-way); items whose display value contains it are shown. |
| `ItemGroups` | `Dictionary<string, string>?` | `null` | Key → group name. |
| `IsGroupCheckable` | `bool` | `true` | Group headers have a tri-state check box that ticks or unticks the shown items of the group. |
| `MaxSelectionCount` | `int?` | `null` | At the limit the unticked items are disabled (not hidden) and the count reads "{0} of max {1}". Select all is disabled when the shown items do not fit. |
| `DisabledKeys` | `ObservableCollection<string>` | empty | Shown but cannot be toggled; Select all, Clear, groups and ranges leave them as they are. |
| `ItemStyle` | `CheckListItemStyle` | `CheckBox` | `CheckBox` (box before text), `Highlight` (no box; ticked rows get the accent background and a trailing ✓), `Switch` (a toggle switch at the end of the row). |
| `Columns` | `int` | `1` | Lay items out in N columns; virtualization only applies at 1. |
| `ItemTemplate` | `DataTemplate?` | `null` | The text part of a row. |

## 🔧 Methods

| Method | Description |
|---|---|
| `SelectAll()` | Ticks every shown, enabled item (one event). Does nothing when the limit has no room for them all. |
| `ClearSelection()` | Unticks every enabled item, shown or not (one event). |
| `GetItem(string key)` | Returns the row of a key; setting its `IsChecked` ticks it as a click would. |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per tick, Select all, Clear, group tick, `Shift+click` range, or `Items` change that drops or adds keys. Changes made to `SelectedKeys` from outside only re-sync the rows and raise nothing. |

## ⌨️ Keyboard and mouse

| Input | Action |
|---|---|
| Click anywhere on a row | Toggles the row |
| `Shift+click` / `Shift+Space` | Ticks or unticks (the new state of the clicked row) every shown, enabled row from the last clicked row |
| `Space` | Toggles the focused row (a group header toggles its group) |
| `Ctrl+A` | Selects all shown items (outside the filter box) |
| Arrow keys, `Home`, `End` | Move between rows; `Down` in the filter box moves to the first row |

## 📝 Notes

- When `SelectedKeys` is data-bound, the keys dropped (or ticked) because `Items` changed are written once the bindings have settled (one dispatcher pass later), so a DataContext swap never writes into the previous view model's collection; unbound, it happens at once.
- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or set `Mode=OneWay` for a get-only collection (the control still updates it in place).
- With a filter, Select all ticks only the shown items, while the count is for all items.
- Disabled keys do not count towards the Select all and group header states.
- Give the control a height (or put it in a container that limits its height) so the list scrolls and stays virtualized.
- `ObservableCollectionEx<string>` as `SelectedKeys` gets a single reset notification for a bulk change.

## 🔗 Related Controls

- **LabelCheckListBox** - The form field around this control (label, mandatory marker, validation)
- **MultiSelectComboBox** - The same list in a drop-down, for little space
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > CheckListBox** for columns to export, notification settings (`Switch`), grouped permissions, the `Highlight` style with a limit, columns and 10,000 items with a filter.
