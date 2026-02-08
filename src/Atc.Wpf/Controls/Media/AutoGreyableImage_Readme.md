# 🖼️ AutoGreyableImage

An image control that automatically converts to greyscale when disabled.

## 🔍 Overview

`AutoGreyableImage` extends the WPF `Image` control to automatically render a greyscale version when `IsEnabled` is false. When re-enabled, the original color image is restored. Greyscale conversions and opacity brushes are cached for performance.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Media;
```

## 🚀 Usage

```xml
<!-- Automatically greys out when disabled -->
<atc:AutoGreyableImage
    Source="/Assets/icon.png"
    Width="32"
    Height="32"
    IsEnabled="{Binding IsFeatureAvailable}" />
```

## 📝 Notes

- Uses `FormatConvertedBitmap` with `Gray32Float` for greyscale conversion
- Applies an `ImageBrush` with 0.5 opacity for the disabled state
- Conversions are cached in `ConcurrentDictionary` keyed by source hash code
- Responds to both `Source` and `IsEnabled` changes

## 🔗 Related Controls

- **SvgImage** - SVG rendering with color overrides

## 🎮 Sample Application

See the AutoGreyableImage sample in the Atc.Wpf.Sample application under **Wpf > Media > AutoGreyableImage** for interactive examples.
