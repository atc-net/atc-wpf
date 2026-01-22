# 📦 StaggeredPanel

A masonry/waterfall layout panel that arranges items in columns, placing each item in the column with the least height.

## 🔍 Overview

`StaggeredPanel` is an extension of the WPF `Panel` control that creates Pinterest-style layouts. Items are automatically placed in the column that has used the least vertical space, creating an efficient waterfall effect perfect for galleries and card layouts.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### Basic Example

```xml
<atc:StaggeredPanel
    DesiredItemWidth="200"
    HorizontalSpacing="10"
    VerticalSpacing="10">
    <Border Height="150" Background="Red" />
    <Border Height="200" Background="Green" />
    <Border Height="120" Background="Blue" />
    <Border Height="180" Background="Yellow" />
</atc:StaggeredPanel>
```

### 🖼️ Image Gallery

```xml
<atc:StaggeredPanel
    DesiredItemWidth="250"
    HorizontalSpacing="12"
    VerticalSpacing="12"
    Padding="16">
    <Image Source="photo1.jpg" />
    <Image Source="photo2.jpg" />
    <Image Source="photo3.jpg" />
    <Image Source="photo4.jpg" />
</atc:StaggeredPanel>
```

### 📋 With ItemsControl

```xml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <atc:StaggeredPanel
                DesiredItemWidth="280"
                HorizontalSpacing="16"
                VerticalSpacing="16" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <atc:Card Header="{Binding Title}">
                <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
            </atc:Card>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### 📐 Stretch to Fill Width

```xml
<atc:StaggeredPanel
    DesiredItemWidth="200"
    HorizontalSpacing="10"
    VerticalSpacing="10"
    HorizontalAlignment="Stretch">
    <!-- Columns will expand beyond DesiredItemWidth to fill available space -->
</atc:StaggeredPanel>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DesiredItemWidth` | `double` | `250` | Target column width. Actual width may exceed this if `HorizontalAlignment="Stretch"` |
| `Padding` | `Thickness` | `0` | Inner padding between panel edge and content |
| `HorizontalSpacing` | `double` | `0` | Gap between columns |
| `VerticalSpacing` | `double` | `0` | Gap between items within columns |

## 📝 Notes

- Items are measured and placed in the column with the least total height
- Number of columns is automatically calculated based on `DesiredItemWidth` and available width
- For large collections (100+ items), consider using `VirtualizingStaggeredPanel` for better performance
- Works best with items that have varying heights

## 🔗 Related Controls

- **VirtualizingStaggeredPanel** - Virtualized version for large collections
- **UniformSpacingPanel** - For uniform spacing without masonry layout
- **FlexPanel** - For CSS Flexbox-style layouts

## 🎮 Sample Application

See the StaggeredPanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > StaggeredPanel** for interactive examples.
