# 🎨 FlyoutPresenter

A content container that provides a standardized header, scrollable content and footer layout for flyout content.

## 🔍 Overview

`FlyoutPresenter` is a `ContentControl` that gives flyout content a consistent structure: a header (title) section, a main content area with optional scrolling, and a footer section typically used for action buttons. Separator lines between the sections, paddings, brushes and scrolling behavior are all configurable. It is commonly placed inside a `Flyout` with the flyout's own close button and header turned off, but it can be used in any container.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Flyouts;
```

## 🚀 Usage

### Basic Example

```xml
<flyouts:Flyout Header="My Flyout" ShowCloseButton="False">
    <flyouts:FlyoutPresenter Header="Section Title">
        <StackPanel>
            <TextBlock Text="Main content goes here." />
            <TextBox Text="{Binding SomeValue}" />
        </StackPanel>
    </flyouts:FlyoutPresenter>
</flyouts:Flyout>
```

### With Footer Buttons

```xml
<flyouts:FlyoutPresenter Header="Edit Settings">
    <flyouts:FlyoutPresenter.Footer>
        <StackPanel HorizontalAlignment="Right" Orientation="Horizontal">
            <Button Margin="0,0,8,0" Content="Cancel" Click="Cancel_Click" />
            <Button Content="Save" Click="Save_Click" />
        </StackPanel>
    </flyouts:FlyoutPresenter.Footer>

    <StackPanel>
        <TextBlock Text="Name" />
        <TextBox Margin="0,4,0,16" Text="{Binding Name}" />
        <TextBlock Text="Description" />
        <TextBox Margin="0,4,0,0" Text="{Binding Description}" />
    </StackPanel>
</flyouts:FlyoutPresenter>
```

### Without Separators and Scrolling

```xml
<flyouts:FlyoutPresenter
    ContentPadding="24"
    Header="Summary"
    IsContentScrollable="False"
    ShowFooterSeparator="False"
    ShowHeaderSeparator="False">
    <TextBlock Text="Static content that should not scroll." />
</flyouts:FlyoutPresenter>
```

## ⚙️ Properties

### Header

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Header` | `object?` | `null` | Header content (header section is hidden when `null`) |
| `HeaderTemplate` | `DataTemplate?` | `null` | Template for the header content |
| `HeaderBackground` | `Brush?` | theme | Background brush for the header area |
| `HeaderForeground` | `Brush?` | theme | Foreground brush for the header text |
| `HeaderPadding` | `Thickness` | `16,12,16,12` | Padding around the header |
| `ShowHeader` | `bool` | `true` | Whether the header section is shown |
| `ShowHeaderSeparator` | `bool` | `true` | Whether a separator line is shown between header and content |

### Content

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ContentPadding` | `Thickness` | `16` | Padding around the main content |
| `IsContentScrollable` | `bool` | `true` | Whether the content area scrolls (`false` disables both scroll bars) |
| `VerticalScrollBarVisibility` | `ScrollBarVisibility` | `Auto` | Vertical scroll bar visibility of the content area |
| `HorizontalScrollBarVisibility` | `ScrollBarVisibility` | `Disabled` | Horizontal scroll bar visibility of the content area |

### Footer

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Footer` | `object?` | `null` | Footer content, typically buttons (footer section is hidden when `null`) |
| `FooterTemplate` | `DataTemplate?` | `null` | Template for the footer content |
| `FooterBackground` | `Brush?` | theme | Background brush for the footer area |
| `FooterPadding` | `Thickness` | `16,12,16,12` | Padding around the footer |
| `ShowFooter` | `bool` | `true` | Whether the footer section is shown |
| `ShowFooterSeparator` | `bool` | `true` | Whether a separator line is shown between content and footer |

### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CornerRadius` | `CornerRadius` | `0` | Corner radius of the presenter border |
| `SeparatorBrush` | `Brush?` | theme | Brush for the separator lines |
| `SeparatorThickness` | `double` | `1.0` | Thickness of the separator lines |

## 📝 Notes

- The header and its separator collapse when `ShowHeader` is `false` or `Header` is `null`; the same applies to the footer with `ShowFooter` / `Footer`
- The header text is rendered with font size 16 and `SemiBold` weight by the default template
- "theme" defaults come from the default style: `HeaderBackground` and `FooterBackground` use `AtcApps.Brushes.ThemeBackground`, `HeaderForeground` uses `AtcApps.Brushes.Text`, and `SeparatorBrush` uses `AtcApps.Brushes.Gray8`
- Template parts: `PART_Header`, `PART_Content`, `PART_Footer` (`ContentPresenter`) and `PART_ScrollViewer` (`ScrollViewer`)

## 🔗 Related Controls

- **Flyout** - Slide-in panel that typically hosts the presenter (see `Flyout_Readme.md`)
- **FlyoutHost** - Container managing multiple nested flyouts
- **Card** - Container for grouping related content

## 🎮 Sample Application

See the Flyout sample in the Atc.Wpf.Sample application under **Wpf.Components > Flyouts > Flyout** ("Open FlyoutPresenter Demo") for interactive examples.
