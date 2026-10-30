# 🔘 Segmented

A joined row of segments for choosing one option (view switcher, filter toggle) or several (weekdays, bold / italic / underline).

## 🔍 Overview

`Segmented` shows 2–6 short options as one joined shape that looks like a toolbar group. By default exactly one segment is selected, as in a tab bar or a view switcher. `SelectionMode` also allows clearing the selection (`SingleOrNone`) or turning on any number of segments (`Multiple`), e.g. a weekday picker (M T W T F S S) or text styles (B / I / U).

It supports keyboard navigation, horizontal or vertical layout, joined segments or separate pill buttons, icon-only segments, three sizes, equal or auto-sized segments, separators between items, and custom icons per segment. The control manages the corner radius of the first and last segments so the row (or column) forms one pill shape.

For more than 6 options or long texts, use `ChipGroup`, which wraps.

### Selection modes

| `SelectionMode` | Behaviour | Clicking the selected segment | UI Automation |
|---|---|---|---|
| `Single` (default) | Exactly one segment is selected once a choice is made | Keeps it selected | Radio buttons (`SelectionItem`); the control has the `Selection` pattern |
| `SingleOrNone` | At most one segment is selected | Clears the selection | Radio buttons (`SelectionItem`); the control has the `Selection` pattern |
| `Multiple` | Any number of segments are selected; each one toggles on its own | Turns it off | Toggle buttons (`Button` with `Toggle`) |

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Segmented Control

```xml
<dataDisplay:Segmented SelectedIndex="0">
    <dataDisplay:SegmentedItem Content="Day" />
    <dataDisplay:SegmentedItem Content="Week" />
    <dataDisplay:SegmentedItem Content="Month" />
</dataDisplay:Segmented>
```

### Weekday Picker (Multiple)

```xml
<dataDisplay:Segmented
    SelectionMode="Multiple"
    SelectedKeys="{Binding SelectedWeekdays}"
    SelectedKeysChanged="OnWeekdaysChanged">
    <dataDisplay:SegmentedItem Key="mon" Content="Mon" />
    <dataDisplay:SegmentedItem Key="tue" Content="Tue" />
    <dataDisplay:SegmentedItem Key="wed" Content="Wed" />
    <dataDisplay:SegmentedItem Key="thu" Content="Thu" />
    <dataDisplay:SegmentedItem Key="fri" Content="Fri" />
    <dataDisplay:SegmentedItem Key="sat" Content="Sat" />
    <dataDisplay:SegmentedItem Key="sun" Content="Sun" />
</dataDisplay:Segmented>
```

```csharp
public ObservableCollection<string> SelectedWeekdays { get; set; } = ["mon", "fri"];
```

`SelectedKeys` binds two-way by default, so give the view-model property a setter.

### Text Style (Icon Only)

```xml
<dataDisplay:Segmented
    SelectionMode="Multiple"
    IconOnly="True"
    Size="Small"
    EqualSegmentWidth="False">
    <dataDisplay:SegmentedItem Key="bold" Content="Bold">
        <dataDisplay:SegmentedItem.Icon>
            <TextBlock FontWeight="Bold" Text="B" />
        </dataDisplay:SegmentedItem.Icon>
    </dataDisplay:SegmentedItem>
    <dataDisplay:SegmentedItem Key="italic" Content="Italic">
        <dataDisplay:SegmentedItem.Icon>
            <TextBlock FontStyle="Italic" Text="I" />
        </dataDisplay:SegmentedItem.Icon>
    </dataDisplay:SegmentedItem>
</dataDisplay:Segmented>
```

The text ("Bold") is hidden; it becomes the tooltip (unless the segment has its own) and the UI Automation name.

### Vertical

```xml
<dataDisplay:Segmented Orientation="Vertical" SelectionMode="Multiple" Width="160">
    <dataDisplay:SegmentedItem Content="Roads" />
    <dataDisplay:SegmentedItem Content="Terrain" />
    <dataDisplay:SegmentedItem Content="Traffic" />
</dataDisplay:Segmented>
```

### Separate Pills

```xml
<dataDisplay:Segmented
    IsJoined="False"
    ItemSpacing="8"
    CornerRadius="14"
    EqualSegmentWidth="False"
    SelectionMode="Multiple">
    <dataDisplay:SegmentedItem Content="E-mail" />
    <dataDisplay:SegmentedItem Content="SMS" />
    <dataDisplay:SegmentedItem Content="Push" />
</dataDisplay:Segmented>
```

### With Icons

```xml
<dataDisplay:Segmented SelectedIndex="0">
    <dataDisplay:SegmentedItem Content="List">
        <dataDisplay:SegmentedItem.Icon>
            <TextBlock Text="☰" />
        </dataDisplay:SegmentedItem.Icon>
    </dataDisplay:SegmentedItem>
    <dataDisplay:SegmentedItem Content="Grid">
        <dataDisplay:SegmentedItem.Icon>
            <TextBlock Text="▦" />
        </dataDisplay:SegmentedItem.Icon>
    </dataDisplay:SegmentedItem>
</dataDisplay:Segmented>
```

### 🎨 Custom Styling

```xml
<dataDisplay:Segmented
    SelectedIndex="1"
    SelectedBackground="DodgerBlue"
    SelectedForeground="White"
    HoverBackground="LightBlue"
    CornerRadius="8"
    EqualSegmentWidth="True">
    <dataDisplay:SegmentedItem Content="All" />
    <dataDisplay:SegmentedItem Content="Active" />
    <dataDisplay:SegmentedItem Content="Completed" />
</dataDisplay:Segmented>
```

### Data Binding

```xml
<dataDisplay:Segmented SelectedIndex="{Binding SelectedViewIndex}">
    <dataDisplay:SegmentedItem Content="Overview" />
    <dataDisplay:SegmentedItem Content="Details" />
    <dataDisplay:SegmentedItem Content="Settings" />
</dataDisplay:Segmented>
```

## ⚙️ Properties

### Segmented Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectionMode` | `SegmentedSelectionMode` | `Single` | `Single`, `SingleOrNone` or `Multiple` (see the modes table) |
| `SelectedIndex` | `int` | `-1` | Index of the selected segment (two-way bindable). In `Multiple`: the most recently selected segment that is still selected |
| `SelectedItem` | `SegmentedItem?` | `null` | Selected segment (two-way bindable). In `Multiple`: the most recently selected segment that is still selected |
| `SelectedItems` | `ReadOnlyObservableCollection<SegmentedItem>` | empty | The selected segments, in item order (read-only) |
| `SelectedKeys` | `ObservableCollection<string>` | empty | Keys of the selected segments; two-way by default, updated in place |
| `Orientation` | `Orientation` | `Horizontal` | `Vertical` stacks the segments; the corner rounding moves to the top and bottom |
| `IsJoined` | `bool` | `true` | `false` shows separate pill buttons with `ItemSpacing` between them |
| `ItemSpacing` | `double` | `4` | Space between separate pills (only when `IsJoined` is `false`) |
| `IconOnly` | `bool` | `false` | Hides the text of segments that have an icon; the text becomes the tooltip and the automation name |
| `Size` | `SegmentedSize` | `Medium` | `Small`, `Medium` or `Large` (font size and padding as `Chip`) |
| `CornerRadius` | `CornerRadius` | `4` | Corner radius of the outer pill shape (or of each pill when not joined) |
| `SelectedBackground` | `Brush?` | theme accent | Background of a selected segment |
| `SelectedForeground` | `Brush?` | theme | Foreground of a selected segment |
| `HoverBackground` | `Brush?` | theme | Background on hover |
| `SeparatorBrush` | `Brush?` | theme | Color of separators between segments |
| `EqualSegmentWidth` | `bool` | `true` | Whether all segments have equal width |
| `Items` | `ObservableCollection<SegmentedItem>` | empty | Collection of segment items |

### SegmentedItem Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Key` | `string?` | `null` | Key in `SelectedKeys`; falls back to the content text (a string, a `TextBlock`'s text, or `Content.ToString()`) |
| `IsSelected` | `bool` | `false` | Whether this segment is selected (set by the parent) |
| `Icon` | `object?` | `null` | Icon content before the label |
| `IconTemplate` | `DataTemplate?` | `null` | Template for icon content |
| `IsEnabled` | `bool` | `true` | A disabled segment is dimmed, skipped by the keyboard and cannot be toggled |
| `IsFirst` / `IsLast` / `Index` | `bool` / `bool` / `int` | — | Position in the parent (set by the parent) |
| `IsJoined` / `IsIconOnly` / `Size` | `bool` / `bool` / `SegmentedSize` | — | Presentation from the parent's `IsJoined`, `IconOnly` and `Size` (set by the parent) |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectionChanged` | `EventHandler<SegmentedSelectionChangedEventArgs>` | Raised when the selected segment changes. In `Multiple` it is raised for each toggle with the toggled segment as `NewItem` (check `NewItem.IsSelected`) |
| `SelectedKeysChanged` | `EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>` | Raised once per click, toggle, key press or `SelectionMode` change that changes `SelectedKeys` (old and new keys). Changes made to `SelectedKeys` from outside raise no event |

### SegmentedSelectionChangedEventArgs

| Property | Type | Description |
|----------|------|-------------|
| `OldIndex` | `int` | Previous selected index |
| `NewIndex` | `int` | New selected index (in `Multiple`: the toggled segment) |
| `OldItem` | `SegmentedItem?` | Previous selected item |
| `NewItem` | `SegmentedItem?` | New selected item (in `Multiple`: the toggled segment) |

## ⌨️ Keyboard Navigation

| Key | `Single` / `SingleOrNone` | `Multiple` |
|-----|---------------------------|------------|
| `Left` / `Right` (`Up` / `Down` when vertical) | Select the previous / next enabled segment | Move focus to the previous / next enabled segment |
| `Home` / `End` | Select the first / last enabled segment | Move focus to the first / last enabled segment |
| `Space` / `Enter` | Select the focused segment (`SingleOrNone`: clear it when selected) | Toggle the focused segment |

## 📝 Notes

- Corner radius is applied to the first and last segments to create one pill shape: left / right when horizontal, top / bottom when vertical; every pill gets the full radius when `IsJoined` is `false`
- Disabled items are skipped during keyboard navigation and cannot be toggled
- Separators are rendered between joined items and hidden next to a selected segment
- `SelectedKeys` is kept in sync in every mode; keys without a matching segment are kept (segments added later pick them up), while the keys of removed segments are dropped with one `SelectedKeysChanged`
- The selection follows the key, not the position: inserting or removing other segments keeps the selected segment (and moves `SelectedIndex`); removing the selected segment in a single mode clears the selection
- Switching from `Multiple` to a single mode keeps only the most recently selected segment (one `SelectedKeysChanged`; with a data-bound `SelectedKeys`, once the bindings have settled, so a DataContext swap that brings a new mode before its keys never writes into the previous view model's collection)
- In `Multiple`, setting `SelectedIndex` from outside selects that segment as well; setting it to `-1` leaves the selection as it is
- `SelectedIndex` and `SelectedItem` support two-way binding and journaling
- No `Label*` wrapper: put it in a `LabelContent` when a form needs a label

## 🔗 Related Controls

- **ChipGroup** - Wrapping row of toggle chips for 2–15 options or longer texts
- **MultiSelectComboBox** - Drop-down with check boxes when space is tight
- **TokenBox** - Text box with chips for large or open-ended sets
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **CheckListBox** - Always-visible check list for longer lists
- **DualListSelector** - Two lists for building an ordered list out of a pool
- **CardSelector** - Wrapping cards for few, visual choices
- **Chip** - Individual selectable elements for filtering
- **ToggleSwitch** - Binary on/off selection

## 🎮 Sample Application

See the Segmented sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Segmented** for interactive examples (weekday picker, icon-only text style, vertical map layers, separate pills and sizes).
