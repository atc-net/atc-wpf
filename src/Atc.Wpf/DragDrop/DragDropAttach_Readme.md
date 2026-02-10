# DragDropAttach - Drag & Drop Framework

## Overview

`DragDropAttach` provides a set of attached properties that enable drag-and-drop on any `ItemsControl` (ListBox, ListView, TreeView) via XAML-only configuration. It supports drag reordering, cross-list transfer, file drops from Explorer, and visual feedback with adorners — all with an MVVM-friendly `IDropHandler` callback.

## Namespace

`Atc.Wpf.Controls.DragDrop`

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
        if (IsAcceptable(dropInfo.DragInfo?.SourceItem))
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
| `DragInfo` | Data bag passed to `IDragHandler.StartDrag` with source item/collection/index |
| `DropInfo` | Data bag passed to `IDropHandler.DragOver`/`Drop` with target info and insert index |
| `IDropHandler` | Interface for custom drop behavior (DragOver + Drop) |
| `IDragHandler` | Optional interface for custom drag initiation |
| `DefaultDropHandler` | Built-in handler supporting reorder, transfer, and file drop |

## Notes

- If `DropHandler` is not set, a `DefaultDropHandler` is used automatically
- `DefaultDropHandler` works with any `IList` source/target collection
- File drops set `DropInfo.IsFileDrop = true` and populate `DropInfo.FileDropList`
- Visual feedback includes a semi-transparent ghost adorner and insertion line indicator
- The highlight adorner uses `AtcApps.Brushes.Accent` when available, falling back to DodgerBlue

## Related Controls

- [DualListSelector](../Selectors/DualListSelector_Readme.md) — Specialized dual-list control with built-in drag-drop

## Sample Application

**Wpf.Controls > Drag and Drop > DragDrop**
