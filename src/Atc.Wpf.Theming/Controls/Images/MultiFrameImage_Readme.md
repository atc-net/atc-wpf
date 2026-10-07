# 🖼️ MultiFrameImage

An `Image` that picks the best-fitting frame from a multi-frame source such as an `.ico` file.

## 🔍 Overview

`MultiFrameImage` extends the standard WPF `Image`. When its `Source` is a `BitmapFrame` whose decoder contains several frames (for example an icon with 16, 32, 48 and 256 px versions), it renders the frame that best matches the current render size instead of always scaling the first frame. For each pixel size, the frame with the highest bits-per-pixel is used. `NiceWindow` uses it to render the window icon (see `NiceWindow.IconScalingMode`).

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Images;
```

## 🚀 Usage

### Basic Example

```xml
<atc:MultiFrameImage
    Width="32"
    Height="32"
    Source="/Assets/app.ico" />
```

### Choosing the Render Mode

```xml
<!-- Pick a frame at least as large as the render size and scale it down (default) -->
<atc:MultiFrameImage
    Width="20"
    Height="20"
    MultiFrameImageMode="ScaleDownLargerFrame"
    Source="/Assets/app.ico" />

<!-- Pick the largest frame that fits and draw it unscaled, centered -->
<atc:MultiFrameImage
    Width="20"
    Height="20"
    MultiFrameImageMode="NoScaleSmallerFrame"
    Source="/Assets/app.ico" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MultiFrameImageMode` | `MultiFrameImageMode` | `ScaleDownLargerFrame` | How the frame is selected and drawn. Affects render |

## 📊 Enumerations

### MultiFrameImageMode

| Value | Description |
|-------|-------------|
| `ScaleDownLargerFrame` | Uses the smallest frame whose width and height are at least the larger render dimension (or the largest frame if none is big enough), stretched to the render size |
| `NoScaleSmallerFrame` | Uses the largest frame that fits within the smaller render dimension (or the smallest frame if none fits), drawn at its own size and centered |

## 📝 Notes

- The frame list is rebuilt whenever `Source` changes
- If `Source` is not a `BitmapFrame` with decoder frames, the control renders like a normal `Image`
- Frames are grouped by pixel area; within each size the frame with the highest `BitsPerPixel` is kept

## 🔗 Related Controls

- **NiceWindow** - Uses `MultiFrameImage` for its title-bar icon (`IconScalingMode`)
- **AutoGreyableImage** - Image that turns grey when disabled

## 🎮 Sample Application

No dedicated sample yet.
