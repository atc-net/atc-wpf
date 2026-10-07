# ✏️ PathIcon

An icon element that renders a vector `Geometry` and follows its parent's foreground.

## 🔍 Overview

`PathIcon` derives from `IconElement` and draws the geometry set in `Data` through a `Path` template part (`PART_Path`). Like other `IconElement` types, it is not focusable and - when its own `Foreground` is not set locally - picks up the foreground of its visual parent, so an icon placed inside a button or menu item recolors with that host.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Icons;
```

## 🚀 Usage

### Basic Example

```xml
<atc:PathIcon
    Width="16"
    Height="16"
    Data="M0,0 L16,8 L0,16 Z" />
```

### Template Requirement

The library does not ship a default style for `PathIcon`, so supply a template that contains a `Path` named `PART_Path`:

```xml
<Style TargetType="{x:Type atc:PathIcon}">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="{x:Type atc:PathIcon}">
                <Viewbox>
                    <Path
                        x:Name="PART_Path"
                        Data="{TemplateBinding Data}"
                        Fill="{TemplateBinding Foreground}"
                        Stretch="Uniform" />
                </Viewbox>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Data` | `Geometry?` | `null` | The geometry to draw. Supports Path Markup Syntax in XAML (owner of `Path.DataProperty`) |
| `InheritsForegroundFromVisualParent` | `bool` | `false` | Read-only (inherited from `IconElement`). `true` when `Foreground` is default/inherited and the logical parent differs from the visual parent |

### Attached Properties (from IconElement)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IconElement.Geometry` | `Geometry` | `null` | General-purpose attached geometry |

## 📝 Notes

- `Focusable` is overridden to `false`
- `Foreground` defaults to `SystemColors.ControlTextBrush` and is inherited
- When `InheritsForegroundFromVisualParent` is `true`, `PART_Path.Fill` is set to the visual parent's `TextElement.Foreground`; when it becomes `false`, the local `Fill` is cleared
- No default style or template for `PathIcon` exists in the library; without a template the control renders nothing

## 🔗 Related Controls

- **FontIcon** - `IconElement` that renders a font glyph
- **SvgImage** - Renders SVG content
- **FontIcons** - Font-based icon sets in `Atc.Wpf.FontIcons`

## 🎮 Sample Application

No dedicated sample yet.
