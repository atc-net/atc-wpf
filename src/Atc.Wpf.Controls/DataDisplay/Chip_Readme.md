# 🏷️ Chip

A compact element for tags, filters, selections, or action triggers.

## 🔍 Overview

`Chip` provides a small, interactive element commonly used for tags, filters, selections, and actions. It supports four variants (Default, Filter, Input, Action), three sizes, optional icons, selection toggling, and a removable close button. Chips follow Material Design patterns with hover, pressed, and selected visual states.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Chip

```xml
<dataDisplay:Chip Content="Technology" />
```

### 🔘 Filter Chips (Toggleable)

```xml
<!-- Filter chips automatically support selection -->
<StackPanel Orientation="Horizontal">
    <dataDisplay:Chip Content="JavaScript" Variant="Filter" Margin="0,0,4,0" />
    <dataDisplay:Chip Content="Python" Variant="Filter" Margin="0,0,4,0" />
    <dataDisplay:Chip Content="C#" Variant="Filter" IsSelected="True" Margin="0,0,4,0" />
</StackPanel>
```

### ❌ Input Chips (Removable)

```xml
<!-- Input chips automatically show a remove button -->
<dataDisplay:Chip Content="john@example.com" Variant="Input" RemoveClick="OnChipRemove" />
```

### 🖱️ Action Chips

```xml
<dataDisplay:Chip Content="Add to Cart" Variant="Action" Click="OnChipClick" />
```

### 🎨 With Icon

```xml
<dataDisplay:Chip Content="Location">
    <dataDisplay:Chip.Icon>
        <TextBlock Text="📍" />
    </dataDisplay:Chip.Icon>
</dataDisplay:Chip>
```

### 📏 Size Variants

```xml
<dataDisplay:Chip Content="Small" Size="Small" />
<dataDisplay:Chip Content="Medium" Size="Medium" />
<dataDisplay:Chip Content="Large" Size="Large" />
```

### 🎨 Custom Styling

```xml
<dataDisplay:Chip
    Content="Custom"
    CornerRadius="4"
    SelectedBackground="DarkBlue"
    SelectedForeground="White"
    HoverBackground="LightBlue"
    IsSelectable="True" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Variant` | `ChipVariant` | `Default` | Chip variant (Default, Filter, Input, Action) |
| `Size` | `ChipSize` | `Medium` | Size preset (Small, Medium, Large) |
| `Icon` | `object?` | `null` | Icon content displayed before main content |
| `IconTemplate` | `DataTemplate?` | `null` | Template for icon content |
| `CanRemove` | `bool` | `false` | Whether the remove button is shown |
| `IsSelectable` | `bool` | `false` | Whether the chip can be toggled on/off |
| `IsSelected` | `bool` | `false` | Whether the chip is currently selected |
| `IsClickable` | `bool` | `true` | Whether the chip shows hover/click effects |
| `CornerRadius` | `CornerRadius` | `16` | Corner radius (pill shape by default) |
| `SelectedBackground` | `Brush?` | Accent | Background when selected |
| `SelectedForeground` | `Brush?` | White | Foreground when selected |
| `HoverBackground` | `Brush?` | Accent3 | Background on hover |
| `PressedBackground` | `Brush?` | Accent2 | Background when pressed |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Click` | `RoutedEventHandler` | Raised when the chip is clicked |
| `RemoveClick` | `RoutedEventHandler` | Raised when the remove button is clicked |
| `SelectionChanged` | `RoutedEventHandler` | Raised when selection state changes |

## 📊 Enumerations

### ChipVariant

| Value | Description | Behavior |
|-------|-------------|----------|
| `Default` | Basic chip for displaying information | Standard display |
| `Filter` | Filter chip that can be toggled | Sets `IsSelectable=true`, shows checkmark when selected |
| `Input` | Input chip with remove button | Sets `CanRemove=true` |
| `Action` | Action chip that triggers an action | Standard clickable |

### ChipSize

| Value | Font Size | Padding | Corner Radius |
|-------|-----------|---------|---------------|
| `Small` | 11 | 8,4 | 12 |
| `Medium` | 13 | 12,6 | 16 |
| `Large` | 15 | 16,8 | 20 |

## 📝 Notes

- `IsSelected` supports two-way binding by default
- Filter variant automatically shows a checkmark icon when selected
- Input variant automatically shows a close/remove button
- Disabled chips render at 50% opacity
- The chip uses `MouseLeftButtonUp` for click handling
- `ShowCheckMark` (`bool?`, default `null`) forces the check mark on (`true`) or off (`false`) for a selected chip; `null` keeps the Filter-variant behaviour
- `SelectedBorderBrush` (default `null` = `SelectedBackground`) sets the border of a selected chip
- UI Automation: a selectable chip is a check box with the Toggle pattern

## 🔗 Related Controls

- **Badge** - Overlay content with counts or status indicators
- **Avatar** - User profile pictures, often combined with chips
- **ChipGroup** - A whole set of toggle chips bound to `Items` / `SelectedKeys` (see [ChipGroup_Readme.md](ChipGroup_Readme.md))
- **Segmented** - Group of mutually exclusive selections

## 🎮 Sample Application

See the Chip sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Chip** for interactive examples.
