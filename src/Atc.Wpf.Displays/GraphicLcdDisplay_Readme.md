# 📟 GraphicLcdDisplay

A 128 x 64 monochrome graphic LCD module (the ST7920 "12864" modules) drawn pixel by pixel, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly.

## 🔍 Overview

`GraphicLcdDisplay` draws the module in the colours of an LCD `CharacterDisplay`: blue (the default) or yellow-green, among others, with the backlight on or off (the grey-green reflective look). The pixels sit in a fine grid with almost no gap.

The control shows a 128 x 64 `MonochromeFrameBuffer`. You can drive it in three ways:

- **Text** - set `Text` and the control draws it with the 5 x 8 glyphs of an HD44780 character ROM (not the ST7920's own font).
- **Whole frames** - send the device's frame buffer (U8g2 `getBufferPtr()` in its layout, converted to pages) and call `MonochromeFrameBuffer.LoadFrame`, or send a MonoFrame packet.
- **Emulator** - feed the bytes your firmware sends to an `St7920Controller`: the serial (SPI) bytes through an `St7920SerialDecoder`, or the logical instruction and data bytes directly. The emulator draws both the text layer (DDRAM, CGRAM user fonts, an 8 x 16 font) and the graphic layer (GDRAM).

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Modules in XAML

```xml
<!-- The default: a blue module with the backlight on -->
<atc:GraphicLcdDisplay Text="Hello, ESP32!" />

<!-- Yellow-green, backlight off -->
<atc:GraphicLcdDisplay Color="YellowGreen" IsBacklightOn="False" Text="Reflective" />
```

### Emulator: the serial bytes

```csharp
var buffer = new MonochromeFrameBuffer(128, 64);
var controller = new St7920Controller(buffer);    // power-on: display and graphics off
var decoder = new St7920SerialDecoder(controller);
display.Buffer = buffer;

// 0xF8 = instructions follow, 0xFA = data follow; each byte as two nibble bytes (0x0C -> 00 C0).
decoder.Write([0xF8, 0x00, 0xC0]);                 // display on
decoder.Write([0xF8, 0x30, 0xE0, 0xF8, 0x30, 0xE0]); // extended set, graphics on
decoder.Write([0xF8, 0x80, 0x00, 0xF8, 0x80, 0x00]); // GDRAM row 0, word 0
decoder.Write([0xFA, 0x80, 0x00, 0x00, 0x00]);       // word 0x8000: pixel (0, 0)
```

The decoder takes both U8g2's framing (one `0xFA` for a whole data run) and ESPHome's (a start byte before every byte), and it resynchronises on every start byte.

### Mirror it from a device

The U8g2 tee of the Arduino library sends the ST7920 serial bytes as they are on the wire, as Data packets ([firmware/arduino/AtcDisplayMirror/README.md](../../firmware/arduino/AtcDisplayMirror/README.md)); from Python send them with `send_data` ([firmware/python/README.md](../../firmware/python/README.md)). Register `DisplayMirrorTargets.For(decoder)` on a [`DisplayMirrorReceiver`](DisplayMirror_Readme.md): Data goes to the decoder, and a Reset packet resets the chip and the decoder's framing. A device that sends logical instruction and data bytes instead registers `DisplayMirrorTargets.For(controller)` (Commands = instructions, Data = data). A whole frame goes to `DisplayMirrorTargets.For(buffer)`.

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Color` | `CharacterDisplayColor` | `Blue` | The LCD colour preset; the VFD-only `BlueGreen` shows as `YellowGreen` |
| `IsBacklightOn` | `bool` | `true` | Off shows the grey-green reflective look |
| `Text` | `string` | `""` | Clears the buffer and draws the text from the top-left; lines separated by `\n` are 8 rows apart |
| `CharacterRom` | `CharacterRom` | `A00` | The HD44780 ROM whose 5 x 8 glyphs draw `Text` |
| `Buffer` | `MonochromeFrameBuffer` | own buffer | The picture to show; resized to 128 x 64. `null` returns to the control's own buffer |
| `GhostOpacity` | `double` | from preset | Opacity of the unlit pixels |
| `PanelBrush`, `LitDotBrush`, `UnlitDotBrush` | `Brush?` | from preset | The panel, lit and unlit pixel colours |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the panel |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |

The constants `GraphicLcdDisplay.PixelColumns` (128) and `PixelRows` (64) give the size. The buffer's `IsDisplayOn` (the ST7920 `D` bit) switches the picture; its contrast is not used.

## 📝 Notes

- ⚠️ **Approximate font.** The ST7920's 8 x 16 half-width ROM is not published in a usable form: the emulator draws codes 0x20 to 0x7E with the public-domain UNSCII `unscii-16` font, which looks close but not identical; codes 0x02 to 0x1F and 0x7F, and the 16 x 16 Chinese characters, are drawn blank.
- ⚠️ Derived, not confirmed on hardware: the text line order (`0x80`, `0x90`, `0x88`, `0x98` top to bottom), the 128 x 64 GDRAM fold (words 8 to 15 are the bottom half), text and graphics combined by XOR, the `D` bit switching both layers, display clear leaving GDRAM, the handling of a pending first byte, and one start byte for a whole data run.
- Not emulated: the cursor and blink, display shift, vertical scroll and line reverse (stored only), standby (dark until the next instruction).
- **Threading.** The buffer and the controller are thread-safe; feed a decoder from one thread. The control redraws on the UI thread, at most once per frame.
- **Accessibility.** The control is an image to UI Automation, named `LCD 128x64`.

## 🔗 Related Controls

- **CharacterDisplay** - HD44780 character LCD/VFD, the same colour presets
- **OledDisplay** - monochrome OLED with an SSD1306 emulator
- **DisplayMirrorReceiver** - receives the packets a device sends

## 🎮 Sample Application

See the GraphicLcdDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > GraphicLcdDisplay**: presets for the blue and the yellow-green module, every setting in the side panel (colour, backlight, bezel, text, character ROM) and three feed modes: Text, Frame (a bouncing ball streamed with `LoadFrame` at 30 fps) and Replay (U8g2's or ESPHome's serial bytes: init, then a border, "Hello ESP32" and a filled box, with start, stop, step and speed).