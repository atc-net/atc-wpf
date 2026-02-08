# 🔴 Badge

A control that overlays content with a small indicator for counts, status dots, or custom markers.

## 🔍 Overview

`Badge` wraps any content and displays a small badge overlay at a configurable position. It supports numeric values with max-value formatting (e.g., "99+"), dot-only mode for simple status indicators, and automatic hiding when the value is zero. The badge position can be placed at any of 8 anchor points around the content.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Count Badge

```xml
<dataDisplay:Badge BadgeContent="5">
    <Button Content="Notifications" />
</dataDisplay:Badge>
```

### Max Value Formatting

```xml
<!-- Displays "99+" when value exceeds 99 -->
<dataDisplay:Badge BadgeContent="142" BadgeMaxValue="99">
    <Button Content="Messages" />
</dataDisplay:Badge>
```

### 🔵 Dot Indicator

```xml
<!-- Simple dot badge without content -->
<dataDisplay:Badge IsDot="True">
    <Button Content="Updates" />
</dataDisplay:Badge>
```

### 📍 Badge Placement

```xml
<!-- Badge at different positions -->
<dataDisplay:Badge BadgeContent="3" BadgePlacementMode="TopLeft">
    <Image Source="/Assets/icon.png" Width="32" Height="32" />
</dataDisplay:Badge>

<dataDisplay:Badge BadgeContent="!" BadgePlacementMode="BottomRight">
    <Image Source="/Assets/icon.png" Width="32" Height="32" />
</dataDisplay:Badge>
```

### 🎨 Custom Styling

```xml
<dataDisplay:Badge
    BadgeContent="NEW"
    BadgeBackground="Green"
    BadgeForeground="White"
    BadgeCornerRadius="4"
    BadgeFontSize="9"
    BadgePadding="6,2,6,2">
    <Button Content="Feature" />
</dataDisplay:Badge>
```

### Hide When Zero

```xml
<!-- Badge disappears when count is 0 -->
<dataDisplay:Badge BadgeContent="{Binding UnreadCount}" HideWhenZero="True">
    <Button Content="Inbox" />
</dataDisplay:Badge>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BadgeContent` | `object?` | `null` | Content displayed in the badge |
| `BadgeContentTemplate` | `DataTemplate?` | `null` | Custom template for badge content |
| `BadgeBackground` | `Brush?` | Accent | Background brush for the badge |
| `BadgeForeground` | `Brush?` | White | Foreground brush for badge text |
| `BadgeBorderBrush` | `Brush?` | Transparent | Border brush for the badge |
| `BadgeBorderThickness` | `Thickness` | `0` | Border thickness |
| `BadgeCornerRadius` | `CornerRadius` | `8` | Corner radius for rounded badge |
| `BadgePlacementMode` | `BadgePlacementMode` | `TopRight` | Position of badge relative to content |
| `BadgeMargin` | `Thickness` | `0` | Additional margin for fine-tuning position |
| `BadgeFontSize` | `double` | `10` | Font size for badge content |
| `BadgeMinWidth` | `double` | `16` | Minimum width of the badge |
| `BadgeMinHeight` | `double` | `16` | Minimum height of the badge |
| `BadgePadding` | `Thickness` | `4,2,4,2` | Inner padding for badge content |
| `IsBadgeVisible` | `bool` | `true` | Whether the badge is visible |
| `IsDot` | `bool` | `false` | Show as simple dot (ignores BadgeContent) |
| `BadgeMaxValue` | `int` | `0` | Max displayable value (0 = no limit). Shows "N+" when exceeded |
| `HideWhenZero` | `bool` | `false` | Auto-hide badge when content is zero |

## 📊 Enumerations

### BadgePlacementMode

| Value | Description |
|-------|-------------|
| `TopLeft` | Top-left corner |
| `Top` | Top center |
| `TopRight` | Top-right corner (default) |
| `Right` | Right center |
| `BottomRight` | Bottom-right corner |
| `Bottom` | Bottom center |
| `BottomLeft` | Bottom-left corner |
| `Left` | Left center |

## 📝 Notes

- In `IsDot` mode, the badge renders as a small 10x10 circle regardless of content
- `BadgeMaxValue` only applies to numeric content (int, long, double, or parseable strings)
- The badge auto-hides when `HideWhenZero` is true and the numeric value is 0
- Default theme uses accent color background with ideal foreground

## 🔗 Related Controls

- **Avatar** - User profile pictures with status indicators
- **Chip** - Compact elements for tags and filters
- **Card** - Content container that badges can overlay

## 🎮 Sample Application

See the Badge sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Badge** for interactive examples.
