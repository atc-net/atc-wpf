# 📦 VirtualizingStaggeredPanel

A virtualizing masonry/waterfall layout panel for efficient rendering of large item collections.

## 🔍 Overview

`VirtualizingStaggeredPanel` provides the same masonry/waterfall layout as `StaggeredPanel` but with UI virtualization support via `IScrollInfo`. Only visible items are realized, making it suitable for large data sets with thousands of items. Items are placed in the shortest column, creating an efficient waterfall effect.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

```xml
<ScrollViewer CanContentScroll="True">
    <ItemsControl ItemsSource="{Binding Items}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <layouts:VirtualizingStaggeredPanel
                    DesiredItemWidth="250"
                    HorizontalSpacing="8"
                    VerticalSpacing="8"
                    Padding="16" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
    </ItemsControl>
</ScrollViewer>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DesiredItemWidth` | `double` | `250` | Target width for each column |
| `Padding` | `Thickness` | `0` | Padding around the panel |
| `HorizontalSpacing` | `double` | `0` | Horizontal gap between columns |
| `VerticalSpacing` | `double` | `0` | Vertical gap between rows |

## 📝 Notes

- Requires `CanContentScroll="True"` on the parent `ScrollViewer` for virtualization
- Column count is calculated automatically from available width and `DesiredItemWidth`
- Implements `IScrollInfo` for smooth scrolling integration
- Unused containers are recycled for performance

## 🔗 Related Controls

- **StaggeredPanel** - Non-virtualizing masonry layout
- **FlexPanel** - CSS Flexbox-style layout
- **ResponsivePanel** - Responsive grid layout

## 🎮 Sample Application

See the VirtualizingStaggeredPanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > VirtualizingStaggeredPanel** for interactive examples.
