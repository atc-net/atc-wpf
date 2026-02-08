# 💀 Skeleton

A content wrapper that displays placeholder elements while data is loading.

## 🔍 Overview

`Skeleton` wraps your actual content and displays placeholder loading content while `IsLoading` is true. When loading completes, the skeleton content is replaced with the real content. Combine with `SkeletonElement` to create realistic loading placeholders that match your UI layout.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Progressing;
```

## 🚀 Usage

### Basic Skeleton

```xml
<progressing:Skeleton IsLoading="{Binding IsLoading}">
    <progressing:Skeleton.LoadingContent>
        <StackPanel>
            <progressing:SkeletonElement Shape="Circle" Width="40" Height="40" />
            <progressing:SkeletonElement Shape="Rectangle" Height="16" Margin="0,8,0,0" />
            <progressing:SkeletonElement Shape="Rectangle" Height="16" Width="200" Margin="0,4,0,0" />
        </StackPanel>
    </progressing:Skeleton.LoadingContent>

    <!-- Actual content shown when IsLoading=false -->
    <StackPanel>
        <TextBlock Text="{Binding UserName}" FontSize="16" />
        <TextBlock Text="{Binding Email}" />
    </StackPanel>
</progressing:Skeleton>
```

### With Data Template

```xml
<progressing:Skeleton IsLoading="{Binding IsLoading}">
    <progressing:Skeleton.LoadingContentTemplate>
        <DataTemplate>
            <StackPanel>
                <progressing:SkeletonElement Shape="Rounded" Height="200" />
                <progressing:SkeletonElement Height="20" Margin="0,12,0,0" />
                <progressing:SkeletonElement Height="14" Width="60%" Margin="0,8,0,0" />
            </StackPanel>
        </DataTemplate>
    </progressing:Skeleton.LoadingContentTemplate>

    <TextBlock Text="{Binding ArticleContent}" />
</progressing:Skeleton>
```

## ⚙️ Properties

### Skeleton Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsLoading` | `bool` | `false` | Whether to show loading content or actual content |
| `LoadingContent` | `object?` | `null` | Placeholder content shown during loading |
| `LoadingContentTemplate` | `DataTemplate?` | `null` | Template for loading content |

### SkeletonElement Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Shape` | `SkeletonShape` | `Rectangle` | Shape of the placeholder |
| `AnimationType` | `SkeletonAnimationType` | `Shimmer` | Type of loading animation |
| `CornerRadius` | `CornerRadius` | `4` | Border radius for the element |
| `IsActive` | `bool` | `true` | Whether the animation is running |

## 📊 Enumerations

### SkeletonShape

| Value | Description |
|-------|-------------|
| `Rectangle` | Rectangular placeholder (default) |
| `Circle` | Circular placeholder |
| `Rounded` | Rectangle with rounded corners |

### SkeletonAnimationType

| Value | Description |
|-------|-------------|
| `Shimmer` | Moving highlight effect (default) |
| `Pulse` | Fading in/out effect |
| `None` | Static gray placeholder |

## 📝 Notes

- `Skeleton` wraps content — when `IsLoading` is false, the child content is displayed normally
- `SkeletonElement` is designed to be used inside `Skeleton.LoadingContent` to create layout-matching placeholders
- Combine multiple `SkeletonElement` controls to mimic the shape of your actual content

## 🔗 Related Controls

- **LoadingIndicator** - Animated spinner indicators
- **BusyOverlay** - Full content overlay during async operations
- **Overlay** - Content dimming overlay

## 🎮 Sample Application

See the Skeleton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Progressing > Skeleton** for interactive examples.
