# DragDropAttach - Drag & Drop Framework

## Overview

`DragDropAttach` provides a set of attached properties that enable drag-and-drop on any `ItemsControl` (ListBox, ListView, TreeView) via XAML-only configuration. It supports drag reordering, cross-list transfer, dragging several selected items at once, auto-scrolling at the edges, file drops from Explorer, and visual feedback with adorners — all with an MVVM-friendly `IDropHandler` callback.

## Namespace

`Atc.Wpf.DragDrop`

## Usage

### Basic Reorder

Enable drag reordering within a single list:

```xml
<ListBox atc:DragDropAttach.IsDragSource="True"
         atc:DragDropAttach.IsDropTarget="True"
         ItemsSource="{Binding Items}" />
```

### Cross-List Transfer

Drag items between two lists:

```xml
<ListBox atc:DragDropAttach.IsDragSource="True"
         atc:DragDropAttach.IsDropTarget="True"
         ItemsSource="{Binding SourceItems}" />

<ListBox atc:DragDropAttach.IsDragSource="True"
         atc:DragDropAttach.IsDropTarget="True"
         ItemsSource="{Binding TargetItems}" />
```

### Multi-Item Drag

With `SelectionMode="Extended"` (or `Multiple`) on a ListBox, or a DataGrid, pressing on one of several
selected items and dragging moves the whole selection. `DragInfo.SourceItems` holds the dragged items in
the order of the source collection; `DragInfo.SourceItem` is the item the drag started on. The
`DefaultDropHandler` keeps the items together and in that order, both when reordering and when transferring:

```xml
<ListBox atc:DragDropAttach.IsDragSource="True"
         atc:DragDropAttach.IsDropTarget="True"
         SelectionMode="Extended"
         ItemsSource="{Binding Items}" />
```

A click (without a drag) on one of several selected items still selects just that item, on mouse up.

### Auto-Scroll

While a drag is over a scrollable drop target, holding the pointer within 20 px of an edge (or a quarter of
the list, for small lists) scrolls the target one line at a time, so items can be dropped beyond the
visible part of a long list. No configuration is needed.

### File Drop Zone

Accept file drops from Windows Explorer:

```xml
<ListBox atc:DragDropAttach.IsDropTarget="True"
         ItemsSource="{Binding DroppedFiles}" />
```

### Custom Drop Handler

Implement `IDropHandler` in your ViewModel to control which items are accepted:

```xml
<ListBox atc:DragDropAttach.IsDropTarget="True"
         atc:DragDropAttach.DropHandler="{Binding}"
         ItemsSource="{Binding AcceptedItems}" />
```

```csharp
public class MyViewModel : ViewModelBase, IDropHandler
{
    public void DragOver(DropInfo dropInfo)
    {
        if (dropInfo.DragInfo?.SourceItems.All(IsAcceptable) == true)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(DropInfo dropInfo) { /* move item */ }
}
```

## Attached Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `IsDragSource` | `bool` | `false` | Enables drag initiation on this element |
| `IsDropTarget` | `bool` | `false` | Enables drop acceptance on this element |
| `DropHandler` | `IDropHandler` | `null` | Custom drop handler (falls back to DefaultDropHandler) |
| `DragHandler` | `IDragHandler` | `null` | Custom drag initiation handler |
| `AllowedEffects` | `DragDropEffects` | `Move` | Allowed drag-drop effects |
| `ShowDragAdorner` | `bool` | `true` | Show ghost visual during drag |
| `ShowDropIndicator` | `bool` | `true` | Show insertion line and highlight at drop position |

## Key Types

| Type | Description |
|---|---|
| `DragInfo` | Data bag passed to `IDragHandler.StartDrag` with the source item, all dragged `SourceItems`, collection and index |
| `DropInfo` | Data bag passed to `IDropHandler.DragOver`/`Drop` with target info and insert index (an index in the target collection) |
| `IDropHandler` | Interface for custom drop behavior (DragOver + Drop) |
| `IDragHandler` | Optional interface for custom drag initiation |
| `DefaultDropHandler` | Built-in handler supporting reorder, transfer, and file drop |

## Notes

- If `DropHandler` is not set, a `DefaultDropHandler` is used automatically
- `DefaultDropHandler` works with any `IList` source/target collection
- File drops set `DropInfo.IsFileDrop = true` and populate `DropInfo.FileDropList`
- The item to drag is taken at the press point; presses inside a TextBox, PasswordBox, ComboBox, ScrollBar, Slider, Thumb or column header never start a drag
- The drop slot follows the items panel: vertical lists use the item's vertical half, horizontal StackPanels and WrapPanels the horizontal half (right-to-left aware)
- `DropInfo.InsertIndex` is an index in the target collection: when the target shows a filtered, sorted or grouped view, the slot under the pointer is mapped through the neighbouring shown item
- Visual feedback includes a semi-transparent ghost adorner and insertion line indicator
- The highlight adorner uses `AtcApps.Brushes.Accent` when available, falling back to DodgerBlue

## Related Controls

- [DualListSelector](../../Atc.Wpf.Components/Selectors/DualListSelector_Readme.md) — Specialized dual-list control with built-in drag-drop

## Sample Application

**Wpf.Controls > Drag and Drop > DragDrop**
