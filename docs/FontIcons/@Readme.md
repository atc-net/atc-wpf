# 🔤 Font Icons

The `Atc.Wpf.FontIcons` library provides font-based and image-based icon controls for rendering scalable vector icons from popular icon font families.

## 📍 Namespace

```csharp
using Atc.Wpf.FontIcons;
```

## 🔍 Overview

The library contains two parallel sets of controls:

- **Font controls** (`Font*`) — extend `TextBlock`, render icons as font glyphs
- **Image controls** (`Image*`) — extend `Image`, render icons as `ImageSource` bitmaps

Both sets support the same 7 icon font families and share common animation/transformation features.

## 🎯 Available Icon Families

| Font Family | Font Control | Image Control | Enum Type |
|-------------|-------------|---------------|-----------|
| **Font Awesome Solid** | `FontAwesomeSolid` | `ImageAwesomeSolid` | `FontAwesomeSolidType` |
| **Font Awesome Regular** | `FontAwesomeRegular` | `ImageAwesomeRegular` | `FontAwesomeRegularType` |
| **Font Awesome Brand** | `FontAwesomeBrand` | `ImageAwesomeBrand` | `FontAwesomeBrandType` |
| **Bootstrap Glyphicons** | `FontBootstrap` | `ImageBootstrap` | `FontBootstrapType` |
| **Material Design** | `FontMaterialDesign` | `ImageMaterialDesign` | `FontMaterialDesignType` |
| **Weather Icons** | `FontWeather` | `ImageWeather` | `FontWeatherType` |
| **IcoFont** | `IcoFont` | `ImageIcoFont` | `IcoFontType` |

## 🚀 Usage

### Font Controls (TextBlock-based)

```xml
<!-- Font Awesome Solid icon -->
<fontIcons:FontAwesomeSolid Icon="Home" Foreground="Blue" FontSize="24" />

<!-- Material Design icon with rotation -->
<fontIcons:FontMaterialDesign Icon="Settings" Rotation="45" FontSize="32" />

<!-- Bootstrap icon with spinning animation -->
<fontIcons:FontBootstrap Icon="Refresh" Spin="True" SpinDuration="2" />

<!-- Weather icon with flip -->
<fontIcons:FontWeather Icon="DaySunny" FlipOrientation="Horizontal" />
```

### Image Controls (Image-based)

```xml
<!-- Font Awesome as Image (for use in menus, buttons, etc.) -->
<fontIcons:ImageAwesomeSolid Icon="Save" Foreground="Green" Width="16" Height="16" />

<!-- Material Design as Image -->
<fontIcons:ImageMaterialDesign Icon="ContentCopy" Foreground="Gray" Width="20" Height="20" />
```

### Data Binding

```xml
<!-- Bind icon type from ViewModel -->
<fontIcons:FontAwesomeSolid Icon="{Binding CurrentIcon}" FontSize="24" />

<!-- Use in a Button -->
<Button>
    <StackPanel Orientation="Horizontal">
        <fontIcons:FontAwesomeSolid Icon="Save" FontSize="14" Margin="0,0,8,0" />
        <TextBlock Text="Save" />
    </StackPanel>
</Button>
```

### Static Image Generation (Code-Behind)

```csharp
// Create ImageSource for use in code
ImageSource icon = ImageAwesomeSolid.CreateImageSource(
    FontAwesomeSolidType.Home,
    Brushes.Blue,
    emSize: 100);

// Create DrawingImage
DrawingImage drawing = ImageMaterialDesign.CreateDrawingImage(
    FontMaterialDesignType.Settings,
    Brushes.Black,
    emSize: 64);
```

## ⚙️ Common Properties

All Font* and Image* controls share these properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Icon` | `*Type` enum | `None` | The icon to display (family-specific enum) |
| `Spin` | `bool` | `false` | Enable continuous spinning animation |
| `SpinDuration` | `double` | `1.0` | Spin animation duration in seconds |
| `Rotation` | `double` | `0` | Static rotation angle (0-360 degrees) |
| `FlipOrientation` | `FlipOrientationType` | `Normal` | Flip: Normal, Horizontal, or Vertical |

Image controls have an additional property:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Foreground` | `Brush` | `Brushes.Black` | Icon color |

Font controls inherit `Foreground` from `TextBlock`.

## 🔄 FlipOrientationType

| Value | Description |
|-------|-------------|
| `Normal` | No flip |
| `Horizontal` | Mirror horizontally (scaleX = -1) |
| `Vertical` | Mirror vertically (scaleY = -1) |

## 📝 Notes

- Font controls render as text glyphs (lighter, scalable via `FontSize`)
- Image controls render as bitmaps (better for `Image.Source` bindings, menus, toolbars)
- All icon enum types include a `None` value (0x0) as default
- Enum values are Unicode code points; the font glyph is resolved via `char.ConvertFromUtf32()`
- Spin, Rotation, and FlipOrientation can be combined simultaneously
- Font files are embedded as resources (no external dependencies)
- Icon enums are auto-generated from font metadata by `Atc.Wpf.Generator.FontIconResources`

## 🔗 Related

- **[ValueConverters](../../src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)** — `FontIconImageSourceValueConverter`, `FontIconDrawingImageValueConverter`
- **FontIcon / PathIcon** — Lower-level icon controls in `Atc.Wpf.Theming`

## 🎮 Sample Application

See the font icon samples in the Atc.Wpf.Sample application under **Wpf.FontIcons** for:
- **Static** — Static icon rendering examples
- **Binding** — Data-bound icon examples
- **Viewer** — Interactive icon browser for all font families
