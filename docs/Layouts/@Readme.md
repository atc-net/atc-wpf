# 🧱 Layout Controls

This document provides a comprehensive overview of all layout controls available in **Atc.Wpf**, including standard WPF panels/grids and custom controls provided by the library.

---

## 📊 Quick Reference

### 🎯 Panel Comparison Table

Use this table to quickly identify which panel best suits your layout needs:

| Panel | Library | Primary Use Case | Direction | Wrapping | Spacing | Proportional | Virtualized |
|-------|---------|------------------|-----------|----------|---------|--------------|-------------|
| [Canvas](#-canvas-wpf-built-in) | WPF | Absolute positioning | Absolute | ❌ No | ❌ No | ❌ No | ❌ No |
| [DockPanel](#-dockpanel-wpf-built-in) | WPF | Edge docking | Dock (TRBL) | ❌ No | ❌ No | ❌ No | ❌ No |
| [DockPanelPro](#-dockpanelpro) | Atc.Wpf | IDE-style resizable docking | Dock (TRBL) | ❌ No | Via splitters | ❌ No | ❌ No |
| [FlexPanel](#-flexpanel) | Atc.Wpf | CSS Flexbox layouts | Row/Column + Reverse | ✅ Yes | ✅ Yes | ✅ Yes (Grow/Shrink) | ❌ No |
| [Panel](#-panel-wpf-built-in) | WPF | Base class for custom panels | N/A | ❌ No | ❌ No | ❌ No | ❌ No |
| [PanelEx](#-panelex) | Atc.Wpf | Layered/overlay content | Overlay | ❌ No | ❌ No | ❌ No | ❌ No |
| [ReversibleStackPanel](#-reversiblestackpanel) | Atc.Wpf | Reversible stacking | Row/Column | ❌ No | ❌ No | ❌ No | ❌ No |
| [ResponsivePanel](#-responsivepanel) | Atc.Wpf | Breakpoint-based responsive | Row | ✅ Yes | ✅ Yes | ❌ No | ❌ No |
| [StackPanel](#-stackpanel-wpf-built-in) | WPF | Simple stacking | Row/Column | ❌ No | ❌ No | ❌ No | ❌ No |
| [StaggeredPanel](#-staggeredpanel) | Atc.Wpf | Masonry/waterfall layout | Vertical columns | Auto | ✅ Yes | ❌ No | ❌ No |
| [UniformGrid](#-uniformgrid-wpf-built-in) | WPF | Equal-sized cells | Row/Column | ❌ No | ❌ No | ✅ Yes | ❌ No |
| [UniformSpacingPanel](#-uniformspacingpanel) | Atc.Wpf | Uniform spacing/wrapping | Row/Column | ✅ Yes | ✅ Yes | ❌ No | ❌ No |
| [VirtualizingStaggeredPanel](#-virtualizingstaggeredpanel) | Atc.Wpf | Large masonry collections | Vertical columns | Auto | ✅ Yes | ❌ No | ✅ Yes |
| [WrapPanel](#-wrappanel-wpf-built-in) | WPF | Flowing wrap layout | Row/Column | ✅ Yes | ❌ No | ❌ No | ❌ No |

### 🔲 Grid Comparison Table

| Grid | Library | Primary Use Case | Auto Rows/Cols | String Definition | Responsive |
|------|---------|------------------|----------------|-------------------|------------|
| [AutoGrid](#-autogrid) | Atc.Wpf | Auto-indexed Grid | ✅ Yes | ✅ Yes | ❌ No |
| [Grid](#-grid-wpf-built-in) | WPF | Complex layouts | ❌ Manual | ❌ No | ❌ No |
| [GridEx](#-gridex) | Atc.Wpf | Simplified Grid | ❌ No | ✅ Yes | ❌ No |
| [Row / Col](#-row--col) | Atc.Wpf | Bootstrap-style responsive | ✅ Yes | ✅ Yes | ✅ Yes |

### 🎨 Container Controls

| Control | Library | Primary Use Case | Collapsible | Theming |
|---------|---------|------------------|-------------|---------|
| [Badge](#-badge) | Atc.Wpf | Status indicator overlay | ❌ No | ✅ Yes |
| [Card](#-card) | Atc.Wpf | Content grouping with elevation | ✅ Yes | ✅ Yes |
| [Chip](#-chip) | Atc.Wpf | Tags, filters, selections | ❌ No | ✅ Yes |
| [Divider](#-divider) | Atc.Wpf | Visual separator | ❌ No | ✅ Yes |
| [DockPanelPro](#-dockpanelpro) | Atc.Wpf | IDE-style resizable docking | ✅ Yes | ✅ Yes |
| [GridLines](#-gridlines) | Atc.Wpf | Debug grid visualization | ❌ No | ❌ No |
| [GroupBoxExpander](#-groupboxexpander) | Atc.Wpf | Collapsible grouped content | ✅ Yes | ✅ Yes |

---

## 🔍 When to Use Which Layout

| Scenario | Best Choice | Why |
|----------|-------------|-----|
| Simple vertical/horizontal list | **StackPanel** | Lightweight, straightforward |
| Toolbar with proportional buttons | **FlexPanel** | `Grow` distributes space |
| Items that flow and wrap | **UniformSpacingPanel** | Consistent spacing + wrapping |
| Pinterest-style image gallery | **StaggeredPanel** | Optimal vertical space usage |
| Large image collection (1000+ items) | **VirtualizingStaggeredPanel** | Virtualization for performance |
| Complex form with rows/columns | **Grid** or **AutoGrid** | Precise cell positioning |
| Quick prototype grid | **AutoGrid** | Auto-indexing saves time |
| Responsive Bootstrap-style layout | **Row / Col** | Breakpoint-based columns |
| Responsive cards with visibility control | **ResponsivePanel** | Auto columns + hide/show at breakpoints |
| Navigation bar: logo left, menu right | **FlexPanel** | `JustifyContent="SpaceBetween"` |
| IDE-style resizable tool windows | **DockPanelPro** | Built-in splitters + collapse |
| Layered content / overlays | **PanelEx** | Children stack on top of each other |
| Custom panel implementation | **Panel** | Base class for custom layout logic |
| Content card with header/footer | **Card** | Built-in elevation + expand |
| Collapsible settings section | **GroupBoxExpander** | Expand/collapse with header |
| Status indicator on icon | **Badge** | Overlay positioning |
| Filter tags / selection chips | **Chip** | Interactive tag controls |

---

## 📦 Panels

### 📘 Canvas (WPF Built-in)

**Description:** Positions children using explicit coordinates (Left, Top, Right, Bottom).

**When to Use:**
- Drawing applications
- Diagram editors
- Absolute positioning requirements

**Example:**

```xml
<Canvas>
    <Rectangle Canvas.Left="50" Canvas.Top="30" Width="100" Height="50" Fill="Blue" />
    <Ellipse Canvas.Left="150" Canvas.Top="80" Width="60" Height="60" Fill="Red" />
</Canvas>
```

---

### 📘 DockPanel (WPF Built-in)

**Description:** Positions children against the edges (Top, Bottom, Left, Right) with the last child filling remaining space.

**When to Use:**
- Application layouts with toolbars/status bars
- Sidebar + main content layouts

**Example:**

```xml
<DockPanel>
    <Menu DockPanel.Dock="Top"><!-- Menu items --></Menu>
    <StatusBar DockPanel.Dock="Bottom">Status</StatusBar>
    <TreeView DockPanel.Dock="Left" Width="200" />
    <ContentControl />  <!-- Fills remaining space -->
</DockPanel>
```

---

### 🟢 DockPanelPro

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** An enhanced dock panel with resizable splitters, collapsible regions, and layout persistence. Perfect for building IDE-style interfaces with tool windows.

**When to Use:**
- IDE-style layouts with resizable tool windows
- Dashboard layouts with collapsible panels
- Applications requiring layout persistence
- Any layout needing user-adjustable regions

**DockPanelPro Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| LayoutId | string | null | Identifier for layout persistence |
| AutoSaveLayout | bool | false | Automatically save layout changes |
| SplitterThickness | double | 5 | Thickness of resizable splitters |
| SplitterBackground | Brush | from theme | Background brush for splitters |

**DockPanelPro Attached Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| DockPanelPro.Dock | DockPosition | Center | Left, Right, Top, Bottom, Center |

**DockRegion Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| RegionId | string | null | Unique identifier for persistence |
| IsCollapsible | bool | false | Enable collapse toggle button |
| IsExpanded | bool | true | Current expanded state |
| IsResizable | bool | true | Allow resizing via splitters |
| Header | object | null | Header content |
| MinWidth / MaxWidth | double | 0 / ∞ | Width constraints |
| MinHeight / MaxHeight | double | 0 / ∞ | Height constraints |

**Examples:**

```xml
<!-- Example 1: IDE Layout -->
<layouts:DockPanelPro LayoutId="IDE" SplitterThickness="5">
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="250"
        MinWidth="150"
        MaxWidth="400"
        Header="Explorer"
        IsCollapsible="True"
        RegionId="Explorer">
        <TreeView />
    </layouts:DockRegion>

    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="150"
        Header="Output"
        IsCollapsible="True"
        RegionId="Output">
        <TextBox IsReadOnly="True" />
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" RegionId="Editor">
        <TabControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>

<!-- Example 2: Dashboard with Navigation -->
<layouts:DockPanelPro>
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="200"
        Header="Navigation"
        IsResizable="True">
        <StackPanel>
            <Button Content="Dashboard" />
            <Button Content="Reports" />
        </StackPanel>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" Header="Content">
        <ContentControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

**Layout Persistence:**

```csharp
// Save layout
var layout = dockPanel.SaveLayout();
File.WriteAllText("layout.json", layout);

// Load layout
var layout = File.ReadAllText("layout.json");
dockPanel.LoadLayout(layout);

// Reset to defaults
dockPanel.ResetLayout();
```

**See Also:** [Full DockPanelPro Documentation](DockPanelPro_Readme.md)

---

### 🟢 FlexPanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A CSS Flexbox-inspired layout panel supporting direction, justify-content, align-items, wrap, gap, and per-child grow/shrink/basis attached properties.

**When to Use:**
- Modern flexible layouts
- Toolbars with proportional sizing
- Navigation bars with space distribution
- Any layout requiring CSS Flexbox behavior

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Direction | FlexDirection | Row | Row, Column, RowReverse, ColumnReverse |
| JustifyContent | FlexJustify | Start | Start, End, Center, SpaceBetween, SpaceAround, SpaceEvenly |
| AlignItems | FlexAlign | Stretch | Stretch, Start, End, Center, Baseline |
| Wrap | FlexWrap | NoWrap | NoWrap, Wrap, WrapReverse |
| Gap | double | 0 | Spacing between items |
| RowGap | double | NaN | Vertical gap (overrides Gap) |
| ColumnGap | double | NaN | Horizontal gap (overrides Gap) |

**Attached Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| FlexPanel.Grow | double | 0 | How much the item should grow |
| FlexPanel.Shrink | double | 1 | How much the item should shrink |
| FlexPanel.Basis | double | NaN | Initial size before grow/shrink |
| FlexPanel.AlignSelf | FlexAlign | Auto | Override parent's AlignItems |

**Examples:**

```xml
<!-- Example 1: Toolbar with spacer -->
<layouts:FlexPanel Direction="Row" AlignItems="Center" Gap="8">
    <Button Content="Save" />
    <Button Content="Load" />
    <!-- Items after this will be pushed to the right -->
    <Border layouts:FlexPanel.Grow="1" />
    <TextBlock Text="Status: Ready" />
</layouts:FlexPanel>

<!-- Example 2: Equal-width cards -->
<layouts:FlexPanel Direction="Row" Gap="16" Wrap="Wrap">
    <Border layouts:FlexPanel.Grow="1" layouts:FlexPanel.Basis="200" Background="LightBlue" Padding="16">
        <TextBlock Text="Card 1" />
    </Border>
    <Border layouts:FlexPanel.Grow="1" layouts:FlexPanel.Basis="200" Background="LightGreen" Padding="16">
        <TextBlock Text="Card 2" />
    </Border>
</layouts:FlexPanel>

<!-- Example 3: Navigation with space-between -->
<layouts:FlexPanel Direction="Row" JustifyContent="SpaceBetween" AlignItems="Center" Height="60">
    <Image Source="logo.png" Height="40" />
    <layouts:FlexPanel Direction="Row" Gap="16">
        <Button Content="Home" />
        <Button Content="About" />
    </layouts:FlexPanel>
</layouts:FlexPanel>

<!-- Example 4: Proportional sizing (1:2:1) -->
<layouts:FlexPanel Direction="Row" Gap="8">
    <Border layouts:FlexPanel.Grow="1" Background="#E3F2FD">
        <TextBlock Text="Sidebar (1x)" />
    </Border>
    <Border layouts:FlexPanel.Grow="2" Background="#BBDEFB">
        <TextBlock Text="Main (2x)" />
    </Border>
    <Border layouts:FlexPanel.Grow="1" Background="#E3F2FD">
        <TextBlock Text="Details (1x)" />
    </Border>
</layouts:FlexPanel>

<!-- Example 5: Centered form -->
<layouts:FlexPanel Direction="Column" AlignItems="Center" Gap="16">
    <TextBox Width="200" />
    <TextBox Width="200" />
    <Button Content="Submit" Width="120" />
</layouts:FlexPanel>
```

---

### 📘 Panel (WPF Built-in)

**Description:** The abstract base class for all WPF panel controls. Panel provides the framework for arranging child elements.

**When to Use:**
- Creating custom layout controls
- As a base class for specialized panels
- Understanding WPF layout fundamentals

**Key Concepts:**
- Override `MeasureOverride(Size)` to determine desired size
- Override `ArrangeOverride(Size)` to position children
- Access children via `InternalChildren` collection

**Example:**

```csharp
public class SimpleOverlayPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var maxSize = new Size();
        foreach (UIElement child in InternalChildren)
        {
            child.Measure(availableSize);
            maxSize.Width = Math.Max(maxSize.Width, child.DesiredSize.Width);
            maxSize.Height = Math.Max(maxSize.Height, child.DesiredSize.Height);
        }
        return maxSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (UIElement child in InternalChildren)
        {
            child.Arrange(new Rect(finalSize));
        }
        return finalSize;
    }
}
```

---

### 🟢 PanelEx

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A simple panel that overlays all children on top of each other, sizing to the largest child. Similar to placing multiple items in the same Grid cell.

**When to Use:**
- Watermarks over content
- Loading overlays
- Badge/notification overlays
- Layered UI elements
- Any scenario requiring stacked content

**Examples:**

```xml
<!-- Example 1: Simple overlay -->
<atc:PanelEx Width="200" Height="150">
    <Border Background="LightBlue">
        <TextBlock Text="Background content" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
    <TextBlock Text="NEW" HorizontalAlignment="Right" VerticalAlignment="Top"
               Foreground="Red" FontWeight="Bold" Margin="5" />
</atc:PanelEx>

<!-- Example 2: Watermark -->
<atc:PanelEx>
    <Border Background="White" BorderBrush="Gray" BorderThickness="1" Padding="16">
        <TextBlock Text="Document content..." />
    </Border>
    <TextBlock Text="DRAFT" HorizontalAlignment="Center" VerticalAlignment="Center"
               FontSize="48" Foreground="#20000000" FontWeight="Bold" />
</atc:PanelEx>

<!-- Example 3: Loading overlay -->
<atc:PanelEx>
    <Border Background="White">
        <StackPanel>
            <TextBlock Text="Your content here" />
        </StackPanel>
    </Border>
    <Border Background="#80000000" Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}">
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
            <TextBlock Text="Loading..." Foreground="White" />
        </StackPanel>
    </Border>
</atc:PanelEx>

<!-- Example 4: Corner badge -->
<atc:PanelEx Width="100" Height="100">
    <Border Background="LightGreen" CornerRadius="8">
        <TextBlock Text="Product" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
    <Border Width="24" Height="24" Background="Orange" CornerRadius="12"
            HorizontalAlignment="Right" VerticalAlignment="Top" Margin="-8,-8,0,0">
        <TextBlock Text="5" HorizontalAlignment="Center" VerticalAlignment="Center"
                   Foreground="White" FontWeight="Bold" FontSize="10" />
    </Border>
</atc:PanelEx>
```

---

### 🟢 ReversibleStackPanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A StackPanel with the ability to reverse the order of children.

**When to Use:**
- Chat messages (newest at bottom vs top)
- Reversible lists
- RTL layouts

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| ReverseOrder | bool | false | Whether to reverse children order |
| Orientation | Orientation | Vertical | Inherited from StackPanel |

**Examples:**

```xml
<!-- Example 1: Reversed vertical stack -->
<layouts:ReversibleStackPanel Orientation="Vertical" ReverseOrder="True">
    <TextBlock Text="First (appears last)" />
    <TextBlock Text="Second" />
    <TextBlock Text="Third (appears first)" />
</layouts:ReversibleStackPanel>

<!-- Example 2: Chat messages (newest at bottom) -->
<layouts:ReversibleStackPanel
    Orientation="Vertical"
    ReverseOrder="{Binding ShowNewestFirst}">
    <TextBlock Text="Message 1" />
    <TextBlock Text="Message 2" />
    <TextBlock Text="Message 3" />
</layouts:ReversibleStackPanel>
```

---

### 🟢 ResponsivePanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A responsive layout panel that adapts its column count based on available width using configurable breakpoints. Supports auto-calculated columns, explicit column counts per breakpoint, visibility control, and item reordering.

**When to Use:**
- Responsive card/tile layouts
- Dashboards that adapt to screen size
- Layouts requiring visibility control at breakpoints
- Mobile-first or desktop-first responsive designs

**Breakpoints:**

| Breakpoint | Width Range | Typical Device |
|------------|-------------|----------------|
| `Xs` | < 576px | Small phones |
| `Sm` | 576px - 767px | Phones, small tablets |
| `Md` | 768px - 991px | Tablets |
| `Lg` | 992px - 1199px | Small laptops |
| `Xl` | >= 1200px | Desktops, large screens |

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Gap | double | 0 | Uniform gap between items |
| RowGap | double | NaN | Vertical gap (overrides Gap) |
| ColumnGap | double | NaN | Horizontal gap (overrides Gap) |
| MinItemWidth | double | NaN | Auto-calculate columns from this width |
| ColumnsXs | int | 1 | Columns at XS breakpoint |
| ColumnsSm | int | 2 | Columns at SM breakpoint |
| ColumnsMd | int | 3 | Columns at MD breakpoint |
| ColumnsLg | int | 4 | Columns at LG breakpoint |
| ColumnsXl | int | 6 | Columns at XL breakpoint |

**Attached Properties:**

| Property | Type | Description |
|----------|------|-------------|
| SpanXs/Sm/Md/Lg/Xl | int | Column span at each breakpoint |
| VisibleFrom | ResponsiveBreakpoint? | Show at this breakpoint and larger |
| HiddenFrom | ResponsiveBreakpoint? | Hide at this breakpoint and larger |
| OrderXs/Sm/Md/Lg/Xl | int | Display order at each breakpoint |

**Examples:**

```xml
<!-- Example 1: Auto column count -->
<layouts:ResponsivePanel MinItemWidth="280" Gap="16">
    <Border Background="LightBlue" Height="100" />
    <Border Background="LightGreen" Height="100" />
    <Border Background="LightCoral" Height="100" />
</layouts:ResponsivePanel>

<!-- Example 2: Explicit column counts -->
<layouts:ResponsivePanel
    ColumnsXs="1"
    ColumnsSm="2"
    ColumnsMd="3"
    ColumnsLg="4"
    Gap="16">
    <Border Background="LightBlue" />
    <Border Background="LightGreen" />
</layouts:ResponsivePanel>

<!-- Example 3: 12-column grid with spans -->
<layouts:ResponsivePanel
    ColumnsLg="12"
    ColumnsMd="12"
    Gap="16">
    <Border
        layouts:ResponsivePanel.SpanLg="4"
        layouts:ResponsivePanel.SpanMd="12"
        Background="LightBlue">
        <TextBlock Text="Sidebar" />
    </Border>
    <Border
        layouts:ResponsivePanel.SpanLg="8"
        layouts:ResponsivePanel.SpanMd="12"
        Background="LightGreen">
        <TextBlock Text="Main Content" />
    </Border>
</layouts:ResponsivePanel>

<!-- Example 4: Visibility control -->
<layouts:ResponsivePanel ColumnsLg="4" Gap="16">
    <Border Background="LightBlue" />
    <Border layouts:ResponsivePanel.VisibleFrom="Lg" Background="LightGreen" />
    <Border layouts:ResponsivePanel.HiddenFrom="Md" Background="LightCoral" />
</layouts:ResponsivePanel>
```

**See Also:** [Full ResponsivePanel Documentation](../src/Atc.Wpf.Controls/Layouts/ResponsivePanel_Readme.md)

---

### 📘 StackPanel (WPF Built-in)

**Description:** Arranges children in a single line (horizontal or vertical).

**When to Use:**
- Simple lists of items
- Vertical forms
- Horizontal toolbars without proportional sizing

**Example:**

```xml
<StackPanel Orientation="Vertical">
    <TextBlock Text="Item 1" />
    <TextBlock Text="Item 2" />
    <TextBlock Text="Item 3" />
</StackPanel>
```

---

### 🟢 StaggeredPanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A masonry/waterfall layout panel that places items in the column with the least height, creating a Pinterest-style effect.

**When to Use:**
- Image galleries with varying heights
- Card layouts with different content sizes
- Pinterest-style layouts
- Small to medium collections (use VirtualizingStaggeredPanel for large collections)

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| DesiredItemWidth | double | 250 | Desired column width |
| Padding | Thickness | 0 | Inner padding |
| HorizontalSpacing | double | 0 | Gap between columns |
| VerticalSpacing | double | 0 | Gap between items |

**Examples:**

```xml
<!-- Example 1: Basic masonry layout -->
<layouts:StaggeredPanel DesiredItemWidth="200" HorizontalSpacing="10" VerticalSpacing="10">
    <Border Height="150" Background="Red" />
    <Border Height="200" Background="Green" />
    <Border Height="120" Background="Blue" />
    <Border Height="180" Background="Yellow" />
</layouts:StaggeredPanel>

<!-- Example 2: Image gallery -->
<layouts:StaggeredPanel DesiredItemWidth="250" HorizontalSpacing="12" VerticalSpacing="12" Padding="16">
    <Image Source="photo1.jpg" />
    <Image Source="photo2.jpg" />
    <Image Source="photo3.jpg" />
</layouts:StaggeredPanel>

<!-- Example 3: Card gallery in ItemsControl -->
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <layouts:StaggeredPanel DesiredItemWidth="280" HorizontalSpacing="16" VerticalSpacing="16" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <layouts:Card Header="{Binding Title}">
                <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
            </layouts:Card>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

### 📘 UniformGrid (WPF Built-in)

**Description:** Arranges children in a grid where all cells are the same size.

**When to Use:**
- Calculator buttons
- Game boards
- Equally-sized item grids

**Example:**

```xml
<UniformGrid Rows="3" Columns="3">
    <Button Content="1" />
    <Button Content="2" />
    <Button Content="3" />
    <Button Content="4" />
    <Button Content="5" />
    <Button Content="6" />
    <Button Content="7" />
    <Button Content="8" />
    <Button Content="9" />
</UniformGrid>
```

---

### 🟢 UniformSpacingPanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A panel that provides uniform spacing between items with optional wrapping support.

**When to Use:**
- Tag/chip lists with consistent spacing
- Button groups
- Any wrapping content with uniform gaps

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Orientation | Orientation | Horizontal | Horizontal or Vertical |
| ChildWrapping | VisualWrappingType | None | None, Wrap, WrapReverse |
| Spacing | double | NaN | Uniform spacing between items |
| HorizontalSpacing | double | NaN | Horizontal spacing (overrides Spacing) |
| VerticalSpacing | double | NaN | Vertical spacing (overrides Spacing) |
| ItemWidth | double | NaN | Fixed width for all items |
| ItemHeight | double | NaN | Fixed height for all items |
| ItemHorizontalAlignment | HorizontalAlignment? | Stretch | Child horizontal alignment |
| ItemVerticalAlignment | VerticalAlignment? | Stretch | Child vertical alignment |

**Examples:**

```xml
<!-- Example 1: Button group with spacing -->
<layouts:UniformSpacingPanel Orientation="Horizontal" Spacing="8">
    <Button Content="Save" />
    <Button Content="Cancel" />
    <Button Content="Help" />
</layouts:UniformSpacingPanel>

<!-- Example 2: Wrapping tags -->
<layouts:UniformSpacingPanel
    Orientation="Horizontal"
    ChildWrapping="Wrap"
    HorizontalSpacing="8"
    VerticalSpacing="8">
    <layouts:Chip Content="C#" />
    <layouts:Chip Content="WPF" />
    <layouts:Chip Content="MVVM" />
    <layouts:Chip Content=".NET" />
</layouts:UniformSpacingPanel>

<!-- Example 3: Vertical list with spacing -->
<layouts:UniformSpacingPanel Orientation="Vertical" Spacing="12">
    <TextBox PlaceholderText="Name" />
    <TextBox PlaceholderText="Email" />
    <TextBox PlaceholderText="Message" />
</layouts:UniformSpacingPanel>
```

---

### 🟢 VirtualizingStaggeredPanel

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A virtualized version of StaggeredPanel for large collections (hundreds or thousands of items).

**When to Use:**
- Large image galleries
- Infinite scroll implementations
- Any masonry layout with 100+ items

**Properties:** Same as StaggeredPanel.

**Example:**

```xml
<ListBox ItemsSource="{Binding LargeCollection}">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <layouts:VirtualizingStaggeredPanel
                DesiredItemWidth="200"
                HorizontalSpacing="8"
                VerticalSpacing="8" />
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>
```

---

### 📘 WrapPanel (WPF Built-in)

**Description:** Positions children sequentially, wrapping to the next line when space runs out.

**When to Use:**
- Tag clouds
- Photo thumbnails
- Any content that should flow and wrap

**Example:**

```xml
<WrapPanel Orientation="Horizontal">
    <Button Content="Tag 1" Margin="4" />
    <Button Content="Tag 2" Margin="4" />
    <Button Content="Tag 3" Margin="4" />
    <!-- Wraps automatically -->
</WrapPanel>
```

---

## 🔲 Grids

### 🟢 AutoGrid

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A Grid that automatically positions children based on their index, eliminating the need for Grid.Row/Grid.Column on each child.

**When to Use:**
- Quick form layouts
- Reducing repetitive Grid.Row/Grid.Column assignments
- Prototyping

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| Rows | string | Comma-separated row definitions |
| Columns | string | Comma-separated column definitions |
| RowCount | int | Number of rows (default: 1) |
| ColumnCount | int | Number of columns (default: 1) |
| RowHeight | GridLength | Default height for auto-generated rows |
| ColumnWidth | GridLength | Default width for auto-generated columns |
| Orientation | Orientation | Fill direction (Horizontal or Vertical) |
| IsAutoIndexing | bool | Enable auto-positioning (default: true) |
| ChildMargin | Thickness? | Margin applied to all children |
| ChildHorizontalAlignment | HorizontalAlignment? | Alignment for all children |
| ChildVerticalAlignment | VerticalAlignment? | Alignment for all children |

**Examples:**

```xml
<!-- Example 1: Simple form (2 columns, auto rows) -->
<layouts:AutoGrid Columns="Auto,*" ChildMargin="4">
    <TextBlock Text="Name:" />
    <TextBox />

    <TextBlock Text="Email:" />
    <TextBox />

    <TextBlock Text="Phone:" />
    <TextBox />
</layouts:AutoGrid>

<!-- Example 2: Button grid (3 columns) -->
<layouts:AutoGrid Columns="*,*,*" RowHeight="40" ChildMargin="4">
    <Button Content="1" />
    <Button Content="2" />
    <Button Content="3" />
    <Button Content="4" />
    <Button Content="5" />
    <Button Content="6" />
</layouts:AutoGrid>

<!-- Example 3: Vertical fill -->
<layouts:AutoGrid Rows="*,*,*" Orientation="Vertical">
    <TextBlock Text="Row 0" />
    <TextBlock Text="Row 1" />
    <TextBlock Text="Row 2" />
</layouts:AutoGrid>

<!-- Example 4: Fixed row/column counts -->
<layouts:AutoGrid RowCount="3" ColumnCount="4" RowHeight="50" ColumnWidth="100">
    <!-- 12 cells will be created -->
</layouts:AutoGrid>
```

---

### 📘 Grid (WPF Built-in)

**Description:** The standard WPF Grid with explicit row/column definitions.

**When to Use:**
- Complex form layouts
- Precise control over cell positioning
- Layouts requiring row/column spanning

**Example:**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Row="0" Grid.Column="0" Text="Label" />
    <TextBox Grid.Row="0" Grid.Column="1" />
</Grid>
```

---

### 🟢 GridEx

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** An enhanced Grid that supports string-based row and column definitions for cleaner XAML.

**When to Use:**
- Simplifying Grid definitions
- Quick prototyping
- Reducing XAML verbosity

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| Rows | string | Comma-separated row definitions (e.g., "Auto,*,2*,100") |
| Columns | string | Comma-separated column definitions (e.g., "200,*,Auto") |

**Examples:**

```xml
<!-- Example 1: Simple 3x3 grid -->
<layouts:GridEx Rows="*,*,*" Columns="*,*,*">
    <TextBlock Grid.Row="0" Grid.Column="0" Text="[0,0]" />
    <TextBlock Grid.Row="1" Grid.Column="1" Text="[1,1]" />
</layouts:GridEx>

<!-- Example 2: Form layout -->
<layouts:GridEx Rows="Auto,Auto,Auto,*" Columns="Auto,*">
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Name:" />
    <TextBox Grid.Row="0" Grid.Column="1" />

    <TextBlock Grid.Row="1" Grid.Column="0" Text="Email:" />
    <TextBox Grid.Row="1" Grid.Column="1" />

    <TextBlock Grid.Row="2" Grid.Column="0" Text="Notes:" />
    <TextBox Grid.Row="2" Grid.Column="1" />
</layouts:GridEx>

<!-- Example 3: Proportional sizing -->
<layouts:GridEx Rows="2*,1*,1*" Columns="2*,1*,1*">
    <!-- Cell [0,0] is double the size of cell [2,2] -->
</layouts:GridEx>

<!-- Example 4: Mixed sizing -->
<layouts:GridEx Rows="Auto,*,66" Columns="200,*,Auto">
    <!-- Row 0: Auto-sized, Row 1: fills remaining, Row 2: 66px -->
    <!-- Col 0: 200px, Col 1: fills remaining, Col 2: Auto-sized -->
</layouts:GridEx>
```

---

### 🟢 Row / Col

**Location:** `Atc.Wpf.Controls.Layouts.Grid`

**Description:** Bootstrap-style responsive grid system with breakpoint-based column spanning.

**When to Use:**
- Responsive layouts
- Bootstrap-familiar developers
- Layouts that adapt to window size

**Col Properties:**

| Property | Type | Description |
|----------|------|-------------|
| Span | ColLayout | Column span at default breakpoint |
| Xs | ColLayout | Column span at extra-small breakpoint |
| Sm | ColLayout | Column span at small breakpoint |
| Md | ColLayout | Column span at medium breakpoint |
| Lg | ColLayout | Column span at large breakpoint |
| Xl | ColLayout | Column span at extra-large breakpoint |
| Xxl | ColLayout | Column span at extra-extra-large breakpoint |

**Examples:**

```xml
<!-- Example 1: Basic responsive columns -->
<layouts:Row>
    <layouts:Col Span="12" Md="6" Lg="4">
        <Border Background="LightBlue" Padding="16">
            <TextBlock Text="Column 1" />
        </Border>
    </layouts:Col>
    <layouts:Col Span="12" Md="6" Lg="4">
        <Border Background="LightGreen" Padding="16">
            <TextBlock Text="Column 2" />
        </Border>
    </layouts:Col>
    <layouts:Col Span="12" Md="12" Lg="4">
        <Border Background="LightCoral" Padding="16">
            <TextBlock Text="Column 3" />
        </Border>
    </layouts:Col>
</layouts:Row>

<!-- Example 2: Sidebar + Main content -->
<layouts:Row>
    <layouts:Col Span="12" Lg="3">
        <Border Background="#E3F2FD" Padding="16">
            <TextBlock Text="Sidebar" />
        </Border>
    </layouts:Col>
    <layouts:Col Span="12" Lg="9">
        <Border Background="#BBDEFB" Padding="16">
            <TextBlock Text="Main Content" />
        </Border>
    </layouts:Col>
</layouts:Row>
```

---

## 🎨 Container Controls

### 🟢 Badge

**Location:** `Atc.Wpf.Controls.Layouts`

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
<!-- Example 1: Notification count -->
<layouts:Badge BadgeContent="5" BadgePlacementMode="TopRight">
    <Button Content="Inbox" />
</layouts:Badge>

<!-- Example 2: Status dot -->
<layouts:Badge IsDot="True" BadgeBackground="LimeGreen" BadgePlacementMode="BottomRight">
    <Border Width="50" Height="50" CornerRadius="25" Background="Gray" />
</layouts:Badge>

<!-- Example 3: Max count with auto-hide -->
<layouts:Badge
    BadgeContent="{Binding UnreadCount}"
    BadgeMaxValue="99"
    HideWhenZero="True">
    <Button Content="Messages" />
</layouts:Badge>

<!-- Example 4: Custom styled badge -->
<layouts:Badge
    BadgeContent="NEW"
    BadgeBackground="#4CAF50"
    BadgeForeground="White"
    BadgePlacementMode="TopRight">
    <Border Width="100" Height="100" Background="LightGray">
        <TextBlock Text="Product" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
</layouts:Badge>
```

---

### 🟢 Card

**Location:** `Atc.Wpf.Controls.Layouts`

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
<!-- Example 1: Basic card -->
<layouts:Card Header="Settings" Elevation="2">
    <StackPanel>
        <CheckBox Content="Enable notifications" />
        <CheckBox Content="Dark mode" />
    </StackPanel>
</layouts:Card>

<!-- Example 2: Card with footer -->
<layouts:Card Header="User Profile" Elevation="3">
    <StackPanel>
        <TextBlock Text="John Doe" FontWeight="Bold" />
        <TextBlock Text="john@example.com" />
    </StackPanel>
    <layouts:Card.Footer>
        <Button Content="Edit" HorizontalAlignment="Right" />
    </layouts:Card.Footer>
</layouts:Card>

<!-- Example 3: Expandable card -->
<layouts:Card
    Header="Advanced Options"
    IsExpandable="True"
    IsExpanded="False"
    ExpanderButtonLocation="Right">
    <StackPanel>
        <CheckBox Content="Option 1" />
        <CheckBox Content="Option 2" />
    </StackPanel>
</layouts:Card>

<!-- Example 4: Custom styled card -->
<layouts:Card
    Header="Important"
    HeaderBackground="#FFF3E0"
    HeaderForeground="#E65100"
    Elevation="4"
    CornerRadius="8">
    <TextBlock Text="This is an important notice." />
</layouts:Card>
```

---

### 🟢 Chip

**Location:** `Atc.Wpf.Controls.Layouts`

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
<!-- Example 1: Basic chip -->
<layouts:Chip Content="C#" />

<!-- Example 2: Filter chip (selectable) -->
<layouts:Chip Content="JavaScript" Variant="Filter" IsSelected="True" />

<!-- Example 3: Input chip (removable) -->
<layouts:Chip
    Content="john@example.com"
    Variant="Input"
    RemoveClick="OnChipRemove" />

<!-- Example 4: Different sizes -->
<StackPanel Orientation="Horizontal">
    <layouts:Chip Content="Small" Size="Small" />
    <layouts:Chip Content="Medium" Size="Medium" />
    <layouts:Chip Content="Large" Size="Large" />
</StackPanel>

<!-- Example 5: Custom colors -->
<layouts:Chip
    Content="Success"
    Background="#E8F5E9"
    Foreground="#2E7D32" />
```

---

### 🟢 Divider

**Location:** `Atc.Wpf.Controls.Layouts`

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
<!-- Example 1: Simple horizontal divider -->
<layouts:Divider />

<!-- Example 2: Divider with text -->
<layouts:Divider Content="OR" />

<!-- Example 3: Dashed divider -->
<layouts:Divider LineStrokeDashArray="4,2" />

<!-- Example 4: Vertical divider -->
<StackPanel Orientation="Horizontal">
    <TextBlock Text="Left" />
    <layouts:Divider Orientation="Vertical" Margin="8,0" />
    <TextBlock Text="Right" />
</StackPanel>

<!-- Example 5: Styled divider -->
<layouts:Divider
    Content="Section"
    LineStroke="Gray"
    LineStrokeThickness="2" />
```

---

### 🟢 GridLines

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A debugging control that displays grid lines for layout visualization.

**When to Use:**
- Debugging layouts
- Visualizing grid structure
- Design-time assistance

---

### 🟢 GroupBoxExpander

**Location:** `Atc.Wpf.Controls.Layouts`

**Description:** A collapsible GroupBox with expand/collapse toggle button.

**When to Use:**
- Collapsible settings sections
- Form sections that can be hidden
- Space-saving UI patterns

**Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Header | object | - | Header content (inherited) |
| IsExpanded | bool | true | Current expanded state |
| ExpanderButtonLocation | ExpanderButtonLocation | Left | Toggle button position |
| CornerRadius | CornerRadius | - | Corner rounding |
| HeaderBackground | Brush | - | Header background color |
| HeaderForeground | Brush | - | Header text color |
| HeaderPadding | Thickness | 4 | Header inner padding |

**Events:**

| Event | Description |
|-------|-------------|
| Expanded | Raised when the control expands |
| Collapsed | Raised when the control collapses |

**Examples:**

```xml
<!-- Example 1: Basic collapsible section -->
<layouts:GroupBoxExpander Header="Optional Settings" IsExpanded="False">
    <StackPanel>
        <CheckBox Content="Enable feature A" />
        <CheckBox Content="Enable feature B" />
    </StackPanel>
</layouts:GroupBoxExpander>

<!-- Example 2: Right-aligned toggle -->
<layouts:GroupBoxExpander
    Header="Advanced"
    ExpanderButtonLocation="Right"
    HeaderBackground="#F5F5F5">
    <TextBox PlaceholderText="Advanced configuration..." />
</layouts:GroupBoxExpander>
```

---

## 📚 Summary

| Category | Count | Key Controls |
|----------|-------|--------------|
| 📦 Panels | 14 | FlexPanel, PanelEx, ResponsivePanel, StaggeredPanel, UniformSpacingPanel |
| 🔲 Grids | 4 | AutoGrid, GridEx, Row/Col |
| 🎨 Containers | 7 | Badge, Card, Chip, GroupBoxExpander |

---

*📅 Last Updated: January 2026*
*🤖 Generated with Claude Code*
