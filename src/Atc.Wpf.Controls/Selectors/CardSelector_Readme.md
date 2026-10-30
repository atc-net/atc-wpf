# 🃏 CardSelector

A wrapping grid of cards for picking one or several visual options: plans, report templates, devices, modules.

## 🔍 Overview

`CardSelector` is for the few choices where a text list is not enough: report templates with a thumbnail, devices with an image and a status, modules with a description, pricing plans. Each option is a card; the user picks one or several; a selected card gets an accent border and a check mark in its corner (a radio dot in the single-select modes).

It is a styled `ListBox`. Your `ItemTemplate` draws the card body (image, title, description); the control draws the [`Card`](../DataDisplay/Card_Readme.md) frame, the hover, focus, selected and disabled states, and the check mark. The control owns the selection, so `SelectionMode`, the selection limit and disabled items apply to every click, key press and UI Automation call.

Highlights:

- Binds **objects** (`ItemsSource`), not a dictionary: cards carry more than a display value
- `Multiple` (check boxes), `Single` (a radio group) or `SingleOrNone` (click the selected card to clear it) selection
- `SelectedItems` always; `SelectedKeys` (two-way, updated in place) when `KeyMemberPath` is set
- A selection limit (`MaxSelectionCount`) and per-item disabled cards (`IsEnabledMemberPath`)
- Fixed `CardWidth` for a tidy grid, `ItemSpacing` between cards and rows
- Arrow keys move in two dimensions, `Space` toggles, `Ctrl+A` selects all
- One `SelectedKeysChanged` event per action

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## 🚀 Usage

### Plan picker (single)

```xml
<atc:CardSelector
    CardWidth="200"
    ItemsSource="{Binding Plans}"
    KeyMemberPath="Id"
    SelectedKeys="{Binding SelectedPlans}"
    SelectionMode="Single">
    <atc:CardSelector.ItemTemplate>
        <DataTemplate>
            <StackPanel AutomationProperties.Name="{Binding Title}">
                <TextBlock FontWeight="SemiBold" Text="{Binding Title}" />
                <TextBlock Text="{Binding Price}" />
            </StackPanel>
        </DataTemplate>
    </atc:CardSelector.ItemTemplate>
</atc:CardSelector>
```

### Device picker with offline devices disabled and a limit

```xml
<atc:CardSelector
    CardWidth="220"
    IsEnabledMemberPath="IsOnline"
    ItemTemplate="{StaticResource DeviceCardTemplate}"
    ItemsSource="{Binding Devices}"
    KeyMemberPath="Id"
    MaxSelectionCount="2"
    SelectedKeys="{Binding SelectedDevices}" />
```

### Without keys

```xml
<atc:CardSelector ItemsSource="{Binding Templates}" ItemTemplate="{StaticResource TemplateCard}" />
```

```csharp
var chosen = cardSelector.SelectedItems.Cast<ReportTemplate>().ToList();
```

### Listening for changes

```csharp
cardSelector.SelectedKeysChanged += (_, e) =>
    Debug.WriteLine($"[{string.Join(", ", e.OldValue)}] -> [{string.Join(", ", e.NewValue)}]");
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ItemsSource` | `IEnumerable` | `null` | The options, one card each |
| `ItemTemplate` | `DataTemplate` | `null` | The card body; the control draws the frame, selection border and check mark |
| `KeyMemberPath` | `string?` | `null` | Path of the key property (e.g. `Id` or `Info.Id`); enables `SelectedKeys` |
| `SelectedKeys` | `ObservableCollection<string>` | empty | Keys of the selected cards (needs `KeyMemberPath`). Two-way; updated in place |
| `SelectedItems` | `IList` | empty | The selected items (read-only, observable); always available |
| `SelectionMode` | `CardSelectionMode` | `Multiple` | `Single`, `SingleOrNone` or `Multiple` |
| `CardWidth` | `double` | `200` | Width of each card; `NaN` sizes each card to its content |
| `CardHeight` | `double` | `NaN` | Height of each card; `NaN` sizes the cards to their content |
| `ItemSpacing` | `double` | `8` | Space between cards and between rows |
| `ShowCheckMark` | `bool` | `true` | Corner ✓ on selected cards (radio dot in the single-select modes) |
| `MaxSelectionCount` | `int?` | `null` | Selection limit in `Multiple`; at the limit the other cards are disabled |
| `IsEnabledMemberPath` | `string?` | `null` | Path of a `bool` property; `false` greys the card out and the keyboard skips it |

## 📡 Events

| Event | Args | When |
|-------|------|------|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per action that changes `SelectedKeys` (with `KeyMemberPath`) |
| `SelectionChanged` | `SelectionChangedEventArgs` | Always (from `ListBox`) |

## 🧩 Methods

| Method | Description |
|--------|-------------|
| `SelectAll()` | Selects every enabled card in one change (`Multiple` only; does nothing when the limit has no room) |
| `ClearSelection()` | Clears every enabled card in one change; disabled cards keep their state |
| `RefreshItemStates()` | Re-reads `IsEnabledMemberPath` after the items changed their availability |

## ⌨️ Keyboard

| Key | Action |
|-----|--------|
| `←` / `→` | Previous / next enabled card (mirrored in right-to-left) |
| `↑` / `↓` | Nearest enabled card in the row above / below |
| `Home` / `End` | First / last enabled card |
| `Space` / `Enter` | Toggles the focused card |
| `Ctrl+A` | Selects all enabled cards (`Multiple` only) |

In `Single` and `SingleOrNone` moving also selects, like a radio group.

## 📝 Notes

- `SelectionMode` hides the `ListBox` property of the same name; the `ListBox` underneath always runs in its multiple mode and the control enforces the card modes itself. Code that reads `ListBox.SelectionMode` through a `ListBox` reference sees `Multiple`.
- Changes made in the control update `SelectedKeys` in place and raise `SelectedKeysChanged` once. Changes made to `SelectedKeys` from outside (or a new instance) only re-sync the cards and raise no event. Keys without a matching item are kept, so `SelectedKeys` can be bound before `ItemsSource`.
- When the items change, keys whose items are gone are dropped (one event), and items with a selected key are selected, so reloading the items keeps the selection.
- UI Automation: in `Multiple` the cards are check boxes with the Toggle pattern; in the single-select modes they are radio buttons with SelectionItem, and the list has the Selection pattern. The card name is the first `AutomationProperties.Name` set in the card template, else the key. Text inside the card template is not used as the name, so set `AutomationProperties.Name` on the template root.
- `IsEnabledMemberPath` is read when a card is created and on every selection or item change; call `RefreshItemStates()` after changing it on an item.
- The cards are not virtualized; the control is meant for a few cards (a soft limit of about 100). Use `CheckListBox` or `MultiSelectComboBox` for long lists.

## 🔗 Related Controls

- **Card** - The frame every card is drawn with
- **MultiSelectComboBox** - Drop-down with check boxes, for 3–1,000 items and little space
- **TokenBox** - Large or open-ended sets, typing is the main way in
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as a wrapping row of toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **CheckListBox** - Always-visible check list for 5–1,000 items
- **DualListSelector** - Building an ordered list out of a pool

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > CardSelector**.
