# ➖ Divider

A visual separator for dividing content sections with optional inline text.

## 🔍 Overview

`Divider` renders a horizontal or vertical line to visually separate content areas. It supports inline text content positioned at the center, left, or right of the line, as well as dashed line styles. The control uses source generator attributes for dependency properties.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Horizontal Divider

```xml
<dataDisplay:Divider />
```

### Divider with Text

```xml
<dataDisplay:Divider Content="OR" />
```

### 📐 Text Alignment

```xml
<!-- Centered text (default) -->
<dataDisplay:Divider Content="Section" HorizontalContentAlignment="Center" />

<!-- Left-aligned text -->
<dataDisplay:Divider Content="Details" HorizontalContentAlignment="Left" />

<!-- Right-aligned text -->
<dataDisplay:Divider Content="More" HorizontalContentAlignment="Right" />
```

### 📏 Vertical Divider

```xml
<StackPanel Orientation="Horizontal" Height="40">
    <TextBlock Text="Left" VerticalAlignment="Center" />
    <dataDisplay:Divider Orientation="Vertical" />
    <TextBlock Text="Right" VerticalAlignment="Center" />
</StackPanel>
```

### 🎨 Custom Styling

```xml
<!-- Dashed line -->
<dataDisplay:Divider LineStrokeDashArray="4,2" Content="Dashed" />

<!-- Custom color and thickness -->
<dataDisplay:Divider LineStroke="Red" LineStrokeThickness="2" />

<!-- Thick dotted line -->
<dataDisplay:Divider LineStrokeThickness="2" LineStrokeDashArray="1,2" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Content` | `object?` | `null` | Inline content displayed between divider lines |
| `ContentTemplate` | `DataTemplate?` | `null` | Custom template for content |
| `ContentStringFormat` | `string?` | `null` | Format string for content display |
| `ContentTemplateSelector` | `DataTemplateSelector?` | `null` | Dynamic template selection |
| `Orientation` | `Orientation` | `Horizontal` | Line direction (Horizontal or Vertical) |
| `LineStroke` | `Brush?` | Accent | Line color brush |
| `LineStrokeThickness` | `double` | `1.0` | Line thickness in pixels |
| `LineStrokeDashArray` | `DoubleCollection` | empty | Dash pattern (e.g., `4,2` for dashed) |

## 📝 Notes

- When `Content` is null, padding is removed and the line spans the full width
- In horizontal mode with left/right alignment, only one line segment is shown
- Vertical mode renders a single line without content support
- Default margin is `0,24` (vertical spacing around horizontal dividers)

## 🔗 Related Controls

- **Card** - Content container that often uses dividers between sections
- **GroupBoxExpander** - Collapsible section with built-in header separator

## 🎮 Sample Application

See the Divider sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Divider** for interactive examples.
