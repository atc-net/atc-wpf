# 🖼️ ImageButton

A button with an image icon supporting bitmap, SVG, and configurable icon placement.

## 🔍 Overview

`ImageButton` extends the standard WPF `Button` with image support. It displays an icon alongside text content with configurable placement (left, top, right, bottom, center), supports both bitmap `ImageSource` and SVG images with color overrides, and includes a built-in loading indicator for async operations.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Buttons;
```

## 🚀 Usage

### Basic Image Button

```xml
<buttons:ImageButton
    Content="Open"
    ImageLocation="Left"
    ImageSource="/Assets/open.png"
    ImageWidth="16"
    ImageHeight="16" />
```

### SVG Image Button

```xml
<buttons:ImageButton
    Content="Settings"
    ImageLocation="Left"
    SvgImageSource="/MyApp;component/Assets/settings.svg"
    SvgImageOverrideColor="White" />
```

### Loading State

```xml
<buttons:ImageButton
    Content="Save"
    ImageLocation="Left"
    SvgImageSource="/MyApp;component/Assets/save.svg"
    IsBusy="{Binding IsSaving}"
    LoadingIndicatorMode="Ring"
    Command="{Binding SaveCommand}" />
```

### Icon Positions

```xml
<buttons:ImageButton Content="Left" ImageLocation="Left" ImageSource="/Assets/icon.png" />
<buttons:ImageButton Content="Top" ImageLocation="Top" ImageSource="/Assets/icon.png" />
<buttons:ImageButton Content="Right" ImageLocation="Right" ImageSource="/Assets/icon.png" />
<buttons:ImageButton Content="Bottom" ImageLocation="Bottom" ImageSource="/Assets/icon.png" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ImageLocation` | `ImageLocation?` | `null` | Icon position relative to content |
| `ImageWidth` | `int` | `16` | Image width in pixels |
| `ImageHeight` | `int` | `16` | Image height in pixels |
| `ImageContentSpacing` | `double` | `5.0` | Space between image and content |
| `ImageBorderSpacing` | `double` | `0.0` | Space between image and button border |
| `ImageSource` | `ImageSource?` | `null` | Bitmap image source |
| `SvgImageSource` | `string` | `""` | SVG resource path |
| `SvgImageOverrideColor` | `Color?` | `null` | Override all SVG fill colors |
| `LoadingIndicatorMode` | `LoadingIndicatorType` | `ArcsRing` | Style of loading indicator |
| `IsBusy` | `bool` | `false` | Show loading indicator instead of image |

## 📊 Enumerations

### ImageLocation

| Value | Description |
|-------|-------------|
| `Left` | Icon to the left of content |
| `Top` | Icon above content |
| `Right` | Icon to the right of content |
| `Bottom` | Icon below content |
| `Center` | Icon centered (no content offset) |

## 📝 Notes

- Use `SvgImageSource` for vector icons with `SvgImageOverrideColor` for theme-aware coloring
- When `IsBusy` is true, the image is replaced with a `LoadingIndicator`
- `ImageLocation` controls the grid layout of image and content

## 🔗 Related Controls

- **SplitButton** - Button with dropdown menu
- **ImageToggledButton** - Button that toggles between two image states
- **AuthenticationButton** - Login/logout toggle button
- **ConnectivityButton** - Connect/disconnect toggle button

## 🎮 Sample Application

See the ImageButton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Buttons > ImageButton** for interactive examples.
