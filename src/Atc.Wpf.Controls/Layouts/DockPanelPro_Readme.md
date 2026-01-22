# 🏗️ DockPanelPro

An enhanced dock panel with resizable splitters, collapsible regions, and layout persistence. Perfect for building IDE-style interfaces with tool windows.

## 🔍 Overview

`DockPanelPro` extends the standard WPF DockPanel with professional features commonly found in development environments:

- 📏 **Resizable splitters** between docked regions
- 📌 **Collapsible regions** with pin/unpin functionality
- 💾 **Layout persistence** via JSON serialization
- 📐 **Size constraints** (MinWidth/MaxWidth/MinHeight/MaxHeight)
- 🎨 **Full theming support** with Light/Dark modes

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Quick Start

```xml
<layouts:DockPanelPro LayoutId="MyLayout" SplitterThickness="5">
    <!-- Left: Solution Explorer -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="250"
        MinWidth="150"
        MaxWidth="400"
        Header="Solution Explorer"
        IsCollapsible="True"
        RegionId="SolutionExplorer">
        <TreeView />
    </layouts:DockRegion>

    <!-- Right: Properties -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Right"
        Width="300"
        Header="Properties"
        IsCollapsible="True"
        RegionId="Properties">
        <StackPanel />
    </layouts:DockRegion>

    <!-- Bottom: Output -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="150"
        Header="Output"
        IsCollapsible="True"
        RegionId="Output">
        <TextBox IsReadOnly="True" />
    </layouts:DockRegion>

    <!-- Center: Main content -->
    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" RegionId="Editor">
        <TabControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

## ⚙️ DockPanelPro Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LayoutId` | `string` | `null` | Identifier for layout persistence |
| `AutoSaveLayout` | `bool` | `false` | Automatically save layout changes |
| `SplitterThickness` | `double` | `5` | Thickness of resizable splitters |
| `SplitterBackground` | `Brush` | from theme | Background brush for splitters |
| `AllowFloating` | `bool` | `false` | Enable floating windows (future feature) |

## 🔗 DockPanelPro Attached Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DockPanelPro.Dock` | `DockPosition` | `Center` | Dock position (Left, Right, Top, Bottom, Center) |

## ⚙️ DockRegion Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RegionId` | `string` | `null` | Unique identifier for persistence |
| `IsCollapsible` | `bool` | `false` | Enable collapse toggle button |
| `IsExpanded` | `bool` | `true` | Current expanded state (two-way binding) |
| `IsResizable` | `bool` | `true` | Allow resizing via splitters |
| `Header` | `object` | `null` | Header content |
| `HeaderBackground` | `Brush` | from theme | Header background color |
| `HeaderForeground` | `Brush` | from theme | Header text color |
| `HeaderPadding` | `Thickness` | `8` | Header inner padding |
| `ContentPadding` | `Thickness` | `0` | Content area padding |
| `CornerRadius` | `CornerRadius` | `0` | Corner rounding |
| `Width` | `double` | `NaN` | Initial/current width (for Left/Right regions) |
| `Height` | `double` | `NaN` | Initial/current height (for Top/Bottom regions) |
| `MinWidth` | `double` | `0` | Minimum width constraint |
| `MaxWidth` | `double` | `∞` | Maximum width constraint |
| `MinHeight` | `double` | `0` | Minimum height constraint |
| `MaxHeight` | `double` | `∞` | Maximum height constraint |

## 📡 DockRegion Events

| Event | Description |
|-------|-------------|
| `Expanded` | Raised when the region expands |
| `Collapsed` | Raised when the region collapses |
| `RegionSizeChanged` | Raised when the region's size changes via splitter drag |

## 📋 DockPosition Enumeration

| Value | Description |
|-------|-------------|
| `Left` | Dock to the left side |
| `Right` | Dock to the right side |
| `Top` | Dock to the top |
| `Bottom` | Dock to the bottom |
| `Center` | Fill the remaining space |

## 💾 Layout Persistence

### Saving Layout

```csharp
// Save current layout to JSON
string layoutJson = dockPanel.SaveLayout();

// Store in settings or file
File.WriteAllText("layout.json", layoutJson);
```

### Loading Layout

```csharp
// Load layout from JSON
string layoutJson = File.ReadAllText("layout.json");
dockPanel.LoadLayout(layoutJson);
```

### Resetting Layout

```csharp
// Reset all regions to default sizes
dockPanel.ResetLayout();
```

### 📄 Layout JSON Format

```json
{
  "layoutId": "MyLayout",
  "regions": [
    {
      "regionId": "SolutionExplorer",
      "dock": 0,
      "width": 250,
      "height": 0,
      "isExpanded": true
    },
    {
      "regionId": "Properties",
      "dock": 1,
      "width": 300,
      "height": 0,
      "isExpanded": true
    }
  ]
}
```

## 🖥️ Examples

### Basic IDE Layout

```xml
<layouts:DockPanelPro LayoutId="IDE" SplitterThickness="5">
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="250"
        Header="Explorer"
        IsCollapsible="True">
        <TreeView />
    </layouts:DockRegion>

    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="150"
        Header="Output"
        IsCollapsible="True">
        <TabControl>
            <TabItem Header="Output"><TextBox /></TabItem>
            <TabItem Header="Errors"><DataGrid /></TabItem>
        </TabControl>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center">
        <TabControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

### 📊 Dashboard with Navigation

```xml
<layouts:DockPanelPro>
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="200"
        MinWidth="150"
        Header="Navigation"
        IsResizable="True">
        <StackPanel>
            <Button Content="Dashboard" />
            <Button Content="Reports" />
            <Button Content="Settings" />
        </StackPanel>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" Header="Dashboard">
        <Grid />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

### 🔒 Non-Resizable Navigation

```xml
<layouts:DockPanelPro>
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="60"
        IsResizable="False">
        <!-- Icon-only navigation -->
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center">
        <ContentControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

## 🎨 Theming

DockPanelPro integrates with the Atc.Wpf theming system. Customize appearance using the helper attached properties:

```xml
<layouts:DockRegion
    helpers:DockPanelProHelper.ToggleCircleFill="LightGreen"
    helpers:DockPanelProHelper.ToggleCircleStroke="Green"
    helpers:DockPanelProHelper.ToggleArrowStroke="DarkGreen"
    Header="Custom Styled"
    IsCollapsible="True">
    <!-- Content -->
</layouts:DockRegion>
```

## 🔀 Comparison with Grid + GridSplitter

| Feature | Grid + GridSplitter | DockPanelPro |
|---------|---------------------|--------------|
| Resizable regions | ⚠️ Manual setup | ✅ Built-in |
| Collapsible panes | ❌ Not available | ✅ Built-in |
| Layout persistence | ⚠️ Manual implementation | ✅ Built-in |
| Min/Max constraints | ✅ Supported | ✅ Supported |
| XAML complexity | 🔴 High | 🟢 Low |
| IDE-style layouts | 🔴 Complex | 🟢 Simple |

## ✅ Best Practices

1. **Always set RegionId** for regions you want to persist
2. **Set reasonable Min/Max constraints** to prevent unusable layouts
3. **Use IsCollapsible sparingly** - typically for tool windows, not main content
4. **Center region should be last** - it fills remaining space
5. **Consider using AutoSaveLayout** for automatic persistence

## ⚠️ Limitations

Current limitations (may be addressed in future versions):

- Only one region per dock position (Left, Right, Top, Bottom)
- No floating/tear-off windows
- No tabbed regions at same dock position
- No snap points during resize
- No drag-and-drop region reordering

## 🔗 Related Controls

- **[FlexPanel](FlexPanel_Readme.md)** - CSS Flexbox-inspired layout
- **[Card](../Card_Readme.md)** - Content container with elevation
- **[GroupBoxExpander](../GroupBoxExpander_Readme.md)** - Collapsible grouped content

## 🎮 Sample Application

See the DockPanelPro sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > DockPanelPro** for interactive examples.
