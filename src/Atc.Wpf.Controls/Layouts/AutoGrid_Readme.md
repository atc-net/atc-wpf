# 🔲 AutoGrid

An enhanced Grid control with automatic child positioning and built-in spacing support.

## 🔍 Overview

`AutoGrid` extends the standard WPF `Grid` with automatic row/column indexing and simplified property-based definitions. It reduces XAML verbosity by automatically placing children in grid cells based on their order, eliminating manual `Grid.Row` and `Grid.Column` assignments.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### 🔢 Basic Auto-Indexing

```xml
<!-- Children are automatically placed in row-first order -->
<atc:AutoGrid ColumnCount="3" RowCount="2" ColumnWidth="*" RowHeight="*">
    <Button Content="1" />  <!-- [0,0] -->
    <Button Content="2" />  <!-- [0,1] -->
    <Button Content="3" />  <!-- [0,2] -->
    <Button Content="4" />  <!-- [1,0] -->
    <Button Content="5" />  <!-- [1,1] -->
    <Button Content="6" />  <!-- [1,2] -->
</atc:AutoGrid>
```

### 📝 String-Based Definitions

```xml
<!-- Define columns and rows as comma-separated strings -->
<atc:AutoGrid Columns="Auto,*,200" Rows="Auto,*">
    <TextBlock Text="Label:" />
    <TextBox />
    <Button Content="Browse" />
    <TextBlock Text="Content:" />
    <TextBox TextWrapping="Wrap" />
</atc:AutoGrid>
```

### 📏 Spacing Between Cells

```xml
<!-- Uniform spacing (10px between all cells) -->
<atc:AutoGrid Columns="*,*,*" Spacing="10">
    <Button Content="A" />
    <Button Content="B" />
    <Button Content="C" />
</atc:AutoGrid>

<!-- Separate horizontal and vertical spacing -->
<atc:AutoGrid Columns="Auto,*" HorizontalSpacing="20" VerticalSpacing="8" Rows="Auto,Auto,Auto">
    <TextBlock Text="Name:" VerticalAlignment="Center" />
    <TextBox />
    <TextBlock Text="Email:" VerticalAlignment="Center" />
    <TextBox />
    <TextBlock Text="Phone:" VerticalAlignment="Center" />
    <TextBox />
</atc:AutoGrid>
```

### 🎯 Child Layout Presets

```xml
<!-- Apply margin and alignment to all children -->
<atc:AutoGrid
    Columns="*,*,*"
    ChildMargin="5"
    ChildHorizontalAlignment="Stretch"
    ChildVerticalAlignment="Center">
    <Button Content="A" />
    <Button Content="B" />
    <Button Content="C" />
</atc:AutoGrid>
```

### 📐 Column and Row Spans

```xml
<!-- Spans are respected in auto-indexing -->
<atc:AutoGrid ColumnCount="3" RowCount="2" ColumnWidth="*" RowHeight="*">
    <Border Grid.ColumnSpan="2" Background="Blue" />
    <Border Background="Red" />
    <Border Background="Yellow" />
    <Border Grid.ColumnSpan="2" Background="Green" />
</atc:AutoGrid>
```

### 🔄 Vertical Orientation

```xml
<!-- Column-first auto-indexing -->
<atc:AutoGrid ColumnCount="3" RowCount="2" Orientation="Vertical" ColumnWidth="*" RowHeight="*">
    <Button Content="1" />  <!-- [0,0] -->
    <Button Content="2" />  <!-- [1,0] -->
    <Button Content="3" />  <!-- [0,1] -->
    <Button Content="4" />  <!-- [1,1] -->
    <Button Content="5" />  <!-- [0,2] -->
    <Button Content="6" />  <!-- [1,2] -->
</atc:AutoGrid>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Columns` | `string` | `""` | Comma-separated column widths (e.g., `"Auto,*,200"`) |
| `Rows` | `string` | `""` | Comma-separated row heights (e.g., `"Auto,*"`) |
| `ColumnCount` | `int` | `1` | Number of columns (when not using `Columns`) |
| `RowCount` | `int` | `1` | Number of rows (when not using `Rows`) |
| `ColumnWidth` | `GridLength` | `Auto` | Default width for columns created via `ColumnCount` |
| `RowHeight` | `GridLength` | `Auto` | Default height for rows created via `RowCount` |
| `Orientation` | `Orientation` | `Horizontal` | Auto-indexing direction (row-first or column-first) |
| `IsAutoIndexing` | `bool` | `true` | Enable/disable automatic child positioning |
| `Spacing` | `double` | `NaN` | Uniform spacing between cells (sets both H & V) |
| `HorizontalSpacing` | `double` | `NaN` | Horizontal gap between columns |
| `VerticalSpacing` | `double` | `NaN` | Vertical gap between rows |
| `ChildMargin` | `Thickness?` | `null` | Default margin for all children |
| `ChildHorizontalAlignment` | `HorizontalAlignment?` | `null` | Default horizontal alignment for all children |
| `ChildVerticalAlignment` | `VerticalAlignment?` | `null` | Default vertical alignment for all children |

## 📋 Sizing Syntax

| Syntax | Description | Example |
|--------|-------------|---------|
| `Auto` | Size to content | `Columns="Auto,Auto"` |
| `*` | Fill remaining space | `Rows="*"` |
| `2*` | Proportional (2x of `*`) | `Columns="2*,1*"` |
| `100` | Fixed pixels | `Rows="50,*"` |

## 📝 Notes

- `Spacing` sets both `HorizontalSpacing` and `VerticalSpacing`
- Individual spacing values (`HorizontalSpacing`, `VerticalSpacing`) override unified `Spacing`
- Spacing is applied as margin to children: right margin for horizontal, bottom margin for vertical
- Last column children get no right margin, last row children get no bottom margin
- `ChildMargin` and spacing are combined when both are specified
- Child preset properties (`ChildMargin`, `ChildHorizontalAlignment`, `ChildVerticalAlignment`) only apply if the child doesn't already have that value set
- Collapsed children are skipped during auto-indexing

## 🔗 Related Controls

- **GridEx** - Simple string-based row/column definitions without auto-indexing
- **UniformSpacingPanel** - Linear panel with uniform spacing
- **FlexPanel** - CSS Flexbox-style layouts

## 🎮 Sample Application

See the AutoGrid sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > AutoGrid** for interactive examples.
