# 📦 GroupBoxExpander

A collapsible group box with optional expand/collapse toggle and customizable header styling.

## 🔍 Overview

`GroupBoxExpander` combines the visual grouping of a GroupBox with expand/collapse functionality. It supports a configurable expander button position, corner radius, and separate header background/foreground styling. Use it to organize form sections or settings groups that can be collapsed to save space.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Usage

### Basic GroupBoxExpander

```xml
<layouts:GroupBoxExpander Header="Advanced Settings">
    <StackPanel>
        <CheckBox Content="Enable logging" />
        <CheckBox Content="Verbose mode" />
    </StackPanel>
</layouts:GroupBoxExpander>
```

### With Expander Button

```xml
<layouts:GroupBoxExpander Header="Options" ExpanderButtonLocation="Left" IsExpanded="False">
    <StackPanel>
        <TextBox Text="Option 1" />
        <TextBox Text="Option 2" />
    </StackPanel>
</layouts:GroupBoxExpander>
```

### 🎨 Custom Styling

```xml
<layouts:GroupBoxExpander
    Header="Styled Section"
    HeaderBackground="#2C5282"
    HeaderForeground="White"
    HeaderPadding="8"
    CornerRadius="8"
    ExpanderButtonLocation="Right"
    IsExpanded="True">
    <TextBlock Text="Content here" Margin="8" />
</layouts:GroupBoxExpander>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsExpanded` | `bool` | `true` | Whether content is expanded (two-way bindable) |
| `ExpanderButtonLocation` | `ExpanderButtonLocation` | `None` | Position of toggle button (None, Left, Right) |
| `CornerRadius` | `CornerRadius` | theme | Corner radius for the group box |
| `HeaderBackground` | `Brush?` | `null` | Header area background |
| `HeaderForeground` | `Brush?` | `null` | Header text color |
| `HeaderPadding` | `Thickness` | `4` | Padding around header |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Expanded` | `RoutedEventHandler` | Raised when content is expanded |
| `Collapsed` | `RoutedEventHandler` | Raised when content is collapsed |

## 📝 Notes

- When `ExpanderButtonLocation` is `None`, no toggle button is shown and the box is always expanded
- `IsExpanded` supports two-way binding and journaling

## 🔗 Related Controls

- **Card** - Content container with elevation and expand support
- **Flyout** - Sliding panel overlay

## 🎮 Sample Application

See the GroupBoxExpander sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > GroupBoxExpander** for interactive examples.
