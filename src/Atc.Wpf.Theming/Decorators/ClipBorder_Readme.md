# ✂️ ClipBorder

A border decorator whose child content is clipped to the (optionally rounded) inner bounds of the border.

## 🔍 Overview

`ClipBorder` behaves like the standard WPF `Border` - it draws a background and a border with `BorderThickness`, `Padding` and `CornerRadius` - but it also clips its child to the rounded inner area. With a standard `Border`, content such as images or colored panels bleeds over rounded corners; `ClipBorder` keeps the child inside them. It is used throughout the theme templates (for example `Button`, `ComboBox` and `NumericBox`) to render rounded control chrome.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Decorators;
```

```xml
xmlns:decorators="clr-namespace:Atc.Wpf.Theming.Decorators;assembly=Atc.Wpf.Theming"
```

## 🚀 Usage

### Basic Example

```xml
<decorators:ClipBorder
    Width="200"
    Height="120"
    BorderBrush="{DynamicResource AtcApps.Brushes.Accent}"
    BorderThickness="2"
    CornerRadius="12">
    <!-- The image corners are clipped to the rounded border -->
    <Image Source="/Assets/photo.jpg" Stretch="UniformToFill" />
</decorators:ClipBorder>
```

### In a Control Template

```xml
<ControlTemplate TargetType="{x:Type ButtonBase}">
    <Grid>
        <decorators:ClipBorder
            x:Name="Border"
            Background="{TemplateBinding Background}"
            BorderBrush="{TemplateBinding BorderBrush}"
            BorderThickness="{TemplateBinding BorderThickness}"
            CornerRadius="{TemplateBinding atc:ControlsHelper.CornerRadius}"
            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" />
        <ContentPresenter Margin="{TemplateBinding BorderThickness}" />
    </Grid>
</ControlTemplate>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BorderThickness` | `Thickness` | `0` | Thickness of the border (affects measure and render) |
| `Padding` | `Thickness` | `0` | Space between the border and the child |
| `CornerRadius` | `CornerRadius` | `0` | Radius of the corners; the child is clipped to the matching inner shape |
| `BorderBrush` | `Brush?` | `null` | Brush used to draw the border |
| `Background` | `Brush?` | `null` | Brush used to fill the area inside the border |
| `OptimizeClipRendering` | `bool` | `false` | When `true`, the whole border area is filled with `BorderBrush` only (background is not drawn separately) |

## 📝 Notes

- `BorderThickness`, `Padding` and `CornerRadius` reject negative, `NaN` and infinite values (validation callbacks)
- The child is arranged inside the border and padding, and its `Clip` is set to a geometry matching the inner rounded rectangle
- When `BorderBrush` and `Background` are both set, the control picks a rendering strategy depending on whether the brushes are equal and/or opaque solid colors, avoiding anti-aliasing seams between border and background
- Use `OptimizeClipRendering="True"` when the border and background share the same brush to render a single geometry
- Border and background geometries are cached and rebuilt on arrange

## 🔗 Related Controls

- **Border** - Standard WPF border (does not clip its child)
- **ContentControlEx** - Content control used together with `ClipBorder` in the theme templates

## 🎮 Sample Application

No dedicated sample yet. `ClipBorder` is used by the themed controls shown under **Wpf.Theming** (for example **Button**) in the Atc.Wpf.Sample application.
