# 📊 MultiColumnComboBox

A multi-select drop-down whose pop-up is a **small sortable grid with a check-box column**, for choosing items by comparing several attributes.

## 🔍 Overview

`MultiColumnComboBox` is for picks where one line of text is not enough to decide: customer number + name + city, part number + description + stock, user + department + e-mail. A `MultiSelectComboBox` row cannot show that, and a full `DataGrid` on the page takes too much room.

- closed: the same box as `MultiSelectComboBox`: names, removable chips with "+N", or a count (`MultiSelectDisplayMode`), and `SummaryText`
- open: an optional filter box, then a grid: a check-box column (its header is a tri-state "select all shown"), then your columns; a footer "{0} of {1} selected" and, in `Deferred` commit mode, OK and Cancel
- a click anywhere on a row toggles its check box; ticked rows get the theme's selected background (`AtcApps.Brushes.Accent3`), the keyboard row only a border
- sort by clicking a column header, resize columns by dragging the grippers; sorting and filtering never change `SelectedKeys`
- the rows are **your objects**: `KeyMemberPath` gives the key (dotted paths such as `Info.Id` work), `DisplayMemberPath` the name for the summary, the chips and the check box's automation name
- the filter matches any **visible column's** text (case-insensitive, contains) or the paths in `FilterMemberPaths`; above 2,000 rows it waits for a short pause in typing
- one `SelectedKeysChanged` per user action: the header "select all" or `Ctrl+A` is one event; in `Deferred` mode one event on OK
- `SelectedKeys` may be bound before the items load: a key is dropped only when an item with that key was present and goes away
- row and column virtualization: 10,000 rows open and filter without a visible delay
- UI Automation: the control is a combo box with `ExpandCollapse` and `Value` (the summary); the grid is a data grid; each row's check box is a check box with `Toggle`, named after the row's display value

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Explicit columns and chips

```xml
<atc:MultiColumnComboBox
    Width="360"
    atc:TextBoxHelper.Watermark="Pick customers"
    DisplayMemberPath="Name"
    DisplayMode="Chips"
    ItemsSource="{Binding Path=Customers}"
    KeyMemberPath="Number"
    SelectedKeys="{Binding Path=SelectedCustomerNumbers}">
    <atc:MultiColumnComboBox.Columns>
        <DataGridTextColumn Binding="{Binding Path=Number}" Header="No." />
        <DataGridTextColumn Binding="{Binding Path=Name}" Header="Name" />
        <DataGridTextColumn Binding="{Binding Path=City}" Header="City" />
    </atc:MultiColumnComboBox.Columns>
</atc:MultiColumnComboBox>
```

### Auto-generated columns

```xml
<atc:MultiColumnComboBox
    AutoGenerateColumns="True"
    DisplayMemberPath="FullName"
    ItemsSource="{Binding Path=Users}"
    KeyMemberPath="UserName"
    SelectedKeys="{Binding Path=UserNames}" />
```

### OK / Cancel, limit and disabled rows

```xml
<atc:MultiColumnComboBox
    CommitMode="Deferred"
    DisplayMemberPath="Name"
    FilterMemberPaths="Name, Contact.Email"
    IsEnabledMemberPath="IsActive"
    ItemsSource="{Binding Path=Customers}"
    KeepSelectedOnTop="True"
    KeyMemberPath="Number"
    MaxSelectionCount="3"
    SelectedKeys="{Binding Path=SelectedCustomerNumbers}">
    <atc:MultiColumnComboBox.Columns>
        <DataGridTextColumn Binding="{Binding Path=Name}" Header="Name" />
        <DataGridTextColumn Binding="{Binding Path=Contact.Email}" Header="E-mail" />
    </atc:MultiColumnComboBox.Columns>
</atc:MultiColumnComboBox>
```

```csharp
box.SelectedKeysChanged += (_, e) =>
    Debug.WriteLine($"{e.OldValue.Count} -> {e.NewValue.Count} customers");

var customers = box.SelectedItems.Cast<Customer>();
```

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | The objects shown as rows; an `INotifyCollectionChanged` source is followed. |
| `KeyMemberPath` | `string` | `""` | The (dotted) property path of the key (turned into a string); empty uses `ToString()`. |
| `DisplayMemberPath` | `string` | `""` | The (dotted) property path of the name used by the summary, the chips and the automation name. |
| `Columns` | `ObservableCollection<DataGridColumn>` | empty | The grid columns after the check-box column, as on `DataGrid` (XAML content). |
| `AutoGenerateColumns` | `bool` | `false` | `true`: one text column per public property when `Columns` is empty. |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The keys of the ticked items (two-way, updated in place). |
| `SelectedItems` | `IReadOnlyList<object>` | empty | Read-only: the ticked objects in `SelectedKeys` order. |
| `MaxSelectionCount` | `int?` | `null` | The most ticked items; at the limit the unticked rows are disabled. |
| `IsEnabledMemberPath` | `string` | `""` | The path of a `bool` property; rows where it is `false` cannot be toggled. |
| `IsFilterVisible` | `bool?` | `null` | The filter box; `null` = shown above 10 items. |
| `FilterText` | `string` | `""` | The filter text (two-way). |
| `FilterMemberPaths` | `string?` | `null` | Comma-separated paths to search instead of the visible columns. |
| `KeepSelectedOnTop` | `bool` | `false` | Ticked rows come first when the pop-up opens (any column sort is cleared then); rows do not move while ticking. |
| `DropDownWidth` | `double` | `NaN` | `NaN`: at least the box width, as wide as the columns need, up to the screen. |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The grid scrolls (virtualized) beyond it. |
| `CommitMode` | `MultiSelectCommitMode` | `Immediate` | `Deferred`: the pop-up shows OK and Cancel; only OK updates `SelectedKeys`. |
| `DisplayMode` | `MultiSelectDisplayMode` | `Text` | `Text`, `Chips` or `Count`. |
| `MaxVisibleChips` | `int?` | `null` | In chips mode, the most chips before "+N". |
| `ChipsWrap` | `bool` | `false` | In chips mode, wrap the chips instead of "+N". |
| `ChipTemplate` | `DataTemplate?` | `null` | In chips mode, the chip template (data context `MultiSelectChipItem`). |
| `MaxNamesInSummary` | `int` | `2` | The most names before the count format is used. |
| `AllText` | `string?` | `null` | The summary when everything is ticked (default: translated "All"). |
| `NoneText` | `string` | `""` | The summary when nothing is ticked (default: the watermark). |
| `CountFormat` | `string?` | `null` | `{0}` ticked, `{1}` items (default: translated "{0} of {1} selected"). |
| `SummaryText` | `string` | `""` | Read-only: what the closed box shows (the automation value). |
| `IsDropDownOpen` | `bool` | `false` | Whether the pop-up is open (two-way). |

Methods: `SelectAll()` (ticks every shown, enabled row), `ClearSelection()` (unticks every enabled row), `ToggleItem(item)` (as a row click).

## 📡 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per user action (row, header select-all, `Ctrl+A`, chip removal, OK in `Deferred` mode, items change that drops keys). |
| `DropDownOpened` | `EventArgs` | The pop-up opened. |
| `DropDownClosed` | `EventArgs` | The pop-up closed. |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `Alt+↓` / `F4` | Open / close |
| typing (closed or in the grid) | Goes to the filter box (when it is shown) |
| `↑` / `↓` / `PgUp` / `PgDn` / `Home` / `End` | Move the focused row (`↓` in the filter box moves into the grid) |
| `Space` | Toggle the focused row |
| `Ctrl+A` | Tick all shown rows (respecting the limit and disabled rows) |
| `Enter` | `Deferred`: OK; otherwise close |
| `Esc` | Clear the filter text first, then close (`Deferred`: cancel) |

## 📝 Notes

- When `SelectedKeys` is data-bound, the keys dropped (or ticked) because `Items` changed are written once the bindings have settled (one dispatcher pass later), so a DataContext swap never writes into the previous view model's collection; unbound, it happens at once.
- **A column belongs to one grid.** WPF lets a `DataGridColumn` sit in one `DataGrid` only, so the columns of `Columns` are moved into the pop-up grid. Do not share a column instance between controls (or with another `DataGrid`); removing it from `Columns` releases it.
- The filter reads the binding path of each visible `DataGridBoundColumn` (for other columns, `SortMemberPath`), not the formatted cell text: `StringFormat` and converters are not applied. Use `FilterMemberPaths` to search other properties.
- The grid is read-only: no cell editing, no column reordering, no grouping.
- `SelectedItems` follows `SelectedKeys`; keys without an item (yet) are skipped.
- The commit modes are the shared `MultiSelectCommitMode` (`Immediate` / `Deferred`), as on `MultiSelectComboBox`.

## 🔗 Related Controls

- **LabelMultiColumnComboBox** - The form field around this control (label, mandatory marker, validation)
- **MultiSelectComboBox** - A flat check list in a drop-down (one line per item)
- **CheckListBox** - A flat, always-visible check list
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **CascaderComboBox** - Deep, uniform hierarchies with one column per level
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > MultiColumnComboBox** for customers with explicit columns, chips and an event log (commit mode, keep-on-top, display mode and filter in the side panel), 10,000 spare parts with a filter and a count summary, OK / Cancel with ticked rows on top, auto-generated columns, and a limit of 3 with disabled (inactive) rows.
