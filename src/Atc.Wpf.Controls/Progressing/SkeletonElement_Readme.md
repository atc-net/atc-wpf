# 💀 SkeletonElement

An animated placeholder shape used to build skeleton loading screens.

## 🔍 Overview

`SkeletonElement` renders a gray placeholder block (rectangle, circle or rounded rectangle) with an optional shimmer or pulse animation. Combine several elements to mimic the layout of the content that is loading, typically inside `Skeleton.LoadingContent` so the placeholders are swapped for the real content when loading completes.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Progressing;
```

## 🚀 Usage

### Basic Example

```xml
<StackPanel>
    <progressing:SkeletonElement Width="40" Height="40" Shape="Circle" />
    <progressing:SkeletonElement Margin="0,8,0,0" Height="16" />
    <progressing:SkeletonElement Margin="0,4,0,0" Width="200" Height="16" />
</StackPanel>
```

### Inside a Skeleton

```xml
<progressing:Skeleton IsLoading="{Binding IsLoading}">
    <progressing:Skeleton.LoadingContent>
        <StackPanel>
            <progressing:SkeletonElement Height="120" Shape="Rounded" CornerRadius="8" />
            <progressing:SkeletonElement Margin="0,12,0,0" Height="20" />
        </StackPanel>
    </progressing:Skeleton.LoadingContent>

    <!-- Actual content shown when IsLoading=false -->
    <TextBlock Text="{Binding ArticleContent}" />
</progressing:Skeleton>
```

### Animation Types

```xml
<progressing:SkeletonElement AnimationType="Shimmer" />
<progressing:SkeletonElement AnimationType="Pulse" />
<progressing:SkeletonElement AnimationType="None" />

<!-- Stop the animation without changing the type -->
<progressing:SkeletonElement IsActive="False" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Shape` | `SkeletonShape` | `Rectangle` | Shape of the placeholder |
| `AnimationType` | `SkeletonAnimationType` | `Shimmer` | Type of loading animation |
| `CornerRadius` | `CornerRadius` | `4` | Corner radius of the placeholder (applied for `Rounded`) |
| `IsActive` | `bool` | `true` | Whether the animation is running |

## 📊 Enumerations

### SkeletonShape

| Value | Description |
|-------|-------------|
| `Rectangle` | Rectangular placeholder with square corners (default) |
| `Circle` | Circular placeholder (use equal `Width` and `Height`) |
| `Rounded` | Rectangle using the `CornerRadius` property |

### SkeletonAnimationType

| Value | Description |
|-------|-------------|
| `Shimmer` | Moving highlight gradient (1.5 s cycle) (default) |
| `Pulse` | Fading in/out effect (1.0 s cycle) |
| `None` | Static gray placeholder |

## 📝 Notes

- The default style sets `Height="16"` and `HorizontalAlignment="Stretch"`, so an element without explicit size fills the available width as a text-line placeholder
- `Shape="Rectangle"` forces square corners and `Shape="Circle"` forces a fully rounded shape; `CornerRadius` only takes effect with `Shape="Rounded"`
- The placeholder is drawn with the `AtcApps.Brushes.Gray8` theme brush and follows Light/Dark theme changes
- The element is not focusable and is excluded from tab navigation
- A separate control template is selected per `AnimationType`

## 🔗 Related Controls

- **Skeleton** - Wrapper that swaps placeholder content for real content when loading completes (see `Skeleton_Readme.md`)
- **LoadingIndicator** - Animated spinner indicators
- **BusyOverlay** - Overlay with a busy indicator over existing content

## 🎮 Sample Application

See the Skeleton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Progressing > Skeleton** for interactive examples.
