# Atc.Wpf.Displays

WPF hardware display controls, chip emulators and decoders, and the display mirroring receiver.

## 🔍 Overview

Every control draws its module dot by dot from a buffer that holds the chip's state. Feed it text, whole frames, or the exact bytes a firmware sends to the chip (through an emulator or a bus decoder), and it shows what the real module shows.

| Area | Types | Documentation |
|------|-------|---------------|
| Character LCD/VFD | `CharacterDisplay`, `CharacterDisplayBuffer`, `Hd44780Controller`, `Pcf8574Hd44780Decoder` | [CharacterDisplay](CharacterDisplay_Readme.md) |
| Monochrome OLED | `OledDisplay`, `MonochromeFrameBuffer`, `Ssd1306Controller`, `Ssd1306I2cDecoder` | [OledDisplay](OledDisplay_Readme.md) |
| Colour TFT/OLED | `ColorGraphicDisplay`, `ColorFrameBuffer`, `TftController`, `Ssd1331Controller`, `Ssd1351Controller` | [ColorGraphicDisplay](ColorGraphicDisplay_Readme.md) |
| E-paper | `EPaperDisplay`, `EPaperFrameBuffer`, `Ssd1680Controller` | [EPaperDisplay](EPaperDisplay_Readme.md) |
| Graphic LCD | `GraphicLcdDisplay`, `St7920Controller`, `St7920SerialDecoder` | [GraphicLcdDisplay](GraphicLcdDisplay_Readme.md) |
| LED matrix | `LedMatrixDisplay`, `Max7219ChainBuffer`, `Max7219Controller` | [LedMatrixDisplay](LedMatrixDisplay_Readme.md) |
| Seven-segment | `SevenSegmentDisplay` (drawn with the `SevenSegment` digit from Atc.Wpf.Controls) | [SevenSegmentDisplay](SevenSegmentDisplay_Readme.md) |
| Display mirroring | `DisplayMirrorReceiver`, `DisplayMirrorProtocol`, `DisplayMirrorTargets` | [DisplayMirror](DisplayMirror_Readme.md), [protocol](../../docs/Displays/DisplayMirrorProtocol.md) |

## 📦 Installation

```xml
<PackageReference Include="Atc.Wpf.Displays" Version="*" />
```

The package depends on `Atc.Wpf.Controls` (for the `SevenSegment` digit) and `Atc.Wpf.Theming`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the controls are in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🎮 Sample Application

The **Wpf.Displays** tab of the Atc.Wpf.Sample application has a sample for every display and for display mirroring.