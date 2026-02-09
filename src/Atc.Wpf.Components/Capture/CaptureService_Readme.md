# CaptureService

## Overview

`ICaptureService` / `CaptureService` provides MVVM-friendly screen and control capture functionality. It supports capturing any visual element, a rectangular region, or a full window to bitmap, file, or clipboard.

## Namespace

`Atc.Wpf.Capture` (interface, settings, result)
`Atc.Wpf.Components.Capture` (implementation)

## Usage

### Capture a control to file

```csharp
ICaptureService captureService = new CaptureService();

var bitmap = captureService.CaptureVisual(myPanel);
var result = captureService.SaveToFile(bitmap, "screenshot.png");
```

### Capture to clipboard

```csharp
var bitmap = captureService.CaptureVisual(myButton);
var result = captureService.CopyToClipboard(bitmap);
```

### Capture a region

```csharp
var region = new Int32Rect(10, 10, 200, 150);
var bitmap = captureService.CaptureRegion(myPanel, region);
var result = captureService.SaveToFile(bitmap, "region.png");
```

### Capture a window

```csharp
var bitmap = captureService.CaptureWindow(Application.Current.MainWindow);
var result = captureService.SaveToFile(bitmap, "window.png");
```

### Custom DPI

```csharp
var settings = new CaptureSettings { DpiX = 192, DpiY = 192 };
var bitmap = captureService.CaptureVisual(myPanel, settings);
```

### Format options

```csharp
var bitmap = captureService.CaptureVisual(myPanel);
captureService.SaveToFile(bitmap, "output.jpg", ImageFormatType.Jpeg);
captureService.SaveToFile(bitmap, "output.bmp", ImageFormatType.Bmp);
```

### Checking results

```csharp
var result = captureService.SaveToFile(bitmap, "output.png");
if (result.IsSuccess)
    Log($"Saved {result.Width}x{result.Height} ({result.FileSizeBytes} bytes)");
else
    Log($"Error: {result.ErrorMessage}");
```

## Properties

### CaptureSettings

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| DpiX | double | 96 | Horizontal rendering DPI |
| DpiY | double | 96 | Vertical rendering DPI |

### CaptureResult

| Property | Type | Description |
|----------|------|-------------|
| IsSuccess | bool | Capture completed successfully |
| FilePath | string? | Path where image was saved |
| Width | int | Captured image width in pixels |
| Height | int | Captured image height in pixels |
| FileSizeBytes | long | File size in bytes (file saves only) |
| ErrorMessage | string? | Error details if failed |

## Notes

- All capture methods are synchronous (WPF rendering APIs require the UI thread)
- The service renders visuals using `RenderTargetBitmap` with `VisualBrush`
- Region capture uses `CroppedBitmap` for zero-copy cropping
- Supported formats: PNG, JPEG, BMP, GIF, TIFF, WMP (via `ImageFormatType`)
- Higher DPI settings produce larger images with more detail

## Related Controls

- [PrintService](../Printing/PrintService.cs) - Similar service pattern for printing

## Sample Application

**Wpf.Components > Capture > CaptureService**
