# 🌈 ColorGraphicDisplay

A colour graphic display module (a TFT LCD such as the ST7735, ST7789 or ILI9341, or a colour OLED such as the SSD1331 or SSD1351, up to 480 x 480 pixels) drawn pixel by pixel, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly.

## 🔍 Overview

`ColorGraphicDisplay` draws the common 0.95" to 2.8" colour modules: the panel, the picture in 24-bit colour and an optional bezel. A `Tft` panel lifts black to the faint glow of its backlight; an `Oled` panel keeps true black. When one pixel is at least 4 device-independent units, a thin pixel grid is drawn between the pixels.

The control shows a `ColorFrameBuffer`, the picture and the display state (on/off, inverted, sleeping, brightness) of the chip. You can drive it in three ways:

- **Text** - set `Text` and the control draws it in `TextColor` with the 5 x 8 glyphs of an HD44780 character ROM.
- **Whole frames** - send the RGB565 frame of the device (a TFT_eSPI sprite, an Adafruit `GFXcanvas16`, an LVGL draw buffer) and pass it to `ColorFrameBuffer.LoadFrameRgb565` with the matching `Rgb565ByteOrder`.
- **Emulator** - feed the exact SPI bytes your firmware sends to the chip to a `TftController` (ST7735, ST7789, ILI9341), an `Ssd1331Controller` or an `Ssd1351Controller`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Modules in XAML

```xml
<!-- The default: a 1.8" 128 x 160 TFT -->
<atc:ColorGraphicDisplay Text="Hello, ESP32!" />

<!-- A 1.14" 135 x 240 TFT with yellow text -->
<atc:ColorGraphicDisplay PixelColumns="135" PixelRows="240" TextColor="Yellow" Text="135 x 240" />

<!-- A 1.5" 128 x 128 colour OLED, without pixel grid and bezel -->
<atc:ColorGraphicDisplay
    PixelColumns="128"
    PixelRows="128"
    PanelKind="Oled"
    ShowPixelGrid="False"
    ShowBezel="False"
    Text="OLED" />
```

### Whole frames from a device

The simplest way to mirror a device: draw into an off-screen frame on the device, send it as it is and load it. A frame is `width * height * 2` bytes (RGB565, two bytes per pixel): 40,960 bytes for 128 x 160, 153,600 bytes for 240 x 320.

The byte order depends on how the device holds the frame in memory:

| Device frame | Bytes in memory | `Rgb565ByteOrder` |
|---|---|---|
| TFT_eSPI `TFT_eSprite` (16-bit), `getPointer()` | high byte first ⚠️ | `BigEndian` |
| Adafruit_GFX `GFXcanvas16`, `getBuffer()` | native `uint16_t`, low byte first | `LittleEndian` |
| LVGL draw buffer without `LV_COLOR_16_SWAP` | native, low byte first | `LittleEndian` |
| LVGL draw buffer with `LV_COLOR_16_SWAP` | swapped, high byte first | `BigEndian` |
| Bytes captured from the SPI bus | wire order, high byte first | `BigEndian` |

⚠️ TFT_eSPI's sprite drawing functions (`drawPixel`, `fillRect`, ...) store each 16-bit colour byte-swapped, so the sprite memory is already in wire order (read from `Extensions/Sprite.cpp`, not measured on hardware). `setSwapBytes()` only changes how `pushImage()` copies an image into the sprite. A frame loaded with the wrong order shows scrambled colours with banding in smooth gradients: try the other order.

Send the frame as a ColorFrame packet of the [display mirroring protocol](../../docs/Displays/DisplayMirrorProtocol.md) and receive it with a [`DisplayMirrorReceiver`](DisplayMirror_Readme.md): the packet has a CRC, so the receiver finds the frames again after line noise or a device reset. A ColorFrame holds the pixels high byte first (`BigEndian`), so a TFT_eSPI sprite is sent as it is. The ESP32 side (Arduino, TFT_eSPI) sends the sprite after each change. `crc16` is the function in the receiver readme; the header, the dimensions and the pixels are written one after the other, so the 64 KB frame is not copied:

```cpp
#include <TFT_eSPI.h>

TFT_eSPI tft;
TFT_eSprite sprite(&tft);

void sendFrame() {
  const uint32_t pixels = 240UL * 135UL * 2UL;       // 64800 bytes
  const uint8_t size[4] = { 240, 0, 135, 0 };          // u16 width, u16 height, little-endian
  const uint32_t length = 4 + pixels;
  uint8_t header[10] = { 0xA7, 0xC0, 1, 0, 0x11, 0,  // display id 0, ColorFrame
                         (uint8_t)length, (uint8_t)(length >> 8), (uint8_t)(length >> 16), (uint8_t)(length >> 24) };
  const uint8_t* data = (const uint8_t*)sprite.getPointer();
  uint16_t crc = crc16(crc16(crc16(0xFFFF, header + 2, 8), size, 4), data, pixels);
  Serial.write(header, 10);
  Serial.write(size, 4);
  Serial.write(data, pixels);
  Serial.write((uint8_t)crc);
  Serial.write((uint8_t)(crc >> 8));
}

void setup() {
  Serial.begin(921600);
  tft.init();
  sprite.setColorDepth(16);
  sprite.createSprite(240, 135);
}

void loop() {
  sprite.fillSprite(TFT_BLACK);
  sprite.setTextColor(TFT_YELLOW);
  sprite.drawString(String(millis()), 4, 4, 4);
  sprite.pushSprite(0, 0);                           // to the real display
  sendFrame();                                       // to the desktop
  delay(50);
}
```

The receiving side registers the buffer for the display id and reads the port. The buffer is thread-safe, so the receiver loads it on the reading thread; the control redraws on the UI thread, at most once per frame, and copies only the rows that changed.

```csharp
public sealed class TftMirrorViewModel : ViewModelBase
{
    private readonly DisplayMirrorReceiver receiver = new();

    public TftMirrorViewModel()
        => receiver.Register(0, DisplayMirrorTargets.For(Buffer));

    public ColorFrameBuffer Buffer { get; } = new(240, 135);

    public async Task MirrorAsync(
        string portName,
        CancellationToken cancellationToken)
    {
        using var port = new SerialPort(portName, 921600);
        port.Open();
        await receiver.RunAsync(port.BaseStream, cancellationToken);
    }
}
```

```xml
<atc:ColorGraphicDisplay PixelColumns="240" PixelRows="135" Buffer="{Binding Buffer}" />
```

An Adafruit_GFX `GFXcanvas16` holds its pixels low byte first: swap each pair of bytes on the device before sending a ColorFrame, or read the frames yourself and load them with `Buffer.LoadFrameRgb565(frame, Rgb565ByteOrder.LittleEndian)`. A frame of another size than the buffer is rejected (`SizeMismatch` in the receiver's statistics). TCP, UDP and MQTT work the same way; see the receiver readme. `LoadFrame(ReadOnlySpan<ushort>)` takes the pixels as 16-bit values. Both throw `ArgumentException` when the length does not match the size and leave the picture as it was. The buffer also has `SetPixel`, `GetPixel`, `DrawLine`, `DrawRectangle`, `FillRectangle`, `DrawText` and `Clear`; `BeginBatch()` groups several changes into one `Changed` event (one redraw).

### Emulator: SPI forwarding

To mirror the bytes on the bus (from an SPI sniffer, or forwarded by a Raspberry Pi or ESP32 that also drives the module), pass them to the controller of the chip with the D/C line split: a command byte (D/C low) with `WriteCommand` / `WriteCommands`, data bytes (D/C high) with `WriteData`. The controller keeps the chip's own RAM and state (address window, MADCTL or remap, pixel format, display on/off, invert, sleep, scrolling) and draws the part of the RAM the panel shows into the buffer.

```csharp
public ColorFrameBuffer Buffer { get; } = new(135, 240);

private TftController? controller;

private void StartMirror()
{
    // A 1.14" ST7789 module shows RAM columns 52..186 and rows 40..279.
    controller = new TftController(Buffer, TftChip.St7789, columnOffset: 52, rowOffset: 40);
}

// On the capture thread, in the order the bytes were sent:
private void OnSpiBytes(bool isDataLineHigh, ReadOnlySpan<byte> bytes)
{
    if (isDataLineHigh)
    {
        controller?.WriteData(bytes);
    }
    else
    {
        controller?.WriteCommands(bytes);
    }
}
```

The ST7735 1.44" and 0.96" modules use a smaller RAM mode, which sets the extents of the MADCTL mirrors; set it with `SetRamSize` (see the preset table):

```csharp
var controller = new TftController(Buffer, TftChip.St7735, columnOffset: 24, rowOffset: 0);
controller.SetRamSize(128, 160);
controller.IsPanelBgr = false;
```

The colour OLEDs frame their parameters differently, which the controllers follow:

- **SSD1331**: every byte except the pixels is a command byte, parameters included: `controller.WriteCommands([0x15, 0x00, 0x5F, 0x75, 0x00, 0x3F])`, then the pixels with `WriteData`. There is no RAM-write command.
- **SSD1351**: the command byte with D/C low, its parameters with D/C high (`WriteCommand(0x15)`, `WriteData([0x00, 0x7F])`), and pixels only after `0x5C`.

A new controller starts in the chip's power-on state: display off (and sleeping on a TFT), so the picture appears when the device's init sequence turns the display on. Every controller implements `IGraphicDisplayController`, so a forwarding loop can feed any of them. The interface is `IDisposable`: dispose the controller (or declare it with `using`) to detach it from the buffer; a disposed controller is inert and ignores every later write, reset or setting, so the buffer keeps the last picture. Dispose the old controller before attaching a new one to the same buffer: a live controller draws its RAM again whenever the buffer is resized.

### Module presets

The panel is a window of the chip's RAM. The values below make the Adafruit libraries' `setRotation(0)` show the picture upright; the `xstart` / `ystart` the library adds at rotation 0 can differ from the controller's offsets, which are the window's first RAM column and row without mirroring (MADCTL 0).

| Module | Chip | Pixels | `columnOffset` / `rowOffset` | RAM (`SetRamSize`) | `IsPanelBgr` | `IsPanelInverted` | Library rotation 0 |
|---|---|---|---|---|---|---|---|
| 0.96" mini (Adafruit `INITR_MINI160x80`) | `St7735` | 80 x 160 | 24 / 0 | 128 x 160 | `false` | `false` | MADCTL `0xC0`, start (24, 0) |
| 1.44" green tab (`INITR_144GREENTAB`) | `St7735` | 128 x 128 | 2 / 1 | 132 x 132 | `true` | `false` | MADCTL `0xC8`, start (2, 3) |
| 1.8" green tab (`INITR_GREENTAB`) | `St7735` | 128 x 160 | 2 / 1 | 132 x 162 (default) | `true` | `false` | MADCTL `0xC8`, start (2, 1) |
| 1.14" | `St7789` | 135 x 240 | 52 / 40 | 240 x 320 | `false` | `true` | MADCTL `0xC0`, start (53, 40) |
| 1.3" | `St7789` | 240 x 240 | 0 / 0 | 240 x 320 | `false` | `true` | MADCTL `0xC0`, start (0, 80) |
| 2.0" | `St7789` | 240 x 320 | 0 / 0 | 240 x 320 | `false` | `true` | MADCTL `0xC0`, start (0, 0) |
| 2.4" / 2.8" | `Ili9341` | 240 x 320 | 0 / 0 | 240 x 320 | `true` | `false` | MADCTL `0x48` (from `begin()`) |
| 0.95" colour OLED | `Ssd1331Controller` | 96 x 64 | - | 96 x 64 | `true` | - | remap `0x76` |
| 1.27" colour OLED | `Ssd1351Controller` | 128 x 96 | - | 128 x 128 | `true` | - | remap `0x74`, start line 96 |
| 1.5" colour OLED | `Ssd1351Controller` | 128 x 128 | - | 128 x 128 | `true` | - | remap `0x74`, start line 128 (0) |

The 0.96" plug-in variant (`INITR_MINI160x80_PLUGIN`) uses 26 / 1, the 132 x 162 RAM, `IsPanelBgr = true` and `IsPanelInverted = true` ⚠️ (derived from its swapped invert commands).

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PixelColumns` | `int` | `128` | Pixel columns of the module, 1 to 480 |
| `PixelRows` | `int` | `160` | Pixel rows of the module, 1 to 480 |
| `PanelKind` | `ColorPanelKind` | `Tft` | `Tft` lifts black to the backlight glow, `Oled` keeps true black |
| `ShowPixelGrid` | `bool` | `true` | Draws thin gaps between the pixels when a pixel is at least 4 units |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the panel |
| `BezelBrush` | `Brush?` | `Black` | Colour of the bezel |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |
| `Text` | `string` | `""` | Clears the buffer to black and draws the text from the top-left; lines separated by `\n` are 8 rows apart |
| `TextColor` | `Color` | `White` | Colour of the glyphs of `Text` |
| `CharacterRom` | `CharacterRom` | `A00` | The HD44780 ROM whose 5 x 8 glyphs draw `Text` |
| `Buffer` | `ColorFrameBuffer` | own buffer | The picture to show; it is resized to `PixelColumns` x `PixelRows`. `null` returns to the control's own buffer |

`ColorFrameBuffer` state: `IsDisplayOn` (default `true`), `IsInverted`, `IsSleeping` and `Brightness` (0-255, default 255, drawn as the opacity of the picture). An emulator sets them from the chip commands; set them yourself when you load whole frames.

## 📝 Notes

- **One way into the buffer.** `Text` writes into the buffer when it (or `TextColor`, `CharacterRom`) changes, and a newly assigned buffer takes over a non-empty `Text`. Set `Text` to empty before a frame stream or an emulator takes over the buffer.
- **One control per buffer.** The rows that changed are handed to the first control that asks for them, so show a buffer in one control at a time.
- **Upright picture.** The panels are wired so that the Adafruit libraries' rotation 0 is upright: an ST7735 or ST7789 panel shows its RAM window turned by 180 degrees (MADCTL MX | MY is rotation 0), an ILI9341 panel shows it mirrored left to right (MX). The other rotations reach the panel through MADCTL or the remap, as on the module. This wiring is derived from the libraries ⚠️, not measured.
- **Colour order.** `IsPanelBgr` is the colour filter of the module: the colour shown has red and blue swapped when the chip's colour order bit (MADCTL bit 3, remap bit 2) differs from it. The defaults follow the libraries ⚠️ (derived, not measured).
- **Inverted panels.** `IsPanelInverted` is the other half of the module wiring: the colour shown is inverted when the chip's inversion (`0x20` / `0x21`) differs from it. The ST7789 modules have IPS panels that show inverted colours unless inversion is on, so `Adafruit_ST7789` sends `0x21` in its init; `IsPanelInverted` defaults to `true` for the ST7789, and the replay shows true colours. The ST7735 and ILI9341 (TN panels) default to `false`. The 0.96" ST7735 plug-in variant, which the library drives with the invert commands swapped, needs `true` ⚠️ (derived).
- ⚠️ **TFT brightness.** A TFT controller starts with `Brightness` 255. The ST7789 datasheet resets `0x51` to 0, but the libraries never send it and the modules dim the backlight through a pin, so 0 would blank every replay. `0x51` is honoured on the ST7789 and ILI9341; the ST7735 has no such command.
- ⚠️ **SSD1331 `begin()` quirk.** Adafruit_SSD1331's init sends `0xA0` (remap) without its parameter, so a chip that counts parameters takes the following `0xA1` as the remap and treats the `0x00` after it as an unknown command; the start line stays 0. The following `setRotation(0)` sends the remap `0x76`, so the state after `begin()` is the same. The emulator follows the chip.
- ⚠️ **SSD1331 start line, offset and multiplex.** `0xA1`, `0xA2` and `0xA8` are mapped with the SSD1306 formula; the SSD1331 datasheet figures were not checked, and the library sets 0 / 0 / 64.
- ⚠️ **SSD1351 rows.** The 128 x 96 module is the top 96 rows of the 128-row RAM, selected by the start line 96 the library sends. The emulator ignores the COM split (remap bit 5) and draws the RAM rows in order, as the library's split-on remaps show them; a real module with the split off shows interleaved rows. The backwards scan (`A[4] = 1`) mapping is derived, not confirmed. The datasheet prints a reset display offset of `0x60`, which looks wrong next to its own figure; the library always sends 0.
- **SSD1351 command lock.** After reset `0xA2`, `0xB1`, `0xB3`, `0xBB`, `0xBE` and `0xC1` are ignored until `0xFD 0xB1`; `0xFD 0x16` locks every command except `0xFD 0x12`. The Adafruit init unlocks both.
- **Contrast of the colour OLEDs.** The SSD1331 / SSD1351 contrast and master current are kept but do not change `Brightness`.
- **Unknown bytes.** A command byte the chip does not have is ignored and written once to `Trace`. No byte throws; only API misuse (a null buffer, a frame of the wrong length, an offset outside the RAM) does.
- **Threading.** `ColorFrameBuffer` and the controllers are thread-safe; `Changed` is raised on the thread that wrote. Feed a controller from one thread so the bytes stay in order. The control redraws on the UI thread, at most once per frame.
- **Accessibility.** The control is an image to UI Automation, named after its resolution (for example `Display 128x160`); set `AutomationProperties.Name` to describe the content.

## 🔗 Related Controls

- **OledDisplay** - monochrome OLED (SSD1306, SSD1309, SH1106) with a chip emulator and an I2C decoder
- **CharacterDisplay** - HD44780 character LCD/VFD with an emulator and a PCF8574 I2C decoder
- **SevenSegment** / **DigitalClock** - seven-segment digits and clock

## 🎮 Sample Application

See the ColorGraphicDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > ColorGraphicDisplay**: presets for the ST7735 (0.96", 1.44", 1.8"), ST7789 (1.14", 1.3", 2.0"), ILI9341 (2.4" / 2.8"), SSD1331 (0.95") and SSD1351 (1.27", 1.5") modules, every setting in the side panel (panel kind, pixel grid, bezel, panel colour order, inverted panel, brightness, display on, invert, text, text colour, character ROM, byte order) and three feed modes: Text, Frame (a moving gradient and a ball streamed with `LoadFrameRgb565` at 30 fps, with a byte-order switch to show a wrong swap) and Replay (the init and a scene as the Adafruit libraries send them over SPI, with start, stop, step and speed; in Replay the display switches send the chip commands, and Inverted is the inversion shown, so on an ST7789 it sends `0x20`).