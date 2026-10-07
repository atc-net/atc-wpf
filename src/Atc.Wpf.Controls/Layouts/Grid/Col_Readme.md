# 🧱 Col

A column element for the Bootstrap-style `Row` panel, with a fixed or responsive span.

## 🔍 Overview

`Col` is a `ContentControl` placed inside a `Row`. It occupies a number of the row's 24 cells - either a fixed `Span`, or a per-breakpoint value supplied through `Layout` (a `ColLayout`). Columns can be shifted with `Offset`, or kept at their natural width with `IsFixed`.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts.Grid;
```

```xml
xmlns:grid="clr-namespace:Atc.Wpf.Controls.Layouts.Grid;assembly=Atc.Wpf.Controls"
```

## 🚀 Usage

### Fixed Span

```xml
<grid:Row Gutter="16">
    <grid:Col Span="16">
        <TextBlock Text="Two thirds" />
    </grid:Col>
    <grid:Col Span="8">
        <TextBlock Text="One third" />
    </grid:Col>
</grid:Row>
```

### Responsive Layout (Property Element)

```xml
<grid:Col>
    <grid:Col.Layout>
        <grid:ColLayout Xs="24" Sm="12" Md="8" Lg="6" Xl="4" Xxl="4" />
    </grid:Col.Layout>
    <TextBlock Text="Responsive column" />
</grid:Col>
```

### Responsive Layout (String Shorthand)

```xml
<!-- Xs,Sm,Md,Lg,Xl,Xxl -->
<grid:Col Layout="24,12,8,6,4,4">
    <TextBlock Text="Responsive column" />
</grid:Col>

<!-- A single number applies to every breakpoint -->
<grid:Col Layout="12">
    <TextBlock Text="Half width everywhere" />
</grid:Col>
```

### Offset

```xml
<grid:Row>
    <grid:Col Span="8" Offset="8">
        <TextBlock Text="Shifted by 8 cells" />
    </grid:Col>
</grid:Row>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Span` | `int` | `24` | Number of cells (1-24) used when `Layout` is not set. Values outside 1-24 are rejected |
| `Layout` | `ColLayout` | `null` | Per-breakpoint cell counts. When set, it takes precedence over `Span` |
| `Offset` | `int` | `0` | Shifts the column to the right by this many cell widths |
| `IsFixed` | `bool` | `false` | When `Layout` is set, the column ignores the breakpoint values and keeps its desired width |

### ColLayout Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Xs` | `int` | `24` | Cells for row width `< 768` |
| `Sm` | `int` | `12` | Cells for row width `768` - `991` |
| `Md` | `int` | `8` | Cells for row width `992` - `1199` |
| `Lg` | `int` | `6` | Cells for row width `1200` - `1919` |
| `Xl` | `int` | `4` | Cells for row width `1920` - `2559` |
| `Xxl` | `int` | `2` | Cells for row width `>= 2560` |

## 📝 Notes

- `ColLayout` can be written as a string of 1-6 comma-separated numbers in `Xs,Sm,Md,Lg,Xl,Xxl` order; a single number sets all breakpoints, and omitted trailing values keep their defaults
- `ColLayout` is also a markup extension, so `{grid:ColLayout Xs=24, Md=12}` is valid
- `IsFixed` only has an effect when `Layout` is set; with `Span` the column always uses its span
- The parent `Row` sets each `Col`'s `Margin` from its `Gutter`

## 🔗 Related Controls

- **Row** - The panel that hosts `Col` elements
- **ResponsivePanel** - Responsive layout panel with breakpoint-based spans
- **GridEx** - Grid with string-based row/column definitions

## 🎮 Sample Application

See the Row/Col sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > Row/Col** for interactive examples.
