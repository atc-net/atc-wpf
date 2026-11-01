# 🔧 PanelHelper

A helper class that adds spacing capabilities to any WPF Panel control through attached properties.

## 🔍 Overview

`PanelHelper` enhances the functionality of WPF Panel controls by providing attached properties for uniform spacing between child elements. It works with any control derived from the `Panel` class, allowing you to add consistent spacing without creating custom panels.

## 📍 Namespace

```csharp
using Atc.Wpf.Helpers;
```

## ✅ Supported Controls

`PanelHelper` seamlessly integrates with all WPF panel controls:

- 📦 Grid / GridEx
- 📦 DockPanel
- 📦 StackPanel
- 📦 WrapPanel
- 📦 UniformGrid
- 📦 TabPanel
- 📦 StaggeredPanel
- 📦 UniformSpacingPanel
- 📦 PanelEx
- 📦 Any custom `Panel` derivative

## 🚀 Usage

### Separate Horizontal and Vertical Spacing

```xml
<WrapPanel
    atc:PanelHelper.HorizontalSpacing="15"
    atc:PanelHelper.VerticalSpacing="20">
    <Button Content="Item 1" />
    <Button Content="Item 2" />
    <Button Content="Item 3" />
</WrapPanel>
```

### Uniform Spacing (Both Directions)

```xml
<WrapPanel atc:PanelHelper.Spacing="10">
    <Button Content="Item 1" />
    <Button Content="Item 2" />
    <Button Content="Item 3" />
</WrapPanel>
```

### With StackPanel

```xml
<StackPanel
    Orientation="Vertical"
    atc:PanelHelper.Spacing="8">
    <TextBox PlaceholderText="Name" />
    <TextBox PlaceholderText="Email" />
    <Button Content="Submit" />
</StackPanel>
```

### With Grid

```xml
<Grid atc:PanelHelper.Spacing="10">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    <Button Grid.Column="0" Content="Left" />
    <Button Grid.Column="1" Content="Right" />
</Grid>
```

## 🔄 Alternative: UniformSpacingPanel

For dedicated spacing control, you can also use `UniformSpacingPanel`:

```xml
<!-- Using PanelHelper -->
<WrapPanel
    atc:PanelHelper.HorizontalSpacing="15"
    atc:PanelHelper.VerticalSpacing="20">
    <!-- content -->
</WrapPanel>

<!-- Using UniformSpacingPanel -->
<atc:UniformSpacingPanel
    HorizontalSpacing="15"
    VerticalSpacing="20"
    ChildWrapping="Wrap">
    <!-- content -->
</atc:UniformSpacingPanel>
```

## ⚙️ Attached Properties

| Property | Type | Description |
|----------|------|-------------|
| `PanelHelper.HorizontalSpacing` | `double` | Horizontal spacing between items |
| `PanelHelper.VerticalSpacing` | `double` | Vertical spacing between items |
| `PanelHelper.Spacing` | `double` | Uniform spacing (sets both horizontal and vertical) |

## 📝 Notes

- `Spacing` sets both `HorizontalSpacing` and `VerticalSpacing` at once
- Individual `HorizontalSpacing` or `VerticalSpacing` values override `Spacing`
- Works by applying margins to child elements
- The last child that is not collapsed gets no trailing margin; collapsing or showing a child re-applies the margins
- Only the side facing the next item gets the spacing: the right side in a horizontal `StackPanel`, the bottom in a vertical one, and in a `DockPanel` the side facing the remaining space. Other panels (such as `WrapPanel`, which needs the gap on both axes, or `Grid`) get both right and bottom

## 🎮 Sample Application

See the PanelHelper sample in the Atc.Wpf.Sample application under **Wpf > Helpers > PanelHelper** for interactive examples.
