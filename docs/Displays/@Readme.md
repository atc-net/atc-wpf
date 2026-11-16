# 🖥️ Display Controls

This document lists the hardware display controls in **Atc.Wpf.Displays** (namespace `Atc.Wpf.Displays`, also mapped into the `atc` XAML namespace). They draw character LCDs, OLEDs, colour TFTs, e-paper, graphic LCDs, LED matrices and seven-segment modules dot by dot, with chip emulators and decoders, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project.

```shell
dotnet add package Atc.Wpf.Displays
```

## 📊 Quick Reference

| Control | Primary Use Case | Interactive | Theming |
|---------|------------------|-------------|---------|
| [CharacterDisplay](../../src/Atc.Wpf.Displays/CharacterDisplay_Readme.md) | Dot-by-dot HD44780 character LCD/VFD (1-40 x 1-4) that mirrors an ESP32 or Raspberry Pi display, with an HD44780 emulator and a PCF8574 I2C decoder | ❌ No | ❌ Own presets |
| [ColorGraphicDisplay](../../src/Atc.Wpf.Displays/ColorGraphicDisplay_Readme.md) | Pixel-by-pixel colour TFT/OLED (ST7735, ST7789, ILI9341, SSD1331, SSD1351, up to 480 x 480) that mirrors an ESP32 or Raspberry Pi display, with RGB565 whole-frame `LoadFrameRgb565` and chip emulators | ❌ No | ❌ Own presets |
| [DisplayMirrorReceiver](../../src/Atc.Wpf.Displays/DisplayMirror_Readme.md) | Receives display mirroring packets (protocol v1, CRC-checked, resyncs on noise) over serial, TCP, UDP or MQTT and applies them to the OledDisplay, ColorGraphicDisplay, CharacterDisplay, EPaperDisplay, LedMatrixDisplay, SevenSegmentDisplay and GraphicLcdDisplay buffers, emulators and decoders, with Describe and per-display statistics | ❌ No | ➖ Not a control |
| [EPaperDisplay](../../src/Atc.Wpf.Displays/EPaperDisplay_Readme.md) | Pixel-by-pixel SSD1680 e-paper (2.13" 122 x 250, 2.9" 128 x 296, black/white or black/white/red) that mirrors an ESP32 or Raspberry Pi display, with the refresh flash, a GxEPD2-compatible rotation and an SSD1680 emulator | ❌ No | ❌ Own presets |
| [GraphicLcdDisplay](../../src/Atc.Wpf.Displays/GraphicLcdDisplay_Readme.md) | Pixel-by-pixel ST7920 128 x 64 graphic LCD in the character LCD colours, with an ST7920 emulator (text and graphic layers) and a serial (SPI) decoder | ❌ No | ❌ Own presets |
| [LedMatrixDisplay](../../src/Atc.Wpf.Displays/LedMatrixDisplay_Readme.md) | LED-by-LED chain of 1-8 MAX7219 8 x 8 matrix modules (FC-16, Generic, Parola, ICStation wiring) with a MAX7219 emulator fed latch frames | ❌ No | ❌ Own presets |
| [OledDisplay](../../src/Atc.Wpf.Displays/OledDisplay_Readme.md) | Pixel-by-pixel monochrome OLED (SSD1306, SSD1309, SH1106, up to 132 x 64) that mirrors an ESP32 or Raspberry Pi display, with whole-frame `LoadFrame`, a chip emulator and an I2C decoder | ❌ No | ❌ Own presets |
| [SevenSegmentDisplay](../../src/Atc.Wpf.Displays/SevenSegmentDisplay_Readme.md) | A MAX7219 seven-segment module (1-8 digits, Code B decode or raw segments, decimal points) drawn with `SevenSegment` digits | ❌ No | ✅ Accent brush |

## 🔗 More

- Wire format of display mirroring: [DisplayMirrorProtocol](DisplayMirrorProtocol.md)
- Raspberry Pi sender: [firmware/python](../../firmware/python/README.md)
- ESP32 sender: [firmware/arduino/AtcDisplayMirror](../../firmware/arduino/AtcDisplayMirror/README.md)
- Sample application: **Wpf.Displays** tab in `sample/Atc.Wpf.Sample`