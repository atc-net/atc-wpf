# 🖥️ OledDisplay

A monochrome graphic OLED module (SSD1306, SSD1309, SH1106 and compatibles, up to 132 x 64 pixels) drawn pixel by pixel, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly.

## 🔍 Overview

`OledDisplay` draws the common 0.91", 0.96", 1.3", 1.54" and 2.42" OLED modules: the black panel, the lit pixels in white, blue, yellow, green or the two-colour yellow-blue, and an optional bezel. The contrast the device sends dims the pixels as on the real panel.

The control shows a `MonochromeFrameBuffer`, the picture and the display state (on/off, inverted, entire display on, contrast) of the chip. Its layout is the one the libraries use: byte `x + (y / 8) * width`, bit `y % 8`, the least significant bit on top. You can drive it in three ways:

- **Text** - set `Text` and the control draws it with the 5 x 8 glyphs of an HD44780 character ROM.
- **Whole frames** - send the frame buffer of the device (Adafruit `display.getBuffer()`, U8g2 `u8g2.getBufferPtr()`) and pass it to `MonochromeFrameBuffer.LoadFrame`.
- **Emulator** - feed the exact bytes your firmware sends to the chip to an `Ssd1306Controller` (SPI: commands and data), or the raw I2C transactions to an `Ssd1306I2cDecoder`.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Modules in XAML

```xml
<!-- The default: a 0.96" 128 x 64 white module -->
<atc:OledDisplay Text="Hello, ESP32!&#10;Line two" />

<!-- A 0.91" 128 x 32 module -->
<atc:OledDisplay PixelRows="32" Text="128 x 32" />

<!-- The two-colour module: the top 16 rows yellow, the rest blue -->
<atc:OledDisplay Color="YellowBlue" Text="Status&#10;&#10;Temperature 21.5" />

<!-- Inverted, without bezel -->
<atc:OledDisplay IsInverted="True" ShowBezel="False" Text="Inverted" />
```

### Whole frames from a device

The simplest way to mirror a device: send the library's frame buffer as it is and load it. A 128 x 64 frame is 1,024 bytes (8 pages of 128 bytes), a 128 x 32 frame 512 bytes.

The ESP32 side (Arduino, Adafruit_SSD1306) sends the buffer after each `display()`, with a small header so the receiver can find the start of a frame:

```cpp
#include <Adafruit_SSD1306.h>

Adafruit_SSD1306 display(128, 64, &Wire, -1);

void sendFrame() {
  const uint16_t length = 128 * 64 / 8;            // 1024 bytes
  Serial.write(0xA5);                               // frame header
  Serial.write(0x5A);
  Serial.write(lowByte(length));
  Serial.write(highByte(length));
  Serial.write(display.getBuffer(), length);
}

void setup() {
  Serial.begin(921600);
  display.begin(SSD1306_SWITCHCAPVCC, 0x3C);
}

void loop() {
  display.clearDisplay();
  display.setCursor(0, 0);
  display.setTextColor(SSD1306_WHITE);
  display.print(millis());
  display.display();
  sendFrame();
  delay(50);
}
```

With U8g2 send `u8g2.getBufferPtr()`, `8 * u8g2.getBufferTileWidth() * u8g2.getBufferTileHeight()` bytes long: the same layout. This holds the whole frame only with a full-buffer constructor (`U8G2_..._F_...`); the page-buffer constructors (`_1_`, `_2_`) hold one strip of 8 or 16 rows, which `LoadFrame` rejects. Over MQTT, publish the same 1,024 bytes as the payload and skip the header.

The receiving side reads the header and the frame and loads it. The buffer is thread-safe, so it can be loaded on the serial thread; the control redraws on the UI thread, at most once per frame.

```csharp
public sealed class OledMirrorViewModel : ViewModelBase
{
    public OledMirrorViewModel(SerialPort port)
    {
        var frame = new byte[Buffer.Width * Buffer.PageCount];

        // ReadTimeout should be set on the port; a timeout ends the read loop.
        _ = Task.Run(() =>
        {
            while (port.IsOpen)
            {
                if (port.ReadByte() != 0xA5 || port.ReadByte() != 0x5A)
                {
                    continue; // not at a frame start yet
                }

                var length = port.ReadByte() | (port.ReadByte() << 8);
                if (length != frame.Length)
                {
                    continue;
                }

                for (var read = 0; read < length;)
                {
                    read += port.Read(frame, read, length - read);
                }

                Buffer.LoadFrame(frame);
            }
        });
    }

    public MonochromeFrameBuffer Buffer { get; } = new(128, 64);
}
```

```xml
<atc:OledDisplay PixelColumns="128" PixelRows="64" Buffer="{Binding Buffer}" />
```

`LoadFrame` throws `ArgumentException` when the length is not `Width * PageCount` and leaves the picture as it was. The buffer also has `SetPixel`, `DrawLine`, `DrawRectangle`, `FillRectangle`, `DrawText`, `Clear` and `Fill`; `BeginBatch()` groups several changes into one `Changed` event (one redraw).

### Emulator: raw I2C traffic

To mirror the bytes on the bus (from an I2C sniffer, or forwarded by a Raspberry Pi that also drives the module), pass each write transaction, without the address byte, to an `Ssd1306I2cDecoder`. Each transaction starts with the control byte (0x00 for commands, 0x40 for data). The decoder feeds an `Ssd1306Controller`, which keeps the chip's own display RAM and state (addressing mode, contrast, invert, display on/off, remap, COM configuration) and draws the part of the RAM the panel shows into the buffer.

```csharp
public MonochromeFrameBuffer Buffer { get; } = new(128, 64);

private Ssd1306Controller? controller;
private Ssd1306I2cDecoder? decoder;

private void StartMirror()
{
    controller = new Ssd1306Controller(Buffer, OledChip.Ssd1306);
    decoder = new Ssd1306I2cDecoder(controller);
}

// On the capture thread, once per I2C write transaction (address byte removed):
private void OnI2cWrite(ReadOnlySpan<byte> transaction)
    => decoder?.Write(transaction);
```

A new controller starts in the chip's power-on state: display off and the RAM cleared, so the picture appears when the device's init sequence sends `0xAF`. Dispose the controller (`IGraphicDisplayController` is `IDisposable`, so a `using` declaration works) to detach it from the buffer and stop the scroll timer; a disposed controller is inert and ignores every later write, reset or setting, so the buffer keeps the last picture.

### Emulator: SPI

4-wire SPI has no control byte: the D/C line tells commands from data. Pass the bytes to the controller with that split:

```csharp
// D/C low
controller.WriteCommands([0xAE, 0xD5, 0x80, 0xA8, 0x3F]);

// D/C high
controller.WriteData(pageBytes);
```

`WriteCommand(byte)` sends one command byte. A command's parameters may arrive in later calls, as on the chip (Adafruit_SSD1306 sends `0xA8` and its parameter in two I2C writes).

### SH1106 (1.3" modules)

```csharp
var controller = new Ssd1306Controller(Buffer, OledChip.Sh1106);
```

The SH1106 has 132 columns of RAM and the 128-wide panel shows columns 2 to 129, so the libraries (U8g2, Adafruit_SH110X) write each page from column 2. `ColumnOffset` (default 2 for the SH1106, 0 otherwise) sets the first RAM column the panel shows; a module whose picture is shifted by two columns needs another offset. Whole frames from `getBuffer()` / `getBufferPtr()` are 128 columns wide and need no offset.

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PixelColumns` | `int` | `128` | Pixel columns of the module, 1 to 132 |
| `PixelRows` | `int` | `64` | Pixel rows of the module, 1 to 64 |
| `Color` | `OledDisplayColor` | `White` | `White`, `Blue`, `Yellow`, `Green` or `YellowBlue` |
| `YellowZoneRows` | `int` | `16` | Rows drawn yellow at the top of a `YellowBlue` module, 0 to `PixelRows` |
| `IsInverted` | `bool` | `false` | Shows the picture inverted; XORed with the buffer's `IsInverted` (the chip's `0xA7`) |
| `Text` | `string` | `""` | Clears the buffer and draws the text from the top-left; lines separated by `\n` are 8 rows apart |
| `CharacterRom` | `CharacterRom` | `A00` | The HD44780 ROM whose 5 x 8 glyphs draw `Text` |
| `Buffer` | `MonochromeFrameBuffer` | own buffer | The picture to show; it is resized to `PixelColumns` x `PixelRows`. `null` returns to the control's own buffer |
| `Brightness` | `double` | from contrast | Set from the buffer's `Contrast`: `0.4 + 0.6 * Contrast / 255` |
| `GhostOpacity` | `double` | `0` | Opacity of the unlit pixels; above 0 shows the pixel grid |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the panel |
| `BezelBrush` | `Brush?` | `Black` | Colour of the bezel |
| `PanelBrush` | `Brush?` | from preset | Colour of the panel |
| `LitDotBrush` | `Brush?` | from preset | Colour of a lit pixel |
| `DotCornerRadius` | `double` | from preset | 0 (square) to 0.5 (round) |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |

`MonochromeFrameBuffer` state: `IsDisplayOn` (default `true`), `IsInverted`, `IsEntireDisplayOn` and `Contrast` (default `0x7F`). An emulator sets them from the chip commands (`0xAE`/`0xAF`, `0xA6`/`0xA7`, `0xA4`/`0xA5`, `0x81`).

## 📝 Notes

- **One way into the buffer.** `Text` writes into the buffer when it (or `CharacterRom`) changes, and a newly assigned buffer takes over a non-empty `Text`. Set `Text` to empty before a frame stream or an emulator takes over the buffer, or the text is drawn over the device's picture.
- **Upright picture.** The picture is upright with the segment remap `0xA1` and COM scan `0xC8` that Adafruit_SSD1306 and U8g2 send; the power-on `0xA0` and `0xC0` show it rotated by 180 degrees, as on a real module. The panel wiring (a panel taller than 32 rows uses the alternative COM configuration `0xDA 0x12`, a 32-row panel the sequential `0xDA 0x02`) is derived from the common modules ⚠️: check it against your module.
- **SH1106 two-column shift.** See above: RAM columns 0, 1, 130 and 131 are not shown with the default `ColumnOffset` of 2. The SH1106 has only page addressing: a page does not continue into the next page, and the horizontal addressing (`0x20`), window (`0x21`, `0x22`), charge pump (`0x8D`) and scroll commands are ignored. Data written past RAM column 131 is ignored ⚠️ (the datasheet does not say what the chip does).
- **Yellow-blue modules** have no yellow or blue in the chip: the top 16 rows of the glass emit yellow and the rest blue. `YellowZoneRows` sets the boundary.
- **Continuous scroll.** `0x26`/`0x27` and `0x29`/`0x2A` followed by `0x2F` move the picture one column per step on a timer, at the step time of the interval code and the frame period (about 88 Hz after the library init of a 128 x 64 module ⚠️ derived from the datasheet formula). Only the horizontal part is emulated:
  - ⚠️ The vertical offset of `0x29`/`0x2A` and the vertical scroll area `0xA3` are recorded but move nothing (the direction of the chip's vertical movement is not confirmed).
  - ⚠️ "Right" (`0x26`) moves the picture towards higher panel columns under the upright `0xA1` + `0xC8`; this follows the library names, the datasheet figures are drawn in segment order.
  - ⚠️ The interval code 110b is 25 frames, as printed in the SSD1306 datasheet. The SSD1309 table differs and is used for that chip.
  - `0x2E` stops the scroll and leaves the picture where it is (the datasheet asks the host to rewrite the RAM) ⚠️.
- **SSD1309.** The command set is the SSD1306's. Adafruit_SSD1306's `0x8D 0x14` is harmless (the SSD1309 has no charge pump command). Adafruit's scroll commands send 6 parameters while the SSD1309 datasheet asks for 7 ⚠️: a library scroll on an SSD1309 is not reproduced reliably.
- **Page commands in every mode.** `0xB0`-`0xB7` and the column nibbles `0x00`-`0x1F` are honoured in horizontal and vertical addressing mode too (U8g2 relies on them) ⚠️: the datasheet lists them for page mode only.
- **Unknown bytes.** A command byte the chip does not have is ignored and written once to `Trace`. No byte throws; only API misuse (a null buffer, a `LoadFrame` of the wrong length) does.
- **Threading.** `MonochromeFrameBuffer` and `Ssd1306Controller` are thread-safe; `Changed` is raised on the thread that wrote (a scroll step: a timer thread). Feed a controller and a decoder from one thread so the bytes stay in order. The control redraws on the UI thread, at most once per frame.
- **Accessibility.** The control is an image to UI Automation, named after its resolution (for example `OLED 128x64`); set `AutomationProperties.Name` to describe the content.

## 🔗 Related Controls

- **CharacterDisplay** - HD44780 character LCD/VFD with an emulator and a PCF8574 I2C decoder
- **SevenSegment** / **DigitalClock** - seven-segment digits and clock
- **Card** - a container to put the display in

## 🎮 Sample Application

See the OledDisplay sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > OledDisplay**: presets for the 0.91" 128 x 32 SSD1306, the 0.96" 128 x 64 SSD1306 (white, blue, yellow-blue), the 1.3" SH1106 and the 1.54" and 2.42" SSD1309 modules, every setting in the side panel (colour, yellow zone, unlit pixel opacity, bezel, contrast, display on, invert, text, character ROM) and three feed modes: Text, Frame (a bouncing ball streamed with `LoadFrame` at 30 fps) and Replay (the init and a drawing as Adafruit_SSD1306 or U8g2 send them, over I2C or SPI, with start, stop, step and speed).