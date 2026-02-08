# 🔃 ReversibleStackPanel

A StackPanel that can reverse the visual order of its children.

## 🔍 Overview

`ReversibleStackPanel` extends `StackPanel` with a `ReverseOrder` property. When enabled, children are arranged in reverse order without modifying the logical collection. This is useful for chat-style layouts, bottom-up lists, or any scenario where display order should differ from collection order.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

```xml
<!-- Normal order -->
<layouts:ReversibleStackPanel>
    <TextBlock Text="First" />
    <TextBlock Text="Second" />
    <TextBlock Text="Third" />
</layouts:ReversibleStackPanel>

<!-- Reversed order: Third, Second, First -->
<layouts:ReversibleStackPanel ReverseOrder="True">
    <TextBlock Text="First" />
    <TextBlock Text="Second" />
    <TextBlock Text="Third" />
</layouts:ReversibleStackPanel>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ReverseOrder` | `bool` | `false` | Whether to reverse the visual order of children |

## 📝 Notes

- Only affects visual arrangement; the logical `Children` collection order is unchanged
- Works with both `Horizontal` and `Vertical` orientations

## 🔗 Related Controls

- **UniformSpacingPanel** - Panel with uniform spacing between items

## 🎮 Sample Application

See the ReversibleStackPanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > ReversibleStackPanel** for interactive examples.
