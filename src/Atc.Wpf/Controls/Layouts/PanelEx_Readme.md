# 📐 PanelEx

A panel that measures and arranges all children to the same size as the panel itself.

## 🔍 Overview

`PanelEx` is a simple layout panel where every child element is measured and arranged to fill the entire panel area. It acts as an overlay container — all children occupy the same space, stacked by z-order.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

```xml
<atc:PanelEx>
    <Image Source="/Assets/background.png" />
    <TextBlock Text="Overlay Text" HorizontalAlignment="Center" VerticalAlignment="Center" />
</atc:PanelEx>
```

## 📝 Notes

- All children receive the full available size during measure and arrange
- Children are stacked visually (last child on top)
- No dependency properties — uses standard Panel behavior

## 🔗 Related Controls

- **GridEx** - String-based grid definitions
- **AutoGrid** - Auto-indexing grid

## 🎮 Sample Application

See the PanelEx sample in the Atc.Wpf.Sample application under **Wpf > Controls > Layouts** for interactive examples.
