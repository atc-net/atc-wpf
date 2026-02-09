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

Both sets support the same icon font families and share common animation/transformation features.

## 🎯 Available Icon Families

### Font Awesome 7 (recommended)

| Font Family | Font Control | Image Control | Enum Type | Icons |
|-------------|-------------|---------------|-----------|-------|
| **FA7 Solid** | `FontAwesomeSolid7` | `ImageAwesomeSolid7` | `FontAwesomeSolid7Type` | 1943 |
| **FA7 Regular** | `FontAwesomeRegular7` | `ImageAwesomeRegular7` | `FontAwesomeRegular7Type` | 283 |
| **FA7 Brand** | `FontAwesomeBrand7` | `ImageAwesomeBrand7` | `FontAwesomeBrand7Type` | 549 |

### Font Awesome 5 (legacy)

| Font Family | Font Control | Image Control | Enum Type | Icons |
|-------------|-------------|---------------|-----------|-------|
| **FA5 Solid** | `FontAwesomeSolid` | `ImageAwesomeSolid` | `FontAwesomeSolidType` | 766 |
| **FA5 Regular** | `FontAwesomeRegular` | `ImageAwesomeRegular` | `FontAwesomeRegularType` | 151 |
| **FA5 Brand** | `FontAwesomeBrand` | `ImageAwesomeBrand` | `FontAwesomeBrandType` | 378 |

### Other Icon Families

| Font Family | Font Control | Image Control | Enum Type | Icons |
|-------------|-------------|---------------|-----------|-------|
| **Bootstrap Glyphicons** | `FontBootstrap` | `ImageBootstrap` | `FontBootstrapType` | 262 |
| **Material Design** | `FontMaterialDesign` | `ImageMaterialDesign` | `FontMaterialDesignType` | 2696 |
| **Weather Icons** | `FontWeather` | `ImageWeather` | `FontWeatherType` | 584 |
| **IcoFont** | `IcoFont` | `ImageIcoFont` | `IcoFontType` | 2095 |

## 🚀 Usage

### Font Controls (TextBlock-based)

```xml
<!-- Font Awesome 7 Solid icon -->
<fontIcons:FontAwesomeSolid7 Icon="Home" Foreground="Blue" FontSize="24" />

<!-- Font Awesome 7 Regular icon -->
<fontIcons:FontAwesomeRegular7 Icon="Flag" Foreground="Red" FontSize="24" />

<!-- Material Design icon with rotation -->
<fontIcons:FontMaterialDesign Icon="Settings" Rotation="45" FontSize="32" />

<!-- Bootstrap icon with spinning animation -->
<fontIcons:FontBootstrap Icon="Refresh" Spin="True" SpinDuration="2" />

<!-- Weather icon with flip -->
<fontIcons:FontWeather Icon="DaySunny" FlipOrientation="Horizontal" />
```

### Image Controls (Image-based)

```xml
<!-- Font Awesome 7 as Image (for use in menus, buttons, etc.) -->
<fontIcons:ImageAwesomeSolid7 Icon="Save" Foreground="Green" Width="16" Height="16" />

<!-- Material Design as Image -->
<fontIcons:ImageMaterialDesign Icon="ContentCopy" Foreground="Gray" Width="20" Height="20" />
```

### Data Binding

```xml
<!-- Bind icon type from ViewModel -->
<fontIcons:FontAwesomeSolid7 Icon="{Binding CurrentIcon}" FontSize="24" />

<!-- Use in a Button -->
<Button>
    <StackPanel Orientation="Horizontal">
        <fontIcons:FontAwesomeSolid7 Icon="Save" FontSize="14" Margin="0,0,8,0" />
        <TextBlock Text="Save" />
    </StackPanel>
</Button>
```

### Static Image Generation (Code-Behind)

```csharp
// Create ImageSource for use in code
ImageSource icon = ImageAwesomeSolid7.CreateImageSource(
    FontAwesomeSolid7Type.Home,
    Brushes.Blue,
    emSize: 100);

// Create DrawingImage (vector, better for scaling)
DrawingImage drawing = ImageAwesomeSolid7.CreateDrawingImage(
    FontAwesomeSolid7Type.Settings,
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
- FA7 Regular and Solid share the WPF font family `"Font Awesome 7 Free"` — the Solid variant is selected via `FontWeights.Black`
- FA5 controls remain fully functional for backward compatibility

## 🔗 Related

- **[ValueConverters](../../src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)** — `FontIconImageSourceValueConverter`, `FontIconDrawingImageValueConverter`
- **FontIcon / PathIcon** — Lower-level icon controls in `Atc.Wpf.Theming`

## 🎮 Sample Application

See the font icon samples in the Atc.Wpf.Sample application under **Wpf.FontIcons** for:
- **Static** — Static icon rendering examples
- **Binding** — Data-bound icon examples
- **Viewer** — Interactive icon browser for all font families
