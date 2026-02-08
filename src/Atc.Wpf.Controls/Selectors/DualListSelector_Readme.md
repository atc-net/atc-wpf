# ↔️ DualListSelector

A dual-list control for transferring and reordering items between an available list and a selected list.

## 🔍 Overview

`DualListSelector` presents two side-by-side lists with transfer buttons for moving items between them. It supports single and bulk transfers, reordering within the selected list, text filtering, drag-and-drop, keyboard shortcuts, and automatic sort order maintenance. The layout can be flipped to show selected items first.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
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
| `ShowItemCount` | `bool` | `true` | Show item count below each list |
| `ButtonAreaWidth` | `double` | `48` | Width of transfer/reorder button columns |
| `AvailableFooter` | `object?` | `null` | Footer content below the available list |
| `SelectedFooter` | `object?` | `null` | Footer content below the selected list |
| `AvailableHeaderTemplate` | `DataTemplate?` | `null` | Custom template for the available header |
| `SelectedHeaderTemplate` | `DataTemplate?` | `null` | Custom template for the selected header |

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

## 📊 Enumerations

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
| `Double-click` | Available item | Transfer to selected |
| `Double-click` | Selected item | Transfer to available |

## 📝 Notes

- `DualListSelectorItem` has `Identifier`, `Name`, `Description`, `SortOrderNumber`, `IsEnabled`, and `Tag` properties
- Items with `IsEnabled = false` appear grayed out and cannot be selected or transferred
- `Tag` can hold arbitrary user data attached to an item
- When `AutoRecalculateSortOrder` is true, `SortOrderNumber` is renumbered after every transfer or reorder
- Filter is case-insensitive and matches against the `Name` property
- Filter TextBox includes a clear (✕) button; when filter matches nothing, a "No items found" placeholder is shown
- When `ShowItemCount` is true, each list shows "X items" (or "X of Y" when filtered)
- Drag-and-drop supports both cross-list transfer and within-list reordering, with ghost adorner and drop insertion indicator
- Double-click an item to transfer it to the other list
- Focus automatically moves to the target list after transfer operations
- Transfer-all respects active filters when no items are multi-selected

## 🔗 Related Controls

- **CountrySelector** - Predefined selector for countries
- **LanguageSelector** - Predefined selector for languages

## 🎮 Sample Application

See the DualListSelector sample in the Atc.Wpf.Sample application under **Wpf.Controls > Selectors > DualListSelector** for interactive examples.
