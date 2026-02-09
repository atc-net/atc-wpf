# 💬 Popover

A control that displays popup content positioned relative to an anchor element.

## 🔍 Overview

`Popover` wraps an anchor element and shows popup content at a configurable position when triggered. It supports 12 placement options (Top/Bottom/Left/Right combined with Start/Center/End alignment), three trigger modes (Click, Hover, Manual), an optional arrow pointer, open/close delays, max size constraints, and automatic flip behavior when the popup would go off-screen.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Click Popover

```xml
<dataDisplay:Popover>
    <dataDisplay:Popover.Anchor>
        <Button Content="Click Me" />
    </dataDisplay:Popover.Anchor>
    <dataDisplay:Popover.PopoverContent>
        <TextBlock Text="Hello from the popover!" />
    </dataDisplay:Popover.PopoverContent>
</dataDisplay:Popover>
```

### Hover Trigger with Delay

```xml
<dataDisplay:Popover TriggerMode="Hover" OpenDelay="200" CloseDelay="300">
    <dataDisplay:Popover.Anchor>
        <TextBlock Text="Hover over me" />
    </dataDisplay:Popover.Anchor>
    <dataDisplay:Popover.PopoverContent>
        <TextBlock Text="Tooltip-style popover" />
    </dataDisplay:Popover.PopoverContent>
</dataDisplay:Popover>
```

### Custom Placement

```xml
<dataDisplay:Popover Placement="RightStart">
    <dataDisplay:Popover.Anchor>
        <Button Content="Open Right" />
    </dataDisplay:Popover.Anchor>
    <dataDisplay:Popover.PopoverContent>
        <TextBlock Text="Aligned to the right, top-aligned" />
    </dataDisplay:Popover.PopoverContent>
</dataDisplay:Popover>
```

### Programmatic Control (Manual Trigger)

```xml
<dataDisplay:Popover x:Name="MyPopover" TriggerMode="Manual" IsOpen="{Binding IsPopoverOpen}">
    <dataDisplay:Popover.Anchor>
        <Button Content="Anchor" Click="OnTogglePopover" />
    </dataDisplay:Popover.Anchor>
    <dataDisplay:Popover.PopoverContent>
        <TextBlock Text="Manually controlled" />
    </dataDisplay:Popover.PopoverContent>
</dataDisplay:Popover>
```

### Constrained Size

```xml
<dataDisplay:Popover MaxPopoverWidth="250" MaxPopoverHeight="150">
    <dataDisplay:Popover.Anchor>
        <Button Content="Show" />
    </dataDisplay:Popover.Anchor>
    <dataDisplay:Popover.PopoverContent>
        <TextBlock Text="Content with max size constraints" TextWrapping="Wrap" />
    </dataDisplay:Popover.PopoverContent>
</dataDisplay:Popover>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Anchor` | `object?` | `null` | The anchor element the popover is positioned relative to |
| `AnchorTemplate` | `DataTemplate?` | `null` | Optional template for the anchor content |
| `PopoverContent` | `object?` | `null` | The content displayed inside the popover |
| `PopoverContentTemplate` | `DataTemplate?` | `null` | Optional template for the popover content |
| `IsOpen` | `bool` | `false` | Whether the popover is currently open (two-way bindable) |
| `Placement` | `PopoverPlacement` | `Bottom` | Position relative to the anchor |
| `TriggerMode` | `PopoverTriggerMode` | `Click` | How the popover is triggered (Click, Hover, Manual) |
| `ShowArrow` | `bool` | `true` | Whether to show the arrow pointer |
| `OpenDelay` | `int` | `0` | Milliseconds delay before opening (for Hover mode) |
| `CloseDelay` | `int` | `200` | Milliseconds delay before closing (for Hover mode) |
| `MaxPopoverWidth` | `double` | `Infinity` | Maximum width of the popover |
| `MaxPopoverHeight` | `double` | `Infinity` | Maximum height of the popover |
| `CornerRadius` | `CornerRadius` | `4` | Corner radius of the popover border |
| `PopoverBackground` | `Brush?` | Theme background | Background brush for the popover |
| `PopoverBorderBrush` | `Brush?` | Gray6 | Border brush for the popover |
| `PopoverBorderThickness` | `Thickness` | `1` | Border thickness of the popover |
| `PopoverPadding` | `Thickness` | `8` | Inner padding of the popover content |

## 🎯 Events

| Event | Type | Description |
|-------|------|-------------|
| `Opened` | `RoutedEventHandler` | Raised when the popover is opened |
| `Closed` | `RoutedEventHandler` | Raised when the popover is closed |

## 📊 Enumerations

### PopoverPlacement

| Value | Description |
|-------|-------------|
| `TopStart` | Above the anchor, left-aligned |
| `Top` | Above the anchor, centered |
| `TopEnd` | Above the anchor, right-aligned |
| `BottomStart` | Below the anchor, left-aligned |
| `Bottom` | Below the anchor, centered (default) |
| `BottomEnd` | Below the anchor, right-aligned |
| `LeftStart` | Left of the anchor, top-aligned |
| `Left` | Left of the anchor, centered |
| `LeftEnd` | Left of the anchor, bottom-aligned |
| `RightStart` | Right of the anchor, top-aligned |
| `Right` | Right of the anchor, centered |
| `RightEnd` | Right of the anchor, bottom-aligned |

### PopoverTriggerMode

| Value | Description |
|-------|-------------|
| `Click` | Opens on click, closes when clicking outside (default) |
| `Hover` | Opens on mouse enter, closes on mouse leave |
| `Manual` | Controlled programmatically via `IsOpen` |

## 📝 Notes

- In `Click` mode, clicking outside the popover dismisses it (light dismiss)
- In `Hover` mode, `OpenDelay` and `CloseDelay` control timing; moving from anchor to popup keeps it open
- The popover automatically flips to the opposite side when constrained by screen edges
- Press `Escape` to close an open popover
- The popover uses `Popup` internally with `AllowsTransparency` for the shadow effect

## 🔗 Related Controls

- **Flyout** - Side panel overlay for larger content areas
- **SplitButton** - Button with dropdown popup
- **Badge** - Small overlay indicator on content

## 🎮 Sample Application

See the Popover sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Popover** for interactive examples.
