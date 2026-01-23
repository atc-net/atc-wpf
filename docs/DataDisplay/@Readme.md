# 📊 Data Display Controls

This document provides a comprehensive overview of all data display controls available in **Atc.Wpf.Controls**. These controls are designed for presenting information visually rather than collecting user input.

---

## 📊 Quick Reference

| Control | Primary Use Case | Interactive | Theming |
|---------|------------------|-------------|---------|
| [Avatar](#-avatar) | User profile pictures | ❌ No | ✅ Yes |
| [AvatarGroup](#-avatargroup) | Overlapping user avatars | ❌ No | ✅ Yes |
| [Badge](#-badge) | Status indicator overlay | ❌ No | ✅ Yes |
| [Card](#-card) | Content grouping with elevation | ✅ Expandable | ✅ Yes |
| [Chip](#-chip) | Tags, filters, selections | ✅ Selectable | ✅ Yes |
| [Divider](#-divider) | Visual separator | ❌ No | ✅ Yes |

---

## 🎨 Controls

### 🟢 Avatar

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** A control for displaying user profile pictures with intelligent fallback behavior. When no image is provided, it shows initials (auto-generated from the display name or explicitly set). Background color can be auto-generated from the user's name using a deterministic hash algorithm.

**When to Use:**
- User profile displays
- Comment/chat user indicators
- Team member lists
- Contact lists

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| ImageSource | ImageSource? | null | Profile image source |
| Initials | string? | null | Explicit initials (e.g., "JD") |
| DisplayName | string? | null | Full name for auto-generating initials and color |
| Size | AvatarSize | Medium | Size preset (ExtraSmall, Small, Medium, Large, ExtraLarge) |
| Status | AvatarStatus | None | Status indicator (None, Online, Offline, Away, Busy, DoNotDisturb) |
| Background | Brush? | auto | Override background (auto-generated from name if null) |
| Foreground | Brush? | White | Initials text color |
| CornerRadius | CornerRadius? | circular | For square/rounded avatars |
| StatusBorderBrush | Brush? | theme | Border around status indicator |
| StatusBorderThickness | double | 2 | Status indicator border thickness |

**Size Presets:**

| Size | Pixels | Font Size | Status Size |
|------|--------|-----------|-------------|
| ExtraSmall | 24 | 10 | 8 |
| Small | 32 | 12 | 10 |
| Medium | 40 | 14 | 12 |
| Large | 56 | 18 | 14 |
| ExtraLarge | 80 | 24 | 18 |

**Examples:**

```xml
<!-- Auto-generated initials and color from name -->
<dataDisplay:Avatar DisplayName="John Doe" Size="Large" />

<!-- With status indicator -->
<dataDisplay:Avatar DisplayName="Jane Smith" Status="Online" Size="Medium" />

<!-- Explicit initials -->
<dataDisplay:Avatar Initials="AB" Size="Large" />

<!-- Square avatar -->
<dataDisplay:Avatar DisplayName="Square" CornerRadius="4" Size="Large" />
```

---

### 🟢 AvatarGroup

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** Displays multiple avatars with overlapping layout and overflow indicator.

**When to Use:**
- Team member lists
- Assignee displays
- Participant indicators

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Spacing | double | -12 | Spacing between avatars (negative for overlap) |
| MaxVisible | int | 5 | Maximum avatars shown before "+N" overflow |
| Size | AvatarSize | Medium | Size applied to all child avatars |
| OverflowBackground | Brush? | Gray | Background for overflow indicator |
| OverflowForeground | Brush? | White | Text color for overflow indicator |

**Examples:**

```xml
<dataDisplay:AvatarGroup MaxVisible="3" Size="Medium">
    <dataDisplay:Avatar DisplayName="Alice Johnson" />
    <dataDisplay:Avatar DisplayName="Bob Smith" />
    <dataDisplay:Avatar DisplayName="Carol Williams" />
    <dataDisplay:Avatar DisplayName="David Brown" />
    <dataDisplay:Avatar DisplayName="Eve Davis" />
</dataDisplay:AvatarGroup>
<!-- Shows 3 avatars + "+2" overflow indicator -->
```

---

### 🟢 Badge

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** A small status indicator that can be attached to other controls.

**When to Use:**
- Notification counts
- Online/offline status
- New item indicators

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| BadgeContent | object | - | Badge content (text, number, icon) |
| BadgePlacementMode | BadgePlacementMode | TopRight | Position (TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left) |
| BadgeBackground | Brush | - | Badge background color |
| BadgeForeground | Brush | - | Badge text color |
| BadgeBorderBrush | Brush | - | Badge border color |
| BadgeCornerRadius | CornerRadius | - | Badge corner rounding |
| BadgeMargin | Thickness | - | Fine position adjustment |
| BadgeFontSize | double | - | Badge text size |
| BadgeMinWidth | double | - | Minimum badge width |
| BadgeMinHeight | double | - | Minimum badge height |
| BadgePadding | Thickness | - | Badge inner padding |
| IsBadgeVisible | bool | true | Manual visibility control |
| IsDot | bool | false | Show as dot (no content) |
| BadgeMaxValue | int | - | Maximum display value (shows "99+" style) |
| HideWhenZero | bool | false | Auto-hide when content is 0 |
| BadgeContentTemplate | DataTemplate | - | Custom content template |

**Examples:**

```xml
<!-- Notification count -->
<dataDisplay:Badge BadgeContent="5" BadgePlacementMode="TopRight">
    <Button Content="Inbox" />
</dataDisplay:Badge>

<!-- Status dot -->
<dataDisplay:Badge IsDot="True" BadgeBackground="LimeGreen" BadgePlacementMode="BottomRight">
    <Border Width="50" Height="50" CornerRadius="25" Background="Gray" />
</dataDisplay:Badge>

<!-- Max count with auto-hide -->
<dataDisplay:Badge
    BadgeContent="{Binding UnreadCount}"
    BadgeMaxValue="99"
    HideWhenZero="True">
    <Button Content="Messages" />
</dataDisplay:Badge>
```

---

### 🟢 Card

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** A modern container control for grouping related content with elevation/shadow, optional header/footer, and expand/collapse functionality.

**When to Use:**
- Content grouping
- Dashboard widgets
- Settings sections
- Any elevated container

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Elevation | int | 2 | Shadow depth (0-5) |
| Header | object | - | Header content |
| HeaderBackground | Brush | - | Header background color |
| HeaderForeground | Brush | - | Header text color |
| HeaderPadding | Thickness | 12 | Header inner padding |
| ShowHeader | bool | true | Whether to show header |
| Footer | object | - | Footer content |
| FooterBackground | Brush | - | Footer background color |
| FooterPadding | Thickness | 12 | Footer inner padding |
| ShowFooter | bool | true | Whether to show footer |
| IsExpandable | bool | false | Enable expand/collapse |
| IsExpanded | bool | true | Current expanded state |
| ExpanderButtonLocation | ExpanderButtonLocation | Left | Toggle button position |
| CornerRadius | CornerRadius | 4 | Corner rounding |
| ContentPadding | Thickness | 12 | Content area padding |

**Examples:**

```xml
<!-- Basic card -->
<dataDisplay:Card Header="Settings" Elevation="2">
    <StackPanel>
        <CheckBox Content="Enable notifications" />
        <CheckBox Content="Dark mode" />
    </StackPanel>
</dataDisplay:Card>

<!-- Expandable card -->
<dataDisplay:Card
    Header="Advanced Options"
    IsExpandable="True"
    IsExpanded="False"
    ExpanderButtonLocation="Right">
    <StackPanel>
        <CheckBox Content="Option 1" />
        <CheckBox Content="Option 2" />
    </StackPanel>
</dataDisplay:Card>
```

---

### 🟢 Chip

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** Small, interactive elements for displaying tags, filters, or selections.

**When to Use:**
- Filter tags
- Selected items display
- Category labels
- Email recipients

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Content | object | - | Chip text/content |
| Icon | object | - | Icon before text |
| IconTemplate | DataTemplate | - | Custom icon template |
| Variant | ChipVariant | Default | Default, Filter, Input, Action |
| Size | ChipSize | Medium | Small, Medium, Large |
| IsSelectable | bool | false | Can be selected/toggled |
| IsSelected | bool | false | Current selection state |
| CanRemove | bool | false | Shows remove button |
| IsClickable | bool | false | Responds to clicks |
| CornerRadius | CornerRadius | - | Corner rounding |
| SelectedBackground | Brush | - | Background when selected |
| SelectedForeground | Brush | - | Foreground when selected |
| HoverBackground | Brush | - | Background on hover |
| PressedBackground | Brush | - | Background when pressed |

**Events:**

| Event | Description |
|-------|-------------|
| Click | Raised when chip is clicked |
| RemoveClick | Raised when remove button is clicked |
| SelectionChanged | Raised when selection state changes |

**Examples:**

```xml
<!-- Basic chip -->
<dataDisplay:Chip Content="C#" />

<!-- Filter chip (selectable) -->
<dataDisplay:Chip Content="JavaScript" Variant="Filter" IsSelected="True" />

<!-- Input chip (removable) -->
<dataDisplay:Chip
    Content="john@example.com"
    Variant="Input"
    RemoveClick="OnChipRemove" />
```

---

### 🟢 Divider

**Location:** `Atc.Wpf.Controls.DataDisplay`

**Description:** A visual separator line, optionally with content.

**When to Use:**
- Separating sections
- Visual breaks in forms
- Section headings

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Content | object | - | Center content (text, icon) |
| Orientation | Orientation | Horizontal | Horizontal or Vertical |
| ContentTemplate | DataTemplate | - | Custom content template |
| LineStroke | Brush | - | Line color |
| LineStrokeThickness | double | 1 | Line thickness |
| LineStrokeDashArray | DoubleCollection | - | Dash pattern for dashed lines |

**Examples:**

```xml
<!-- Simple horizontal divider -->
<dataDisplay:Divider />

<!-- Divider with text -->
<dataDisplay:Divider Content="OR" />

<!-- Dashed divider -->
<dataDisplay:Divider LineStrokeDashArray="4,2" />

<!-- Vertical divider -->
<StackPanel Orientation="Horizontal">
    <TextBlock Text="Left" />
    <dataDisplay:Divider Orientation="Vertical" Margin="8,0" />
    <TextBlock Text="Right" />
</StackPanel>
```

---

## 📚 Summary

| Control | Interactive | Status Support | Grouping |
|---------|-------------|----------------|----------|
| Avatar | ❌ | ✅ Yes | via AvatarGroup |
| Badge | ❌ | ✅ Yes | ❌ |
| Card | ✅ Expandable | ❌ | ✅ Yes |
| Chip | ✅ Selectable | ❌ | ❌ |
| Divider | ❌ | ❌ | ❌ |

---

*📅 Last Updated: January 2026*
*🤖 Generated with Claude Code*
