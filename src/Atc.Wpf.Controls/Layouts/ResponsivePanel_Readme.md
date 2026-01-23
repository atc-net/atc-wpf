# 📱 ResponsivePanel

A responsive layout panel that adapts its column count based on available width using configurable breakpoints.

## 🔍 Overview

`ResponsivePanel` provides Bootstrap-style responsive layouts for WPF. It automatically adjusts the number of columns based on the container width, and supports visibility control, item spanning, and reordering at different breakpoints.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### 🔢 Auto Column Count (MinItemWidth)

```xml
<atc:ResponsivePanel MinItemWidth="280" Gap="16">
    <Border Background="LightBlue" Height="100" />
    <Border Background="LightGreen" Height="100" />
    <Border Background="LightCoral" Height="100" />
    <Border Background="LightYellow" Height="100" />
</atc:ResponsivePanel>
```

### 📊 Explicit Column Count per Breakpoint

```xml
<atc:ResponsivePanel
    ColumnsXs="1"
    ColumnsSm="2"
    ColumnsMd="3"
    ColumnsLg="4"
    ColumnsXl="6"
    Gap="16">
    <Border Background="LightBlue" />
    <Border Background="LightGreen" />
    <Border Background="LightCoral" />
    <Border Background="LightYellow" />
</atc:ResponsivePanel>
```

### 📐 12-Column Grid with Spans

```xml
<atc:ResponsivePanel
    ColumnsXs="12"
    ColumnsSm="12"
    ColumnsMd="12"
    ColumnsLg="12"
    ColumnsXl="12"
    Gap="16">
    <!-- Sidebar: 4 cols on large screens, full width on small -->
    <Border
        atc:ResponsivePanel.SpanXs="12"
        atc:ResponsivePanel.SpanLg="4"
        Background="LightBlue">
        <TextBlock Text="Sidebar" />
    </Border>
    <!-- Main content: 8 cols on large screens, full width on small -->
    <Border
        atc:ResponsivePanel.SpanXs="12"
        atc:ResponsivePanel.SpanLg="8"
        Background="LightGreen">
        <TextBlock Text="Main Content" />
    </Border>
</atc:ResponsivePanel>
```

### 👁️ Visibility Control

```xml
<atc:ResponsivePanel ColumnsLg="4" Gap="16">
    <!-- Always visible -->
    <Border Background="LightBlue" />

    <!-- Desktop only (visible at Lg and above) -->
    <Border
        atc:ResponsivePanel.VisibleFrom="Lg"
        Background="LightGreen" />

    <!-- Mobile only (hidden at Md and above) -->
    <Border
        atc:ResponsivePanel.HiddenFrom="Md"
        Background="LightCoral" />
</atc:ResponsivePanel>
```

### 🔀 Reorder at Breakpoints

```xml
<atc:ResponsivePanel ColumnsXs="1" ColumnsLg="3" Gap="16">
    <!-- Image: shows first on mobile, second on desktop -->
    <Border
        atc:ResponsivePanel.OrderXs="1"
        atc:ResponsivePanel.OrderLg="2"
        Background="LightBlue">
        <TextBlock Text="Image" />
    </Border>
    <!-- Text: shows second on mobile, first on desktop -->
    <Border
        atc:ResponsivePanel.OrderXs="2"
        atc:ResponsivePanel.OrderLg="1"
        Background="LightGreen">
        <TextBlock Text="Text Content" />
    </Border>
</atc:ResponsivePanel>
```

### 📋 With ItemsControl

```xml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <atc:ResponsivePanel
                MinItemWidth="250"
                Gap="16" />
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

## ⚙️ Properties

### Panel Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Gap` | `double` | `0` | Uniform gap between items (both horizontal and vertical) |
| `RowGap` | `double` | `NaN` | Vertical gap between rows (overrides Gap) |
| `ColumnGap` | `double` | `NaN` | Horizontal gap between columns (overrides Gap) |
| `MinItemWidth` | `double` | `NaN` | Auto-calculates columns based on this minimum width |
| `ColumnsXs` | `int` | `1` | Number of columns at XS breakpoint (< 576px) |
| `ColumnsSm` | `int` | `2` | Number of columns at SM breakpoint (576-767px) |
| `ColumnsMd` | `int` | `3` | Number of columns at MD breakpoint (768-991px) |
| `ColumnsLg` | `int` | `4` | Number of columns at LG breakpoint (992-1199px) |
| `ColumnsXl` | `int` | `6` | Number of columns at XL breakpoint (>= 1200px) |

### Attached Properties (Span)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SpanXs` | `int` | `0` | Column span at XS breakpoint (0 = use default) |
| `SpanSm` | `int` | `0` | Column span at SM breakpoint |
| `SpanMd` | `int` | `0` | Column span at MD breakpoint |
| `SpanLg` | `int` | `0` | Column span at LG breakpoint |
| `SpanXl` | `int` | `0` | Column span at XL breakpoint |

### Attached Properties (Visibility)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `VisibleFrom` | `ResponsiveBreakpoint?` | `null` | Element visible at this breakpoint and larger |
| `HiddenFrom` | `ResponsiveBreakpoint?` | `null` | Element hidden at this breakpoint and larger |

### Attached Properties (Order)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `OrderXs` | `int` | `MaxValue` | Display order at XS breakpoint |
| `OrderSm` | `int` | `MaxValue` | Display order at SM breakpoint |
| `OrderMd` | `int` | `MaxValue` | Display order at MD breakpoint |
| `OrderLg` | `int` | `MaxValue` | Display order at LG breakpoint |
| `OrderXl` | `int` | `MaxValue` | Display order at XL breakpoint |

## 📏 Breakpoint Thresholds

| Breakpoint | Width Range | Typical Device |
|------------|-------------|----------------|
| `Xs` | < 576px | Small phones |
| `Sm` | 576px - 767px | Phones, small tablets |
| `Md` | 768px - 991px | Tablets |
| `Lg` | 992px - 1199px | Small laptops |
| `Xl` | >= 1200px | Desktops, large screens |

## 📝 Notes

- When `MinItemWidth` is set, it takes precedence over explicit column counts
- Span values of `0` mean the item takes default width (1 column)
- Span values cascade up: if only `SpanLg` is set, it applies to Lg and Xl
- Order values cascade up: if only `OrderXs` is set, it applies to all breakpoints
- Use the 12-column grid pattern for maximum layout flexibility
- Visibility properties can be combined (e.g., visible only at Sm and Md)

## 🔗 Related Controls

- **Row / Col** - Bootstrap-style grid with per-child span configuration
- **FlexPanel** - CSS Flexbox-style layouts with grow/shrink
- **UniformSpacingPanel** - Simple wrapping with uniform spacing
- **AutoGrid** - Auto-indexed Grid with string-based definitions

## 🎮 Sample Application

See the ResponsivePanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > ResponsivePanel** for interactive examples.
