# 👥 AvatarGroup

A control for displaying a group of overlapping avatars with a "+N" overflow indicator.

## 🔍 Overview

`AvatarGroup` lays out a collection of `Avatar` controls horizontally with configurable (typically negative) spacing so they overlap. It applies a unified size to all child avatars, shows at most `MaxVisible` of them, and appends a "+N" avatar for the remaining count. Later avatars are placed on top of earlier ones.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Example

```xml
<atc:AvatarGroup MaxVisible="3" Size="Medium">
    <atc:Avatar DisplayName="Alice Johnson" />
    <atc:Avatar DisplayName="Bob Smith" />
    <atc:Avatar DisplayName="Carol Williams" />
    <atc:Avatar DisplayName="David Brown" />
    <atc:Avatar DisplayName="Eve Davis" />
</atc:AvatarGroup>
<!-- Shows 3 avatars + "+2" overflow indicator -->
```

### Custom Spacing and Overflow Colors

```xml
<atc:AvatarGroup
    MaxVisible="4"
    OverflowBackground="DarkSlateBlue"
    OverflowForeground="White"
    Size="Large"
    Spacing="-20">
    <atc:Avatar DisplayName="Alice Johnson" Status="Online" />
    <atc:Avatar DisplayName="Bob Smith" Status="Away" />
    <atc:Avatar DisplayName="Carol Williams" />
    <atc:Avatar DisplayName="David Brown" />
    <atc:Avatar DisplayName="Eve Davis" />
    <atc:Avatar DisplayName="Frank Miller" />
</atc:AvatarGroup>
```

### Non-overlapping Group

```xml
<atc:AvatarGroup Spacing="4" Size="Small">
    <atc:Avatar Initials="AB" />
    <atc:Avatar Initials="CD" />
    <atc:Avatar Initials="EF" />
</atc:AvatarGroup>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Spacing` | `double` | `-12` | Left margin applied to each avatar after the first (negative values overlap) |
| `MaxVisible` | `int` | `5` | Maximum avatars shown before the "+N" overflow indicator |
| `Size` | `AvatarSize` | `Medium` | Size applied to all child avatars and the overflow indicator |
| `OverflowBackground` | `Brush?` | `null` | Background of the overflow indicator (falls back to `Gray` when `null`) |
| `OverflowForeground` | `Brush?` | `null` | Text color of the overflow indicator (falls back to `White` when `null`) |
| `Children` | `ObservableCollection<Avatar>` | empty | The avatars in the group (content property, read-only collection) |

## 📝 Notes

- `Children` is the XAML content property, so `Avatar` elements can be declared directly inside the group
- Setting `Size` on the group overrides the `Size` of each child `Avatar`
- The group overrides each child's `Margin` and `Panel.ZIndex` to produce the overlap (later items on top)
- The overflow indicator is itself an `Avatar` with `Initials` set to `+N`
- Layout is rebuilt when `Children` changes or when `Spacing`, `MaxVisible` or `Size` change; changing `OverflowBackground`/`OverflowForeground` takes effect on the next rebuild
- The default template hosts the avatars in a horizontal `StackPanel` named `PART_ItemsHost`

## 🔗 Related Controls

- **Avatar** - The individual avatar control (see `Avatar_Readme.md` for `AvatarSize` values)
- **Badge** - Overlay content with a small indicator (count, status dot)
- **Chip** - Compact elements for tags, filters, or selections

## 🎮 Sample Application

See the Avatar sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Avatar** for interactive examples.
