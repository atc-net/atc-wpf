# ↔️ DualListSelector

A dual-list control for transferring and reordering items between an available list and a selected list.

## 🔍 Overview

`DualListSelector` presents two side-by-side lists with transfer buttons for moving items between them. It supports single and bulk transfers, reordering within the selected list, text filtering, drag-and-drop, keyboard shortcuts, and automatic sort order maintenance. The layout can be flipped to show selected items first.

Optional extras: a check box per row with a tri-state **Select all** per list (`ItemSelectionStyle="CheckBox"`), a **one-way** mode with a × per selected row, **locked** items that cannot leave the selected list, a **limit** on the selected list, **grouping**, and **virtualization** (on by default) for lists of thousands of items.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Selectors;
```

## 🚀 Usage

### Basic DualListSelector

```xml
<selectors:DualListSelector
    AvailableItems="{Binding AvailableItems}"
    SelectedItems="{Binding SelectedItems}"
    AvailableHeader="Available"
    SelectedHeader="Selected" />
```

### With Filter and Drag-Drop

```xml
<selectors:DualListSelector
    AvailableItems="{Binding AvailableItems}"
    SelectedItems="{Binding SelectedItems}"
    AvailableHeader="Available Columns"
    SelectedHeader="Visible Columns"
    ShowFilter="True"
    AllowDragDrop="True"
    ShowReorderButtons="True" />
```

### Custom Item Template

```xml
<selectors:DualListSelector
    AvailableItems="{Binding AvailableItems}"
    SelectedItems="{Binding SelectedItems}">
    <selectors:DualListSelector.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Name}" FontWeight="Bold" />
                <TextBlock Text="{Binding Description}" Margin="8,0,0,0" Opacity="0.6" />
            </StackPanel>
        </DataTemplate>
    </selectors:DualListSelector.ItemTemplate>
</selectors:DualListSelector>
```

### Check Boxes with Select All

```xml
<!-- A check box per row; each list header gets a tri-state "Select all" (of the shown rows) and "3/10" -->
<selectors:DualListSelector
    AvailableItems="{Binding AvailableItems}"
    SelectedItems="{Binding SelectedItems}"
    ItemSelectionStyle="CheckBox"
    ShowFilter="True" />
```

### One-Way, Locked Items and a Limit

```xml
<!-- No "move back" buttons: each selected row has a × button and Delete removes the highlighted rows.
     At most 5 items can be picked; the footer shows "3 of max 5". -->
<selectors:DualListSelector
    AvailableItems="{Binding AvailableColumns}"
    SelectedItems="{Binding VisibleColumns}"
    IsOneWay="True"
    MaxSelectedCount="5" />
```

```csharp
// Locked items stay in the selected list (no ×, skipped by "move all back" and drag), but can be reordered.
VisibleColumns.Add(new DualListSelectorItem { Identifier = "id", Name = "Id", IsLocked = true });
```

### Grouping and Large Lists

```xml
<!-- Grouped by DualListSelectorItem.Group (or set GroupMemberPath); the lists virtualize when their height is bounded -->
<selectors:DualListSelector
    AvailableItems="{Binding ManyItems}"
    SelectedItems="{Binding SelectedItems}"
    ListMaxHeight="300"
    ShowFilter="True" />
```

### Reversed Layout

```xml
<!-- Selected list on the left, Available on the right -->
<selectors:DualListSelector
    AvailableItems="{Binding AvailableItems}"
    SelectedItems="{Binding SelectedItems}"
    LayoutMode="SelectedFirst" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AvailableItems` | `ObservableCollection<DualListSelectorItem>` | empty | Items in the available list (two-way bindable) |
| `SelectedItems` | `ObservableCollection<DualListSelectorItem>` | empty | Items in the selected list (two-way bindable) |
| `LayoutMode` | `DualListSelectorLayoutMode` | `AvailableFirst` | Which list appears on the left |
| `AvailableHeader` | `object?` | `null` | Header for the available list |
| `SelectedHeader` | `object?` | `null` | Header for the selected list |
| `ListMinHeight` | `double` | `200` | Minimum height of list boxes |
| `ListMinWidth` | `double` | `200` | Minimum width of list boxes |
| `ListMaxHeight` | `double` | `Infinity` | Maximum height of list boxes |
| `ShowReorderButtons` | `bool` | `true` | Show move up/down/top/bottom buttons |
| `ShowTransferAllButtons` | `bool` | `true` | Show transfer-all buttons |
| `AutoRecalculateSortOrder` | `bool` | `true` | Auto-renumber SortOrderNumber after changes |
| `ItemTemplate` | `DataTemplate?` | `null` | Custom template for list items |
| `ShowFilter` | `bool` | `false` | Show text filter boxes above lists |
| `AllowDragDrop` | `bool` | `false` | Enable drag-and-drop between lists |
| `AllowMultiSelect` | `bool` | `true` | Allow selecting multiple items (Extended vs Single) |
| `ShowItemCount` | `bool` | `false` | Show item count below each list |
| `ButtonAreaWidth` | `double` | `48` | Width of transfer/reorder button columns |
| `AvailableFooter` | `object?` | `null` | Footer content below the available list |
| `SelectedFooter` | `object?` | `null` | Footer content below the selected list |
| `AvailableHeaderTemplate` | `DataTemplate?` | `null` | Custom template for the available header |
| `SelectedHeaderTemplate` | `DataTemplate?` | `null` | Custom template for the selected header |
| `ItemSelectionStyle` | `DualListItemSelectionStyle` | `Highlight` | `Highlight` (picked rows are highlighted) or `CheckBox` (a check box per row and a tri-state **Select all** with a "checked / total" count in each list header) |
| `HeaderCountFormat` | `string?` | `null` | Format of the "checked / total" count in `CheckBox` style (`{0}` checked, `{1}` shown); `null` uses the translated `"{0}/{1}"` |
| `IsOneWay` | `bool` | `false` | Hides the "move to available" buttons; each selected row gets a × button (named "Remove {name}") and `Delete` removes the highlighted rows |
| `MaxSelectedCount` | `int?` | `null` | Maximum number of items in the selected list; transfers and drops that would exceed it are blocked, and the selected footer shows "{0} of max {1}" |
| `IsVirtualizing` | `bool` | `true` | Virtualizes both lists with container recycling (also when grouped) |
| `GroupMemberPath` | `string?` | `null` | Item property to group both lists by; when `null`, the lists are grouped by `DualListSelectorItem.Group` as soon as an item has one |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ItemsTransferred` | `EventHandler<DualListSelectorItemsTransferredEventArgs>` | Raised after items are transferred between lists |
| `ItemsReordered` | `EventHandler<DualListSelectorItemsReorderedEventArgs>` | Raised after items are reordered in the selected list |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | Raised when selection changes in either list |

## 🔧 Commands

| Command | Description |
|---------|-------------|
| `MoveToSelected` | Transfer selected item(s) to the selected list |
| `MoveToAvailable` | Transfer selected item(s) back to the available list |
| `MoveAllToSelected` | Transfer all (or filtered) items to the selected list |
| `MoveAllToAvailable` | Transfer all (or filtered) items back to the available list |
| `MoveToTop` | Move selected item to the top of the selected list |
| `MoveUp` | Move selected item up one position |
| `MoveDown` | Move selected item down one position |
| `MoveToBottom` | Move selected item to the bottom of the selected list |
| `RemoveFromSelected` | Return one item (the command parameter) to the available list; the × button in one-way mode. Locked items stay |

## 📊 Enumerations

### DualListItemSelectionStyle

| Value | Description |
|-------|-------------|
| `Highlight` | Picked rows are highlighted; `Ctrl`/`Shift`-click picks several (default) |
| `CheckBox` | A check box per row; a plain click toggles a row; tri-state **Select all** per list |

### DualListSelectorLayoutMode

| Value | Description |
|-------|-------------|
| `AvailableFirst` | Available list on the left, Selected on the right (default) |
| `SelectedFirst` | Selected list on the left, Available on the right |

## ⌨️ Keyboard Shortcuts

| Key | Context | Action |
|-----|---------|--------|
| `Enter` | Available list | Transfer to selected |
| `Enter` | Selected list | Transfer to available |
| `Ctrl+Up` | Selected list | Move item up |
| `Ctrl+Down` | Selected list | Move item down |
| `Delete` | Selected list (one-way) | Remove the highlighted items (locked ones stay) |
| `Space` | Either list (`CheckBox` style) | Toggle the focused row |
| `Double-click` | Available item | Transfer to selected |
| `Double-click` | Selected item | Transfer to available |

## 📝 Notes

- `DualListSelectorItem` has `Identifier`, `Name`, `Description`, `SortOrderNumber`, `IsEnabled`, `Tag`, `IsLocked` and `Group` properties
- Locked items (`IsLocked = true`) in the selected list show a lock icon; they cannot be moved back, dragged back or removed, and "move all back" skips them; they can still be reordered
- With `MaxSelectedCount`, the transfer buttons are disabled and drops are refused when the transfer would exceed the limit; reordering is always allowed
- In `CheckBox` style a checked row is a picked row (the list box selection), so the transfer buttons move the checked rows. **Select all** checks the shown (filtered) enabled rows, or unchecks all when they all are checked; rows hidden by a filter are unchecked
- Virtualization needs a bounded list height (e.g. `ListMaxHeight`); in a `ScrollViewer` or `StackPanel` without one, every row is realized
- With grouping, the filter keeps only the groups that have matches
- A transfer of many items updates the counts once. The control's own `AvailableItems` / `SelectedItems` are `ObservableCollectionEx<DualListSelectorItem>`, so each list gets one `Reset` per transfer; bind your own `ObservableCollectionEx` to keep that, as a plain `ObservableCollection` gets one change per moved item (or one `Reset` when it is emptied)
- `ItemsTransferred` is raised once per action (button, key, double-click, drop or ×)
- Items with `IsEnabled = false` appear grayed out and cannot be selected or transferred
- `Tag` can hold arbitrary user data attached to an item
- When `AutoRecalculateSortOrder` is true, `SortOrderNumber` is renumbered after every transfer or reorder
- Filter is case-insensitive and matches against the `Name` property
- Filter TextBox includes a clear (✕) button; when filter matches nothing, a "No matches" placeholder is shown
- When `ShowItemCount` is true, each list shows "X items" (or "X of Y" when filtered); with `MaxSelectedCount` the selected list always shows "X of max Y"
- Drag-and-drop supports both cross-list transfer and within-list reordering, with ghost adorner and drop insertion indicator
- Double-click an item to transfer it to the other list
- Focus automatically moves to the target list after transfer operations
- Transfer-all respects active filters when no items are multi-selected

## 🔗 Related Controls

- **CountrySelector** - Predefined selector for countries
- **LanguageSelector** - Predefined selector for languages

Other multi-select controls:

| Control | Best for |
|---|---|
| **MultiSelectComboBox** | 3–1,000 known items, little space (drop-down with check boxes) |
| **TokenBox** | large or open-ended sets; typing is the main way in |
| **CheckTreeView** / **TreeSelectComboBox** | hierarchies |
| **ChipGroup** | 2–15 short options, all visible |
| **Segmented** (multiple) | 2–6 very short options or icons |
| **CheckListBox** | 5–1,000 items in a settings page or side panel |
| **DualListSelector** | building an ordered list out of a pool |

## 🎮 Sample Application

See the DualListSelector sample in the Atc.Wpf.Sample application under **Wpf.Components > Selectors > DualListSelector** for interactive examples.
