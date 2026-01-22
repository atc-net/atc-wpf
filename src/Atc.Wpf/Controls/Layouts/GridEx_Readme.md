# 🔲 GridEx

An enhanced Grid control that supports string-based row and column definitions for cleaner, more concise XAML.

## 🔍 Overview

`GridEx` is an extension of the standard WPF `Grid` control that simplifies grid definitions by allowing you to specify rows and columns as comma-separated strings instead of verbose `RowDefinitions` and `ColumnDefinitions` collections.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### Basic Example

```xml
<atc:GridEx Rows="2*,1*,1*" Columns="2*,1*,1*">
    <Button Content="Button 1" />
    <Button Grid.Column="1" Content="Button 2" />
    <Button Grid.Column="2" Content="Button 3" />
    <Button Grid.Row="1" Content="Button 4" />
    <Button Grid.Row="1" Grid.Column="1" Content="Button 5" />
    <Button Grid.Row="1" Grid.Column="2" Content="Button 6" />
    <Button Grid.Row="2" Content="Button 7" />
    <Button Grid.Row="2" Grid.Column="1" Content="Button 8" />
    <Button Grid.Row="2" Grid.Column="2" Content="Button 9" />
</atc:GridEx>
```

### 🔄 Equivalent Standard Grid

The above is equivalent to this verbose standard Grid definition:

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="2*" />
        <ColumnDefinition Width="1*" />
        <ColumnDefinition Width="1*" />
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="2*" />
        <RowDefinition Height="1*" />
        <RowDefinition Height="1*" />
    </Grid.RowDefinitions>

    <Button Content="Button 1" />
    <Button Grid.Column="1" Content="Button 2" />
    <!-- ... -->
</Grid>
```

> 💡 **Tip:** `ColumnDefinitions` and `RowDefinitions` are reduced to simple `Columns` and `Rows` string properties in GridEx!

### 📐 Sizing Options

```xml
<!-- Auto-sized rows with star columns -->
<atc:GridEx Rows="Auto,Auto,*" Columns="200,*">
    <!-- Form layout -->
</atc:GridEx>

<!-- Mixed sizing: pixels, auto, and proportional -->
<atc:GridEx Rows="50,Auto,2*,*" Columns="100,*,Auto">
    <!-- Complex layout -->
</atc:GridEx>
```

## ⚙️ Properties

| Property | Type | Description |
|----------|------|-------------|
| `Rows` | `string` | Comma-separated row heights. Supports: `Auto`, `*`, `2*`, `100` (pixels) |
| `Columns` | `string` | Comma-separated column widths. Supports: `Auto`, `*`, `2*`, `100` (pixels) |

## 📋 Sizing Syntax

| Syntax | Description | Example |
|--------|-------------|---------|
| `Auto` | Size to content | `Rows="Auto,Auto"` |
| `*` | Fill remaining space | `Columns="*"` |
| `2*` | Proportional (2x of `*`) | `Rows="2*,1*"` |
| `100` | Fixed pixels | `Columns="200,*"` |

## 🎮 Sample Application

See the GridEx sample in the Atc.Wpf.Sample application under **Wpf > Layouts > GridEx** for interactive examples.
