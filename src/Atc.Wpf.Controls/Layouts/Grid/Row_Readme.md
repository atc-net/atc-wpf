# 🧱 Row

A Bootstrap-style 24-column row panel that lays out `Col` children with an optional gutter.

## 🔍 Overview

`Row` is a `Panel` that arranges its `Col` children on a 24-cell grid. Each `Col` takes a number of cells (its `Span`, or a responsive value from its `Layout`), and when the running total of cells exceeds 24 the next column wraps to a new line. The `Gutter` property adds uniform spacing around each column. The active responsive breakpoint (`Xs`-`Xxl`) is chosen from the row's own arranged width.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts.Grid;
```

```xml
xmlns:grid="clr-namespace:Atc.Wpf.Controls.Layouts.Grid;assembly=Atc.Wpf.Controls"
```

## 🚀 Usage

### Basic Example

```xml
<grid:Row Gutter="16">
    <grid:Col Span="8">
        <Border Padding="16" Background="#E3F2FD">
            <TextBlock Text="Col Span=8" />
        </Border>
    </grid:Col>
    <grid:Col Span="8">
        <Border Padding="16" Background="#BBDEFB">
            <TextBlock Text="Col Span=8" />
        </Border>
    </grid:Col>
    <grid:Col Span="8">
        <Border Padding="16" Background="#90CAF9">
            <TextBlock Text="Col Span=8" />
        </Border>
    </grid:Col>
</grid:Row>
```

### Responsive Columns

```xml
<grid:Row Gutter="16">
    <grid:Col Layout="24,12,8,6,4,4">
        <TextBlock Text="Column 1" />
    </grid:Col>
    <grid:Col Layout="24,12,8,6,4,4">
        <TextBlock Text="Column 2" />
    </grid:Col>
</grid:Row>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Gutter` | `double` | `0` | Spacing between columns. Each `Col` gets a uniform margin of `Gutter / 2`. Must be a finite value `>= 0`; invalid values are coerced to `0` |

## 📐 Responsive Breakpoints

The row picks the active breakpoint from its arranged width (constants on `ColLayout`):

| Breakpoint | Row width |
|------------|-----------|
| `Xs` | `< 768` |
| `Sm` | `768` - `991` |
| `Md` | `992` - `1199` |
| `Lg` | `1200` - `1919` |
| `Xl` | `1920` - `2559` |
| `Xxl` | `>= 2560` |

## 📝 Notes

- Only children of type `Col` are measured and arranged; other children are ignored
- The grid has 24 cells per line (`ColLayout.ColMaxCellCount`)
- Columns with a cell count of `0` or with `IsFixed="True"` keep their desired width; the remaining width is shared between the cell-based columns
- All lines use the height of the tallest column
- The `Gutter` overwrites the `Margin` of every `Col` child

## 🔗 Related Controls

- **Col** - The column element placed inside a `Row`
- **ResponsivePanel** - Responsive layout panel with breakpoint-based spans
- **GridEx** - Grid with string-based row/column definitions
- **AutoGrid** - Grid with automatic child positioning

## 🎮 Sample Application

See the Row/Col sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > Row/Col** for interactive examples.
