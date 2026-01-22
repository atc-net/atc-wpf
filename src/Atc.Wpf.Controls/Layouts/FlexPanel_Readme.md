# 📦 FlexPanel

A CSS Flexbox-inspired layout panel for WPF that provides flexible box layout capabilities similar to CSS Flexbox.

## 🔍 Overview

`FlexPanel` is a custom WPF panel that implements a flex container layout algorithm. It supports:

- 🔄 **Direction**: Control the main axis direction (row, column, row-reverse, column-reverse)
- ↔️ **Justify Content**: Align children along the main axis (start, end, center, space-between, space-around, space-evenly)
- ↕️ **Align Items**: Align children along the cross axis (stretch, start, end, center, baseline)
- 🔁 **Wrap**: Control whether children wrap to new lines
- 📏 **Gap**: Set spacing between children
- 🎛️ **Per-child properties**: Grow, Shrink, Basis, and AlignSelf attached properties

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## ⚙️ Panel Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Direction` | `FlexDirection` | `Row` | The direction children are laid out |
| `JustifyContent` | `FlexJustify` | `Start` | How children are justified along the main axis |
| `AlignItems` | `FlexAlign` | `Stretch` | How children are aligned along the cross axis |
| `Wrap` | `FlexWrap` | `NoWrap` | Whether children should wrap |
| `Gap` | `double` | `0` | Gap between children (both row and column) |
| `RowGap` | `double` | `NaN` | Gap between rows (overrides Gap for cross-axis) |
| `ColumnGap` | `double` | `NaN` | Gap between columns (overrides Gap for main-axis) |

## 🔗 Attached Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FlexPanel.Grow` | `double` | `0` | How much the child should grow relative to siblings |
| `FlexPanel.Shrink` | `double` | `1` | How much the child should shrink relative to siblings |
| `FlexPanel.Basis` | `double` | `NaN` (auto) | The initial main size of the child |
| `FlexPanel.AlignSelf` | `FlexAlign` | `Auto` | Override AlignItems for this specific child |

## 📋 Enumerations

### 🔄 FlexDirection

| Value | Description |
|-------|-------------|
| `Row` | Children are laid out horizontally from left to right |
| `Column` | Children are laid out vertically from top to bottom |
| `RowReverse` | Children are laid out horizontally from right to left |
| `ColumnReverse` | Children are laid out vertically from bottom to top |

### ↔️ FlexJustify

| Value | Description |
|-------|-------------|
| `Start` | Children are packed at the start of the main axis |
| `End` | Children are packed at the end of the main axis |
| `Center` | Children are centered along the main axis |
| `SpaceBetween` | Children are evenly distributed; first at start, last at end |
| `SpaceAround` | Children are evenly distributed with equal space around each |
| `SpaceEvenly` | Children are evenly distributed with equal space between each |

### ↕️ FlexAlign

| Value | Description |
|-------|-------------|
| `Auto` | Uses the parent's AlignItems value (for AlignSelf only) |
| `Stretch` | Children are stretched to fill the cross axis |
| `Start` | Children are aligned at the start of the cross axis |
| `End` | Children are aligned at the end of the cross axis |
| `Center` | Children are centered along the cross axis |
| `Baseline` | Children are aligned at their baselines |

### 🔁 FlexWrap

| Value | Description |
|-------|-------------|
| `NoWrap` | Children are laid out in a single line and may overflow |
| `Wrap` | Children wrap onto multiple lines from top to bottom |
| `WrapReverse` | Children wrap onto multiple lines from bottom to top |

## 🚀 Basic Usage

### Simple Row Layout

```xml
<atc:FlexPanel Gap="10">
    <Button Content="Button 1" />
    <Button Content="Button 2" />
    <Button Content="Button 3" />
</atc:FlexPanel>
```

### Column Layout with Center Alignment

```xml
<atc:FlexPanel
    Direction="Column"
    AlignItems="Center"
    Gap="10">
    <Button Content="Button 1" />
    <Button Content="Button 2" />
    <Button Content="Button 3" />
</atc:FlexPanel>
```

### Justify Content Examples

```xml
<!-- Space between items -->
<atc:FlexPanel JustifyContent="SpaceBetween">
    <Button Content="Start" />
    <Button Content="Middle" />
    <Button Content="End" />
</atc:FlexPanel>

<!-- Center items -->
<atc:FlexPanel JustifyContent="Center">
    <Button Content="Centered" />
    <Button Content="Items" />
</atc:FlexPanel>
```

### 📈 Using Grow Property

The `Grow` property distributes extra space among children proportionally:

```xml
<atc:FlexPanel Gap="10">
    <Border atc:FlexPanel.Grow="1" Background="Blue" />
    <Border atc:FlexPanel.Grow="2" Background="Green" />
    <Border atc:FlexPanel.Grow="1" Background="Red" />
</atc:FlexPanel>
```

In this example, the green border will take twice as much extra space as the blue and red borders (1:2:1 ratio).

### 🔁 Wrapping Items

```xml
<atc:FlexPanel Wrap="Wrap" Gap="10">
    <Button Width="100" Content="Item 1" />
    <Button Width="100" Content="Item 2" />
    <Button Width="100" Content="Item 3" />
    <Button Width="100" Content="Item 4" />
    <Button Width="100" Content="Item 5" />
</atc:FlexPanel>
```

### 🎯 Individual Child Alignment with AlignSelf

```xml
<atc:FlexPanel AlignItems="Stretch" Gap="10" Height="100">
    <Border atc:FlexPanel.AlignSelf="Start" Background="Blue" />
    <Border atc:FlexPanel.AlignSelf="Center" Background="Green" />
    <Border atc:FlexPanel.AlignSelf="End" Background="Red" />
    <Border atc:FlexPanel.AlignSelf="Stretch" Background="Purple" />
</atc:FlexPanel>
```

### 📐 Using Basis Property

The `Basis` property sets the initial size of an item before grow/shrink is applied:

```xml
<atc:FlexPanel Gap="10">
    <Border atc:FlexPanel.Basis="100" atc:FlexPanel.Grow="1" Background="Blue" />
    <Border atc:FlexPanel.Basis="200" atc:FlexPanel.Grow="1" Background="Green" />
</atc:FlexPanel>
```

## 🔀 CSS Flexbox Comparison

| CSS Property | FlexPanel Property |
|--------------|-------------------|
| `flex-direction` | `Direction` |
| `justify-content` | `JustifyContent` |
| `align-items` | `AlignItems` |
| `flex-wrap` | `Wrap` |
| `gap` | `Gap` |
| `row-gap` | `RowGap` |
| `column-gap` | `ColumnGap` |
| `flex-grow` | `FlexPanel.Grow` |
| `flex-shrink` | `FlexPanel.Shrink` |
| `flex-basis` | `FlexPanel.Basis` |
| `align-self` | `FlexPanel.AlignSelf` |

## 📝 Notes

- The `AlignItems` property with value `Auto` is equivalent to `Stretch` for the panel itself
- The `AlignSelf` attached property with value `Auto` inherits from the parent's `AlignItems`
- Negative values are not allowed for `Gap`, `Grow`, `Shrink`, or `Basis`
- The `Shrink` property defaults to `1`, meaning items will shrink by default when there isn't enough space
- The `Basis` property defaults to `NaN` (auto), meaning the item's natural size is used

## 🎮 Sample Application

See the FlexPanel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > FlexPanel** for an interactive demonstration of all features.
