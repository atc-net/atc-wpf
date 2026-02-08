# 🃏 Card

A content container with header, footer, elevation shadow, and optional expand/collapse behavior.

## 🔍 Overview

`Card` provides a Material Design-inspired container for grouping related content. It supports a header area, footer area, configurable shadow elevation (0-5 levels), and expand/collapse functionality with an animated toggle button. The card inherits from `HeaderedContentControl` for natural WPF content composition.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Card

```xml
<dataDisplay:Card Header="Settings">
    <StackPanel Margin="12">
        <TextBlock Text="Configure your application preferences." />
    </StackPanel>
</dataDisplay:Card>
```

### Card with Footer

```xml
<dataDisplay:Card Header="User Profile">
    <StackPanel Margin="12">
        <TextBlock Text="John Doe" FontSize="16" />
        <TextBlock Text="john.doe@example.com" />
    </StackPanel>
    <dataDisplay:Card.Footer>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="12">
            <Button Content="Edit" Margin="0,0,8,0" />
            <Button Content="Save" />
        </StackPanel>
    </dataDisplay:Card.Footer>
</dataDisplay:Card>
```

### 🎚️ Elevation Levels

```xml
<!-- No shadow -->
<dataDisplay:Card Header="Flat" Elevation="0">
    <TextBlock Text="No shadow" Margin="12" />
</dataDisplay:Card>

<!-- Subtle shadow -->
<dataDisplay:Card Header="Subtle" Elevation="1">
    <TextBlock Text="Light shadow" Margin="12" />
</dataDisplay:Card>

<!-- Default shadow -->
<dataDisplay:Card Header="Default" Elevation="2">
    <TextBlock Text="Standard shadow" Margin="12" />
</dataDisplay:Card>

<!-- Prominent shadow -->
<dataDisplay:Card Header="Raised" Elevation="5">
    <TextBlock Text="Deep shadow" Margin="12" />
</dataDisplay:Card>
```

### 📂 Expandable Card

```xml
<dataDisplay:Card Header="Advanced Settings" IsExpandable="True" IsExpanded="False">
    <StackPanel Margin="12">
        <CheckBox Content="Enable logging" />
        <CheckBox Content="Enable debug mode" />
    </StackPanel>
</dataDisplay:Card>
```

### 🎨 Custom Styling

```xml
<dataDisplay:Card
    Header="Branded"
    HeaderBackground="#2C5282"
    HeaderForeground="White"
    HeaderPadding="16"
    ContentPadding="16"
    CornerRadius="8"
    Elevation="3">
    <TextBlock Text="Custom styled card content." />
</dataDisplay:Card>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Elevation` | `int` | `2` | Shadow depth level (0-5, where 0 = no shadow) |
| `HeaderBackground` | `Brush?` | `null` | Background brush for the header area |
| `HeaderForeground` | `Brush?` | `null` | Foreground brush for header text |
| `HeaderPadding` | `Thickness` | `12` | Padding around the header |
| `ShowHeader` | `bool` | `true` | Whether to show the header area |
| `Footer` | `object?` | `null` | Footer content |
| `FooterTemplate` | `DataTemplate?` | `null` | Custom template for footer |
| `FooterBackground` | `Brush?` | `null` | Background brush for the footer |
| `FooterPadding` | `Thickness` | `12` | Padding around the footer |
| `ShowFooter` | `bool` | `true` | Whether to show the footer (auto-hides if Footer is null) |
| `IsExpandable` | `bool` | `false` | Whether the card supports expand/collapse |
| `IsExpanded` | `bool` | `true` | Whether the card is currently expanded |
| `ExpanderButtonLocation` | `ExpanderButtonLocation` | `Left` | Position of toggle button (Left or Right) |
| `CornerRadius` | `CornerRadius` | `4` | Corner radius for the card |
| `ContentPadding` | `Thickness` | `12` | Padding around the content area |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Expanded` | `RoutedEventHandler` | Raised when the card is expanded |
| `Collapsed` | `RoutedEventHandler` | Raised when the card is collapsed |

## 📊 Enumerations

### ExpanderButtonLocation

| Value | Description |
|-------|-------------|
| `Left` | Toggle button on the left side of the header (default) |
| `Right` | Toggle button on the right side of the header |

## 📝 Notes

- Elevation levels 0-5 provide increasingly deep shadows via blur radius and depth
- The footer auto-hides when `Footer` is null, regardless of `ShowFooter`
- Corner radius automatically adjusts based on which sections are visible
- `IsExpanded` supports two-way binding and journaling

## 🔗 Related Controls

- **GroupBoxExpander** - Collapsible grouping control
- **Carousel** - Cards can be used as carousel slide content
- **Badge** - Overlay cards with notification counts

## 🎮 Sample Application

See the Card sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Card** for interactive examples.
