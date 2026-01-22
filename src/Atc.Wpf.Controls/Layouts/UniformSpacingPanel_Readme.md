# 📦 UniformSpacingPanel

A panel that provides uniform spacing between items with optional wrapping support.

## 🔍 Overview

`UniformSpacingPanel` is an extension of the WPF `Panel` control that automatically applies consistent spacing between child elements. It supports both horizontal and vertical orientations with optional wrapping, making it ideal for button groups, tag lists, and form layouts.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### Basic Horizontal Layout

```xml
<atc:UniformSpacingPanel
    Orientation="Horizontal"
    Spacing="10">
    <Button Content="Save" />
    <Button Content="Cancel" />
    <Button Content="Help" />
</atc:UniformSpacingPanel>
```

### Separate Horizontal and Vertical Spacing

```xml
<atc:UniformSpacingPanel
    Orientation="Horizontal"
    HorizontalSpacing="15"
    VerticalSpacing="20"
    ChildWrapping="Wrap">
    <Button Content="Item 1" />
    <Button Content="Item 2" />
    <Button Content="Item 3" />
    <Button Content="Item 4" />
</atc:UniformSpacingPanel>
```

### 📝 Vertical Form Layout

```xml
<atc:UniformSpacingPanel
    Orientation="Vertical"
    Spacing="12">
    <TextBox PlaceholderText="Name" />
    <TextBox PlaceholderText="Email" />
    <TextBox PlaceholderText="Message" />
    <Button Content="Submit" />
</atc:UniformSpacingPanel>
```

### 🏷️ Wrapping Tags

```xml
<atc:UniformSpacingPanel
    Orientation="Horizontal"
    ChildWrapping="Wrap"
    HorizontalSpacing="8"
    VerticalSpacing="8">
    <atc:Chip Content="C#" />
    <atc:Chip Content="WPF" />
    <atc:Chip Content="MVVM" />
    <atc:Chip Content=".NET" />
    <atc:Chip Content="XAML" />
</atc:UniformSpacingPanel>
```

### 📐 Fixed Item Sizes

```xml
<atc:UniformSpacingPanel
    Orientation="Horizontal"
    Spacing="10"
    ItemWidth="100"
    ItemHeight="100">
    <Border Background="Red" />
    <Border Background="Green" />
    <Border Background="Blue" />
</atc:UniformSpacingPanel>
```

## 🔄 Alternative: PanelHelper

You can achieve similar spacing on standard panels using `PanelHelper`:

```xml
<!-- Using UniformSpacingPanel -->
<atc:UniformSpacingPanel
    HorizontalSpacing="15"
    VerticalSpacing="20"
    ChildWrapping="Wrap">
    <!-- content -->
</atc:UniformSpacingPanel>

<!-- Using PanelHelper with WrapPanel -->
<WrapPanel
    atc:PanelHelper.HorizontalSpacing="15"
    atc:PanelHelper.VerticalSpacing="20">
    <!-- content -->
</WrapPanel>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Horizontal` | Layout direction (Horizontal or Vertical) |
| `Spacing` | `double` | `NaN` | Uniform spacing between items (sets both H & V) |
| `HorizontalSpacing` | `double` | `NaN` | Horizontal gap between items |
| `VerticalSpacing` | `double` | `NaN` | Vertical gap between items |
| `ChildWrapping` | `VisualWrappingType` | `None` | None, Wrap, or WrapReverse |
| `ItemWidth` | `double` | `NaN` | Fixed width for all items |
| `ItemHeight` | `double` | `NaN` | Fixed height for all items |
| `ItemHorizontalAlignment` | `HorizontalAlignment?` | `Stretch` | Child horizontal alignment |
| `ItemVerticalAlignment` | `VerticalAlignment?` | `Stretch` | Child vertical alignment |

## 📋 ChildWrapping Enumeration

| Value | Description |
|-------|-------------|
| `None` | Items stay on a single line (may overflow) |
| `Wrap` | Items wrap to next line when space runs out |
| `WrapReverse` | Items wrap in reverse direction |

## 📝 Notes

- `Spacing` sets both `HorizontalSpacing` and `VerticalSpacing`
- Individual spacing values override the unified `Spacing` property
- `ItemWidth` and `ItemHeight` apply uniform sizes to all children
- For masonry layouts with varying heights, use `StaggeredPanel` instead

## 🔗 Related Controls

- **PanelHelper** - Attach spacing to any Panel via attached properties
- **StaggeredPanel** - Masonry/waterfall layout for varying heights
- **FlexPanel** - CSS Flexbox-style layouts with grow/shrink

## 🎮 Sample Application

See the UniformSpacingPanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > UniformSpacingPanel** for interactive examples.
