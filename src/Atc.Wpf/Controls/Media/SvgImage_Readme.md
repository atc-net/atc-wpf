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
| `Source` | `string` | `null` | Relative URL to SVG resource. Set "Build Action=Resource" for embedded files |
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

## 🔗 Related Controls

- **AutoGreyableImage** - Image control that automatically greys out when disabled

## 🔗 References

**SVG** - Scalable Vector Graphics

- [W3C SVG Tiny 1.2 Specification](https://www.w3.org/TR/SVGTiny12)

## 🎮 Sample Application

See the SvgImage sample in the Atc.Wpf.Sample application under **Wpf > Media > SvgImage** for interactive examples.
