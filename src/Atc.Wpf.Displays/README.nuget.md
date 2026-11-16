# Atc.Wpf.Displays

WPF controls that draw hardware displays dot by dot, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly:

- **Character LCD/VFD** - `CharacterDisplay` (HD44780, 1-40 x 1-4) with the `Hd44780Controller` emulator and the `Pcf8574Hd44780Decoder` I2C backpack decoder
- **Monochrome OLED** - `OledDisplay` (SSD1306, SSD1309, SH1106) with the `Ssd1306Controller` emulator and the `Ssd1306I2cDecoder`
- **Colour TFT/OLED** - `ColorGraphicDisplay` (ST7735, ST7789, ILI9341, SSD1331, SSD1351) with chip emulators
- **E-paper** - `EPaperDisplay` (SSD1680, black/white or black/white/red) with the `Ssd1680Controller` emulator
- **Graphic LCD** - `GraphicLcdDisplay` (ST7920 128 x 64) with the `St7920Controller` emulator and the `St7920SerialDecoder`
- **LED matrix and seven-segment** - `LedMatrixDisplay` and `SevenSegmentDisplay` (MAX7219) with the `Max7219Controller` emulator
- **Display mirroring** - `DisplayMirrorReceiver` applies protocol v1 packets sent over serial, TCP, UDP or MQTT to the displays

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.Displays
```

For the MVVM base classes and the `[ObservableProperty]` / `[RelayCommand]` / `[DependencyProperty]` source generators, also reference [Atc.XamlToolkit](https://www.nuget.org/packages/Atc.XamlToolkit) and [Atc.XamlToolkit.Wpf](https://www.nuget.org/packages/Atc.XamlToolkit.Wpf) directly; the generators do not flow through package dependencies.

## Usage

The controls are in the `atc` XAML namespace and in the `Atc.Wpf.Displays` C# namespace:

```xml
<UserControl xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <atc:OledDisplay Text="Hello, ESP32!" />
</UserControl>
```

## Documentation

- [Display controls overview](https://github.com/atc-net/atc-wpf/blob/main/docs/Displays/%40Readme.md)
- [Display mirroring protocol](https://github.com/atc-net/atc-wpf/blob/main/docs/Displays/DisplayMirrorProtocol.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code