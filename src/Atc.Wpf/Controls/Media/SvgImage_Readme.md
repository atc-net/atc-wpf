# 🖼️ SvgImage

A control that renders SVG (Scalable Vector Graphics) drawings as images in WPF.

## 🔍 Overview

`SvgImage` is a WPF control that loads and displays SVG files with support for various sizing modes, color overrides, and animations. It provides a simple way to use vector graphics in your WPF applications.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Media;
```

## 🚀 Usage

### Basic Example

```xml
<atc:SvgImage
    ControlSizeType="ContentToSizeStretch"
    Source="/Atc.Wpf.Sample;component/Assets/icon.svg" />
```

### 🎨 With Color Override

```xml
<atc:SvgImage
    Source="/MyApp;component/Assets/logo.svg"
    OverrideColor="DodgerBlue"
    ControlSizeType="ContentToSizeNoStretch" />
```

### 📐 Different Size Modes

```xml
<!-- Stretch to fill -->
<atc:SvgImage
    Source="/MyApp;component/Assets/background.svg"
    ControlSizeType="ContentToSizeStretch" />

<!-- Maintain aspect ratio -->
<atc:SvgImage
    Source="/MyApp;component/Assets/icon.svg"
    ControlSizeType="ContentToSizeNoStretch" />

<!-- Size control to image -->
<atc:SvgImage
    Source="/MyApp;component/Assets/badge.svg"
    ControlSizeType="SizeToContent" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Source` | `string` | `null` | Relative URL or absolute pack URI (`pack://application:,,,/...`) of an SVG resource. Set "Build Action=Resource" for embedded files |
| `ControlSizeType` | `ControlSizeType` | `None` | How to stretch/resize/scale the drawing |
| `Background` | `Brush` | `null` | Background brush behind the SVG |
| `OverrideColor` | `Color?` | `null` | Override all fill colors in the SVG |
| `OverrideStrokeColor` | `Color?` | `null` | Override all stroke colors in the SVG |
| `OverrideStrokeWidth` | `double?` | `null` | Override stroke width in the SVG |
| `FileSource` | `string` | `null` | Path to external SVG file |
| `ImageSource` | `ImageSource` | `null` | Pre-loaded image source |
| `UseAnimations` | `bool` | `false` | Enable SVG animations |
| `CustomBrushes` | `Dictionary` | `null` | Custom brush mappings |
| `ExternalFileLoader` | `IExternalFileLoader` | `null` | Custom file loader for external resources |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `ImageFailed` | `SvgImageFailedEventArgs` | Raised (bubbling routed event) when `Source`, `FileSource` or `SetImage(string/Stream)` cannot load the SVG: a missing file or resource, an invalid path or malformed SVG. `ErrorException` holds the cause. The image then shows nothing; nothing is thrown, like `Image.ImageFailed` |

```xml
<atc:SvgImage
    FileSource="{Binding IconPath}"
    ImageFailed="OnSvgImageFailed" />
```

```csharp
private void OnSvgImageFailed(object? sender, SvgImageFailedEventArgs e)
    => logger.LogWarning(e.ErrorException, "Could not load the SVG icon");
```

## 📋 ControlSizeType Enumeration

| Value | Description |
|-------|-------------|
| `None` | Image is not scaled. Top-left corner aligned to control |
| `ContentToSizeNoStretch` | Image scaled to fit without stretching (maintains aspect ratio) |
| `ContentToSizeStretch` | Image stretched to fill entire width and height |
| `SizeToContent` | Control resized to fit the un-scaled image |

## 🔧 Methods

| Method | Description |
|--------|-------------|
| `SetImage(string)` | Load SVG from a string path |
| `SetImage(Stream)` | Load SVG from a stream |
| `SetImage(Drawing)` | Set content from a Drawing object |

## 📝 Notes

- For embedded resources, set the SVG file's **Build Action** to `Resource`
- SVG files are rendered as vector graphics, maintaining quality at any size
- Color overrides apply to all matching elements in the SVG
- Images loaded through `Source` or `FileSource` share one frozen drawing per source and render settings (`OverrideColor`, `OverrideStrokeColor`, `OverrideStrokeWidth`, `UseAnimations`), so an icon repeated in an item template is parsed and rendered once. Animated SVGs, images with `CustomBrushes` set by the application, and a custom `ExternalFileLoader` are not shared; `SetImage(...)` always loads its own copy. A file is identified by its full path, write time and size, so a changed file is loaded again
- A broken element (e.g. invalid path data or animation values) is skipped and the rest of the image still renders
- Hostile or runaway SVGs are bounded: elements nested deeper than 256 levels are dropped, at most 100,000 elements are rendered (counting what `<use>` expands to), DTDs are ignored (no entity expansion), and documents and decompressed `.svgz` files are capped at 64 MB
- `CustomBrushes` exposes the SVG's paint servers after loading; brushes set before loading replace the matching paint servers. After changing an entry in place, call `ReRenderSvg()`

## 🔗 Related Controls

- **AutoGreyableImage** - Image control that automatically greys out when disabled

## 🔗 References

**SVG** - Scalable Vector Graphics

- [W3C SVG Tiny 1.2 Specification](https://www.w3.org/TR/SVGTiny12)

## 🎮 Sample Application

See the SvgImage sample in the Atc.Wpf.Sample application under **Wpf > Media > SvgImage** for interactive examples.
