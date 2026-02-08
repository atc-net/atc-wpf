# 📅 Timeline

A vertical or horizontal timeline for displaying chronological events with connecting lines and dots.

## 🔍 Overview

`Timeline` displays a sequence of events along a vertical or horizontal axis with customizable dots, connecting lines, and content. It supports left, right, and alternating layout modes, dashed/dotted/solid connector styles, per-item dot customization, and opposite-side content for alternating layouts.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Vertical Timeline

```xml
<dataDisplay:Timeline>
    <dataDisplay:TimelineItem Content="Project started" />
    <dataDisplay:TimelineItem Content="Design phase completed" />
    <dataDisplay:TimelineItem Content="Development in progress" />
    <dataDisplay:TimelineItem Content="Testing" />
</dataDisplay:Timeline>
```

### Alternating Layout

```xml
<dataDisplay:Timeline Mode="Alternate">
    <dataDisplay:TimelineItem Content="2024-01-15" OppositeContent="Sprint 1 started" />
    <dataDisplay:TimelineItem Content="2024-02-01" OppositeContent="First release" />
    <dataDisplay:TimelineItem Content="2024-03-15" OppositeContent="Major update" />
</dataDisplay:Timeline>
```

### 🎨 Custom Dots

```xml
<dataDisplay:Timeline DotBrush="DodgerBlue" DotSize="16" LineBrush="LightGray">
    <dataDisplay:TimelineItem Content="Step 1">
        <dataDisplay:TimelineItem.DotContent>
            <TextBlock Text="1" Foreground="White" FontSize="10" />
        </dataDisplay:TimelineItem.DotContent>
    </dataDisplay:TimelineItem>
    <dataDisplay:TimelineItem Content="Step 2" DotBrush="Green">
        <dataDisplay:TimelineItem.DotContent>
            <TextBlock Text="✓" Foreground="White" FontSize="10" />
        </dataDisplay:TimelineItem.DotContent>
    </dataDisplay:TimelineItem>
</dataDisplay:Timeline>
```

### Dashed Connectors

```xml
<dataDisplay:Timeline ConnectorStyle="Dashed" LineThickness="2">
    <dataDisplay:TimelineItem Content="Planned" />
    <dataDisplay:TimelineItem Content="In Progress" />
    <dataDisplay:TimelineItem Content="Complete" />
</dataDisplay:Timeline>
```

### Horizontal Timeline

```xml
<dataDisplay:Timeline Orientation="Horizontal" ItemSpacing="40">
    <dataDisplay:TimelineItem Content="Q1" />
    <dataDisplay:TimelineItem Content="Q2" />
    <dataDisplay:TimelineItem Content="Q3" />
    <dataDisplay:TimelineItem Content="Q4" />
</dataDisplay:Timeline>
```

## ⚙️ Properties

### Timeline Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Vertical` | Layout direction |
| `Mode` | `TimelineMode` | `Left` | Content positioning mode |
| `DotBrush` | `Brush?` | `null` | Default dot color |
| `DotSize` | `double` | `12` | Default dot diameter |
| `LineBrush` | `Brush?` | `null` | Default connector line color |
| `LineThickness` | `double` | `2` | Connector line thickness |
| `ConnectorStyle` | `TimelineConnectorStyle` | `Solid` | Line dash style |
| `ItemSpacing` | `double` | `20` | Spacing between items |
| `Items` | `ObservableCollection<TimelineItem>` | empty | Collection of timeline items |

### TimelineItem Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DotContent` | `object?` | `null` | Custom content inside the dot |
| `DotTemplate` | `DataTemplate?` | `null` | Template for dot content |
| `DotBrush` | `Brush?` | `null` | Override dot color (falls back to Timeline.DotBrush) |
| `DotSize` | `double` | `12` | Override dot size |
| `OppositeContent` | `object?` | `null` | Content on the opposite side (for Alternate mode) |
| `OppositeContentTemplate` | `DataTemplate?` | `null` | Template for opposite content |
| `LineStroke` | `Brush?` | `null` | Override connector line color |
| `Position` | `TimelineItemPosition` | `Default` | Force item position (overrides Mode) |

## 📊 Enumerations

### TimelineMode

| Value | Description |
|-------|-------------|
| `Left` | All content on the left of the axis (default) |
| `Right` | All content on the right of the axis |
| `Alternate` | Content alternates sides (even=left, odd=right) |

### TimelineConnectorStyle

| Value | Description |
|-------|-------------|
| `Solid` | Continuous line (default) |
| `Dashed` | 4px dash, 2px gap |
| `Dotted` | 1px dot, 2px gap |

### TimelineItemPosition

| Value | Description |
|-------|-------------|
| `Default` | Use parent Timeline's Mode |
| `Left` | Force this item to the left |
| `Right` | Force this item to the right |

## 📝 Notes

- Per-item `DotBrush` and `LineStroke` override the parent Timeline's defaults
- The last item's connector line is hidden automatically
- In Alternate mode, use `OppositeContent` to display secondary information on the other side
- `TimelineItemPosition` can override the parent Mode for individual items

## 🔗 Related Controls

- **Stepper** - Step-by-step progress indicator
- **Divider** - Simple visual separator
- **Breadcrumb** - Navigation path indicator

## 🎮 Sample Application

See the Timeline sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Timeline** for interactive examples.
