# 🔘 Segmented

A group of mutually exclusive segments for single-selection (tab bar, view switcher, filter toggle).

## 🔍 Overview

`Segmented` provides a horizontal row of selectable segments where only one can be active at a time. It supports keyboard navigation, equal or auto-sized segments, visual separators between items, and custom icons per segment. The control automatically manages corner radius for the first and last items to create a unified pill-shaped appearance.

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
| `SelectedIndex` | `int` | `-1` | Index of the selected segment (two-way bindable) |
| `SelectedItem` | `SegmentedItem?` | `null` | Currently selected item (two-way bindable) |
| `CornerRadius` | `CornerRadius` | theme | Corner radius for the outer pill shape |
| `SelectedBackground` | `Brush?` | `null` | Background of the selected segment |
| `SelectedForeground` | `Brush?` | `null` | Foreground of the selected segment |
| `HoverBackground` | `Brush?` | `null` | Background on hover |
| `SeparatorBrush` | `Brush?` | `null` | Color of separators between segments |
| `EqualSegmentWidth` | `bool` | `true` | Whether all segments have equal width |
| `Items` | `ObservableCollection<SegmentedItem>` | empty | Collection of segment items |

### SegmentedItem Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsSelected` | `bool` | `false` | Whether this segment is selected |
| `Icon` | `object?` | `null` | Icon content before the label |
| `IconTemplate` | `DataTemplate?` | `null` | Template for icon content |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectionChanged` | `EventHandler<SegmentedSelectionChangedEventArgs>` | Raised when selection changes |

### SegmentedSelectionChangedEventArgs

| Property | Type | Description |
|----------|------|-------------|
| `OldIndex` | `int` | Previous selected index |
| `NewIndex` | `int` | New selected index |
| `OldItem` | `SegmentedItem?` | Previous selected item |
| `NewItem` | `SegmentedItem?` | New selected item |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Left` | Select previous enabled segment |
| `Right` | Select next enabled segment |
| `Home` | Select first enabled segment |
| `End` | Select last enabled segment |

## 📝 Notes

- Corner radius is automatically applied to first/last items to create a unified pill shape
- Disabled items are skipped during keyboard navigation
- Separators are rendered dynamically between items
- `SelectedIndex` and `SelectedItem` support two-way binding and journaling

## 🔗 Related Controls

- **Chip** - Individual selectable elements for filtering
- **ToggleSwitch** - Binary on/off selection

## 🎮 Sample Application

See the Segmented sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Segmented** for interactive examples.
