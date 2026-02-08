# 🔗 Breadcrumb

A navigation path indicator showing the current location in a hierarchy with clickable segments.

## 🔍 Overview

`Breadcrumb` displays a hierarchical navigation path with clickable items separated by customizable separators. It supports overflow mode that collapses middle items into a dropdown menu when the breadcrumb exceeds `MaxVisibleItems`, keeping the first and last items always visible.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Breadcrumb

```xml
<dataDisplay:Breadcrumb>
    <dataDisplay:BreadcrumbItem Content="Home" />
    <dataDisplay:BreadcrumbItem Content="Products" />
    <dataDisplay:BreadcrumbItem Content="Electronics" />
    <dataDisplay:BreadcrumbItem Content="Laptops" />
</dataDisplay:Breadcrumb>
```

### With Custom Separator

```xml
<dataDisplay:Breadcrumb Separator=">">
    <dataDisplay:BreadcrumbItem Content="Root" />
    <dataDisplay:BreadcrumbItem Content="Folder" />
    <dataDisplay:BreadcrumbItem Content="File" />
</dataDisplay:Breadcrumb>
```

### Overflow Mode

```xml
<!-- Collapse middle items into dropdown when more than 3 visible -->
<dataDisplay:Breadcrumb OverflowMode="Collapsed" MaxVisibleItems="3">
    <dataDisplay:BreadcrumbItem Content="Home" />
    <dataDisplay:BreadcrumbItem Content="Category" />
    <dataDisplay:BreadcrumbItem Content="Subcategory" />
    <dataDisplay:BreadcrumbItem Content="Product" />
    <dataDisplay:BreadcrumbItem Content="Details" />
</dataDisplay:Breadcrumb>
```

### With Commands

```xml
<dataDisplay:Breadcrumb>
    <dataDisplay:BreadcrumbItem Content="Home" Command="{Binding NavigateCommand}" CommandParameter="home" />
    <dataDisplay:BreadcrumbItem Content="Settings" Command="{Binding NavigateCommand}" CommandParameter="settings" />
    <dataDisplay:BreadcrumbItem Content="Profile" />
</dataDisplay:Breadcrumb>
```

## ⚙️ Properties

### Breadcrumb Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Separator` | `object` | `"/"` | Separator between items |
| `SeparatorTemplate` | `DataTemplate?` | `null` | Template for separator |
| `OverflowMode` | `BreadcrumbOverflowMode` | `None` | Overflow behavior |
| `MaxVisibleItems` | `int` | `0` | Max visible items before overflow (0 = no limit) |
| `Items` | `ObservableCollection<BreadcrumbItem>` | empty | Breadcrumb items |

### BreadcrumbItem Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsLast` | `bool` | `false` | Whether this is the last (current) item |
| `Command` | `ICommand?` | `null` | Command executed on click |
| `CommandParameter` | `object?` | `null` | Parameter for the command |
| `HoverBackground` | `Brush?` | `null` | Background on hover |
| `HoverForeground` | `Brush?` | `null` | Foreground on hover |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ItemClicked` | `EventHandler<BreadcrumbItemClickedEventArgs>` | Raised when an item is clicked |

## 📊 Enumerations

### BreadcrumbOverflowMode

| Value | Description |
|-------|-------------|
| `None` | All items displayed (default) |
| `Collapsed` | Middle items collapse into overflow dropdown |

## 📝 Notes

- The last item is not clickable (represents the current location)
- In `Collapsed` mode, the first item and last N items remain visible
- Overflow dropdown shows the hidden middle items

## 🔗 Related Controls

- **Timeline** - Chronological event display
- **Stepper** - Step-by-step progress indicator

## 🎮 Sample Application

See the Breadcrumb sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Breadcrumb** for interactive examples.
