# 👤 Avatar

A control for displaying user profile pictures with fallback options for initials.

## 🔍 Overview

`Avatar` displays user profile pictures with intelligent fallback behavior. When no image is provided, it shows initials (auto-generated from the display name or explicitly set). The background color can be auto-generated from the user's name using a deterministic hash algorithm, ensuring consistent colors for the same user across the application.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Avatar with Image

```xml
<atc:Avatar ImageSource="/Assets/profile.png" Size="Large" />
```

### Avatar with Auto-Generated Initials

```xml
<!-- Initials "JD" and a consistent color will be generated -->
<atc:Avatar DisplayName="John Doe" Size="Medium" />
```

### Avatar with Explicit Initials

```xml
<atc:Avatar Initials="AB" Size="Large" />
```

### 🟢 Avatar with Status Indicator

```xml
<atc:Avatar DisplayName="John Doe" Status="Online" Size="Large" />
<atc:Avatar DisplayName="Jane Smith" Status="Away" Size="Large" />
<atc:Avatar DisplayName="Bob Wilson" Status="Busy" Size="Large" />
<atc:Avatar DisplayName="Alice Brown" Status="DoNotDisturb" Size="Large" />
<atc:Avatar DisplayName="Sam Davis" Status="Offline" Size="Large" />
```

### 🎨 Avatar with Custom Styling

```xml
<!-- Custom background color -->
<atc:Avatar DisplayName="Custom" Background="Purple" Size="Large" />

<!-- Square avatar with rounded corners -->
<atc:Avatar DisplayName="Square" CornerRadius="4" Size="Large" />

<!-- Rounded square -->
<atc:Avatar DisplayName="Rounded" CornerRadius="12" Size="Large" />
```

### 👥 Avatar Group (Overlapping Avatars)

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

## ⚙️ Properties

### Avatar Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ImageSource` | `ImageSource?` | `null` | Profile image source |
| `Initials` | `string?` | `null` | Explicit initials to display (e.g., "JD") |
| `DisplayName` | `string?` | `null` | Full name for auto-generating initials and background color |
| `Size` | `AvatarSize` | `Medium` | Size preset (ExtraSmall, Small, Medium, Large, ExtraLarge) |
| `Status` | `AvatarStatus` | `None` | Status indicator (None, Online, Offline, Away, Busy, DoNotDisturb) |
| `Background` | `Brush?` | auto | Override background (auto-generated from name if null) |
| `Foreground` | `Brush?` | `White` | Initials text color |
| `CornerRadius` | `CornerRadius?` | circular | For square/rounded avatars (circular by default) |
| `StatusBorderBrush` | `Brush?` | theme | Border around status indicator |
| `StatusBorderThickness` | `double` | `2` | Status indicator border thickness |

### AvatarGroup Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Spacing` | `double` | `-12` | Spacing between avatars (negative for overlap) |
| `MaxVisible` | `int` | `5` | Maximum avatars shown before "+N" overflow |
| `Size` | `AvatarSize` | `Medium` | Size applied to all child avatars |
| `OverflowBackground` | `Brush?` | `Gray` | Background for overflow indicator |
| `OverflowForeground` | `Brush?` | `White` | Text color for overflow indicator |

## 📊 Enumerations

### AvatarSize

| Value | Pixels | Font Size | Status Size |
|-------|--------|-----------|-------------|
| `ExtraSmall` | 24 | 10 | 8 |
| `Small` | 32 | 12 | 10 |
| `Medium` | 40 | 14 | 12 |
| `Large` | 56 | 18 | 14 |
| `ExtraLarge` | 80 | 24 | 18 |

### AvatarStatus

| Value | Color | Description |
|-------|-------|-------------|
| `None` | — | No status indicator shown |
| `Online` | LimeGreen | User is online and available |
| `Away` | Orange | User is away from their device |
| `Busy` | Red | User is busy |
| `DoNotDisturb` | Red + dash | User does not want to be disturbed |
| `Offline` | Gray | User is offline |

## 🔤 Initials Generation

When `DisplayName` is provided but `Initials` is not, initials are automatically generated:

| Input | Output |
|-------|--------|
| `"John"` | `"JO"` |
| `"John Doe"` | `"JD"` |
| `"Mary Jane Watson"` | `"MW"` (first + last) |
| `""` or `null` | `"?"` |

## 🎨 Color Generation

Background colors are deterministically generated from names using HSV color space:

- Same name always produces the same color
- Colors are visually pleasant (fixed saturation: 0.65, value: 0.75)
- Only the hue varies (0-360) based on a stable hash of the name
- Empty/null names default to gray

## 📝 Notes

- The `ImageSource` takes precedence over initials when both are provided
- Background color is auto-generated only when not explicitly set
- Circular shape is default; use `CornerRadius` for square/rounded variants
- `AvatarGroup` automatically manages z-order (later items appear on top)
- Status indicator is positioned at bottom-right
- `DoNotDisturb` status includes a small dash icon

## 🔗 Related Controls

- **Badge** - Overlay content with a small indicator (count, status dot)
- **Chip** - Compact elements for tags, filters, or selections

## 🎮 Sample Application

See the Avatar sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Avatar** for interactive examples.
