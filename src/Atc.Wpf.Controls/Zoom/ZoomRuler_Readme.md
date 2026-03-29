# 📏 ZoomRuler

A zoom-aware ruler control that displays tick marks and position labels synchronized with a `ZoomBox`.

## 🔍 Overview

`ZoomRuler` renders adaptive tick marks and numeric labels that scale with the current zoom level. Place it along the top or left edge of a `ZoomBox` to provide position reference. Tick spacing adjusts automatically — coarse ticks at low zoom, fine ticks at high zoom. Every 5th tick is drawn as a major tick with a position label.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Zoom;
```

## 🚀 Usage

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="20" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="20" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <zoom:ZoomRuler Grid.Row="0" Grid.Column="1"
                    ZoomBox="{Binding ElementName=MyZoomBox}"
                    Orientation="Horizontal" />

    <zoom:ZoomRuler Grid.Row="1" Grid.Column="0"
                    ZoomBox="{Binding ElementName=MyZoomBox}"
                    Orientation="Vertical" />

    <zoom:ZoomScrollViewer Grid.Row="1" Grid.Column="1">
        <!-- Content -->
    </zoom:ZoomScrollViewer>
</Grid>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ZoomBox` | `ZoomBox?` | `null` | The ZoomBox to synchronize with |
| `Orientation` | `ZoomRulerOrientation` | `Horizontal` | Horizontal (top) or Vertical (left) |
| `TickBrush` | `Brush` | `Gray` | Brush for tick marks |
| `LabelBrush` | `Brush` | `Gray` | Brush for position labels |
| `FontSize` | `double` | `8.0` | Font size for position labels |

## 🔗 Related Controls

- **ZoomBox** — The zoom/pan control this ruler tracks
- **ZoomGridOverlay** — Zoom-adaptive grid overlay adorner